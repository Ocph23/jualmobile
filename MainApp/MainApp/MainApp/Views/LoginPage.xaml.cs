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
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
            this.BindingContext = new LoginViewModel();
        }
    }

    public class LoginViewModel : BaseViewModel
    {

        public User Model { get; set; } = new User();

        private Command loginCommand;

        public Command LoginCommand
        {
            get { return loginCommand; }
            set { SetProperty(ref loginCommand , value); }
        }


        public LoginViewModel()
        {
            LoginCommand = new Command((x)=> OnLoginClicked(x), LogiValidate);
            Model.PropertyChanged += Model_PropertyChanged;
        }

        private void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            LoginCommand = new Command((x) => OnLoginClicked(x), LogiValidate);
        }

        private bool LogiValidate(object arg)
        {
            if (string.IsNullOrEmpty(Model.UserName) || string.IsNullOrEmpty(Model.Password))
                return false;

            return true;
        }

        private async void OnLoginClicked(object obj)
        {

            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                await Account.Login(Model);
                await Task.Delay(500);

                Application.Current.MainPage = new AppShell();

            }
            catch (Exception ex)
            {
               _= Helper.ErrorMessage(ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private string nama;

        public string Nama
        {
            get { return nama; }
            set { SetProperty(ref nama, value); }
        }

    }
}