using System.Linq;
using TheBall;

namespace Titan
{
  class AddStockCompanyToPortfolioImplementation
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

    public static void ExecuteMethod_AddStockCompanyToCollection(StockCompanyCollection stockCompanyCollection, StockCompany stockCompany)
    {
      if(stockCompanyCollection.CollectionContent.Count(x => x.ID == stockCompany.ID) == 0)
        stockCompanyCollection.CollectionContent.Add(stockCompany);
    }

    public static void ExecuteMethod_StoreObjects(StockCompanyCollection stockCompanyCollection)
    {
      stockCompanyCollection.StoreInformation();
    }
  }
}
