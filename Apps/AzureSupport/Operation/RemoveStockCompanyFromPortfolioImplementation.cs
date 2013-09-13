
using System.Linq;
using TheBall;

namespace Titan
{
  public class RemoveStockCompanyFromPortfolioImplementation
  {
    public static StockCompanyCollection GetTarget_StockCompanyCollection(string portfolioId)
    {
      return StockCompanyCollection.RetrieveFromOwnerContent(InformationContext.Current.Owner,
        "Portfolios/" + portfolioId);
    }

    public static StockCompany GetTarget_StockCompany(string stockCompanyId)
    {
      return StockCompany.RetrieveFromOwnerContent(InformationContext.Current.Owner, stockCompanyId);
    }

    public static void ExecuteMethod_RemoveStockCompanyFromCollection(StockCompanyCollection stockCompanyCollection, StockCompany stockCompany)
    {
      var item = stockCompanyCollection.CollectionContent.FirstOrDefault(x => x.ID == stockCompany.ID);
      if (item != null)
        stockCompanyCollection.CollectionContent.Remove(item);
    }

    public static void ExecuteMethod_StoreObjects(StockCompanyCollection stockCompanyCollection)
    {
      stockCompanyCollection.StoreInformation();
    }
  }
}