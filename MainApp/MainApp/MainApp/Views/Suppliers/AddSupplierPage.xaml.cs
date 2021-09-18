using MainApp.Models;
using MainApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MainApp.Views.Suppliers
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddSupplierPage : ContentPage
    {
        public AddSupplierPage()
        {
            InitializeComponent();
            this.BindingContext = new AddSupplierViewModel();
        }

        public AddSupplierPage(Supplier model)
        {
            InitializeComponent();
            this.BindingContext = new AddSupplierViewModel(model);
        }
    }

    public class AddSupplierViewModel : BaseViewModel
    {
        public Supplier Model { get; }
        public Command SaveCommand { get; private set; }

        public AddSupplierViewModel()
        {
            Title = "Supplier Baru";
            Model = new Supplier();
            Load();
        }

        public AddSupplierViewModel(Supplier supplier)
        {
            Title = "Edit Supplier";
            Model = supplier;
            Load();
        }


        void Load()
        {
            SaveCommand = new Command( async() => {

                try
                {
                    if (string.IsNullOrEmpty(Model.Nama))
                    {
                        throw new SystemException("Nama Tidak Boleh Kosong !");
                    }

                    var saved = false;
                    if(Model.Id <= 0)
                    {
                        var result = await SupplierStore.AddItemAsync(Model);
                        if (result > 0)
                            saved = true;
                    }
                    else
                    {
                        var result = await SupplierStore.UpdateItemAsync(Model);
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
                       await AppShell.Current.Navigation.PopAsync();
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