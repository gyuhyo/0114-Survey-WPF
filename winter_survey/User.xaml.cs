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
    /// User.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class User : Page
    {
        private static string mysql_str = "server=localhost;port=3306;Database=pkh;Uid=root;Pwd=1234;Charset=utf8"; // 데이터베이스 접속
        MySqlConnection conn = new MySqlConnection(mysql_str);
        MySqlCommand query;
        MySqlDataReader rs;
        int SelectedUsNo;

        public User()
        {
            InitializeComponent();
            conn.Open();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Load_UserList();
        }

        public void Load_UserList()
        {
            string view_uslist = "select * from sv_user where user_id like @suid and user_name like @suname and user_gender like @sugender";
            if (rs != null) rs.Close();
            query = new MySqlCommand();
            query.Connection = conn;
            query.CommandText = view_uslist;
            query.Prepare();
            query.Parameters.AddWithValue("@suid", "%" + Sch_User_Id.Text + "%");
            query.Parameters.AddWithValue("@suname", "%" + Sch_User_Name.Text + "%");

            String SuGender;
            switch (Sch_User_Gender.Text)
            {
                case "남": SuGender = "M"; break;
                case "여": SuGender = "W"; break;
                default: SuGender = null; break;
            }

            query.Parameters.AddWithValue("@sugender", "%" + SuGender + "%");

            rs = query.ExecuteReader();

            DataTable Dtable = new DataTable();

            Dtable.Columns.Add("UserNo", typeof(string));
            Dtable.Columns.Add("UserId", typeof(string));
            Dtable.Columns.Add("UserPwd", typeof(string));
            Dtable.Columns.Add("UserName", typeof(string));
            Dtable.Columns.Add("UserBirth", typeof(string));
            Dtable.Columns.Add("UserGender", typeof(string));
            Dtable.Columns.Add("UserRgdate", typeof(string));

            while (rs.Read())
            {
                Dtable.Rows.Add(
                    rs["user_no"].ToString(),
                    rs["user_id"].ToString(),
                    rs["user_pwd"].ToString(),
                    rs["user_name"].ToString(),
                    rs["user_birth"].ToString(),
                    rs["user_gender"].ToString(),
                    rs["sysdate"].ToString().Substring(0, 10)
                );
            }
            Console.WriteLine(Dtable.DefaultView);
            User_List.ItemsSource = Dtable.DefaultView;
        }

        private void SchBtn_Click(object sender, RoutedEventArgs e)
        {
            Load_UserList();
        }

        private void User_set(String sType, int uNo)
        {
            String Query = null;

            switch(sType)
            {
                case "insert":
                    Query = "insert into sv_user (user_id, user_pwd, user_name, user_birth, user_gender)" +
                            " values(@us_id, @us_pwd, @us_name, @us_birth, @us_gender)";
                    break;
                case "update":
                    Query = "update sv_user set user_pwd = @us_pwd, user_name = @us_name, user_birth = @us_birth, user_gender = @us_gender" +
                            " where user_no = @us_no";
                    break;
                case "delete":
                    Query = "delete from sv_user where user_no = @us_no";
                    break;
            }

            if (rs != null) rs.Close();
            query = new MySqlCommand();
            query.Connection = conn;
            query.CommandText = Query;
            query.Prepare();
            query.Parameters.AddWithValue("@us_id", us_id.Text);
            query.Parameters.AddWithValue("@us_pwd", us_pwd.Text);
            query.Parameters.AddWithValue("@us_name", us_name.Text);
            query.Parameters.AddWithValue("@us_birth", us_birth.Text);

            String uGender = (us_genderM.IsChecked.Value == true) ? "M" : "W";

            query.Parameters.AddWithValue("@us_gender", uGender);
            query.Parameters.AddWithValue("@us_no", uNo);
            query.ExecuteNonQuery();

            Load_UserList();
            reset();
        }

        private void reset()
        {
            us_id.Text = "";
            us_pwd.Text = "";
            us_name.Text = "";
            us_birth.Text = "";
            us_genderM.IsChecked = true;
            uInsBtn.IsEnabled = true;
            uUpdBtn.IsEnabled = false;
            uDelBtn.IsEnabled = false;
            us_id.IsEnabled = true;
            SelectedUsNo = 0;
            User_List.SelectedIndex = -1;
        }

        private void uInsBtn_Click(object sender, RoutedEventArgs e)
        {
            User_set("insert", 0);
        }

        private void uCclBtn_Click(object sender, RoutedEventArgs e)
        {
            reset();
        }

        private void uUpdBtn_Click(object sender, RoutedEventArgs e)
        {
            User_set("update", SelectedUsNo);
        }

        private void uDelBtn_Click(object sender, RoutedEventArgs e)
        {
            User_set("delete", SelectedUsNo);
        }

        private void User_List_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataRowView drv = User_List.SelectedItem as DataRowView;

            if (drv == null) return;

            us_id.Text = drv[1].ToString();
            us_pwd.Text = drv[2].ToString();
            us_name.Text = drv[3].ToString();
            us_birth.Text = drv[4].ToString();

            if (drv[5].ToString() == "M")
            {
                us_genderM.IsChecked = true;
            }
            else
            {
                us_genderW.IsChecked = true;
            }

            SelectedUsNo = Convert.ToInt32(drv[0].ToString());

            us_id.IsEnabled = false;
            uInsBtn.IsEnabled = false;
            uUpdBtn.IsEnabled = true;
            uDelBtn.IsEnabled = true;
        }
    }
}
