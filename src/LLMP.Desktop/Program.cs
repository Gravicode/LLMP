using LLMP.Desktop.Data;
using System.Configuration;

namespace LLMP.Desktop
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Setup();
            Application.Run(new Form1());
        }

        static void Setup()
        {
            AppConstants.OpenAIKey = ConfigurationManager.AppSettings["OpenAIKey"];
            AppConstants.OpenAIOrg = ConfigurationManager.AppSettings["OpenAIOrg"];
            AppConstants.PalmKey = ConfigurationManager.AppSettings["PalmKey"];

            AppConstants.BlobPrefix = ConfigurationManager.AppSettings["BlobPrefix"];
            AppConstants.BlobConn = ConfigurationManager.AppSettings["BlobConn"];
            AppConstants.ContainerName = ConfigurationManager.AppSettings["ContainerName"];
            AppConstants.GoogleSearchKey = ConfigurationManager.AppSettings["GoogleSearchKey"];
            AppConstants.GoogleSearchID = ConfigurationManager.AppSettings["GoogleSearchID"];
            AppConstants.AzureSearchApiKey = ConfigurationManager.AppSettings["AzureSearchApiKey"];
            AppConstants.AzureSearchEndpoint = ConfigurationManager.AppSettings["AzureSearchEndpoint"];
            AppConstants.AzureFormApiKey = ConfigurationManager.AppSettings["AzureFormApiKey"];
            AppConstants.AzureFormEndpoint = ConfigurationManager.AppSettings["AzureFormEndpoint"];
            AppConstants.AzureAIKey = ConfigurationManager.AppSettings["AzureAIKey"];
            AppConstants.AzureAIEndpoint = ConfigurationManager.AppSettings["AzureAIEndpoint"];



        }
    }
}