using SQLite;
using System;

namespace MainApp.Models
{
    public class Supplier:BaseEntity
    {
        private int id;

        [PrimaryKey,AutoIncrement]
        public int Id
        {
            get { return id; }
            set { SetProperty(ref id , value); }
        }


        private string nama;
        public string Nama
        {
            get { return nama; }
            set { SetProperty(ref nama , value); }
        }

        private string kontak;
        public string Kontak
        {
            get { return kontak; }
            set { SetProperty(ref kontak, value); }
        }

        private string alamat;
        public string Alamat
        {
            get { return alamat; }
            set { SetProperty(ref alamat, value); }
        }

    }
}