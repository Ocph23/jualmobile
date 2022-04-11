using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Text;

namespace MainApp.Models
{
    public class ExcelStructure
    {
        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public List<HeaderCell> Headers { get; set; } = new List<HeaderCell>();
        public List<Penjualan> Values { get; set; } = new List<Penjualan>();
    }

    public class HeaderCell
    {
       
        public string Title { get; set; }
        public int  Width{ get; set; }

    }



    



}
