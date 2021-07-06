using SQLite;
using System;
using System.Collections.Generic;

namespace MainApp.Models
{
    public class Penjualan:BaseEntity
    {
        private int id;
        [PrimaryKey, AutoIncrement]
        public int Id
        {
            get { return id; }
            set { SetProperty(ref id , value); }
        }

        private DateTime tanggal;

        public DateTime Tanggal
        {
            get { return tanggal; }
            set { SetProperty(ref tanggal , value); }
        }

        private string customer = "No Name";

        public string Pelanggan
        {
            get { return customer; }
            set { customer = value; }
        }


        public List<PenjualanItem> Items{get;set;}
    }
}