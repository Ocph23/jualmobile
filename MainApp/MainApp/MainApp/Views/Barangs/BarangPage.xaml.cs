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
    public partial class BarangPage : ContentPage
    {
        public BarangPage()
        {
            InitializeComponent();
            this.BindingContext = new BarangViewModel();
        }

        protected override void OnAppearing()
        {
            var vm = this.BindingContext as BarangViewModel;
            vm.RefreshCommand.Execute(null);
            base.OnAppearing();
        }
    }


    public class BarangViewModel : BaseViewModel
    {
        public ObservableCollection<Barang> Items { get; set; } = new ObservableCollection<Barang>();
        public Command AddCommand { get; }
        public Command ShowSatuanCommand { get; }
        public Command EditCommand { get; }
        public Command DeleteCommand { get; }
        public Command RefreshCommand { get; }
        private bool isRefreshing;
        public bool IsRefreshing
        {
            get { return isRefreshing; }
            set { SetProperty(ref isRefreshing, value); }
        }

        private List<Barang> datas;
        private string searchText;

        public string SearchText
        {
            get { return searchText; }
            set { SetProperty(ref searchText , value);
                if (datas!=null && !string.IsNullOrEmpty(value))
                {
                    FilterData(value);

                }
            }
        }

        private void FilterData(string value)
        {
            Items.Clear();
            foreach (var item in datas.Where(x=>x.Nama.ToLower().Contains(value.ToLower())))
            {
                Items.Add(item);
            }
        }

        public BarangViewModel()
        {
            Title = "Barang";
            AddCommand = new Command(() =>
            {
                var page = new AddBarangPage();
                AppShell.Current.Navigation.PushAsync(page);
                var vm = page.BindingContext as AddBarangViewModel;
                if (vm.Model.Id > 0)
                {
                    Items.Add(vm.Model);
                }
            });

            ShowSatuanCommand = new Command(async (object obj) => {
                var model = obj as Barang;
                var page = new SatuanPage(model);
                await AppShell.Current.Navigation.PushAsync(page);
                var vm = page.BindingContext as SatuanViewModel;
            });

            EditCommand = new Command(async (object obj) =>
            {
                var model = obj as Barang;
                var page = new AddBarangPage(model);
                await AppShell.Current.Navigation.PushAsync(page);
                var vm = page.BindingContext as AddBarangViewModel;
                if (vm.Model.Id > 0)
                {
                    Items.Add(vm.Model);
                }
            });


            DeleteCommand = new Command(async (object data) =>
            {
                var supplier = data as Barang;
                bool answer = await Application.Current.MainPage.DisplayAlert("Yakin ?", "Hapus Data !", "Yes", "No");

                if (answer)
                {
                    await BarangStore.DeleteItemAsync(supplier.Id);
                }

            });
            RefreshCommand = new Command(async () => await Load());
        }

        private async Task Load()
        {
            try
            {
                //var supplier = new Barang { Nama = "Barang A", Alamat = "Jln. Sudirman ", Kontak = "08114810279" };
                //await BarangStore.AddItemAsync(supplier);
                IsRefreshing = true;
                datas = await BarangStore.GetItemsAsync();
                Items.Clear();
                foreach (var item in datas)
                {
                    Items.Add(item);
                }
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

        private Barang selected;

        public Barang SelectedItem
        {
            get { return selected; }
            set { SetProperty(ref selected, value); }
        }
    }
}