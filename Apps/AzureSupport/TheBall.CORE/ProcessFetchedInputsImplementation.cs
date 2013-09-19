// ===================================
// <copyright>Copyright © 2013 All Right Reserved</copyright>
// <author>Gurschler Michael</author>
// <creationDate>19.09.2013</creationDate>
// <email>mailto:info@gurschlermichael.com</email>
// <internet>http://www.gurschlermichael.com</internet>
// ===================================

using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security;
using JsonFx.Json;
using LumenWorks.Framework.IO.Csv;
using Titan;

namespace TheBall.CORE
{
  public class ProcessFetchedInputsImplementation
  {
    public static InformationInput GetTarget_InformationInput(IContainerOwner owner, string informationInputId)
    {
      return InformationInput.RetrieveFromDefaultLocation(informationInputId, owner);
    }

    public static void ExecuteMethod_VerifyValidInput(InformationInput informationInput)
    {
      if (informationInput.IsValidatedAndActive == false)
        throw new SecurityException("InformationInput is not active");
    }

    public static string GetTarget_InputFetchLocation(InformationInput informationInput)
    {
      return informationInput.RelativeLocation + "_Input";
    }

    public static ProcessFetchedInputs.ProcessInputFromStorageReturnValue ExecuteMethod_ProcessInputFromStorage(
      string processingOperationName, InformationInput informationInput, string inputFetchLocation)
    {
      var result = new ProcessFetchedInputs.ProcessInputFromStorageReturnValue();
      var targetBlob = StorageSupport.CurrActiveContainer.GetBlob(inputFetchLocation + "/bulkdump.all",
        InformationContext.Current.Owner);
      var rawData = targetBlob.DownloadText();
      var parts = processingOperationName.Split(';');
      processingOperationName = parts[0];
      var parameter = parts.Length > 1 ? parts[1] : string.Empty;
      using (var csv = new CsvReader(new StreamReader(GenerateStreamFromString(rawData)), false))
      {
        switch (processingOperationName)
        {
          case "AddNewStockCompanies":
            var collAdd = StockCompanyCollection.RetrieveFromOwnerContent(InformationContext.Current.Owner, "default");
            var reader = new JsonReader();
            dynamic jsonData = reader.Read(rawData);
            foreach (var jsonCompany in jsonData)
            {
              var jsonCompany1 = jsonCompany;
              if (collAdd.CollectionContent.Count(x => x.Symbol == jsonCompany1.Symbol) == 0)
              {
                var stockCompany = new StockCompany
                {
                  Symbol = jsonCompany1.Symbol,
                  CompanyName = jsonCompany1.Name
                };
                stockCompany.SetLocationAsOwnerContent(InformationContext.Current.Owner, stockCompany.ID);
                collAdd.CollectionContent.Add(stockCompany);
                stockCompany.StoreInformation();
              }
            }
            collAdd.StoreInformationMasterFirst(InformationContext.Current.Owner, true);
            break;
          case "UpdateStockCompanies":
            if (csv.FieldCount >= 11)
            {
              var coll = StockCompanyCollection.RetrieveFromOwnerContent(InformationContext.Current.Owner, "default");
              while (csv.ReadNextRecord())
              {
                var collectionStockCompany = coll.CollectionContent.FirstOrDefault(x => x.Symbol == csv[0]);
                if (collectionStockCompany != null)
                {
                  var stockCompany = StockCompany.RetrieveFromOwnerContent(InformationContext.Current.Owner,
                    collectionStockCompany.ID);
                  if (stockCompany != null)
                  {
                    stockCompany.CompanyName = csv[1].Trim('"');
                    stockCompany.PriceActual = ConvertToDouble(csv[2]);
                    stockCompany.PriceHigh = ConvertToDouble(csv[4]);
                    stockCompany.PriceLow = ConvertToDouble(csv[5]);
                    stockCompany.PriceOpen = ConvertToDouble(csv[3]);
                    stockCompany.Change = ConvertToDouble(csv[6]);
                    stockCompany.ChangePercent = ConvertToDouble(csv[7]);
                    stockCompany.ChangeYTD = ConvertToDouble(csv[8]);
                    stockCompany.ChangePercentYTD = ConvertToDouble(csv[9]);
                    stockCompany.Volume = ConvertToLong(csv[10]);
                    stockCompany.StoreInformation();
                  }
                }
              }
            }
            break;
          case "UpdateChart_Days":
          {
            var chartPointsDays = GetChartPointCollection(parameter, "days");
            chartPointsDays.CollectionContent.Clear();
            if (csv.FieldCount > 1)
            {
              csv.ReadNextRecord(); //ignore the header row
              while (csv.ReadNextRecord())
              {
                chartPointsDays.CollectionContent.Insert(0, new ChartPoint
                {
                  Timestamp = csv[0],
                  Value = ConvertToDouble(csv[1])
                });
              }
            }
            chartPointsDays.StoreInformation();
          }
            break;
          case "UpdateChart_Months":
            var chartPointsMonths = GetChartPointCollection(parameter, "months");
            chartPointsMonths.CollectionContent.Clear();
            if (csv.FieldCount > 1)
            {
              csv.ReadNextRecord(); //ignore the header row
              while (csv.ReadNextRecord())
              {
                chartPointsMonths.CollectionContent.Insert(0, new ChartPoint
                {
                  Timestamp = csv[0],
                  Value = ConvertToDouble(csv[1])
                });
              }
            }
            chartPointsMonths.StoreInformation();
            break;
          case "UpdateChart_Years":
            var chartPointsYears = GetChartPointCollection(parameter, "years");
            chartPointsYears.CollectionContent.Clear();
            if (csv.FieldCount > 1)
            {
              csv.ReadNextRecord(); //ignore the header row
              while (csv.ReadNextRecord())
              {
                var timestamp = csv[0];
                if (timestamp.Length > 7)
                  timestamp = timestamp.Substring(0, 7);
                chartPointsYears.CollectionContent.Insert(0, new ChartPoint
                {
                  Timestamp = timestamp,
                  Value = ConvertToDouble(csv[1])
                });
              }
            }
            chartPointsYears.StoreInformation();
            break;
          default:
            throw new NotImplementedException("The dynamic Operation Type is not yet implemented.");
        }
      }


      return result;
    }

    public static void ExecuteMethod_StoreObjects(IInformationObject[] processingResultsToStore)
    {
      if (processingResultsToStore == null)
        return;
      foreach (var iObj in processingResultsToStore)
        iObj.StoreInformation();
    }

    public static void ExecuteMethod_DeleteObjects(IInformationObject[] processingResultsToDelete)
    {
      if (processingResultsToDelete == null)
        return;
      foreach (var iObj in processingResultsToDelete)
        iObj.DeleteInformationObject();
    }

    private static Stream GenerateStreamFromString(string s)
    {
      var stream = new MemoryStream();
      var writer = new StreamWriter(stream);
      writer.Write(s);
      writer.Flush();
      stream.Position = 0;
      return stream;
    }

    private static double ConvertToDouble(string str)
    {
      if (string.IsNullOrEmpty(str))
        return 0;
      str = str.Trim(new[] {'"', '%', '$', '€', '+'});
      double value;
      double.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out value);
      return value;
    }

    private static long ConvertToLong(string str)
    {
      if (string.IsNullOrEmpty(str))
        return 0;
      str = str.Trim(new[] {'"', '%', '$', '€', '+'});
      long value;
      long.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out value);
      return value;
    }

    private static ChartPointCollection GetChartPointCollection(string companyId, string type)
    {
      var chartPoints = ChartPointCollection.RetrieveFromOwnerContent(InformationContext.Current.Owner,
        companyId + "_" + type);
      if (chartPoints == null)
      {
        chartPoints = new ChartPointCollection();
        chartPoints.SetLocationAsOwnerContent(InformationContext.Current.Owner, companyId + "_" + type);
        chartPoints.StoreInformation();
      }
      return chartPoints;
    }
  }
}