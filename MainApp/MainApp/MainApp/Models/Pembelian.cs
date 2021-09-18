using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MainApp.Models
{
    public class Pembelian:BaseNotify
    {
        [PrimaryKey, AutoIncrement]
        public int Id
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }


        private int supplierId;

        public int SupplierId
        {
            get { return supplierId; }
            set { SetProperty(ref supplierId , value); }
        }


        private DateTime tanggal;

        public DateTime Tanggal
        {
            get { return tanggal; }
            set { SetProperty(ref tanggal , value); }
        }


        private Supplier supplier;
        [Ignore]
        public Supplier Supplier
        {
            get { return supplier; }
            set { SetProperty(ref supplier , value);
                if (value != null)
                {
                    SupplierId = value.Id;
                }
            }
        }


        [Ignore]
        public List<PembelianItem> Items{get;set;}



        private double total;
        private int _id;

        [Ignore]
        public double Total
        {
            get {
                return Items == null ? 0 : Items.Sum(x => x.Jumlah * x.Harga);
            }
            set { total = value; }
        }
    }
}