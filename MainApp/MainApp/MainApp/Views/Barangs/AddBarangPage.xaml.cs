using MainApp.Models;
using MainApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
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
        private Barang barang;

        public Barang Model
        {
            get { return barang; }
            set { SetProperty(ref barang , value); }
        }
        public ObservableCollection<Supplier> Suppliers { get; set; } = new ObservableCollection<Supplier>();
        public Command ShowSatuanCommand { get; private set; }
        public Command ScanCommand { get; private set; }
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
          
            Barcode = model.Barcode;
            if (!string.IsNullOrEmpty(model.Photo))
            {
                ImagePreview = ImageSource.FromFile(model.Photo);
            }
            Load();
            Model = model;
        }

        void Load()
        {
            FolderCommand = new Command(FolderAction);
            CameraCommand = new Command(CameraAction);

            var suppliers = SupplierStore.GetItemsAsync().Result;

            Suppliers.Clear();
            foreach (var item in suppliers)
            {
                Suppliers.Add(item);
            }


            ShowSatuanCommand = new Command(async () => {
                var page = new SatuanPage(Model);
               await AppShell.Current.Navigation.PushAsync(page);
                var vm = page.BindingContext as SatuanViewModel;
            });
            ScanCommand = new Command(() => {

                Model.Keterangan = "Kterangan";
                var page = new BarcodeScanner();
                AppShell.Current.Navigation.PushModalAsync(page);
                var vm = page.BindingContext as BarcodeScannerViewModel;
                vm.OnResultScanHandler += Vm_OnResultScanHandler1;
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
                        ShowSatuan = true;
                        await Helper.InfoMessage("Data Berhasil Disimpan !");

                    }
                }
                catch (Exception ex)
                {
                    await Helper.ErrorMessage(ex.Message);
                }

            });
        }

        private void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
           
        }

        private Task Vm_OnResultScanHandler1(object obj)
        {
            Barcode = obj.ToString();
            Model.Barcode = Barcode;
            return Task.CompletedTask;
        }

        private bool showSatuan;

        public bool ShowSatuan
        {
            get { return Model.Id > 0 ? true:false; }
            set { SetProperty(ref showSatuan , value); }
        }


        private string barCode;

        public string Barcode
        {
            get { return barCode; }
            set { SetProperty(ref barCode, value); }
        }



        private ImageSource  imagePreview;

        public ImageSource ImagePreview
        {
            get { return imagePreview; }
            set {SetProperty(ref imagePreview , value); }
        }


        private async void CameraAction(object obj)
        {
            var options = new MediaPickerOptions
            {
                Title = "Pilih File"             
            };
            var file = await MediaPicker.CapturePhotoAsync(options);
            if (file != null)
            {
                Model.Photo = file.FullPath;
                await RefreshPreview(file);
            }

        }
        private async void FolderAction(object obj)
        {
            var options = new MediaPickerOptions
            {
                Title = "Pilih File"
            };

            var file = await MediaPicker.PickPhotoAsync(options);
            if (file != null)
            {
                Model.Photo = file.FullPath;
                await RefreshPreview(file);
            }
        }


        Task RefreshPreview(FileResult result)
        {
            if (result != null)
            {
                ImagePreview = ImageSource.FromStream(x => result.OpenReadAsync());
            }
            return Task.CompletedTask;
        }

        public Command FolderCommand { get; set; }
        public Command CameraCommand { get; set; }

    }
}