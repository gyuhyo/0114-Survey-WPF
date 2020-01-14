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

namespace winter_survey
{
    /// <summary>
    /// Adm_Login.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Adm_Login : Page
    {
        private static string mysql_str = "server=localhost;port=3306;Database=pkh;Uid=root;Pwd=1234;Charset=utf8"; // 데이터베이스 접속
        MySqlConnection conn = new MySqlConnection(mysql_str);
        MySqlCommand query;
        MySqlDataReader reader;
        MainWindow w2;

        Page dash = new DashBoard();

        public Adm_Login(MainWindow w1)
        {
            InitializeComponent();
            conn.Open();

            w2 = w1;
        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            String sql = "select * from sv_admin where adm_id = @adm_id";
            if (reader != null) reader.Close();
            try
            {
                query = new MySqlCommand();
                query.Connection = conn;
                query.CommandText = sql;
                query.Prepare();
                query.Parameters.AddWithValue("@adm_id", adm_id.Text);
                reader = query.ExecuteReader();

                if (reader.Read())
                {
                    if (reader["adm_pwd"].ToString() == adm_pwd.Password)
                    {
                        w2.LoginLabel.Content = reader["adm_name"].ToString() + "님 반갑습니다.";
                        w2.Contents_Section.Navigate(dash);
                        w2.Btn_DashBoard.IsEnabled = true;
                        w2.Btn_SvList.IsEnabled = true;
                        w2.Btn_Member.IsEnabled = true;
                        w2.Btn_SvChart.IsEnabled = true;
                        w2.Btn_Setting.IsEnabled = true;
                        w2.Btn_Admin.IsEnabled = true;
                    }
                    else
                    {
                        MessageBox.Show("비밀번호가 일치하지 않습니다.");
                    }
                }
                else
                {
                    MessageBox.Show("존재하지 않는 아이디 입니다.");
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
