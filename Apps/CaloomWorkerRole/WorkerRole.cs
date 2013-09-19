// ===================================
// <copyright>Copyright © 2013 All Right Reserved</copyright>
// <author>Gurschler Michael</author>
// <creationDate>19.09.2013</creationDate>
// <email>mailto:info@gurschlermichael.com</email>
// <internet>http://www.gurschlermichael.com</internet>
// ===================================

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AaltoGlobalImpact.OIP;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;
using TheBall;
using TheBall.CORE;
using Titan;

namespace CaloomWorkerRole
{
  public class WorkerRole : RoleEntryPoint
  {
    // QueueClient is thread-safe. Recommended that you cache 
    // rather than recreating it on every request
    public const string CURRENT_HARDCODED_CONTAINER_NAME = "theball-gurschlermichael-com";
    private CloudBlobContainer AnonWebContainer;
    private CloudQueueClient Client;
    private CloudQueue CurrQueue;
    private CloudTableClient CurrTable;
    private bool GracefullyStopped;
    private bool IsStopped;
    private LocalResource LocalStorageResource;
    protected string CurrWorkerID { get; private set; }

    public override void Run()
    {
      GracefullyStopped = false;
      //ThreadPool.SetMinThreads(3, 3);
      Task[] tasks = new Task[]
      {
        Task.Factory.StartNew(() => { }),
        Task.Factory.StartNew(() => { }),
        Task.Factory.StartNew(() => { }), 
        //Task.Factory.StartNew(() => {}), 
        //Task.Factory.StartNew(() => {}), 
        //Task.Factory.StartNew(() => {}), 
      };
      QueueSupport.ReportStatistics("Starting worker: " + CurrWorkerID, TimeSpan.FromDays(1));
      Task.Factory.StartNew(RefreshStockCompanyData);
      while (!IsStopped)
      {
        try
        {
          Task.WaitAny(tasks);
          if (IsStopped)
            break;
          int availableIx;
          Task availableTask = WorkerSupport.GetFirstCompleted(tasks, out availableIx);
          bool handledSubscriptionChain = PollAndHandleSubscriptionChain(tasks, availableIx, availableTask);
          if (handledSubscriptionChain)
          {
            // TODO: Fix return value check
            Thread.Sleep(1000);
            continue;
          }
          bool handledMessage = PollAndHandleMessage(tasks, availableIx, availableTask);
          if (handledMessage)
            continue;
          Thread.Sleep(1000);
        }
        catch (AggregateException ae)
        {
          foreach (var e in ae.Flatten().InnerExceptions)
          {
            ErrorSupport.ReportException(e);
          }
          Thread.Sleep(10000);
          // or ...
          // ae.Flatten().Handle((ex) => ex is MyCustomException);
        }
          /*
                catch (MessagingException e)
                {
                    if (!e.IsTransient)
                    {
                        Trace.WriteLine(e.Message);
                        throw;
                    }
                    Thread.Sleep(10000);
                }*/
        catch (OperationCanceledException e)
        {
          if (!IsStopped)
          {
            Trace.WriteLine(e.Message);
            throw;
          }
        }
        catch (Exception ex)
        {
          ErrorSupport.ReportException(ex);
          throw;
        }
      }
      Task.WaitAll(tasks);
      foreach (var task in tasks.Where(task => task.Exception != null))
      {
        ErrorSupport.ReportException(task.Exception);
      }
      QueueSupport.ReportStatistics("Stopped: " + CurrWorkerID, TimeSpan.FromDays(1));
      GracefullyStopped = true;
    }

    private bool PollAndHandleSubscriptionChain(Task[] tasks, int availableIx, Task availableTask)
    {
      var result = SubscribeSupport.GetOwnerChainsInOrderOfSubmission();
      if (result.Length == 0)
        return false;
      string acquiredEtag = null;
      var firstLockedOwner =
        result.FirstOrDefault(
          lockCandidate => SubscribeSupport.AcquireChainLock(lockCandidate, out acquiredEtag));
      if (firstLockedOwner == null)
        return false;
      var executing =
        Task.Factory.StartNew(
          () =>
            WorkerSupport.ProcessOwnerSubscriptionChains(firstLockedOwner, acquiredEtag,
              CURRENT_HARDCODED_CONTAINER_NAME));
      tasks[availableIx] = executing;
      if (availableTask.Exception != null)
        ErrorSupport.ReportException(availableTask.Exception);
      return true;
    }


    private bool PollAndHandleMessage(Task[] tasks, int availableIx, Task availableTask)
    {
      CloudQueueMessage message;
      QueueEnvelope envelope = QueueSupport.GetFromDefaultQueue(out message);
      if (envelope != null)
      {
        Task executing = Task.Factory.StartNew(() => WorkerSupport.ProcessMessage(envelope));
        tasks[availableIx] = executing;
        QueueSupport.CurrDefaultQueue.DeleteMessage(message);
        if (availableTask.Exception != null)
          ErrorSupport.ReportException(availableTask.Exception);
        return true;
      }
      else
      {
        if (message != null)
        {
          QueueSupport.CurrDefaultQueue.DeleteMessage(message);
          ErrorSupport.ReportMessageError(message);
        }
        GC.Collect();
        return false;
      }
    }

    public override bool OnStart()
    {
      // Set the maximum number of concurrent connections 
      CurrWorkerID = DateTime.Now.ToString();
      ServicePointManager.DefaultConnectionLimit = 12;
      ServicePointManager.UseNagleAlgorithm = false;
      string connStr;
      const string ConnStrFileName = @"C:\Users\Michael\Documents\theballconnstr.txt";
      if (File.Exists(ConnStrFileName))
        connStr = File.ReadAllText(ConnStrFileName);
      else
        connStr = CloudConfigurationManager.GetSetting("StorageConnectionString");
      StorageSupport.InitializeWithConnectionString(connStr);
      InformationContext.InitializeFunctionality(3, allowStatic: true);
      InformationContext.Current.InitializeCloudStorageAccess(CURRENT_HARDCODED_CONTAINER_NAME);
      CurrQueue = QueueSupport.CurrDefaultQueue;
      IsStopped = false;
      return base.OnStart();
    }

    public override void OnStop()
    {
      // Close the connection to Service Bus Queue
      IsStopped = true;
      while (GracefullyStopped == false)
        Thread.Sleep(1000);
      base.OnStop();
    }


    private void RefreshStockCompanyData()
    {
      InformationContext.Current.InitializeCloudStorageAccess(CURRENT_HARDCODED_CONTAINER_NAME);
      var oldDay = -1;
      while (true)
      {
        var refreshCharts = oldDay != DateTime.Now.DayOfYear || oldDay == -1;
        oldDay = DateTime.Now.Day;
        foreach (var groupId in TBRGroupRoot.GetAllGroupIDs())
        {
          var group = TBRGroupRoot.RetrieveFromDefaultLocation(groupId).Group;
          var titanLock = GetLock(group);
          if (titanLock.IsLocked && titanLock.LastLocked > DateTime.Now.AddHours(-1) || titanLock.LastLocked > DateTime.Now.AddMinutes(-5))
            continue;
          try
          {
            titanLock.IsLocked = true;
            titanLock.LastLocked = DateTime.Now;
            titanLock.StoreInformation();
            RefreshStockCompaniesOfOwner(group);
            if (refreshCharts)
              UpdateChartsOfOwner(group);
          }
          catch (Exception ex)
          {
            ErrorSupport.ReportException(ex);
          }
          finally
          {
            titanLock.IsLocked = false;
            titanLock.StoreInformation();
          }
        }
        Thread.Sleep(60000);
      }
    }

    private void RefreshStockCompaniesOfOwner(IContainerOwner owner)
    {
      InformationContext.Current.Owner = owner;
      var inputCollection = InformationInputCollection.RetrieveFromOwnerContent(owner, "MasterCollection");
      if (inputCollection == null || inputCollection.CollectionContent == null)
        return;
      var coll = StockCompanyCollection.RetrieveFromOwnerContent(owner, "default");
      if (coll == null)
      {
        coll = new StockCompanyCollection();
        if(coll.CollectionContent == null)
          coll.CollectionContent = new List<StockCompany>();
        coll.SetLocationAsOwnerContent(owner, "default");
        coll.StoreInformation();
      }
      if (coll.CollectionContent.Count < 10)
      {
        var input = inputCollection.CollectionContent.FirstOrDefault(x => x.LocationURL.Contains("Lookup"));
        if (input == null)
          return;
        //Load the stock companies
        var letters = new[]
        {
          'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T',
          'U', 'V', 'W', 'X', 'Y', 'Z'
        };
        foreach (var letter in letters)
        {
          FetchInputInformation.Execute(new FetchInputInformationParameters
          {
            InformationInputID = input.ID,
            Owner = owner,
            QueryParameters = "?input=" + letter
          });
          ProcessFetchedInputs.Execute(new ProcessFetchedInputsParameters
          {
            InformationInputID = input.ID,
            Owner = owner,
            ProcessingOperationName = "AddNewStockCompanies"
          });
        }
      }
      else
      {
        var input = inputCollection.CollectionContent.FirstOrDefault(x => x.LocationURL.Contains("quotes.csv"));
        if (input == null)
          return;
        foreach (var group in coll.CollectionContent.Select((x, i) => new {Index = i, Value = x})
          .GroupBy(x => x.Index/190))
        {
          var parameters = new StringBuilder("?s=");
          parameters = group.Aggregate(parameters,
            (current, stockCompany) => current.Append(stockCompany.Value.Symbol + ","));
          parameters = parameters.Remove(parameters.Length - 1, 1);
          parameters.Append("&d=t&f=snl1ohgc1p2m5m6v");
          FetchInputInformation.Execute(new FetchInputInformationParameters
          {
            InformationInputID = input.ID,
            Owner = owner,
            QueryParameters = parameters.ToString()
          });
          ProcessFetchedInputs.Execute(new ProcessFetchedInputsParameters
          {
            InformationInputID = input.ID,
            Owner = owner,
            ProcessingOperationName = "UpdateStockCompanies"
          });
        }
      }
    }

    private void UpdateChartsOfOwner(IContainerOwner owner)
    {
      InformationContext.Current.Owner = owner;
      var inputCollection = InformationInputCollection.RetrieveFromOwnerContent(owner, "MasterCollection");
      if (inputCollection == null || inputCollection.CollectionContent == null)
        return;
      var input = inputCollection.CollectionContent.FirstOrDefault(x => x.LocationURL.Contains("table.csv"));
      if (input == null)
        return;
      var coll = StockCompanyCollection.RetrieveFromOwnerContent(InformationContext.Current.Owner, "default");
      foreach (var company in coll.CollectionContent)
      {
        try
        {
          FetchInputInformation.Execute(new FetchInputInformationParameters
          {
            InformationInputID = input.ID,
            Owner = owner,
            QueryParameters = GetChartInputParameters(company.Symbol, 'd', DateTime.Now.AddMonths(-1))
          });
          ProcessFetchedInputs.Execute(new ProcessFetchedInputsParameters
          {
            InformationInputID = input.ID,
            Owner = owner,
            ProcessingOperationName = "UpdateChart_Days;" + company.ID,
          });
          FetchInputInformation.Execute(new FetchInputInformationParameters
          {
            InformationInputID = input.ID,
            Owner = owner,
            QueryParameters = GetChartInputParameters(company.Symbol, 'w', DateTime.Now.AddYears(-1))
          });
          ProcessFetchedInputs.Execute(new ProcessFetchedInputsParameters
          {
            InformationInputID = input.ID,
            Owner = owner,
            ProcessingOperationName = "UpdateChart_Months;" + company.ID,
          });
          FetchInputInformation.Execute(new FetchInputInformationParameters
          {
            InformationInputID = input.ID,
            Owner = owner,
            QueryParameters = GetChartInputParameters(company.Symbol, 'm', DateTime.Now.AddYears(-5))
          });
          ProcessFetchedInputs.Execute(new ProcessFetchedInputsParameters
          {
            InformationInputID = input.ID,
            Owner = owner,
            ProcessingOperationName = "UpdateChart_Years;" + company.ID,
          });
        }
        catch (Exception ex)
        {
          Debug.WriteLine("Updating charts for " + company.Symbol + " failed. " + ex.Message);
        }
      }
    }

    private static string GetChartInputParameters(string symbol, char type, DateTime startTime)
    {
      return string.Format("?s={0}&a={1}&b={2}&c={3}&d={4}&e={5}&f={6}&g={7}", symbol, startTime.Month - 1,
        startTime.Day,
        startTime.Year, DateTime.Now.Month - 1, DateTime.Now.Day, DateTime.Now.Year, type);
    }

    private static TitanLock GetLock(IContainerOwner owner)
    {
      var titanLock = TitanLock.RetrieveFromOwnerContent(owner, "lock");
      if (titanLock == null)
      {
        titanLock = new TitanLock();
        titanLock.SetLocationAsOwnerContent(owner, "lock");
        titanLock.IsLocked = false;
        titanLock.LastLocked = DateTime.MinValue;
        titanLock.StoreInformation();
        titanLock = TitanLock.RetrieveFromOwnerContent(owner, "lock");
      }
      return titanLock;
    }
  }
}