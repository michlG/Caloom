
using System.Linq;
using TheBall;

namespace Titan
{
  public class RemoveStockCompanyFromPortfolioImplementation
  {
    public static Portfolio GetTarget_Portfolio(string portfolioId)
    {
      return Portfolio.RetrieveFromOwnerContent(InformationContext.Current.Owner, portfolioId);
    }

    public static StockCompany GetTarget_StockCompany(string stockCompanyId)
    {
      return StockCompany.RetrieveFromOwnerContent(InformationContext.Current.Owner, stockCompanyId);
    }

    public static void ExecuteMethod_RemoveStockCompanyFromPortfolio(Portfolio portfolio, StockCompany stockCompany)
    {
      var item = portfolio.StockCompanies.CollectionContent.FirstOrDefault(x => x.ID == stockCompany.ID);
      if (item != null)
        portfolio.StockCompanies.CollectionContent.Remove(item);
    }

    public static void ExecuteMethod_StoreObjects(Portfolio portfolio)
    {
      portfolio.StoreInformation();
    }
  }
}