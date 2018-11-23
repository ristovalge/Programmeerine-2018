using System;
using System.Data.SqlClient;
class Baasiproov1
{
    public static void Main(string[] arg)
    {
        string constr = "Data Source=KLASSB315-13\\SQLEXPRESS;" +
          "Initial Catalog=proovibaas; " +
          "Integrated Security=SSPI; Persist Security Info=False";
        string lause = "SELECT * FROM inimesed";
        SqlConnection cn = new SqlConnection(constr);
        cn.Open();
        SqlCommand cm = new SqlCommand(lause, cn);
        SqlDataReader reader = cm.ExecuteReader();
        while (reader.Read())
        {
            Console.Write("{0} {1}  {2} {3}",reader.GetString(1), reader.GetString(2), reader.GetString(3),reader.GetInt32(0));
            Console.WriteLine();
        }
        cn.Close();
        Console.ReadKey(); 
    }
}
