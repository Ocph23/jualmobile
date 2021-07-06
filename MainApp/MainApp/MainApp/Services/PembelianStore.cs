using MainApp.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MainApp.Services
{

    public interface IPembelianStore : IDataStore<Pembelian>
    {
        Task<PembelianItem> AddPembelianItem(int barangId, PembelianItem satuan);
        Task<List<PembelianItem>> GetPembelianItems(int barangId);
        Task<bool> EditPembelianItem(PembelianItem satuan);
    }


    public class PembelianStore : IPembelianStore
    {
        static SQLiteAsyncConnection Database;

        public PembelianStore()
        {
            var service = DependencyService.Get<DatabaseIntialization>();
            Database = service.GetDatabase();
        }

        public async Task<int> AddItemAsync(Pembelian item)
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
                        data.PembelianId = item.Id;
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

        public async Task<Pembelian> GetItemAsync(int id)
        {
            try
            {
                var data = await Database.Table<Pembelian>().Where(x => x.Id == id).FirstOrDefaultAsync();
                if (data == null)
                    throw new SystemException("Data Tidak Ditemukan !");

                data.Supplier = await Database.Table<Supplier>().Where(x => x.Id == data.SupplierId).FirstOrDefaultAsync();

                data.Items = await Database.Table<PembelianItem>().Where(x => x.PembelianId== id).ToListAsync();

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

        public async Task<List<Pembelian>> GetItemsAsync(bool forceRefresh = false)
        {
            try
            {
                var datas = await Database.Table<Pembelian>().ToListAsync();
                if (datas== null)
                    throw new SystemException("Data Tidak Ditemukan !");
                foreach (var data in datas)
                {

                    data.Supplier = await Database.Table<Supplier>().Where(x => x.Id == data.SupplierId).FirstOrDefaultAsync();
                    data.Items = await Database.Table<PembelianItem>().Where(x => x.PembelianId == data.Id).ToListAsync();
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

        public async Task<bool> UpdateItemAsync(Pembelian item)
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

        public Task<PembelianItem> AddPembelianItem(int barangId, PembelianItem satuan)
        {
            var result = Database.InsertAsync(satuan);
            return Task.FromResult(satuan);
        }

        public Task<List<PembelianItem>> GetPembelianItems(int barangId)
        {
            return Database.Table<PembelianItem>().Where(x => x.Id == barangId).ToListAsync();
        }

        public async Task<bool> EditPembelianItem(PembelianItem satuan)
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

        public async Task<bool> DeletePembelianItem(PembelianItem satuan)
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