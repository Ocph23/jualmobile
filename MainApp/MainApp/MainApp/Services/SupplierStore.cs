using MainApp.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MainApp.Services
{

    public class SupplierStore : IDataStore<Supplier>
    {
        public SQLiteAsyncConnection Database { get; }

        public SupplierStore()
        {
             var service= DependencyService.Get<DatabaseIntialization>();
             Database=   service.GetDatabase();
        }

        public Task<int> AddItemAsync(Supplier item)
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

        public Task<Supplier> GetItemAsync(int id)
        {
            return Database.Table<Supplier>().Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public Task<List<Supplier>> GetItemsAsync(bool forceRefresh = false)
        {
            try
            {
                return Database.Table<Supplier>().ToListAsync();
            }
            catch (Exception ex)
            {

                throw new SystemException(ex.Message);
            }
        }

        public async Task<bool> UpdateItemAsync(Supplier item)
        {
            var result = await Database.UpdateAsync(item);
            return result > 0 ? true : false;
        }
       
    }
}