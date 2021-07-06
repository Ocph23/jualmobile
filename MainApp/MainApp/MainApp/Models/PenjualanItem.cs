using SQLite;
using System;

namespace MainApp.Models
{
    public class PenjualanItem
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public int PenjualanId { get; set; }

        [Indexed]
        public int BarangId { get; set; }

        [Indexed]
        public int SatuanId { get; set; }

        public double Harga { get; set; }
        public double Jumlah { get; set; }

        [Ignore]
        public Barang Barang { get; set; }

        [Ignore]
        public Satuan Satuan { get; set; }

    }
}