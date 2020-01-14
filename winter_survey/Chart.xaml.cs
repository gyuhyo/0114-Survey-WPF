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
using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Defaults;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.Data;


namespace winter_survey
{
    /// <summary>
    /// Chart.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Chart : Page
    {
        private static string mysql_str = "server=localhost;port=3306;Database=pkh;Uid=root;Pwd=1234;Charset=utf8"; // 데이터베이스 접속
        MySqlConnection conn = new MySqlConnection(mysql_str);
        MySqlCommand query;
        MySqlDataReader rs;

        public Chart()
        {
            InitializeComponent();
            conn.Open();
        }

        public SeriesCollection SeriesCollection { get; set; }

        private void sch_btn_Click(object sender, RoutedEventArgs e)
        {
            string view_svlist = "SELECT sv_top.sv_no sv_no, sv_subject, sv_edate, sv_realname, COUNT(*) sv_pcnt" +
                                 " FROM (SELECT * from sv_top WHERE sv_subject like @SvSubject) sv_top" +
                                 " LEFT JOIN (SELECT sr_user, sv_no FROM sv_result GROUP BY sv_no, sr_user) sv_result" +
                                 " ON sv_top.sv_no = sv_result.sv_no GROUP by sv_no";
            if (rs != null) rs.Close();
            query = new MySqlCommand();
            query.Connection = conn;
            query.CommandText = view_svlist;
            query.Prepare();
            query.Parameters.AddWithValue("@SvSubject", "%" + Sch_SvSubject.Text + "%");
            rs = query.ExecuteReader();

            DataTable Dtable = new DataTable();

            Dtable.Columns.Add("SvIndexNo", typeof(string));
            Dtable.Columns.Add("SvSubject", typeof(string));
            Dtable.Columns.Add("SvRealname", typeof(string));
            Dtable.Columns.Add("SvEdate", typeof(string));
            Dtable.Columns.Add("SvPcnt", typeof(string));

            while (rs.Read())
            {
                Dtable.Rows.Add(
                    rs["sv_no"].ToString(),
                    rs["sv_subject"].ToString(),
                    rs["sv_realname"].ToString(),
                    rs["sv_edate"].ToString(),
                    rs["sv_pcnt"].ToString()
                );
            }

            ch_sv_top.ItemsSource = Dtable.DefaultView;
        }

        private void ch_sv_top_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataRowView srv = ch_sv_top.SelectedItem as DataRowView;

            Console.WriteLine(srv[0].ToString());

            string view_svlist = "SELECT" +
                                 " sc_subject, sc_type, sc_tcnt, sc_tval1, sc_tval2, sc_tval3, sc_tval4, sc_tval5," +
                                 " ifnull(sel1, 0) sel1, ifnull(sel2, 0) sel2, ifnull(sel3, 0) sel3, ifnull(sel4, 0) sel4, ifnull(sel5, 0) sel5" +
                                 " FROM(SELECT * FROM sv_child WHERE sv_no = @svno AND sc_type IN('체크박스', '라디오박스')) sv_child" +
                                 " LEFT JOIN(SELECT sv_no, sc_no," +
                                 " COUNT(if (sr_result = '1', sr_result, null)) sel1," +
                                 " COUNT(if (sr_result = '2', sr_result, null)) sel2," +
                                 " COUNT(if (sr_result = '3', sr_result, null)) sel3," +
                                 " COUNT(if (sr_result = '4', sr_result, null)) sel4," +
                                 " COUNT(if (sr_result = '5', sr_result, null)) sel5" +
                                 " FROM sv_result GROUP BY sv_no, sc_no) sv_result" +
                                 " ON sv_child.sv_no = sv_result.sv_no AND sv_child.sc_no = sv_result.sc_no";
            if (rs != null) rs.Close();
            query = new MySqlCommand();
            query.Connection = conn;
            query.CommandText = view_svlist;
            query.Prepare();
            query.Parameters.AddWithValue("@svno", srv[0].ToString());
            query.Parameters.AddWithValue("@SvSubject", "%" + Sch_SvSubject.Text + "%");
            rs = query.ExecuteReader();

            DataTable CHtable = new DataTable();

            CHtable.Columns.Add("SvCSubject", typeof(string));
            CHtable.Columns.Add("SvCType", typeof(string));
            CHtable.Columns.Add("SvCTcnt", typeof(string));
            CHtable.Columns.Add("SvCTV1", typeof(string));
            CHtable.Columns.Add("SvCTV2", typeof(string));
            CHtable.Columns.Add("SvCTV3", typeof(string));
            CHtable.Columns.Add("SvCTV4", typeof(string));
            CHtable.Columns.Add("SvCTV5", typeof(string));
            CHtable.Columns.Add("SvCTVc1", typeof(string));
            CHtable.Columns.Add("SvCTVc2", typeof(string));
            CHtable.Columns.Add("SvCTVc3", typeof(string));
            CHtable.Columns.Add("SvCTVc4", typeof(string));
            CHtable.Columns.Add("SvCTVc5", typeof(string));

            while (rs.Read())
            {
                CHtable.Rows.Add(
                    rs["sc_subject"].ToString(),
                    rs["sc_type"].ToString(),
                    rs["sc_tcnt"].ToString(),
                    rs["sc_tval1"].ToString(),
                    rs["sc_tval2"].ToString(),
                    rs["sc_tval3"].ToString(),
                    rs["sc_tval4"].ToString(),
                    rs["sc_tval5"].ToString(),
                    rs["sel1"].ToString(),
                    rs["sel2"].ToString(),
                    rs["sel3"].ToString(),
                    rs["sel4"].ToString(),
                    rs["sel5"].ToString()
                );
            }

            ch_sv_child.ItemsSource = CHtable.DefaultView;
        }

        private void ch_sv_child_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataContext = null;

            DataRowView drv = ch_sv_child.SelectedItem as DataRowView;
            if (drv != null)
            {
                int s1 = Convert.ToInt32(drv[8]), s2 = Convert.ToInt32(drv[9]), s3 = Convert.ToInt32(drv[10]), s4 = Convert.ToInt32(drv[11]), s5 = Convert.ToInt32(drv[12]);
                /*
                int total = s1 + s2 + s3 + s4 + s5;
                int oneScore;
                if (total > 0) oneScore = 100 / total;
                else oneScore = 0;
                int v1 = oneScore * s1, v2 = oneScore * s2, v3 = oneScore * s3, v4 = oneScore * s4, v5 = oneScore * s5;
                */
                SeriesCollection = new SeriesCollection
                {
                    new PieSeries
                    {
                        Title = drv[3].ToString(),
                        Values = new ChartValues<ObservableValue> { new ObservableValue(s1) },
                        DataLabels = true
                    },
                    new PieSeries
                    {
                        Title = drv[4].ToString(),
                        Values = new ChartValues<ObservableValue> { new ObservableValue(s2) },
                        DataLabels = true
                    },
                    new PieSeries
                    {
                        Title = drv[5].ToString(),
                        Values = new ChartValues<ObservableValue> { new ObservableValue(s3) },
                        DataLabels = true
                    },
                    new PieSeries
                    {
                        Title = drv[6].ToString(),
                        Values = new ChartValues<ObservableValue> { new ObservableValue(s4) },
                        DataLabels = true
                    },
                    new PieSeries
                    {
                        Title = drv[7].ToString(),
                        Values = new ChartValues<ObservableValue> { new ObservableValue(s5) },
                        DataLabels = true
                    }
                };

                Console.WriteLine(this);
                DataContext = this;
            }
        }
    }
}
