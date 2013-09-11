using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheBall;

namespace Titan
{
  public class SetAlarmPriceOfStockCompanyImplementation
  {
    public static StockCompany GetTarget_StockCompany(string id)
    {
      return StockCompany.RetrieveFromOwnerContent(InformationContext.Current.Owner, id);
    }

    public static void ExecuteMethod_SetAlarmPrice(double alarmPrice, StockCompany stockCompany)
    {
      stockCompany.PriceAlarm =alarmPrice;
    }

    public static void ExecuteMethod_StoreObjects(StockCompany stockCompany)
    {
      stockCompany.StoreInformation();
    }
  }
}
