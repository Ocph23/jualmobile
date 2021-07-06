﻿using MainApp.Models;
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
            set { SetProperty(ref search , value);
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
            set { SetProperty(ref isRefreshing , value); }
        }


        private void FilterData(string value)
        {
            IsRefreshing = true;
            if (datas == null)
                RefreshCommand.Execute(null);

           if (datas != null)
            {
                Items.Clear();
                foreach (var item in datas.Where(x=>x.Nama.ToLower().Contains(value.ToLower())))
                {
                    Items.Add(item);
                }
            }

            IsRefreshing = false;
        }

        public ObservableCollection<Barang> Items { get; set; } = new ObservableCollection<Barang>();
        public Command RefreshCommand { get; }
        public Command SelectBarang { get; }
        private Command okCommand;

        public Command OkCommand
        {
            get { return okCommand; }
            set { SetProperty(ref okCommand , value); }
        }

        public Command CancelCommand { get; }

        public CariBarangDialogViewModel()
        {
            RefreshCommand = new Command(async () => await Load());
            SelectBarang = new Command((object obj) => {
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

        private async Task Load()
        {
            try
            {
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
               await Helper.ErrorMessage(ex.Message);
            }
            finally
            {
                IsRefreshing = false;
            }
        }



        private Barang selectedItem;
        private List<Barang> datas;

        public Barang SelectedItem
        {
            get { return selectedItem; }
            set { SetProperty(ref selectedItem , value); OkCommand = new Command(async () => await OkAction(), () => true); }
        }

    }
}