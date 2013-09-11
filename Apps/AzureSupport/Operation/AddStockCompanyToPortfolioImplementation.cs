using TheBall;

namespace Titan
{
  class AddStockCompanyToPortfolioImplementation
  {
    public static Portfolio GetTarget_Portfolio(string portfolioId)
    {
      return Portfolio.RetrieveFromOwnerContent(InformationContext.Current.Owner, portfolioId);
    }

    public static StockCompany GetTarget_StockCompany(string stockCompanyId)
    {
      return StockCompany.RetrieveFromOwnerContent(InformationContext.Current.Owner, stockCompanyId);
    }

    public static void ExecuteMethod_AddStockCompanyToPortfolio(Portfolio portfolio, StockCompany stockCompany)
    {
      portfolio.StockCompanies.CollectionContent.Add(stockCompany);
    }

    public static void ExecuteMethod_StoreObjects(Portfolio portfolio)
    {
      portfolio.StoreInformation();
    }
  }
}
