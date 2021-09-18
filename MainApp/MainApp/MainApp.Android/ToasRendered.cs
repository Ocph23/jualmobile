using Android.Widget;
using MainApp.Droid;
using MainApp.Helpers;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(ToasRendered))]
namespace MainApp.Droid
{
    public class ToasRendered : IToas
    {
        public Task ShowLong(string message)
        {
            Android.Widget.Toast.MakeText(Android.App.Application.Context, message, ToastLength.Long).Show();
            return Task.CompletedTask;
        }
        public Task ShowShort(string message)
        {
            Android.Widget.Toast.MakeText(Android.App.Application.Context, message, ToastLength.Short).Show();
            return Task.CompletedTask;
        }

    }
}