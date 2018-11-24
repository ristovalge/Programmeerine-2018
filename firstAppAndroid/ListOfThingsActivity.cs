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
using static Android.Widget.AdapterView;
using SQLite;
using System.IO;

namespace FirstApp
{
    [Activity(Label = "ListOfThingsActivity")]
    public class ListOfThingsActivity : Activity
    {
        List<Car> ListOfCars;
        SQLiteConnection db;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.list_layout);
            // Create your application here
            var listView = FindViewById<ListView>(Resource.Id.listView1);
            var nameEditText = FindViewById<EditText>(Resource.Id.editText1);
            var modelEditText = FindViewById<EditText>(Resource.Id.editText2);
            var kwEditText = FindViewById<EditText>(Resource.Id.editText3);
            var addButton = FindViewById<Button>(Resource.Id.button1);
            GenerateCars();
           // ListOfCars = GenerateCars();

            listView.Adapter = new CustomAdapter(this,  ListOfCars);
            listView.ItemClick += (object sender, ItemClickEventArgs e) =>


                {
                    var car = listView.GetItemAtPosition(e.Position);
               Toast.MakeText(this, "Vajutasid", ToastLength.Short).Show();
                };

            addButton.Click += delegate
            {
                var car = new Car();
                car.Name = nameEditText.Text;
                car.Model = modelEditText.Text;
                car.Kw = int.Parse(kwEditText.Text);
                ListOfCars.Add(car);
                listView.Adapter = new CustomAdapter(this, ListOfCars);
                SaveCarsToDatabase();
            };


            CreateDatabase();
            CreateTable();
        }
        public void SaveCarsToDatabase()
        {
            foreach (var car in ListOfCars)
            {
                db.Insert(car);
            }
        }

        public void CreateDatabase()
        {
            string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "mydatabase.db3");
            db = new SQLiteConnection(dbPath);
        }

        public void CreateTable()
        {
            db.CreateTable<Car>();
            
        }

        private void GenerateCars()
        {
            var listOfCars = new List<Car>();
            var car = new Car();
            car.Name = "Ferrari";
            car.Model = "Modena";
            car.Kw = 360;
            car.Year = 2013;
            listOfCars.Add(car);

         
             car = new Car();
            car.Name = "Volkswage";
            car.Model = "Passat";
            car.Kw = 110;
            car.Year = 2018;
            listOfCars.Add(car);

     
            car = new Car();
            car.Name = "Lada";
            car.Model = "07";
            car.Kw = 85;
            car.Year = 2016;
            listOfCars.Add(car);

           
             car = new Car();
            car.Name = "Lexus";
            car.Model = "RX450h";
            car.Kw = 127;
            car.Year = 2006;
            listOfCars.Add(car);

            
            car = new Car();
            car.Name = "Tesla";
            car.Model = "Model S";
            car.Kw = 280;
            car.Year = 2009;
            listOfCars.Add(car);

             ListOfCars = listOfCars;
        }
    }
}



//{ "volvo", "volkswagen", "bmw", "Lexus", "Ford", "Toyota", "bmw", "Lexus", "volvo", "volkswagen", "bmw", "Lexus", "volvo", "volkswagen", "bmw", "Lexus", "volvo", "volkswagen", "bmw", "Lexus" };