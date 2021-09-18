using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MainApp.Models
{
    public class Penjualan:BaseNotify
    {
        private int id;
        [PrimaryKey, AutoIncrement]
        public int Id
        {
            get { return id; }
            set { SetProperty(ref id , value); }
        }

        private DateTime tanggal= DateTime.Now;

        public DateTime Tanggal
        {
            get { return tanggal; }
            set { SetProperty(ref tanggal , value); }
        }

        private string customer = "No Name";
        private double total;

        public string Pelanggan
        {
            get { return customer; }
            set { customer = value; }
        }

        [Ignore]
        public List<PenjualanItem> Items{get;set;}


        [Ignore]
        public double Total
        {
            get
            {
                return Items == null ? 0 : Items.Sum(x => x.Jumlah * x.Harga);
            }
            set { total = value; }
        }
    }
}