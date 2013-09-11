// ===================================
// <copyright>Copyright © 2013 All Right Reserved</copyright>
// <author>Gurschler Michael</author>
// <creationDate>10.09.2013</creationDate>
// <email>mailto:info@gurschlermichael.com</email>
// <internet>http://www.gurschlermichael.com</internet>
// ===================================

using System.IO;
using System.Runtime.Serialization.Json;
using AaltoGlobalImpact.OIP;

namespace Titan
{
  partial class StockCompanyCollection : IAdditionalFormatProvider
  {
    AdditionalFormatContent[] IAdditionalFormatProvider.GetAdditionalContentToStore()
    {
      var serializer = new DataContractJsonSerializer(GetType());
      byte[] dataContent;
      using (var memoryStream = new MemoryStream())
      {
        serializer.WriteObject(memoryStream, this);
        dataContent = memoryStream.ToArray();
      }

      return new[]
      {
        new AdditionalFormatContent {Extension = "json", Content = dataContent}
      };
    }

    string[] IAdditionalFormatProvider.GetAdditionalFormatExtensions()
    {
      return new[] { "json" };
    }
  }
}