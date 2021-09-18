using MainApp.Services;
using MainApp.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MainApp
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();
            var instance = DatabaseIntialization.Instance.GetAwaiter();
            DependencyService.Register<DatabaseIntialization>();
            DependencyService.Register<SupplierStore>();
            DependencyService.Register<BarangStore>();
            DependencyService.Register<PembelianStore>();
            DependencyService.Register<PenjualanStore>();
            DependencyService.Register<SupplierStore>();
            MessagingCenter.Subscribe<Page, MessageData>( this, "message", async (sender, arg) =>
            {
                await MainPage.DisplayAlert(arg.Title, arg.Message, arg.OK, arg.Cancel);
            });

            var profile = Account.GetProfile().Result;
            if (profile == null)
            {
                MainPage = new  RegisterPage();
            }
            else
            {
                var user = Account.GetUser().Result;
                if(user==null)
                    MainPage = new  LoginPage();
                else    
                    MainPage = new AppShell();
            }
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
