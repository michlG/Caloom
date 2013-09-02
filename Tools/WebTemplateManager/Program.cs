﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using SecuritySupport;
using TheBall;
using TheBall.CORE;

namespace WebTemplateManager
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Running test EKE...");
                TheBallEKE.TestExecution();
                Console.WriteLine("Running test EKE complete.");
                //return;
                //SecurityNegotiationManager.EchoClient().Wait();
                SecurityNegotiationManager.EchoClient();
                Console.ReadLine(); // Enter to exit
                return;
                //return;
                if (args.Length != 4 || args[0].Length != 4)
                {
                    Console.WriteLine("Usage: WebTemplateManager.exe <-pub name/-pri name> grp<groupID>/acc<acctID> <connection string>");
                    return;
                }
                Debugger.Launch();
                string pubPriPrefixWithDash = args[0];
                string templateName = args[1];
                if(String.IsNullOrWhiteSpace(templateName))
                    throw new ArgumentException("Template name must be given");
                string connStr = args[3];
                string grpacctID = args[2];
                if(pubPriPrefixWithDash != "-pub" && pubPriPrefixWithDash != "-pri")
                    throw new ArgumentException("-pub or -pri misspelled or missing");
                string pubPriPrefix = pubPriPrefixWithDash.Substring(1);
                string ownerPrefix = grpacctID.Substring(0, 3);
                string ownerID = grpacctID.Substring(3);
                VirtualOwner owner = VirtualOwner.FigureOwner(ownerPrefix + "/" + ownerID);

                //string connStr = String.Format("DefaultEndpointsProtocol=http;AccountName=theball;AccountKey={0}",
                //                               args[0]);
                //connStr = "UseDevelopmentStorage=true";
                bool debugMode = false;

                StorageSupport.InitializeWithConnectionString(connStr, debugMode);
                InformationContext.InitializeFunctionality(3, true);
                InformationContext.Current.InitializeCloudStorageAccess(
                    Properties.Settings.Default.CurrentActiveContainerName);

                string directory = Directory.GetCurrentDirectory();
                if (directory.EndsWith("\\") == false)
                    directory = directory + "\\";
                string[] allFiles =
                    Directory.GetFiles(directory, "*", SearchOption.AllDirectories)
                             .Select(str => str.Substring(directory.Length))
                             .ToArray();
                if (pubPriPrefix == "pub" && templateName == "legacy")
                {
                    FileSystemSupport.UploadTemplateContent(allFiles, owner,
                                                            RenderWebSupport.DefaultPublicWwwTemplateLocation, true,
                                                            Preprocessor, ContentFilterer, InformationTypeResolver);
                    RenderWebSupport.RenderWebTemplate(owner.LocationPrefix, true, "AaltoGlobalImpact.OIP.Blog",
                                                       "AaltoGlobalImpact.OIP.Activity");
                }
                else
                {
                    FileSystemSupport.UploadTemplateContent(allFiles, owner, templateName, true);
                }
            }
            catch(Exception exception)
            {
                Console.WriteLine("EXCEPTION: " + exception.ToString());
            }
        }

        private static void Preprocessor(BlobStorageContent content)
        {
            if (content.FileName.EndsWith("_DefaultView.html"))
                ReplaceHtmlExtensionWithPHtml(content);
            if (content.FileName.EndsWith("oip-layout-landing.html"))
                ReplaceHtmlExtensionWithPHtml(content);
        }

        private static void ReplaceHtmlExtensionWithPHtml(BlobStorageContent content)
        {
            content.FileName = content.FileName.Substring(0, content.FileName.LastIndexOf(".html")) + ".phtml";
        }

        private static bool ContentFilterer(BlobStorageContent content)
        {
            string fileName = content.FileName;
            if (fileName.EndsWith("readme.txt"))
                return false;
            if (fileName.Contains("_DefaultView.phtml"))
            {
                bool isBlogDefaultView = fileName.EndsWith(".Blog_DefaultView.phtml");
                bool isActivityDefaultView = fileName.EndsWith(".Activity_DefaultView.phtml");
                if (isBlogDefaultView == false && isActivityDefaultView == false)
                    return false;
            }
            return true;
        }

        private static string InformationTypeResolver(BlobStorageContent content)
        {
            string webtemplatePath = content.FileName;
            string blobInformationType;
            if (webtemplatePath.EndsWith(".phtml"))
            {
                //if (webtemplatePath.Contains("/oip-viewtemplate/"))
                if (webtemplatePath.EndsWith("_DefaultView.phtml"))
                    blobInformationType = StorageSupport.InformationType_RuntimeWebTemplateValue;
                else
                    blobInformationType = StorageSupport.InformationType_WebTemplateValue;
            }
            else if (webtemplatePath.EndsWith(".html"))
            {
                string htmlContent = Encoding.UTF8.GetString(content.BinaryContent);
                bool containsMarkup = htmlContent.Contains("THEBALL-CONTEXT");
                if (containsMarkup == false)
                    blobInformationType = StorageSupport.InformationType_GenericContentValue;
                else
                {
                    blobInformationType = webtemplatePath.EndsWith("_DefaultView.html")
                                              ? StorageSupport.InformationType_RuntimeWebTemplateValue
                                              : StorageSupport.InformationType_WebTemplateValue;
                }
            }
            else
                blobInformationType = StorageSupport.InformationType_GenericContentValue;
            return blobInformationType;
        }

    }
}
