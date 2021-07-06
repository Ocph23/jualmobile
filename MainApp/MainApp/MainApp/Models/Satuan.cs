using SQLite;
using System;

namespace MainApp.Models
{
    public class Satuan:BaseEntity
    {
        private int id;

        [PrimaryKey, AutoIncrement]
        public int Id
        {
            get { return id; }
            set { SetProperty(ref  id , value); }
        }

        private int barangid;

        public int BarangId
        {
            get { return barangid; }
            set { SetProperty(ref barangid , value); }
        }

        private int level;

        public int Level
        {
            get { return level; }
            set { SetProperty(ref level , value); }
        }

        private string nama;

        public string Nama
        {
            get { return nama; }
            set { SetProperty(ref nama , value); }
        }

        private double hargaBeli;

        public double HargaBeli
        {
            get { return hargaBeli; }
            set {SetProperty(ref hargaBeli , value); }
        }

        private double hargaJual;

        public double HargaJual
        {
            get { return hargaJual; }
            set { SetProperty(ref hargaJual , value); }
        }

        private int quantity;

        public int Quantity
        {
            get { return quantity; }
            set { SetProperty(ref quantity , value); }
        }

    }
}