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



        }
    }
}