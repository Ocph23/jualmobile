using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;

namespace MainApp.Models
{
    public class Barang :BaseNotify
    {
        private int id;

        [PrimaryKey, AutoIncrement]
        public int Id
        {
            get { return id; }
            set { SetProperty(ref id, value); }
        }


        private int supplierId;

        public int SupplierId
        {
            get { return supplierId; }
            set { SetProperty(ref supplierId , value); }
        }


        private string nama;
        public string Nama
        {
            get { return nama; }
            set { SetProperty(ref nama, value); }
        }
        private string  keterangan;

        public string  Keterangan
        {
            get { return keterangan; }
            set { SetProperty(ref keterangan , value); }
        }


        private string barCode;

        public string Barcode
        {
            get { return barCode; }
            set { SetProperty(ref barCode , value); }
        }

        private string photo;
        public string Photo
        {
            get { return photo; }
            set { SetProperty(ref photo, value); }
        }


        [Ignore]
        public List<Satuan> Satuans {get;set;}


        private Supplier supplier;
        [Ignore]

        public Supplier Supplier
        {
            get { return supplier; }
            set { SetProperty(ref supplier , value);
                if (value != null)
                {
                    supplierId = value.Id;
                }
            }
        }

    }
}