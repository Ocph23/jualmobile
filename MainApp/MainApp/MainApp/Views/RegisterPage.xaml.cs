using MainApp.Models;
using MainApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MainApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegisterPage : ContentPage
    {
        public RegisterPage()
        {
            InitializeComponent();
            this.BindingContext = new RegisterViewModel();
        }
    }

    internal class RegisterViewModel     :BaseViewModel
    {
        public Profile Model { get; set; } = new Profile();
        private Command registerCommand;

        public Command RegisterCommand
        {
            get { return registerCommand; }
            set { SetProperty(ref registerCommand , value); }
        }

        public RegisterViewModel()
        {
            RegisterCommand = new Command(async (x)=> { await RegisterAction(); }, RegisterValidate);
            Model.PropertyChanged += Model_PropertyChanged;

        }

        private void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            RegisterCommand = new Command(async (x) => { await RegisterAction(); }, RegisterValidate);
        }

        private async Task RegisterAction()
        {
            try
            {
                if (IsBusy)
                    return;
                IsBusy = true;
                await Account.SetProfile(Model);
              _ =  Helper.InfoMessage("Berhasil !, Silahkan Login");
                Application.Current.MainPage = new LoginPage();
            }
            catch (Exception ex)
            {
                _=Helper.ErrorMessage(ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private bool RegisterValidate(object arg)
        {

            if (string.IsNullOrEmpty(Model.NamaUsaha) ||
                string.IsNullOrEmpty(Model.Alamat) ||
                string.IsNullOrEmpty(Model.Nama) ||
                string.IsNullOrEmpty(Model.UserName) ||
                string.IsNullOrEmpty(Model.Password))
                return false;

            return true ;
        }
    }
}