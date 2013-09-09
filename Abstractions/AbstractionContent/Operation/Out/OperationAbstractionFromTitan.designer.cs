 

using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

		namespace Titan { 
				public class SetFavouriteStatusOfStockCompanyParameters 
		{
				public string StockSymbol ;
				public bool IsFavourite ;
				}
		
		public class SetFavouriteStatusOfStockCompany 
		{
				private static void PrepareParameters(SetFavouriteStatusOfStockCompanyParameters parameters)
		{
					}
				public static void Execute(SetFavouriteStatusOfStockCompanyParameters parameters)
		{
						PrepareParameters(parameters);
					StockCompany StockCompany = SetFavouriteStatusOfStockCompanyImplementation.GetTarget_StockCompany(parameters.StockSymbol);	
				SetFavouriteStatusOfStockCompanyImplementation.ExecuteMethod_SetFavouriteStatus(parameters.IsFavourite, StockCompany);		
				SetFavouriteStatusOfStockCompanyImplementation.ExecuteMethod_StoreObjects(StockCompany);		
				}
				}
		 } 