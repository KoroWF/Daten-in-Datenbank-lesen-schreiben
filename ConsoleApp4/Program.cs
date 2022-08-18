using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp4
{
    class Program
    {
        static void Main(string[] args)
        {
            SqlConnection conny = new SqlConnection();

            conny.ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\geierT\Documents\datenbank.mdf;Integrated Security=True;Connect Timeout=30";

            conny.Open();

            //ein Nutzer greift auf DB zu
            //ein Nutzer möchte seine Arbeit eventuell rückgängig machen
            //weil viele SQL werden zu einer Transaktion zusammengefasst
            //zum Beispiel: eintragen 50 neue Mitarbeiter -- entweder alle oder keiner 

            //wir starten eine Transaktion
            SqlTransaction trans = conny.BeginTransaction();

            SqlCommand sql = new SqlCommand("update mitarbeiter set gehalt=2000 where id = 3", conny, trans);

            sql.ExecuteNonQuery();

            //ausgeben alle Daten aus Tabelle
            sql = new SqlCommand("select * from mitarbeiter", conny, trans);
            SqlDataReader reader = sql.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine(reader.GetInt32(0));
                Console.WriteLine(reader.GetString(1));
                Console.WriteLine(reader.GetDecimal(2));
                Console.WriteLine(reader.GetString(3));
            }
            reader.Close();


            //wir möchten dieses Update rückgangig machen
            //mit Rollback ist die Transaktion beendet
            trans.Rollback();

            Console.WriteLine("---------------------------------");
            //ausgeben alle Daten aus Tabelle
            sql = new SqlCommand("select * from mitarbeiter", conny, trans);
            reader = sql.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine(reader.GetInt32(0));
                Console.WriteLine(reader.GetString(1));
                Console.WriteLine(reader.GetDecimal(2));
                Console.WriteLine(reader.GetString(3));
            }
            reader.Close();

            //wir bestätogen alle Änderungen in der Datenbank
            //Commit() wird automatisch bei Rollback() gemacht
            //trans.Commit();                                               <----

            Console.WriteLine("---------------------------------");

            //Transaktion mit Haltepunkten - Rollback bis zu einem haltepunkt

            trans = conny.BeginTransaction();

            sql = new SqlCommand("update mitarbeiter set gehalt=9000 where id = 1",conny,trans);
            sql.ExecuteNonQuery();
            
            //wir erstellen einen sicherungspunkt - Rollback bis dahin
            trans.Save("SP1");


            sql = new SqlCommand("delete from mitarbeiter where id = 2", conny, trans);
            sql.ExecuteNonQuery();
            //ausgeben alle Daten aus Tabelle
            sql = new SqlCommand("select * from mitarbeiter", conny, trans);
            reader = sql.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine(reader.GetInt32(0));
                Console.WriteLine(reader.GetString(1));
                Console.WriteLine(reader.GetDecimal(2));
                Console.WriteLine(reader.GetString(3));
            }
            reader.Close();

            //Löschen rückgängig, aber Änderung gehalt soll bleiben
            trans.Rollback("SP1");

            Console.WriteLine("---------------------------------");

            //ausgeben alle Daten aus Tabelle
            sql = new SqlCommand("select * from mitarbeiter", conny, trans);
            reader = sql.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine(reader.GetInt32(0));
                Console.WriteLine(reader.GetString(1));
                Console.WriteLine(reader.GetDecimal(2));
                Console.WriteLine(reader.GetString(3));
            }
            reader.Close();

            conny.Close();
        }
    }
}
