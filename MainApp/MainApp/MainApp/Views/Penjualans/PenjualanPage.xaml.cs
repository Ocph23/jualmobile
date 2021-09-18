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
            RefreshCommand = new Command( async (x)=> {
                await RefreshAction();
            }, RefreshValidate);

            this.PropertyChanged += PenjualanViewModel_PropertyChanged;

            RefreshCommand.Execute(null);

        }

        private void PenjualanViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            RefreshCommand.ChangeCanExecute();
        }

        private async Task RefreshAction()
        {
            {
                try
                {
                    IsRefreshing = true;
                    var datas = await PenjualanStore.GetItemsByDateAsync(DateStart, DateEnd);
                    Items.Clear();
                    foreach (var item in datas.OrderByDescending(x=>x.Tanggal))
                    {
                        Items.Add(item);
                    }


                    Transaksi = Items.Count;
                    TotalPenjualan = Items.Sum(x => x.Total);
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

            }
        }

        private bool RefreshValidate(object arg)
        {
            return DateStart <= DateEnd;
        
        }

        private DateTime dateStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

        public DateTime DateStart
        {
            get { return dateStart; }
            set { SetProperty(ref dateStart , value); }
        }



        private DateTime dateEnd=DateTime.Now;

        public DateTime DateEnd
        {
            get { return dateEnd; }
            set { SetProperty(ref dateEnd, value); }
        }


        private int transaksi;

        public int Transaksi
        {
            get { return transaksi; }
            set { SetProperty(ref transaksi, value); }
        }


        private double penjualan;

        public double TotalPenjualan
        {
            get { return penjualan; }
            set { SetProperty(ref penjualan, value); }
        }


    }
}