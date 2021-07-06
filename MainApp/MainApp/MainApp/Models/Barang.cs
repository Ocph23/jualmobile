using SQLite;
using System;
using System.Collections.Generic;

namespace MainApp.Models
{
    public class Barang :BaseEntity
    {
        private int id;

        [PrimaryKey, AutoIncrement]
        public int Id
        {
            get { return id; }
            set { SetProperty(ref id, value); }
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





        [Ignore]
        public List<Satuan> Satuans {get;set;}
    }
}