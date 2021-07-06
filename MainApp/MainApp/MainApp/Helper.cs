using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MainApp
{
  public  class Helper
    {


        public static Task InfoMessage(string message)
        {
           return Application.Current.MainPage.DisplayAlert("Info", message, "OK");
        }


        public static Task ErrorMessage(string message)
        {
            return Application.Current.MainPage.DisplayAlert("Error", message, "OK");
        }
    }
}
