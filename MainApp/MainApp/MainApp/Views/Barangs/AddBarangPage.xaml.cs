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
    public partial class AddBarangPage : ContentPage
    {
        public AddBarangPage()
        {
            InitializeComponent();
            this.BindingContext = new AddBarangViewModel();
        }

        public AddBarangPage(Models.Barang model)
        {
            InitializeComponent();
            this.BindingContext = new AddBarangViewModel(model);
        }
    }

    public class AddBarangViewModel : BaseViewModel
    {
        public Barang Model { get; }
        public Command ShowSatuanCommand { get; private set; }
        public Command SaveCommand { get; private set; }

        public AddBarangViewModel()
        {
            Title = "Barang Baru";
            Model = new Barang();
            Load();
        }

        public AddBarangViewModel(Barang model)
        {
            Title = "Edit Barang";
            Model = model;
            Load();
        }


        void Load()
        {

            ShowSatuanCommand = new Command(async () => {
                var page = new SatuanPage(Model);
               await AppShell.Current.Navigation.PushAsync(page);
                var vm = page.BindingContext as SatuanViewModel;
            });


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
                        var result = await BarangStore.AddItemAsync(Model);
                        if (result > 0)
                            saved = true;
                    }
                    else
                    {
                        var result = await BarangStore.UpdateItemAsync(Model);
                        if (result)
                            saved = true;
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


        private bool showSatuan;

        public bool ShowSatuan
        {
            get { return Model.Id > 0 ? true:false; }
            set { SetProperty(ref showSatuan , value); }
        }

    }
}