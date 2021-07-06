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

namespace MainApp.Views.Penjualans
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PenjualanPage : ContentPage
    {
        public PenjualanPage()
        {
            InitializeComponent();
            BindingContext = new PenjualanViewModel();
        }
    }

    public class PenjualanViewModel : BaseViewModel
    {

        public ObservableCollection<Penjualan> Items { get; set; } = new ObservableCollection<Penjualan>();
        public Command AddCommand { get; }
        public Command RefreshCommand { get; }

        private bool isRefreshing;
        public bool IsRefreshing
        {
            get { return isRefreshing; }
            set { SetProperty(ref isRefreshing, value); }
        }

        public PenjualanViewModel()
        {
            Title = "Penjualan";
            AddCommand = new Command(() =>
            {
                var page = new AddPenjualanPage();
                AppShell.Current.Navigation.PushAsync(page);
                var vm = page.BindingContext as AddPenjualanViewModel;

            });
            RefreshCommand = new Command(async () => {
                try
                {
                    IsRefreshing = true;
                    var datas = await PenjualanStore.GetItemsAsync();
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