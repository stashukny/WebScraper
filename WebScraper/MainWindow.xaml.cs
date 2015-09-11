using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Net;
using System.Data.SqlClient;
using System.Data;

namespace WebScraper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            // URL: http://en.wikipedia.org/wiki/Main_Page
            WebClient w = new WebClient();
            string s = w.DownloadString("http://www.rotowire.com/daily/nba/defense-vspos.htm");
            //string s = w.DownloadString("http://rotoguru1.com/cgi-bin/hstats.cgi?pos=0&sort=4&game=d&colA=0&daypt=0&xavg=4&show=2&fltr=00");

            // 2.
           List <string> xList = new List <string>();
           //string[] result;

            foreach (LinkItem i in LinkFinder.Find(s))
            {                
                //var result = i.ToString().Split(new[] { '\r', '\n' });
                //xList = result.ToList<string>();
                Debug.WriteLine(i);
            }

            for (int i = xList.Count - 1; i >= 0; i--)
            {             
                if (xList[i].Length > 3)
                {
                    string input = xList[i].Substring(0, 4);
                    if (Regex.IsMatch(input, @"^\d+$") == true)
                    {
                        //InsertData(xList[i]);
                    }
                }    
                else
                {
                    xList.RemoveAt(i);
                }        
            }
        }

        private void InsertData (string row)
        {

            try {

                    int DaysAgo;
                    //List<String> list = new List<String>() { "A", "B", "C" };
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



                            string [] columns = row.Split(';');


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

                    }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.InnerException.ToString());
            }
        }
    }
}
