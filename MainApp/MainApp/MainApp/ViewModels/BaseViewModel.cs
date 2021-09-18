using MainApp.Models;
using MainApp.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace MainApp.ViewModels
{
    public class BaseViewModel : BaseNotify
    {
        public IDataStore<Supplier> SupplierStore => DependencyService.Get<IDataStore<Supplier>>();
        public IBarangStore BarangStore => DependencyService.Get<IBarangStore>();
        public IPembelianStore PembelianStore => DependencyService.Get<IPembelianStore>();
        public IPenjualanStore PenjualanStore => DependencyService.Get<IPenjualanStore>();

        bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }

        string title = string.Empty;
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }
    }
}
