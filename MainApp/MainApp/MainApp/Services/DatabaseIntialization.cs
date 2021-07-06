using MainApp.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace MainApp.Services
{
    public class DatabaseIntialization
    {

        static SQLiteAsyncConnection Database;

        public static readonly AsyncLazy<DatabaseIntialization> Instance = new AsyncLazy<DatabaseIntialization>(async () =>
        {
            var instance = new DatabaseIntialization();
            await Database.CreateTableAsync<Supplier>();
            await Database.CreateTableAsync<Barang>();
            await Database.CreateTableAsync<Satuan>();
            await Database.CreateTableAsync<Pembelian>();
            await Database.CreateTableAsync<PembelianItem>();
            await Database.CreateTableAsync<Penjualan>();
            await Database.CreateTableAsync<PenjualanItem>();
            return instance;
        });

        internal SQLiteAsyncConnection GetDatabase()
        {
            return Database;
        }

        public DatabaseIntialization()
        {
            Database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
        }
    }
}
