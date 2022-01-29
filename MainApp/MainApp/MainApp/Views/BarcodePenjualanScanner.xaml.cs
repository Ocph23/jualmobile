using MainApp.Models;
using MainApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;
using ZXing;

namespace MainApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BarcodePenjualanScanner : ContentPage
    {
        public BarcodePenjualanScanner()
        {
            InitializeComponent();
            scanView.Options = new ZXing.Mobile.MobileBarcodeScanningOptions
            {
                TryHarder = true,
                PossibleFormats = new List<ZXing.BarcodeFormat>
                {
                    ZXing.BarcodeFormat.EAN_8, ZXing.BarcodeFormat.EAN_13, ZXing.BarcodeFormat.UPC_A, ZXing.BarcodeFormat.UPC_E
                },
                DelayBetweenContinuousScans = 100,
                AutoRotate = true
            };
            scanView.AutoFocus();
            BindingContext = new BarcodePenjualanScannerViewModel();
        }

        private void zXingDefaultOverlay_FlashButtonClicked(Button sender, EventArgs e)
        {
            scanView.IsTorchOn = !scanView.IsTorchOn;
        }
    }


    internal class BarcodePenjualanScannerViewModel : BaseViewModel
    {

        public event OnResultBarcode OnResultScanHandler;

        public ObservableCollection<Satuan> Satuans { get; set; } = new ObservableCollection<Satuan>();

        private Satuan satuanSelected;

        public Satuan SatuanSelected
        {
            get { return satuanSelected; }
            set { SetProperty(ref satuanSelected , value); 
                if(value != null)
                {
                    Stock = stockSource / value.Quantity;
                }
            }
        }


        private bool showScan=true;

        public bool ShowScan
        {
            get { return showScan; }
            set { SetProperty(ref showScan , value); }
        }




        private int stock;

        public int Stock
        {
            get { return stock; }
            set { SetProperty(ref stock , value); }
        }





        public BarcodePenjualanScannerViewModel()
        {
            ScanningCommand = new Command(ScanningAction, x => IsScanning);
            ScanAgainCommand = new Command(() => { IsScanning = true; SatuanSelected = null; Stock = 0; ScanAgain = false; TextResult = string.Empty; ImagePreview = null; BarangModel = null; });
            TakeCommand = new Command(() =>
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    OnResultScanHandler?.Invoke(BarangModel);
                    await Task.Delay(1000);
                    ScanAgainCommand.Execute(null);

                   
                });

            });

            CancalCommand = new Command(() =>
            {
                TextResult = string.Empty;
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await Task.Delay(1000);
                    await Application.Current.MainPage.Navigation.PopModalAsync();
                });
            });

            Task.Run(() => StartScan());
        }



        private string textResult;

        public string TextResult
        {
            get { return textResult; }
            set { SetProperty(ref textResult, value); }
        }


        private bool scanAgain;

        public bool ScanAgain
        {
            get { return scanAgain; }
            set { SetProperty(ref scanAgain, value);
            }
        }



        public async Task StartScan()
        {
            await Task.Delay(1000);
            IsScanning = true;
            SatuanSelected = null;
            Stock = 0;
        }




        private void ScanningAction(object obj)
        {
            IsScanning = false;
            ScanAgain = true;
            Device.BeginInvokeOnMainThread(async () =>
                {
                    await Task.Delay(100);
                    try
                    {
                        IsBusy = true;
                        var data = obj as Result;
                        TextResult = data.Text;
                        try
                        {
                            var barcode = TextResult;
                            if (barangs == null)
                                barangs = await BarangStore.GetItemsAsync();

                            BarangModel = barangs.Where(x => x.Barcode == barcode).FirstOrDefault();

                            if (BarangModel == null)
                            {
                                await Task.Delay(1000);
                                IsScanning = true;
                                SatuanSelected = null;
                                Stock = 0;
                                throw new SystemException("Barang Tidak Ditemukan !");
                            }

                            ShowScan = false;

                            if (!string.IsNullOrEmpty(BarangModel.Photo))
                            {
                                ImagePreview = ImageSource.FromFile(BarangModel.Photo); 
                            }

                            BarangModel.Satuans = await BarangStore.GetSatuans(BarangModel.Id);

                            Satuans.Clear();
                            var satuansx = BarangModel.Satuans;
                            foreach (var item in satuansx)
                            {
                                Satuans.Add(item);
                            }
                            stockSource = await BarangStore.GetStock(BarangModel.Id);
                            SatuanSelected = satuansx.FirstOrDefault();
                        }
                        catch (Exception ex)
                        {
                          await  Helper.LongToas(ex.Message);
                        }
                        Console.WriteLine(obj.ToString());
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    finally
                    {
                        IsBusy = false;
                       
                    }
                });

        }


        public Command ScanningCommand
        {
            get
            {
                return _scanningCommand;
            }
            set
            {
                SetProperty(ref _scanningCommand, value);
            }
        }

        public Command ScanAgainCommand { get; private set; }
        public Command TakeCommand { get; }
        public Command CancalCommand { get; }

        private bool isScaning;

        public bool IsScanning
        {
            get { return isScaning; }
            set { SetProperty(ref isScaning, value); }
        }

        public bool IsAnalyzing
        {
            get { return isAnalyzing; }
            set { SetProperty(ref isAnalyzing, value); }
        }

        private Barang barangModel;

        public Barang BarangModel
        {
            get { return barangModel; }
            set { SetProperty(ref barangModel , value); }
        }

        public ImageSource ImagePreview
        {
            get { return imagePreview; }
            set { SetProperty(ref imagePreview, value); }
        }

        private Command _scanningCommand;
        private bool isAnalyzing;
        private List<Models.Barang> barangs;
        private ImageSource imagePreview;
        private int stockSource;
    }
}