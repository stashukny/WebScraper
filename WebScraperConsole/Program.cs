using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace WebScraperConsole
{
    internal class Program
    {
        private const string path = "http://rotoguru1.com/cgi-bin/hstats.cgi?pos=0&sort=4&game=d&colA=0&daypt=0&xavg=4&show=2&fltr=00";
        private const string pattern = "pre";

        private static void Main(string[] args)
        {
            WebClient w = new WebClient();
            string s = w.DownloadString(path);

            List<string> xList = new List<string>();

            foreach (LinkItem i in LinkFinder.Find(s, pattern))
            {
                var result = i.ToString().Split(new[] { '\r', '\n' });
                xList = result.ToList<string>();
            }

            for (int i = xList.Count - 1; i >= 0; i--)
            {
                if (xList[i].Length > 3)
                {
                    string input = xList[i].Substring(0, 4);
                    if (Regex.IsMatch(input, @"^\d+$") == true)
                    {
                        InsertPlayers(xList[i]);
                    }
                }
                else
                {
                    xList.RemoveAt(i);
                }
            }
        }

        private static void InsertPlayers(string row)
        {
            try
            {
                using (var con = new SqlConnection("Persist Security Info=False;Integrated Security=true;Initial Catalog=NBA;server=(local)"))
                {
                    con.Open();

                    using (var cmd = new SqlCommand("INSERT INTO RotoPlayersRaw([GID],[ESPN ID],[Pos],[Name],[Team],[Salary],[Salary Change],[Points],[GP],[Pts Game],[Pts G $],[Pts G(alt)],[Last pts],[Days ago],[Schedule],[Period],[DateTimeStamp])VALUES(@GID,@ESPNID,@Pos,@Name,@Team,@Salary,@SalaryChange,@Points,@GP,@PtsGame,@PtsG$,@PtsGalt,@Lastpts,@Daysago,@Schedule,@Period,@DateTimeStamp)", con))
                    {
                        cmd.Parameters.Add("@GID", SqlDbType.SmallInt);
                        cmd.Parameters.Add("@ESPNID", SqlDbType.Int);
                        cmd.Parameters.Add("@POS", SqlDbType.SmallInt);
                        cmd.Parameters.Add("@Name", SqlDbType.VarChar);
                        cmd.Parameters.Add("@Team", SqlDbType.VarChar);
                        cmd.Parameters.Add("@Salary", SqlDbType.Int);
                        cmd.Parameters.Add("@SalaryChange", SqlDbType.Int);
                        cmd.Parameters.Add("@Points", SqlDbType.Float);
                        cmd.Parameters.Add("@GP", SqlDbType.SmallInt);
                        cmd.Parameters.Add("@PtsGame", SqlDbType.Float);
                        cmd.Parameters.Add("@PtsG$", SqlDbType.Float);
                        cmd.Parameters.Add("@PtsGalt", SqlDbType.Float);
                        cmd.Parameters.Add("@LastPts", SqlDbType.Float);
                        cmd.Parameters.Add("@Daysago", SqlDbType.SmallInt);
                        cmd.Parameters.Add("@Schedule", SqlDbType.VarChar);
                        cmd.Parameters.Add("@Period", SqlDbType.SmallInt);
                        cmd.Parameters.Add("@DateTimeStamp", SqlDbType.DateTime);

                        string[] columns = row.Split(';');

                        cmd.Parameters["@GID"].Value = columns[0];
                        cmd.Parameters["@ESPNID"].Value = columns[1];
                        cmd.Parameters["@POS"].Value = columns[2];
                        cmd.Parameters["@Name"].Value = columns[3];
                        cmd.Parameters["@Team"].Value = columns[4];
                        cmd.Parameters["@Salary"].Value = columns[5];
                        cmd.Parameters["@SalaryChange"].Value = columns[6];
                        cmd.Parameters["@Points"].Value = columns[7];
                        cmd.Parameters["@GP"].Value = columns[8];
                        cmd.Parameters["@PtsGame"].Value = columns[9];
                        cmd.Parameters["@PtsG$"].Value = columns[10];
                        cmd.Parameters["@PtsGalt"].Value = columns[11];
                        cmd.Parameters["@LastPts"].Value = columns[12];

                        if (columns[13] == "")
                        {
                            cmd.Parameters["@Daysago"].Value = 0;
                        }
                        else
                        {
                            cmd.Parameters["@Daysago"].Value = columns[13];
                        }

                        cmd.Parameters["@Schedule"].Value = "";
                        cmd.Parameters["@Period"].Value = 0;
                        cmd.Parameters["@DateTimeStamp"].Value = DateTime.Today;

                        int rowsAffected = cmd.ExecuteNonQuery();
                    }

                    con.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException.ToString());
            }
        }
    }
}