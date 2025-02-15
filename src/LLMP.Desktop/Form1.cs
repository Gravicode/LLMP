using LLMP.Desktop.Data;
using LLMP.Desktop.Services;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using WaterPositive.Kiosk.Helpers;
using Microsoft.AspNetCore.Components.WebView.WindowsForms;
namespace LLMP.Desktop
{
    public partial class Form1 : Form
    {
        System.Timers.Timer SyncTimer;
        AppState? state;
        public Form1()
        {
            InitializeComponent();
            this.Text = "LLM Playground v0.1";
            var services = new ServiceCollection();
            services.AddWindowsFormsBlazorWebView();
            //services.AddOpenAIService(settings => { settings.ApiKey = AppConstants.OpenAIKey; settings.Organization = AppConstants.OpenAIOrg; });

            services.AddSingleton<ChatService>();
            services.AddSingleton<CompletionService>();
            services.AddSingleton<SearchService>();
            services.AddSingleton<ChatDocService>();
            services.AddSingleton<ImageGeneratorService>();
            services.AddSingleton<AppState>();
            //services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            services.AddMudServices();
            services.AddSingleton<AzureBlobHelper>();
            blazorWebView1.HostPage = "wwwroot\\index.html";
            blazorWebView1.Services = services.BuildServiceProvider();
            blazorWebView1.RootComponents.Add<LLMP.Desktop.App>("#app");
            state = blazorWebView1.Services.GetService<AppState>();
            CheckInternet();
            SyncTimer = new System.Timers.Timer(10000);
            SyncTimer.Elapsed += async (a, b) =>
            {
                var res = CheckInternet();
                await Console.Out.WriteLineAsync($"internet is {(res ? "on" : "off")}");
            };
            SyncTimer.Start();
            GoFullscreen(true);
        }
        bool CheckInternet()
        {
            var res = InternetHelper.IsConnectedToInternet();
            AppConstants.InternetOK = res;
            state?.RefreshInternet(res);
            return res;
        }
        private void GoFullscreen(bool fullscreen)
        {
            if (fullscreen)
            {
                this.WindowState = FormWindowState.Normal;
                //this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                this.Bounds = Screen.PrimaryScreen.Bounds;
            }
            else
            {
                this.WindowState = FormWindowState.Maximized;
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            }
        }
    }
}
