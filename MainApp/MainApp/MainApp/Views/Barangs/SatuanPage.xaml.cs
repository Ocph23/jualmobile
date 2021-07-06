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

namespace MainApp.Views.Barangs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SatuanPage : ContentPage
    {
        public SatuanPage(Barang barang)
        {
            InitializeComponent();
            BindingContext = new SatuanViewModel(barang);
        }


        protected override void OnAppearing()
        {
            var vm = BindingContext as SatuanViewModel;
            vm.RefreshCommand.Execute(null);
            base.OnAppearing();
        }
    }


    
    public class SatuanViewModel : BaseViewModel
    {
        
        public ObservableCollection<Satuan> Items { get; set; } = new ObservableCollection<Satuan>();
        public Command AddCommand { get; }
        public Command EditCommand { get; }
        public Command DeleteCommand { get; }
        public Command RefreshCommand { get; }
        private bool isRefreshing;
        public bool IsRefreshing
        {
            get { return isRefreshing; }
            set { SetProperty(ref isRefreshing, value); }
        }

        public SatuanViewModel(Barang barang)
        {
            Barang = barang;

            if(barang!=null && barang.Satuans!=null)
            {
                foreach (var item in barang.Satuans)
                {
                    Items.Add(item);
                }
            }
            
            Title = barang.Nama;
            AddCommand = new Command(() =>
            {
                var page = new AddSatuanPage(Barang);
                AppShell.Current.Navigation.PushAsync(page);
                var vm = page.BindingContext as AddSatuanViewModel;

                vm.OnAdd += Vm_OnAdd;
                vm.OnUpdate += Vm_OnUpdate; ;



                
            });

            EditCommand = new Command(async (object obj) =>
            {
                var model = obj as Satuan;
                var page = new AddSatuanPage(Barang, model);
                await AppShell.Current.Navigation.PushAsync(page);
                var vm = page.BindingContext as AddSatuanViewModel;
                if (vm.Model.Id > 0)
                {
                    Items.Add(vm.Model);
                }
            });


            DeleteCommand = new Command(async (object data) =>
            {
                var supplier = data as Satuan;
                bool answer = await Application.Current.MainPage.DisplayAlert("Yakin ?", "Hapus Data !", "Yes", "No");

                if (answer)
                {
                    await BarangStore.DeleteItemAsync(supplier.Id);
                }

            });
            RefreshCommand = new Command(async () => await Load());
        }

        private void Vm_OnUpdate(object data)
        {
            throw new NotImplementedException();
        }

        private void Vm_OnAdd(object data)
        {
            var satuan = data as Satuan;
                Items.Add(satuan);
        }

        private async Task Load()
        {
            try
            {
                //var supplier = new Satuan { Nama = "Satuan A", Alamat = "Jln. Sudirman ", Kontak = "08114810279" };
                //await SatuanStore.AddItemAsync(supplier);
                IsRefreshing = true;
                var datas = await BarangStore.GetSatuans(Barang.Id);
                Items.Clear();
                foreach (var item in datas)
                {
                    Items.Add(item);
                }
                Barang.Satuans = datas;
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

        private Satuan selected;

        public Satuan SelectedItem
        {
            get { return selected; }
            set { SetProperty(ref selected, value); }
        }

        public Barang Barang { get; }
    }
}