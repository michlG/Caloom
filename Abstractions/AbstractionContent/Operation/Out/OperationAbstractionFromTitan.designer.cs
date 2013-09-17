 

using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

		namespace Titan { 
				public class SetFavouriteStatusOfStockCompanyParameters 
		{
				public string Id ;
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
					StockCompany StockCompany = SetFavouriteStatusOfStockCompanyImplementation.GetTarget_StockCompany(parameters.Id);	
				SetFavouriteStatusOfStockCompanyImplementation.ExecuteMethod_SetFavouriteStatus(parameters.IsFavourite, StockCompany);		
				SetFavouriteStatusOfStockCompanyImplementation.ExecuteMethod_StoreObjects(StockCompany);		
				}
				}
				public class SetAlarmPriceOfStockCompanyParameters 
		{
				public string Id ;
				public double AlarmPrice ;
				}
		
		public class SetAlarmPriceOfStockCompany 
		{
				private static void PrepareParameters(SetAlarmPriceOfStockCompanyParameters parameters)
		{
					}
				public static void Execute(SetAlarmPriceOfStockCompanyParameters parameters)
		{
						PrepareParameters(parameters);
					StockCompany StockCompany = SetAlarmPriceOfStockCompanyImplementation.GetTarget_StockCompany(parameters.Id);	
				SetAlarmPriceOfStockCompanyImplementation.ExecuteMethod_SetAlarmPrice(parameters.AlarmPrice, StockCompany);		
				SetAlarmPriceOfStockCompanyImplementation.ExecuteMethod_StoreObjects(StockCompany);		
				}
				}
				public class AddOrUpdatePortfolioParameters 
		{
				public string Id ;
				public string Name ;
				}
		
		public class AddOrUpdatePortfolio 
		{
				private static void PrepareParameters(AddOrUpdatePortfolioParameters parameters)
		{
					}
				public static void Execute(AddOrUpdatePortfolioParameters parameters)
		{
						PrepareParameters(parameters);
					Portfolio Portfolio = AddOrUpdatePortfolioImplementation.GetTarget_Portfolio(parameters.Id);	
				AddOrUpdatePortfolioImplementation.ExecuteMethod_SetNameOfPortfolio(parameters.Name, Portfolio);		
				AddOrUpdatePortfolioImplementation.ExecuteMethod_StoreObjects(Portfolio);		
				}
				}
				public class RemovePortfolioParameters 
		{
				public string Id ;
				}
		
		public class RemovePortfolio 
		{
				private static void PrepareParameters(RemovePortfolioParameters parameters)
		{
					}
				public static void Execute(RemovePortfolioParameters parameters)
		{
						PrepareParameters(parameters);
					Portfolio Portfolio = RemovePortfolioImplementation.GetTarget_Portfolio(parameters.Id);	
				RemovePortfolioImplementation.ExecuteMethod_RemovePortfolio(Portfolio);		
				}
				}
				public class AddStockCompanyToPortfolioParameters 
		{
				public string PortfolioId ;
				public string StockCompanyId ;
				}
		
		public class AddStockCompanyToPortfolio 
		{
				private static void PrepareParameters(AddStockCompanyToPortfolioParameters parameters)
		{
					}
				public static void Execute(AddStockCompanyToPortfolioParameters parameters)
		{
						PrepareParameters(parameters);
					Portfolio Portfolio = AddStockCompanyToPortfolioImplementation.GetTarget_Portfolio(parameters.PortfolioId);	
				StockCompany StockCompany = AddStockCompanyToPortfolioImplementation.GetTarget_StockCompany(parameters.StockCompanyId);	
				AddStockCompanyToPortfolioImplementation.ExecuteMethod_AddStockCompanyToPortfolio(Portfolio, StockCompany);		
				AddStockCompanyToPortfolioImplementation.ExecuteMethod_StoreObjects(Portfolio);		
				}
				}
				public class RemoveStockCompanyFromPortfolioParameters 
		{
				public string PortfolioId ;
				public string StockCompanyId ;
				}
		
		public class RemoveStockCompanyFromPortfolio 
		{
				private static void PrepareParameters(RemoveStockCompanyFromPortfolioParameters parameters)
		{
					}
				public static void Execute(RemoveStockCompanyFromPortfolioParameters parameters)
		{
						PrepareParameters(parameters);
					Portfolio Portfolio = RemoveStockCompanyFromPortfolioImplementation.GetTarget_Portfolio(parameters.PortfolioId);	
				StockCompany StockCompany = RemoveStockCompanyFromPortfolioImplementation.GetTarget_StockCompany(parameters.StockCompanyId);	
				RemoveStockCompanyFromPortfolioImplementation.ExecuteMethod_RemoveStockCompanyFromPortfolio(Portfolio, StockCompany);		
				RemoveStockCompanyFromPortfolioImplementation.ExecuteMethod_StoreObjects(Portfolio);		
				}
				}
		 } 