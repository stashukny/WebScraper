using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;

namespace WebScraperTeams
{
    //struct Teams
    //{
    //    public string Team;
    //    public float VsPos;
    //    public float SeasonAvg;
    //    public float Last4Avg;
    //    public float Last10Avg;
    //    public float Pts;
    //    public float Reb;
    //    public float Ast;
    //    public float Stl;
    //    public float Blk;
    //    public float ThreePM;
    //    public float FG;
    //    public float FT;
    //    public float TO;        

    //}

    class Program
    {
        static void Main(string[] args)
        {
            string path = "http://www.rotowire.com/daily/nba/defense-vspos.htm";            
            
            WebClient w = new WebClient();
            string s = w.DownloadString(path);

            List<string> xList = new List<string>();

            int counter = 0;
            //string next;
            //Teams t;
            string all = "";

            foreach (LinkItem i in LinkFinder.Find(s))
            {

                counter += 1;
                all = all + i + ";";

                int r = counter % 14;
                if (r == 0)
                {
                    all = all.Replace("\n\t", "") + "\n";
                    if (!String.IsNullOrEmpty(all))
                    {
                        Debug.WriteLine(all);
                        //var result = all.ToString().Split(new[] { ';', '\n' });
                        //xList = result.ToList<string>();                
                        xList.Add(all);
                        all = "";
                    }                    
                }

                
            }

            for (int i = xList.Count - 1; i >= 0; i--)
            {
                if (xList[i].Length > 3)
                {
                    InsertTeams(xList[i]);
                }
                else
                {
                    xList.RemoveAt(i);
                }
            }
        }

        private static void InsertTeams(string row)
        {

            try
            {

                using (var con = new SqlConnection("Persist Security Info=False;Integrated Security=true;Initial Catalog=NBA;server=(local)"))
                {
                    con.Open();

                    using (var cmd = new SqlCommand("INSERT INTO RotoTeamsDefense([Team],[VsPos],[SeasonAvg],[Last5Avg],[Last10Avg],[Pts],[Reb],[Ast],[Stl],[Blk],[ThreePM],[FG],[FT],[TO],[DateTimeStamp]) VALUES(@Team,@VsPos,@SeasonAvg,@Last5Avg,@Last10Avg,@Pts,@Reb,@Ast,@STL,@Blk,@ThreePM,@FG,@FT,@TO,@DateTimeStamp)", con))
                    {
                        cmd.Parameters.Add("@Team", SqlDbType.VarChar);
                        cmd.Parameters.Add("@VsPos", SqlDbType.VarChar);
                        cmd.Parameters.Add("@SeasonAvg", SqlDbType.Float);
                        cmd.Parameters.Add("@Last5Avg", SqlDbType.Float);
                        cmd.Parameters.Add("@Last10Avg", SqlDbType.Float);
                        cmd.Parameters.Add("@Pts", SqlDbType.Float);
                        cmd.Parameters.Add("@Reb", SqlDbType.Float);
                        cmd.Parameters.Add("@Ast", SqlDbType.Float);
                        cmd.Parameters.Add("@Stl", SqlDbType.Float);
                        cmd.Parameters.Add("@Blk", SqlDbType.Float);
                        cmd.Parameters.Add("@ThreePM", SqlDbType.Float);
                        cmd.Parameters.Add("@FG", SqlDbType.Float);
                        cmd.Parameters.Add("@FT", SqlDbType.Float);
                        cmd.Parameters.Add("@TO", SqlDbType.Float);
                        cmd.Parameters.Add("@DateTimeStamp", SqlDbType.DateTime);
                     



                        string[] columns = row.Split(';');


                        cmd.Parameters["@Team"].Value = columns[0];
                        cmd.Parameters["@VsPos"].Value = columns[1];
                        cmd.Parameters["@SeasonAvg"].Value = columns[2];
                        cmd.Parameters["@Last5Avg"].Value = columns[3];
                        cmd.Parameters["@Last10Avg"].Value = columns[4];
                        cmd.Parameters["@Pts"].Value = columns[5];
                        cmd.Parameters["@Reb"].Value = columns[6];
                        cmd.Parameters["@Ast"].Value = columns[7];
                        cmd.Parameters["@Stl"].Value = columns[8];
                        cmd.Parameters["@Blk"].Value = columns[9];
                        cmd.Parameters["@ThreePM"].Value = columns[10];
                        cmd.Parameters["@FG"].Value = columns[11];
                        cmd.Parameters["@FT"].Value = columns[12];
                        cmd.Parameters["@TO"].Value = columns[13];

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
