using MainApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MainApp.Views.Penjualans
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BayarPage : ContentPage
    {
        public BayarPage()
        {
            InitializeComponent();
        }
    }


    public class BayarViewModel : ViewModels.BaseViewModel
    {
        

        private double grandTotal;

        public double GrandTotal
        {
            get { return grandTotal; }
            set { SetProperty(ref grandTotal , value); }
        }

        public Penjualan model { get; set; }

        public BayarViewModel(Penjualan model)
        {
            this.model = model;
            GrandTotal = model.Items.Sum(x => x.Total);
        }

        private double bayar;

        public double Bayar
        {
            get { return bayar; }
            set { 
                SetProperty(ref bayar , value);
                Kembalian = value - GrandTotal;
                if (Kembalian < 0)
                {
                    Helper.ShortToas($"Pembayaran Tidak Cukup (Kurang Rp.{Kembalian.ToString("N")})");
                }
            
            }
        }

        private double  kembalian;

        public double  Kembalian
        {
            get { return kembalian; }
            set { SetProperty(ref kembalian , value); }
        }

    }
}