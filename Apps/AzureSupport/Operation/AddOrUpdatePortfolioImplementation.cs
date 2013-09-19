using System.Collections.Generic;
using TheBall;

namespace Titan
{
  public class AddOrUpdatePortfolioImplementation
  {
    public static Portfolio GetTarget_Portfolio(string id)
    {
      if (string.IsNullOrEmpty(id))
      {
        var portfolio = new Portfolio();
        portfolio.SetLocationAsOwnerContent(InformationContext.Current.Owner, portfolio.ID);
        portfolio.StockCompanies = new StockCompanyCollection();
        var collection = PortfolioCollection.RetrieveFromOwnerContent(InformationContext.Current.Owner, "default");
        collection.CollectionContent.Add(portfolio);
        collection.StoreInformationMasterFirst(InformationContext.Current.Owner, true);
        return portfolio;
      }
      return Portfolio.RetrieveFromOwnerContent(InformationContext.Current.Owner, id);
    }

    public static void ExecuteMethod_SetNameOfPortfolio(string name, Portfolio portfolio)
    {
      portfolio.PortfolioName = name;
    }

    public static void ExecuteMethod_StoreObjects(Portfolio portfolio)
    {
      portfolio.StoreInformationMasterFirst(InformationContext.Current.Owner, true);
    }
  }
}