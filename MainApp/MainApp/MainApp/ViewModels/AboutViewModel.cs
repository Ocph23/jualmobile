using MainApp.Models;
using System;
using System.Diagnostics;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MainApp.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        public AboutViewModel()
        {
            Title = "Welcome";
            OpenWebCommand = new Command( () => {
                UserName += "Test";
            });
            Load();

        }

        private async void Load()
        {
            try
            {

                /////Barang
                //var barang = new Models.Barang { Nama = "Mie Sedap", Keterangan = "Makanan Apakek" };
                //var saved = await BarangStore.AddItemAsync(barang);

                //var results = await BarangStore.GetItemAsync(1);
                //var satuan = new Models.Satuan { BarangId = 1, Level = 1, Quantity = 24, HargaBeli = 10000, HargaJual = 15000, Nama = "Karton" };
                //if (results != null)
                //{
                //    var satuans = await BarangStore.AddSatuan(results.Id, satuan);
                //}

                ////Pembelian

                //var pembelian = new Pembelian() { Tanggal = DateTime.Now, Items = new System.Collections.Generic.List<PembelianItem>() { 
                //    new PembelianItem{ BarangId=1, Harga=20500, Jumlah=2, SatuanId=1 }
                //} };
                //var data = await PembelianStore.AddItemAsync(pembelian);



                var pembelian = await PembelianStore.GetItemsAsync();

            }
            catch (Exception ex)
            {

                Debug.WriteLine(ex.Message);
            }

        }

        public ICommand OpenWebCommand { get; }

        private string userName;

        public string UserName
        {
            get { return userName; }
            set { SetProperty(ref userName , value); }
        }


    }
}