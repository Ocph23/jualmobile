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
    public partial class AddPembelianPage : ContentPage
    {
        public AddPembelianPage()
        {
            InitializeComponent();
            this.BindingContext = new AddPembelianViewModel();
        }

        public AddPembelianPage(Pembelian model)
        {
            InitializeComponent();
            this.BindingContext = new AddPembelianViewModel(model);
        }
    }


    public class AddPembelianViewModel : BaseViewModel
    {
        public ObservableCollection<PembelianItem> Items { get; set; } = new ObservableCollection<PembelianItem>();
        public ObservableCollection<Supplier> Suppliers{ get; set; } = new ObservableCollection<Supplier>();
        public Pembelian Model { get; set; } = new Pembelian() { Items = new List<PembelianItem>(), Tanggal=DateTime.Now };
        public Command SearchCommand { get; private set; }
        public Command SaveCommand { get; private set; }

        public AddPembelianViewModel()
        {
            Title = "Buat Pembelian";
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

            SearchCommand = new Command(async () => {


                if (Model.SupplierId <= 0)
                {
                    await Helper.ErrorMessage("Anda Belum Memilih Supplier !");
                    return;
                }

                var page = new Helpers.CariBarangDialog(Model.SupplierId);
                await AppShell.Current.Navigation.PushModalAsync(page);
                var vm = page.BindingContext as Helpers.CariBarangDialogViewModel; ;
                vm.onFoundItem += Vm_onFoundItem;
            });

            SaveCommand = new Command( async () =>
            {
                try
                {
                    if (Model.Supplier == null && Model.SupplierId <= 0)
                        throw new SystemException("Supplier Harus Diisi !");

                    Model.Items = Items.ToList();
                    if(Model.Items==null ||Model.Items.Count<=0)
                        throw new SystemException("Anda belum memilih barang  !");

                    if (Model.Id <= 0)
                    {
                       var data =  await PembelianStore.AddItemAsync(Model);
                        Model.Id = data;
                    }
                    else
                    {
                        await PembelianStore.UpdateItemAsync(Model);
                    }

                    await Helper.InfoMessage("Data Berhasil Disimpan !");
                }
                catch (Exception ex)
                {
                 await Helper.ErrorMessage(ex.Message);
                }

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
                if (barang.Satuans == null || barang.Satuans.Count<=0)
                {
                    barang.Satuans = await BarangStore.GetSatuans(barang.Id);
                    if (barang.Satuans == null || barang.Satuans.Count <= 0)
                        throw new SystemException("Barang Belum Memiliki Satuan !");
                }
                Items.Add(new PembelianItem { Barang = barang, Jumlah=1, Satuan = barang.Satuans.First(), BarangId = barang.Id, Harga = barang.Satuans.First().HargaBeli }); ;
                await Helper.InfoMessage("Barang Ditambahkan !");
            }
            catch (Exception ex)
            {
               await Helper.ErrorMessage(ex.Message);
            }
        }

        public AddPembelianViewModel(Pembelian model)
        {

            Title = "Edit Pembelian";
            this.Model = model;

            foreach (var item in model.Items)
            {
                Items.Add(item);
            }

            Load();
        }
    }
}