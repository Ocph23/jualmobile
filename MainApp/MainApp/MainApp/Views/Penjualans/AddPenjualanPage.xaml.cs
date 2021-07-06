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
        }
    }

    public class AddPenjualanViewModel : BaseViewModel
    {

        public ObservableCollection<PenjualanItem> Items { get; set; } = new ObservableCollection<PenjualanItem>();
        public ObservableCollection<Supplier> Suppliers { get; set; } = new ObservableCollection<Supplier>();

        public Penjualan Model { get; set; } = new Penjualan() { Items = new List<PenjualanItem>() };
        public Command SearchCommand { get; private set; }
        public Command SaveCommand { get; private set; }

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

            SearchCommand = new Command(() => {
                var page = new Helpers.CariBarangDialog();
                AppShell.Current.Navigation.PushModalAsync(page);
                var vm = page.BindingContext as Helpers.CariBarangDialogViewModel; ;
                vm.onFoundItem += Vm_onFoundItem;
            });

            SaveCommand = new Command(async () =>
            {
                Model.Items = Items.ToList();

              await  PenjualanStore.AddItemAsync(Model);
            });
        }

        private async Task Vm_onFoundItem(Barang barang)
        {
            try
            {

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
                Items.Add(new PenjualanItem { Barang = barang, Jumlah = 1, Satuan = barang.Satuans.First(), BarangId = barang.Id, Harga = barang.Satuans.First().HargaBeli }); ;
            }
            catch (Exception ex)
            {
                await Helper.ErrorMessage(ex.Message);
            }
        }

        public AddPenjualanViewModel(Penjualan model)
        {
            this.Model = model;
        }
    }
}