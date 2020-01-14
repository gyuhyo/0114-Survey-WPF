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

namespace winter_survey
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        Page login;
        Page DashBoard;
        Page SvList;
        Page User;
        Page Chart;
        /*
        Page PgSetting;
        Page AdmList;
        */
        public MainWindow()
        {
            InitializeComponent();

            login = new Adm_Login(this);
            DashBoard = new DashBoard();
            SvList = new Survey_list();
            User = new User();
            Chart = new Chart();
            /*
            login = new Adm_Login();
            login = new Adm_Login();
            */
            Contents_Section.Navigate(login);
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.DragMove();
            }catch(Exception)
            {
                return;
            }
        }

        private void Label_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void LoginLabel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Contents_Section.Navigate(login);
        }

        private void Btn_DashBoard_Click(object sender, RoutedEventArgs e)
        {
            Contents_Section.Navigate(DashBoard);
        }

        private void Btn_SvList_Click(object sender, RoutedEventArgs e)
        {
            Contents_Section.Navigate(SvList);
        }

        private void Btn_Member_Click(object sender, RoutedEventArgs e)
        {
            Contents_Section.Navigate(User);
        }

        private void Btn_SvChart_Click(object sender, RoutedEventArgs e)
        {
            Contents_Section.Navigate(Chart);
        }
    }
}
