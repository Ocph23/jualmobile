using System;

namespace MainApp.Models
{
    public class User  :BaseNotify
    {
        private string nama;

        public string Nama
        {
            get { return nama; }
            set { SetProperty(ref nama , value); }
        }


        private string userName;

        public string UserName
        {
            get { return userName; }
            set { SetProperty(ref userName , value); }
        }

        private string password;

        public string Password
        {
            get { return password; }
            set { SetProperty(ref password , value); }
        }
    }
}