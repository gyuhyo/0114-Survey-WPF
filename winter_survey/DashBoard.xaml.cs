using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.Data;

namespace winter_survey
{
    /// <summary>
    /// DashBoard.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class DashBoard : Page
    {
        private static string mysql_str = "server=localhost;port=3306;Database=pkh;Uid=root;Pwd=1234;Charset=utf8"; // 데이터베이스 접속
        MySqlConnection conn = new MySqlConnection(mysql_str);
        MySqlCommand query;
        MySqlDataReader rs;
        public DashBoard()
        {
            InitializeComponent();
            conn.Open();
            DashBoard_load();
        }

        private void DashBoard_load()
        {
            string view_svlist = "select * from sv_top order by sv_wdate desc";
            query = new MySqlCommand();
            query.Connection = conn;
            query.CommandText = view_svlist;
            query.Prepare();

            rs = query.ExecuteReader();

            DataTable Dtable = new DataTable();

            Dtable.Columns.Add("SvSubject", typeof(string));
            Dtable.Columns.Add("SvWdate", typeof(string));

            while (rs.Read())
            {
                Dtable.Rows.Add(new string[] { rs["sv_subject"].ToString(), rs["sv_wdate"].ToString().Substring(0, 10) });
            }

            sv_list_Grid.ItemsSource = Dtable.DefaultView;
        }
    }
}
