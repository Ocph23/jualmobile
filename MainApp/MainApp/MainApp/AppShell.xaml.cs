using MainApp.ViewModels;
using MainApp.Views;
using MainApp.Views.Pembelians;
using MainApp.Views.Suppliers;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace MainApp
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(ItemDetailPage), typeof(ItemDetailPage));
            Routing.RegisterRoute(nameof(NewItemPage), typeof(NewItemPage));
            Routing.RegisterRoute(nameof(PembelianPage), typeof(PembelianPage));
            Routing.RegisterRoute(nameof(SupplierPage), typeof(SupplierPage));
        }

        private async void OnMenuItemClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
