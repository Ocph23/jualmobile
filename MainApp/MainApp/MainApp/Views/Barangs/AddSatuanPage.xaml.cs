using MainApp.Models;
using MainApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MainApp.Views.Barangs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddSatuanPage : ContentPage
    {
        public AddSatuanPage(Barang barang)
        {
            InitializeComponent();
            this.BindingContext = new AddSatuanViewModel(barang);
        }
        public AddSatuanPage(Barang barang, Satuan model)
        {
            InitializeComponent();
            this.BindingContext = new AddSatuanViewModel(barang, model);
        }
    }


    public delegate void AddDelegate(object data);
    public class AddSatuanViewModel : BaseViewModel
    {

        public event AddDelegate OnAdd;
        public event AddDelegate OnUpdate;
        public Satuan Model { get; }
        public Command ShowSatuanCommand { get; private set; }
        public Command SaveCommand { get; private set; }
        public Barang Barang { get; }

        public AddSatuanViewModel(Barang barang)
        {
            Barang = barang;
            Title = "Satuan Baru";
            Model = new Satuan() {Level = barang.Satuans==null?0:barang.Satuans.Count, BarangId=barang.Id, Quantity=1 };
            Load();
        }

        public AddSatuanViewModel(Barang barang, Satuan model)
        {
            Barang = barang;
            Title = "Edit Satuan";
            Model = model;
            Load();
        }


        void Load()
        {


            SaveCommand = new Command(async () =>
            {

                try
                {
                    if (string.IsNullOrEmpty(Model.Nama))
                    {
                        throw new SystemException("Nama Tidak Boleh Kosong !");
                    }

                    var saved = false;
                    if (Model.Id <= 0)
                    {
                        var result = await BarangStore.AddSatuan(Barang.Id, Model);
                        if (result !=null)
                        { saved = true;
                            OnAdd?.Invoke(Model);
                        }
                    }
                    else
                    {
                        var result = await BarangStore.EditSatuan(Model);
                        if (result)
                        {
                            saved = true;
                            OnUpdate?.Invoke(Model);
                        }
                    }

                    if (!saved)
                    {
                        throw new SystemException("Data Tidak Tersimpan !");
                    }
                    else
                    {
                        await Helper.InfoMessage("Data Berhasil Disimpan !");
                    }
                }
                catch (Exception ex)
                {
                    await Helper.ErrorMessage(ex.Message);
                }

            });
        }
    }
}