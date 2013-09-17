using System;
using System.Diagnostics;
using System.Linq;
using TheBall;

namespace Titan
{
  internal class CollectionUpdateImplementation
  {
    public static void Update_Portfolio_StockCompanies(Portfolio portfolio, StockCompanyCollection localCollection, StockCompanyCollection masterCollection)
    {
      if (portfolio == null || string.IsNullOrEmpty(portfolio.ID))
        return;
      Console.WriteLine("UPDATE " + portfolio.ID);
      foreach (var newItem in masterCollection.CollectionContent)
      {
        var oldItem = localCollection.CollectionContent.FirstOrDefault(x => x.ID == newItem.ID);
        if (oldItem != null)
        {
          Debug.Write("REPLACING " + oldItem + "; ");
          localCollection.CollectionContent.Insert(localCollection.CollectionContent.IndexOf(oldItem), newItem);
          localCollection.CollectionContent.Remove(oldItem);
        }
      }
      Debug.WriteLine("FINISHED");
    }
  }
}