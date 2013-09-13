using System.Linq;
using TheBall;

namespace Titan
{
  class RemovePortfolioImplementation
  {
    public static Portfolio GetTarget_Portfolio(string id)
    {
      return Portfolio.RetrieveFromOwnerContent(InformationContext.Current.Owner, id);
    }

    public static void ExecuteMethod_RemovePortfolio(Portfolio portfolio)
    {
      if (portfolio == null)
        return;
      var collection = PortfolioCollection.RetrieveFromOwnerContent(InformationContext.Current.Owner, "default");
      var item = collection.CollectionContent.FirstOrDefault(x => x.ID == portfolio.ID);
      collection.CollectionContent.Remove(item);
      portfolio.DeleteInformationObject();
      collection.StoreInformation();
      var companyCollection= StockCompanyCollection.RetrieveFromOwnerContent(InformationContext.Current.Owner,
        "Portfolios/" + portfolio.ID);
      companyCollection.DeleteInformationObject();
    }
  }
}
