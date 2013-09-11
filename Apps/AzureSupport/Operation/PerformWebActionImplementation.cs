using System;
using System.Collections.Specialized;
using TheBall.CORE;
using Titan;

namespace AaltoGlobalImpact.OIP
{
  public static class PerformWebActionImplementation
  {
    public static bool ExecuteMethod_ExecuteActualOperation(string targetObjectID, string commandName,
      IContainerOwner owner, InformationSourceCollection informationSources, string[] formSourceNames,
      NameValueCollection formSubmitContent)
    {
      switch (commandName)
      {
        case "ChangeIsFavouriteStatus":
          bool isFavourite;
          bool.TryParse(formSubmitContent["IsFavourite"] ?? string.Empty, out isFavourite);
          return CallSetFavouriteStatus(formSubmitContent["ID"], isFavourite);
        default:
          throw new NotImplementedException("Operation mapping for command not implemented: " + commandName);
      }
    }

    private static bool CallSetFavouriteStatus(string id, bool isFavourite)
    {
      SetFavouriteStatusOfStockCompany.Execute(new SetFavouriteStatusOfStockCompanyParameters
      {
        Id = id,
        IsFavourite = isFavourite
      });
      return false;
    }
    
    public static PerformWebActionReturnValue Get_ReturnValue(bool executeActualOperationOutput)
    {
      return new PerformWebActionReturnValue() { RenderPageAfterOperation = executeActualOperationOutput };
    }
  }
}