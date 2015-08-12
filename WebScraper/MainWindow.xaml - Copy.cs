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
            // Scrape links from wikipedia.org

            // 1.
            // URL: http://en.wikipedia.org/wiki/Main_Page
            WebClient w = new WebClient();
            //string s = w.DownloadString("http://www.basketball-reference.com/leagues/NBA_2015_ratings.html");
            string s = w.DownloadString("http://rotoguru1.com/cgi-bin/hstats.cgi?pos=0&sort=4&game=d&colA=0&daypt=0&xavg=4&show=2&fltr=00");

            // 2.
           List <string> xList = new List <string>();
           //string[] result;

            foreach (LinkItem i in LinkFinder.Find(s, "pre"))
            {                
                var result = i.ToString().Split(new[] { '\r', '\n' });
                xList = result.ToList<string>();
                Debug.WriteLine(i);
            }

            for (int i = xList.Count - 1; i >= 0; i--)
            {             
                if (xList[i].Length > 3)
                {
                    string input = xList[i].Substring(0, 4);
                    if (Regex.IsMatch(input, @"^\d+$") == false)
                    {
                        xList.RemoveAt(i);
                    }
                }    
                else
                {
                    xList.RemoveAt(i);
                }        
            }
        }

        private void InsertDate (List<string> list)
        {
            //List<String> list = new List<String>() { "A", "B", "C" };
            using (var con = new SqlConnection("Persist Security Info=False;Integrated Security=true;Initial Catalog=NBA;server=(local)"))
            {
                con.Open();
                using (var cmd = new SqlCommand("INSERT INTO RotoPlayersRaw([GID],[ESPN ID],[Pos],[Name],[Team],[Salary],[Salary Change],[Points],[GP],[Pts Game],[Pts G $],[Pts G(alt)],[Last pts],[Days ago],[Schedule],[Period],[DateTimeStamp])VALUES(@GID,@ESPNID,@Pos,@Name,@Team,@Salary,@SalaryChange,@Points,@GP,@PtsGame,@PtsG$,@PtsGalt,@Lastpts,@Daysago,@Schedule,@Period,@DateTimeStamp)", con))
                {
                    cmd.Parameters.Add("@GID", SqlDbType.VarChar);
                    cmd.Parameters.Add("@ESPNID", SqlDbType.VarChar);
                    cmd.Parameters.Add("@POS", SqlDbType.VarChar);
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

                    foreach (string value in list)
                    {
                        //Split the values

                        string [] data = value.Split(';');


                        cmd.Parameters["@Column"].Value = value;
                        int rowsAffected = cmd.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
