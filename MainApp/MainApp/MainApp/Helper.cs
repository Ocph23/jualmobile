using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using MainApp.Helpers;
using MainApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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


        public static async Task LongToas(string message)
        {
            var service = DependencyService.Get<IToas>();
           await service.ShowLong(message);
        }
        public static async Task ShortToas(string message)
        {
            var service = DependencyService.Get<IToas>();
            await service.ShowShort(message);
        }

        


      
    }
}
