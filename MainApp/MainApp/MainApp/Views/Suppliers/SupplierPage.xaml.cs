using MainApp.Models;
using MainApp.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MainApp.Views.Suppliers
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SupplierPage : ContentPage
    {
        public SupplierPage()
        {
            InitializeComponent();
            this.BindingContext = new SupplierViewModel();
        }


        protected override void OnAppearing()
        {

            var vm = this.BindingContext as SupplierViewModel;
            vm.RefreshCommand.Execute(null);
            base.OnAppearing();
        }
    }


    public class SupplierViewModel : BaseViewModel
    {
        public ObservableCollection<Supplier> Items { get; set; } = new ObservableCollection<Supplier>();
        public Command AddNewSupplier { get; }
        public Command EditCommand { get; }
        public Command DeleteCommand { get; }
        public Command RefreshCommand { get; }
        private bool isRefreshing;
        public bool IsRefreshing
        {
            get { return isRefreshing; }
            set { SetProperty(ref isRefreshing, value); }
        }

        public SupplierViewModel()
        {
            Title = "Supplier";
            AddNewSupplier = new Command(() =>
            {
                var page = new AddSupplierPage();
                AppShell.Current.Navigation.PushAsync(page);
                var vm = page.BindingContext as AddSupplierViewModel;
                if (vm.Model.Id > 0)
                {
                    Items.Add(vm.Model);
                }
            });



            EditCommand = new Command(async (object obj) =>
            {
                var supplier = obj as Supplier;
                var page = new AddSupplierPage(supplier);
                await AppShell.Current.Navigation.PushAsync(page);
                var vm = page.BindingContext as AddSupplierViewModel;
                if (vm.Model.Id > 0)
                {
                    Items.Add(vm.Model);
                }
            });


            DeleteCommand = new Command(async (object data) =>
            {
                var supplier = data as Supplier;
                bool answer = await Application.Current.MainPage.DisplayAlert("Yakin ?", "Hapus Data !", "Yes", "No");

                if (answer)
                {
                    await SupplierStore.DeleteItemAsync(supplier.Id);
                }
              
            });

            RefreshCommand = new Command(async () => await Load());
        }

        private async Task Load()
        {
            try
            {
                //var supplier = new Supplier { Nama = "Supplier A", Alamat = "Jln. Sudirman ", Kontak = "08114810279" };
                //await SupplierStore.AddItemAsync(supplier);
                IsRefreshing = true;
                var datas = await SupplierStore.GetItemsAsync();
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

        private Supplier selected;

        public Supplier SelectedItem
        {
            get { return selected; }
            set { SetProperty(ref selected, value); }
        }
    }
}