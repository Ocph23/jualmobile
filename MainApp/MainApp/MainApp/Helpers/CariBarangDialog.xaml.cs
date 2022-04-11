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

namespace MainApp.Helpers
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CariBarangDialog : ContentPage
    {

        
        public CariBarangDialog()
        {
            InitializeComponent();
            this.BindingContext = new CariBarangDialogViewModel();
        }


        public CariBarangDialog(int supplierId)
        {
            InitializeComponent();
            this.BindingContext = new CariBarangDialogViewModel(supplierId);
        }

        protected override void OnAppearing()
        {
            var vm = BindingContext as CariBarangDialogViewModel;
            vm.RefreshCommand.Execute(null);
            base.OnAppearing();
        }

    }


    public delegate Task SearchDelegate(Barang barang);
    public class CariBarangDialogViewModel : BaseViewModel
    {
        public event SearchDelegate onFoundItem;

        private string search;

        public string Search
        {
            get { return search; }
            set
            {
                SetProperty(ref search, value);
                if (!string.IsNullOrEmpty(value))
                {
                    FilterData(value);
                }
            }
        }

        private bool isRefreshing;

        public bool IsRefreshing
        {
            get { return isRefreshing; }
            set { SetProperty(ref isRefreshing, value); }
        }


        private async void FilterData(string value)
        {
            try
            {
                if (IsRefreshing || string.IsNullOrEmpty(value))
                    return;

                IsRefreshing = true;
                if (datas == null)
                    RefreshCommand.Execute(null);

                if (datas != null)
                {
                    Items.Clear();

                    var sources = datas.Where(x => x.Nama.ToLower().Contains(value.ToLower()));
                    if (supplierId > 0)
                        sources = sources.Where(x => x.SupplierId == supplierId);

                    foreach (var item in sources)
                    {
                        if (item.Stock <= 0)
                        {
                            item.Stock = await BarangStore.GetStock(item.Id);
                        }
                        Items.Add(item);
                    }
                }

                IsRefreshing = false;
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        public ObservableCollection<Barang> Items { get; set; } = new ObservableCollection<Barang>();
        public Command RefreshCommand { get; set; }
        public Command SelectBarang { get; set; }
        private Command okCommand;

        public Command OkCommand
        {
            get { return okCommand; }
            set { SetProperty(ref okCommand, value); }
        }

        public Command CancelCommand { get; set; }

        public CariBarangDialogViewModel()
        {
            Load();
        }

        public CariBarangDialogViewModel(int supplierId)
        {
            this.supplierId = supplierId;
            Load();
        }

        private void Load()
        {
            RefreshCommand = new Command(async () => await Refresh());
            SelectBarang = new Command((object obj) =>
            {
                SelectedItem = obj as Barang;
            });
            OkCommand = new Command(async () => await OkAction(), () => false);
            CancelCommand = new Command(async () => await CancelAction());
        }

        private Task CancelAction()
        {
            return AppShell.Current.Navigation.PopModalAsync();
        }

        private Task OkAction()
        {
            onFoundItem?.Invoke(SelectedItem);
            CancelCommand.Execute(null);
            return Task.CompletedTask;
        }

        private async Task Refresh()
        {
            try
            {
                IsRefreshing = true;
                datas = await BarangStore.GetItemsAsync();
                FilterData("");
            }
            catch (Exception ex)
            {
                await Helper.ErrorMessage(ex.Message);
            }
            finally
            {
                IsRefreshing = false;
            }
        }



        private Barang selectedItem;
        private List<Barang> datas;
        private int supplierId;

        public Barang SelectedItem
        {
            get { return selectedItem; }
            set
            {
                SetProperty(ref selectedItem, value);
                OkCommand = new Command(async () => await OkAction(), () => (value.Stock > 0));
            }
        }
    }
}