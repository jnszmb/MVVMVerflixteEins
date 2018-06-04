using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Collections.ObjectModel;

namespace Model
{
    public class Spiel
    {
        ObservableCollection<Spieler> lstSpieler;
        OleDbConnection con = new OleDbConnection();

        public ObservableCollection<Spieler> LstSpieler
        {
            get
            {
                return lstSpieler;
            }

            set
            {
                lstSpieler = value;
            }
        }

        public Spiel()
        {
            con = new OleDbConnection(Properties.Settings1.Default.DB);
            LstSpieler = new ObservableCollection<Spieler>();
        }

        public int Wuerfeln(Spieler spieler)
        {
            Random rand = new Random();
            int zahl = rand.Next(1, 7);
            if (zahl != 1) spieler.Gesamtpunkte += zahl;
            else spieler.Gesamtpunkte = 0;
            return zahl;
        }
        public int LoadSpieler()
        {
            int anzahl = 0;

            OleDbCommand cmd = con.CreateCommand();
            cmd.CommandText = "tSpieler";
            cmd.CommandType = System.Data.CommandType.TableDirect;
            OpenConnection();
            OleDbDataReader reader = cmd.ExecuteReader();
            LstSpieler.Clear();
            while (reader.Read())
            {
                Spieler sp = new Spieler { Id = reader.GetInt32(0), Spielername = reader.GetString(1), Gesamtpunkte = reader.GetInt32(2) };
                LstSpieler.Add(sp);
                anzahl++;
            }
            reader.Close();
            con.Close();
            return anzahl;
        }
        private void OpenConnection()
        {
            if (con.State != System.Data.ConnectionState.Open)
            {
                con.Open();
            }
        }
        public Spieler Registrieren(String Name)
        {
            Spieler sp = null;
            OleDbCommand cmd = con.CreateCommand();
            cmd.CommandText = "Select * from tSpieler where Spielername = '" + Name + "'";
            cmd.CommandType = System.Data.CommandType.Text;
            OpenConnection();
            OleDbDataReader reader = cmd.ExecuteReader();
            OpenConnection();
            if (reader.Read() == false)
            {
                sp = new Spieler();

                sp.Spielername = Name;
                sp.Gesamtpunkte = 0;

                this.SaveSpieler(sp);
                this.LstSpieler.Add(sp);
            }
            reader.Close();
            con.Close();
            return sp;


        }
        private void SaveSpieler(Spieler sp)
        {
            OleDbCommand cmd = con.CreateCommand();
            cmd.Parameters.AddWithValue("name", sp.Spielername);
            cmd.Parameters.AddWithValue("punkte", sp.Gesamtpunkte);
            cmd.CommandText = "Insert Into tSpieler(Spielername,Gesamtpunkte) Values(name,punkte)";
            cmd.CommandType = System.Data.CommandType.Text;
            OpenConnection();
            cmd.ExecuteNonQuery();
            cmd.CommandText = "Select @@identity from tSpieler";
            Int32 id = (Int32)cmd.ExecuteScalar();
            sp.Id = id;
        }
        public void SaveSpieler()
        {
            OleDbCommand cmd = con.CreateCommand();

            cmd.Parameters.Add("punkte", OleDbType.Integer);
            cmd.Parameters.Add("key", OleDbType.Integer);
            cmd.CommandText = "Update tSpieler set Gesamtpunkte = punkte where SpielerId = key";
            OpenConnection();
            foreach (Spieler sp in LstSpieler)
            {
                cmd.Parameters["punkte"].Value = sp.Gesamtpunkte;
                cmd.Parameters["key"].Value = sp.Id;

                cmd.ExecuteNonQuery();
            }
            con.Close();
        }
    }
}
