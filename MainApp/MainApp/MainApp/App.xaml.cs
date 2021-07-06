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
            DependencyService.Register<MockDataStore>();
            DependencyService.Register<SupplierStore>();
            DependencyService.Register<BarangStore>();
            DependencyService.Register<PembelianStore>();
            DependencyService.Register<PenjualanStore>();
            DependencyService.Register<SupplierStore>();


            MessagingCenter.Subscribe<Page, MessageData>( this, "message", async (sender, arg) =>
            {
                await MainPage.DisplayAlert(arg.Title, arg.Message, arg.OK, arg.Cancel);
            });

            MainPage = new AppShell();
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
