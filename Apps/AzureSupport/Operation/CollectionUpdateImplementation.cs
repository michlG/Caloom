using System.Linq;

namespace Titan
{
  internal class CollectionUpdateImplementation
  {
    public static void Update_Portfolio_StockCompanies(Portfolio portfolio, StockCompanyCollection localCollection, StockCompanyCollection masterCollection)
    {
      if (portfolio == null || string.IsNullOrEmpty(portfolio.ID))
        return;
      foreach (var newItem in masterCollection.CollectionContent)
      {
        var oldItem = localCollection.CollectionContent.FirstOrDefault(x => x.ID == newItem.ID);
        if (oldItem != null)
        {
          localCollection.CollectionContent.Insert(localCollection.CollectionContent.IndexOf(oldItem), newItem);
          localCollection.CollectionContent.Remove(oldItem);
        }
      }
    }
  }
}