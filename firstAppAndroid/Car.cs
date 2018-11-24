using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SQLite;

namespace FirstApp
{
    public class Car
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set }
        public string Name { get; set; }
        public string Model { get; set; }
        public int Kw{ get; set; }
        public int Year { get; set; }

    }

    
}