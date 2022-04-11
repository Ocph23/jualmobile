using MainApp.Helpers;
using SQLite;
using System;

namespace MainApp.Models
{

    
    public class PenjualanItem   :BaseNotify
    {
        private int id;

        [PrimaryKey, AutoIncrement]
        public int Id
        {
            get { return id; }
            set { SetProperty(ref id, value); }
        }


        [Indexed]
        public int PenjualanId { get; set; }

        [Indexed]
        public int BarangId { get; set; }

        [Indexed]
        public int SatuanId { get; set; }

        public double Harga { get; set; }

        private double jumlah;

        public double Jumlah
        {
            get { return jumlah; }
            set
            {
                SetProperty(ref jumlah, value);
                Total = Harga * Jumlah;
            }
        }

        [Ignore]
        public Barang Barang { get; set; }

        private Satuan satuan;

        [Ignore]
        public Satuan Satuan
        {
            get { return satuan; }
            set
            {
                SetProperty(ref satuan, value);
                if (value != null)
                {
                    Harga = value.HargaBeli;
                    SatuanId = value.Id;
                    Total = Harga * Jumlah;
                }
            }
        }

        private double total;

        [Ignore]
        public double Total
        {
            get { return total = Harga * Jumlah; }
            set { SetProperty(ref total, value); }
        }


        public event JumlahChangeDelegate OnChangeJumlah;

    }
}