using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MainApp.Models
{
    public class Pembelian:BaseEntity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        private int supplierId;

        [Indexed]

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