﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheBall;

namespace Titan
{
  public class SetFavouriteStatusOfStockCompanyImplementation
  {
    public static StockCompany GetTarget_StockCompany(string id)
    {
      return StockCompany.RetrieveFromOwnerContent(InformationContext.Current.Owner, id);
    }

    public static void ExecuteMethod_SetFavouriteStatus(bool isFavourite, StockCompany stockCompany)
    {
      stockCompany.IsFavourite = isFavourite;
    }

    public static void ExecuteMethod_StoreObjects(StockCompany stockCompany)
    {
      stockCompany.StoreInformation();
    }
  }
}
