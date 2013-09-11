using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
      var collection = PortfolioCollection.RetrieveFromOwnerContent(InformationContext.Current.Owner, "default");
      var item = collection.CollectionContent.FirstOrDefault(x => x.ID == portfolio.ID);
      collection.CollectionContent.Remove(item);
      portfolio.DeleteInformationObject();
      collection.StoreInformation();
    }

    public static void ExecuteMethod_StoreObjects(Portfolio portfolio)
    {
      portfolio.StoreInformation();
    }
  }
}
