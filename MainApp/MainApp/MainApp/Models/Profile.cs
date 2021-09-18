using System;
using System.Collections.Generic;
using System.Text;

namespace MainApp.Models
{
    public class Profile:BaseNotify
    {

        private string namaUsaha;

        public string NamaUsaha
        {
            get { return namaUsaha; }
            set { SetProperty(ref namaUsaha , value); }
        }

        private string alamat;

        public string Alamat
        {
            get { return alamat; }
            set { SetProperty(ref alamat , value); }
        }


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
