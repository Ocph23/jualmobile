using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MainApp.Helpers
{
    public interface IToas
    {
        Task ShowLong(string message);
        Task ShowShort(string message);
    }
}
