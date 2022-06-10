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
    public partial class AddPenjualanPage : ContentPage
    {
        public AddPenjualanPage()
        {
            InitializeComponent();              
            BindingContext = new AddPenjualanViewModel();
        }
    }

    public class AddPenjualanViewModel : BaseViewModel
    {
        private List<Barang> barangs;

        public ObservableCollection<PenjualanItem> Items { get; set; } = new ObservableCollection<PenjualanItem>();
        public ObservableCollection<Supplier> Suppliers { get; set; } = new ObservableCollection<Supplier>();

        public Penjualan Model { get; set; } = new Penjualan() { Items = new List<PenjualanItem>() };
        public Command SearchCommand { get; private set; }
        public Command DeleteItemCommand { get; private set; }
        public Command BayarCommand { get; private set; }
        public Command ScanCommand { get; private set; }
        public Command SaveCommand { get; private set; }

        private double grandTotal;

        public double GrandTotal
        {
            get { return grandTotal; }
            set { SetProperty(ref grandTotal , value); }
        }



        public AddPenjualanViewModel()
        {
            Title = "Buat Penjualan";
            Load();
        }

        private void Load()
        {
            var suppliers = SupplierStore.GetItemsAsync().Result;

            Suppliers.Clear();
            foreach (var item in suppliers)
            {
                Suppliers.Add(item);
            }

            SearchCommand = new Command(() =>
            {
                var page = new Helpers.CariBarangDialog();
                AppShell.Current.Navigation.PushModalAsync(page);
                var vm = page.BindingContext as Helpers.CariBarangDialogViewModel; ;
                vm.onFoundItem += Vm_onFoundItem;
            });



            DeleteItemCommand = new Command(async (object obj)=> {
                if (SelectedItem != null)
                    Items.Remove(SelectedItem);

            });

            BayarCommand = new Command(() => {
                Model.Items = Items.ToList();
                var page = new BayarPage();
                page.BindingContext = new BayarViewModel(Model);
                AppShell.Current.Navigation.PushModalAsync(page);
            });

            ScanCommand = new Command(() =>
            {
                var page = new BarcodePenjualanScanner();
                AppShell.Current.Navigation.PushModalAsync(page);
                var vm = page.BindingContext as BarcodePenjualanScannerViewModel;
                vm.OnResultScanHandler += Vm_OnResultScanHandler1;
            });

            SaveCommand = new Command(async () =>
            {
                try
                {

                    Model.Items = Items.ToList();
                    if (Model.Items == null || Model.Items.Count <= 0)
                        throw new SystemException("Anda belum memilih barang  !");

                    if(Model.Id <= 0)
                    {
                        await PenjualanStore.AddItemAsync(Model);
                    }
                    else
                    {
                        await PenjualanStore.UpdateItemAsync(Model);
                    }


                    await Helper.InfoMessage("Data Berhasil Disimpan !");
                    await AppShell.Current.Navigation.PopAsync();
                }
                catch (Exception ex)
                {
                    await Helper.ErrorMessage(ex.Message);
                }

            });
        }


        private PenjualanItem selectedItem;

        public PenjualanItem SelectedItem
        {
            get { return selectedItem; }
            set {SetProperty(ref selectedItem , value); }
        }



        private async Task Vm_OnResultScanHandler1(object obj)
        {
            try
            {
                var barang = (Barang)obj;
                if (barang == null)
                        throw new SystemException("Barang Tidak Ditemukan !");

                if (barang.Stock <= 0)
                    throw new SystemException("Stock Barang Tidak Ada !");


                var itemPenjualan = Items.Where(x => x.Barang.Id== barang.Id).FirstOrDefault();
                if (itemPenjualan == null)
                {

                    Satuan satuan=null;
                    if(barang.Satuans!=null)
                        satuan = barang.Satuans.FirstOrDefault();
                    if (satuan == null)
                    {
                        throw new SystemException("Satuan Belum Ada !");
                    }
                    var item = new PenjualanItem
                    {
                        Barang = barang,
                        BarangId = barang.Id,
                        Harga = satuan == null ? 0 : satuan.HargaJual,
                        Satuan = satuan,
                        Jumlah = 1,
                        SatuanId = satuan.Id,
                    };

                    Items.Add(item);




                   await Helper.LongToas("Barang Berhasil Ditambahkan !");
                }
                else
                {
                    itemPenjualan.Jumlah += 1;
                    await Helper.LongToas($"Barang {itemPenjualan.Barang.Nama} Bertambah 1 !");
                }


                GrandTotal = Items.Sum(x=>x.Total);

            }
            catch (Exception ex)
            {
                await Helper.LongToas(ex.Message);
            }

        }

       

        private async Task Vm_onFoundItem(Barang barang)
        {
            try
            {
                if (barang.Stock <= 0)
                    throw new SystemException("Stock Barang Tidak Ada !");


                var item = Items.Where(x => x.BarangId == barang.Id).FirstOrDefault();

                if (item != null)
                {
                    item.Jumlah += 1;
                    return;
                }

                if (barang.Satuans == null)
                {
                    barang.Satuans = await BarangStore.GetSatuans(barang.Id);
                    if (barang.Satuans == null || barang.Satuans.Count <= 0)
                        throw new SystemException("Barang Belum Memiliki Satuan !");
                }
                var itemData = new PenjualanItem { Barang = barang, Jumlah = 1, Satuan = barang.Satuans.First(), BarangId = barang.Id, Harga = barang.Satuans.First().HargaBeli };
                itemData.PropertyChanged += ItemData_PropertyChanged;
                Items.Add(itemData) ;

                GrandTotal = Items.Sum(x => x.Total);
            }
            catch (Exception ex)
            {
                await Helper.ErrorMessage(ex.Message);
            }
        }

        private void ItemData_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var item = (PenjualanItem)sender;
            if (item != null)
            {
                item.Total = item.Jumlah * item.Harga;
            }   

             GrandTotal = Items.Sum(x => x.Total);
        }

        public AddPenjualanViewModel(Penjualan model)
        {
            this.Model = model;
        }
    }
}