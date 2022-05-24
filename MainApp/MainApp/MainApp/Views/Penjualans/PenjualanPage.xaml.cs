using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Spreadsheet;
using MainApp.Models;
using MainApp.Services;
using MainApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
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

            ExportCommand = new Command(async (x) => {
                await ExportAction();
            }, x=>true);


            RefreshCommand = new Command( async (x)=> {
                await RefreshAction();
            }, RefreshValidate);





            this.PropertyChanged += PenjualanViewModel_PropertyChanged;

            RefreshCommand.Execute(null);

        }
       
        

        private async Task ExportAction()
        {
           
            var  excelService = new ExcelService();
            var fileName = $"Penjualan-{Guid.NewGuid()}.xlsx";
            string filepath = excelService.GenerateExcel(fileName);

            var data = new ExcelStructure
            {
                From = DateStart, To = DateEnd,
                Values = Items.ToList(),
                Headers = new List<HeaderCell>() { 
                    new HeaderCell { Title="Tanggal" }, 
                    new HeaderCell { Title = "Kode" }, 
                    new HeaderCell { Title = "Barang" }, 
                    new HeaderCell { Title = "Jumlah" }, 
                    new HeaderCell { Title = "Satuan" }, 
                    new HeaderCell { Title = "Harga" }, 
                    new HeaderCell { Title = "Total" } }
            };

            excelService.InsertDataPenjualanIntoSheet(filepath, "Penjualan", data);

            await Launcher.OpenAsync(new OpenFileRequest()
            {
                File = new ReadOnlyFile(filepath)
            });



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

                    if (Items.Count > 0)
                        ShowExport = true;
                    else
                        ShowExport = false;


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

        private bool showExport;

        public bool ShowExport
        {
            get { return showExport; }
            set { SetProperty(ref showExport , value); }
        }



        private Command exportCommand;

        public Command ExportCommand
        {
            get { return exportCommand; }
            set { SetProperty(ref exportCommand , value); }
        }



        private bool RefreshValidate(object arg)
        {
            return DateStart <= DateEnd;
        
        }

        private DateTime dateStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

        public DateTime DateStart
        {
            get { return dateStart; }
            set { SetProperty(ref dateStart , value); }
        }



        private DateTime dateEnd=DateTime.Now.AddHours(1);

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