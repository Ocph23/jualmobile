using MainApp.Models;
using MainApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MainApp.Views.Pembelians
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PembelianPage : ContentPage
    {
        public PembelianPage()
        {
            InitializeComponent();
            this.BindingContext = new PembelianViewModel();
        }


        protected override void OnAppearing()
        {
            var vm = BindingContext as PembelianViewModel;
            vm.RefreshCommand.Execute(null);
            base.OnAppearing();
        }
    }

    public class PembelianViewModel:BaseViewModel
    {

        public ObservableCollection<Pembelian> Items { get; set; } = new ObservableCollection<Pembelian>();
        public Command AddCommand { get; }
        public Command RefreshCommand { get; }

        private bool isRefreshing;
        public bool IsRefreshing
        {
            get { return isRefreshing; }
            set { SetProperty(ref isRefreshing ,value); }
        }

        public PembelianViewModel()
        {
            Title = "Pembelian";
            AddCommand = new Command(() =>
            {
                var page = new AddPembelianPage();
                AppShell.Current.Navigation.PushAsync(page);
                var vm = page.BindingContext as AddPembelianViewModel;
              
            });
            RefreshCommand = new Command(async () => {
                try
                {
                    IsRefreshing = true;
                    var datas = await PembelianStore.GetItemsAsync();
                    Items.Clear();
                    foreach (var item in datas)
                    {
                        Items.Add(item);
                    }
                }
                catch (Exception ex)
                {
                    IsRefreshing = false;
                    throw new SystemException(ex.Message);
                }
                finally
                {
                    IsRefreshing = false;
                }

            });
        }
    }
}