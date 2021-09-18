using MainApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xamarin.Essentials;


namespace MainApp
{
    public class Account
    {
        public static async Task<string> GetUrl()
        {
            var dataString = await SecureStorage.GetAsync("url");
            if (string.IsNullOrEmpty(dataString))
                return "http://192.168.1.1";
            else
                return dataString;
        }

        public static async Task SetUrl(string url)
        {
            await SecureStorage.SetAsync("url", url);
        }



        public static Task SetProfile(Profile model)
        {
            var modelString = JsonSerializer.Serialize(model);
            SecureStorage.SetAsync("pemilik", modelString);
            return Task.CompletedTask;
        }



        public static async Task<Profile> GetProfile()
        {
            try
            {
                var data = await SecureStorage.GetAsync("pemilik");
                var result = JsonSerializer.Deserialize<Profile>(data);
                return result;
            }
            catch
            {
                return null; ;
            }
        }

        public static Task SetUser(User model)
        {
            var modelString = JsonSerializer.Serialize(model);
            SecureStorage.SetAsync("User", modelString);
            return Task.CompletedTask;
        }

        internal static async Task<bool> Login(User model)
        {
            try
            {
                var profile = await GetProfile();
                if (model.UserName == profile.UserName && model.Password == profile.Password)
                {
                    await SetUser(model);
                    return true;
                }
                throw new SystemException("User Atau  Password Anda Salah !");
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public static async Task<User> GetUser()
        {
            try
            {
                var data = await SecureStorage.GetAsync("User");
                var result = JsonSerializer.Deserialize<User>(data);
                return result;
            }
            catch
            {
                return null; ;
            }
        }


        public static async Task LogOut()
        {
            try
            {
                await SecureStorage.SetAsync("User", string.Empty);
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }


       


    }
}
