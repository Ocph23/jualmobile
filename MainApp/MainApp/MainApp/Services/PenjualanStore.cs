using MainApp.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MainApp.Services
{

    public interface IPenjualanStore : IDataStore<Penjualan>
    {
        Task<PenjualanItem> AddPenjualanItem(int barangId, PenjualanItem satuan);
        Task<List<PenjualanItem>> GetPenjualanItems(int barangId);
        Task<bool> EditPenjualanItem(PenjualanItem satuan);
    }


    public class PenjualanStore : IPenjualanStore
    {
        static SQLiteAsyncConnection Database;

        public PenjualanStore()
        {
            var service = DependencyService.Get<DatabaseIntialization>();
            Database = service.GetDatabase();
        }

        public async Task<int> AddItemAsync(Penjualan item)
        {
            var con = Database.GetConnection();
            con.BeginTransaction();
            try
            {
                var pembelianInserted = await Database.InsertAsync(item);
                if (pembelianInserted > 0 )
                {

                    foreach (var data in item.Items)
                    {
                        data.PenjualanId = item.Id;
                    }

                    var itemResult = await Database.InsertAllAsync(item.Items);
                    
                }
                con.Commit();
                return pembelianInserted;
            }
            catch (Exception ex)
            {
                con.Rollback();
                throw new SystemException(ex.Message);
            }
        }

        public async Task<bool> DeleteItemAsync(int id)
        {
            var item = await GetItemAsync(id);
            var result = await Database.DeleteAsync(item);
            return result > 0 ? true : false;
        }

        public async Task<Penjualan> GetItemAsync(int id)
        {
            try
            {
                var data = await Database.Table<Penjualan>().Where(x => x.Id == id).FirstOrDefaultAsync();
                if (data == null)
                    throw new SystemException("Data Tidak Ditemukan !");

                data.Items = await Database.Table<PenjualanItem>().Where(x => x.PenjualanId== id).ToListAsync();

                foreach (var item in data.Items)
                {
                    item.Barang = await Database.Table<Barang>().Where(x => x.Id == item.BarangId).FirstOrDefaultAsync();
                    item.Satuan = await Database.Table<Satuan>().Where(x => x.Id == item.SatuanId).FirstOrDefaultAsync();
                }


                return data;
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public async Task<List<Penjualan>> GetItemsAsync(bool forceRefresh = false)
        {
            try
            {
                var datas = await Database.Table<Penjualan>().ToListAsync();
                if (datas== null)
                    throw new SystemException("Data Tidak Ditemukan !");
                foreach (var data in datas)
                {
                    data.Items = await Database.Table<PenjualanItem>().Where(x => x.PenjualanId == data.Id).ToListAsync();
                    foreach (var item in data.Items)
                    {
                        item.Barang =await Database.Table<Barang>().Where(x => x.Id == item.BarangId).FirstOrDefaultAsync();
                        item.Satuan= await Database.Table<Satuan>().Where(x => x.Id == item.SatuanId).FirstOrDefaultAsync();
                    }
                }


                return datas;

            }
            catch (Exception ex)
            {

                throw new SystemException(ex.Message);
            }
        }

        public async Task<bool> UpdateItemAsync(Penjualan item)
        {
            var con = Database.GetConnection();
            con.BeginTransaction();
            try
            {
                var pembelianInserted = await Database.InsertOrReplaceAsync(item);
                if (pembelianInserted > 0)
                {
                    var itemResult = await Database.InsertOrReplaceAsync(item.Items);

                }
                con.Commit();
                return pembelianInserted > 0 ? true : false;
            }
            catch (Exception ex)
            {
                con.Rollback();
                throw new SystemException(ex.Message);
            }
        }

        public Task<PenjualanItem> AddPenjualanItem(int barangId, PenjualanItem satuan)
        {
            var result = Database.InsertAsync(satuan);
            return Task.FromResult(satuan);
        }

        public Task<List<PenjualanItem>> GetPenjualanItems(int barangId)
        {
            return Database.Table<PenjualanItem>().Where(x => x.Id == barangId).ToListAsync();
        }

        public async Task<bool> EditPenjualanItem(PenjualanItem satuan)
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

        public async Task<bool> DeletePenjualanItem(PenjualanItem satuan)
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
    }
}