using System;
using System.Data.Odbc;
class Baasiproov2a
{
    public static void Main(string[] arg)
    {
        string constr = "Driver={Microsoft Access Driver (*.mdb)}; " +
          //"DBQ=z:\\B315\\KTA17E\\Kauplused.mdb; " +
          "DBQ=c:\\siin\\Kauplused.mdb; " +
          "Trusted_Connection=yes";
        string lause = "SELECT * FROM Kauplused WHERE Kauplus LIKE '%Selver' ";
        OdbcConnection cn = new OdbcConnection(constr);
        cn.Open();
        OdbcCommand cm = new OdbcCommand(lause, cn);
        OdbcDataReader reader = cm.ExecuteReader();
        while (reader.Read())
        {
            Console.WriteLine("{0} {1} {2} ", reader.GetString(0), reader.GetString(1), reader.GetString(2));
        }
        cn.Close();
        Console.ReadKey();
    }
}
