using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheBall;

namespace Titan
{
  public class SetFavouriteStatusOfStockCompanyImplementation
  {
    public static StockCompany GetTarget_StockCompany(string stockSymbol)
    {
      var stockCompanies = StockCompanyCollection.RetrieveCollectionFromOwnerContent(InformationContext.Current.Owner);
      return stockCompanies.FirstOrDefault(x => x.)
    }

    public static void ExecuteMethod_SetFavouriteStatus(bool isFavourite, StockCompany stockCompany)
    {
      throw new NotImplementedException();
    }

    public static void ExecuteMethod_StoreObjects(StockCompany stockCompany)
    {
      throw new NotImplementedException();
    }
  }
}
