using MainApp.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MainApp.Services
{

    public interface IBarangStore : IDataStore<Barang>
    {
        Task<Satuan> AddSatuan(int barangId, Satuan satuan);
        Task<List<Satuan>> GetSatuans(int barangId);
        Task<bool> EditSatuan(Satuan satuan);
        Task<int> GetStock(int barangId);
    }


    public class BarangStore : IBarangStore
    {
        public SQLiteAsyncConnection Database { get; }

        public BarangStore()
        {
             var service= DependencyService.Get<DatabaseIntialization>();
             Database=   service.GetDatabase();
        }

        public Task<int> AddItemAsync(Barang item)
        {
            var result = Database.InsertAsync(item);
            return result;
        }

        public async Task<bool> DeleteItemAsync(int id)
        {
            var item = await GetItemAsync(id);
            var result = await Database.DeleteAsync(item);
            return result > 0 ? true : false;
        }

        public async Task<Barang> GetItemAsync(int id)
        {
            try
            {
                var data = await Database.Table<Barang>().Where(x => x.Id == id).FirstOrDefaultAsync();
                if (data == null)
                    throw new SystemException("Data Tidak Ditemukan !");
                data.Satuans = await Database.Table<Satuan>().Where(x => x.BarangId == id).ToListAsync();
                return data;
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public async Task<List<Barang>> GetItemsAsync(bool forceRefresh = false)
        {
            try
            {
                var barangs = await  Database.Table<Barang>().ToListAsync();

               var satuans = await Database.Table<Satuan>().ToListAsync();
                var q = from a in barangs
                        join b in satuans on a.Id equals b.Id into g
                        from b in g.DefaultIfEmpty()
                        select new Barang { Satuans = g.ToList(), Photo=a.Photo, Barcode = a.Barcode, Id = a.Id, Keterangan = a.Keterangan, Nama = a.Nama, Supplier = a.Supplier, SupplierId = a.SupplierId };

                return q.ToList();
            }
            catch (Exception ex)
            {

                throw new SystemException(ex.Message);
            }
        }

        public async Task<bool> UpdateItemAsync(Barang item)
        {
            var result = await Database.UpdateAsync(item);
            return result > 0 ? true : false;
        }

        public Task<Satuan> AddSatuan(int barangId, Satuan satuan)
        {
            satuan.BarangId = barangId;
            var result = Database.InsertAsync(satuan);
            return Task.FromResult(satuan);
        }

        public Task<List<Satuan>> GetSatuans(int barangId)
        {
            return Database.Table<Satuan>().Where(x => x.BarangId == barangId).ToListAsync();
        }

        public async Task<bool> EditSatuan(Satuan satuan)
        {
            try
            {
                var results = await Database.InsertOrReplaceAsync(satuan);
                if (results > 0)
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public async Task<bool> DeleteSatuan(Satuan satuan)
        {
            try
            {
                var results = await Database.DeleteAsync(satuan);
                if (results > 0)
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public async Task<int> GetStock(int barangId)
        {
            try
            {
                var barang= await Database.Table<Barang>().Where(x => x.Id== barangId).FirstAsync();
                var satuans = await Database.Table<Satuan>().Where(x => x.BarangId == barangId).ToListAsync();

                if (satuans == null || satuans.Count <= 0)
                {
                   await Helper.ErrorMessage($"Satuan Barang({barang.Nama} ) Belum Ada !");
                    return 0;
                }
                var pembelian = from data in await Database.Table<PembelianItem>().Where(x => x.BarangId == barangId).ToListAsync()
                                join satuan in await Database.Table<Satuan>().ToListAsync() on data.SatuanId equals satuan.Id
                                select data.Jumlah * satuan.Quantity ;



                var penjualan = from data in await Database.Table<PenjualanItem>().Where(x => x.BarangId == barangId).ToListAsync()
                                join satuan in await Database.Table<Satuan>().ToListAsync() on data.SatuanId equals satuan.Id
                                select data.Jumlah * satuan.Quantity;

                return Convert.ToInt32((pembelian.Sum() - penjualan.Sum()));
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }

        }
    }
}