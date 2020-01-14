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
    /// Survey_list.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Survey_list : Page
    {
        private static string mysql_str = "server=localhost;port=3306;Database=pkh;Uid=root;Pwd=1234;Charset=utf8"; // 데이터베이스 접속
        MySqlConnection conn = new MySqlConnection(mysql_str);
        MySqlCommand query;
        MySqlDataReader rs;

        DataTable ChildTable = new DataTable();
        List<int> UpdateRow = new List<int>();
        List<int> DeleteRow = new List<int>();
        List<int> AddRow = new List<int>();
        int UpdClickRow, ReadSvIndex, ChildNo = 1;
        String Item_Val3, Item_Val4, Item_Val5 = null;
        TextBox tbox1, tbox2, tbox3, getObject;
        bool UpdateMode = false;

        #region 그리드 헤더 초기화
        public Survey_list()
        {
            InitializeComponent();
            conn.Open();
            SvList_load();

            ChildTable.Columns.Add("SvCNo", typeof(string));
            ChildTable.Columns.Add("SvCSubject", typeof(string));
            ChildTable.Columns.Add("SvCType", typeof(string));
            ChildTable.Columns.Add("SvCTypeCnt", typeof(string));
            ChildTable.Columns.Add("SvCTV1", typeof(string));
            ChildTable.Columns.Add("SvCTV2", typeof(string));
            ChildTable.Columns.Add("SvCTV3", typeof(string));
            ChildTable.Columns.Add("SvCTV4", typeof(string));
            ChildTable.Columns.Add("SvCTV5", typeof(string));
        }
        #endregion

        #region 페이지 로드시 설문 목록 출력 부분
        private void SvList_load()
        {
            string view_svlist = "select * from sv_top where sv_subject Like @SvSubject and sv_writer Like @SvWriter and sv_sdate >= @SvSdate and sv_edate <= @SvEdate order by sv_wdate desc";
            if (rs != null) rs.Close();
            query = new MySqlCommand();
            query.Connection = conn;
            query.CommandText = view_svlist;
            query.Prepare();
            query.Parameters.AddWithValue("@SvSubject", "%" + SchSubject.Text + "%");
            query.Parameters.AddWithValue("@SvWriter", "%" + SchWriter.Text + "%");

            if (SchSdate.Text == "" || SchSdate == null) query.Parameters.AddWithValue("@SvSdate", "1990-01-01");
            else query.Parameters.AddWithValue("@SvSdate", SchSdate.Text);

            if (SchEdate.Text == "" || SchEdate == null) query.Parameters.AddWithValue("@SvEdate", "2099-12-31");
            else query.Parameters.AddWithValue("@SvEdate", SchEdate.Text);

            rs = query.ExecuteReader();

            DataTable Dtable = new DataTable();

            Dtable.Columns.Add("SvNo", typeof(string));
            Dtable.Columns.Add("SvIndexNo", typeof(string));
            Dtable.Columns.Add("SvSubject", typeof(string));         
            Dtable.Columns.Add("SvRealname", typeof(string));
            Dtable.Columns.Add("SvWdate", typeof(string));
            Dtable.Columns.Add("SvSdate", typeof(string));
            Dtable.Columns.Add("SvEdate", typeof(string));
            Dtable.Columns.Add("SvOpen", typeof(string));
            Dtable.Columns.Add("SvWriter", typeof(string));

            int i = 1;

            while (rs.Read())
            {
                Dtable.Rows.Add(
                    i.ToString(),
                    rs["sv_no"].ToString(),
                    rs["sv_subject"].ToString(),
                    rs["sv_realname"].ToString(),
                    rs["sv_wdate"].ToString().Substring(0, 10),
                    rs["sv_sdate"].ToString().Substring(0, 10),
                    rs["sv_edate"].ToString().Substring(0, 10),
                    rs["sv_open"].ToString(),
                    rs["sv_writer"].ToString()
                );

                i++;
            }

            sv_list_Grid.ItemsSource = Dtable.DefaultView;
        }
        #endregion

        #region 검색 버튼 클릭
        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SvList_load();
        }
        #endregion

        #region 항목 타입 선택시 컨트롤 Visbility 유무
        private void Add_Item_type_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Add_Item_type.SelectedIndex >= 2)
            {
                Add_Item_Val_1.Visibility = Visibility.Visible;
                Add_Item_Val_2.Visibility = Visibility.Visible;
                Add_Item.Visibility = Visibility.Visible;
            }
            else
            {
                Add_Item_Val_1.Visibility = Visibility.Hidden;
                Add_Item_Val_2.Visibility = Visibility.Hidden;

                reset1();

                Add_frm.Children.Remove(tbox1);
                Add_frm.Children.Remove(tbox2);
                Add_frm.Children.Remove(tbox3);

                Add_Item.Visibility = Visibility.Hidden;
                Canvas.SetLeft(Add_Item, 374);
            }
        }
        #endregion

        void reset1()
        {
            Add_Item_Val_1.Text = null;
            Add_Item_Val_2.Text = null;

            if (Add_frm.Children.Contains(tbox1)) { tbox1.Text = null; }
            if (Add_frm.Children.Contains(tbox2)) { tbox2.Text = null; }
            if (Add_frm.Children.Contains(tbox3)) { tbox3.Text = null; }
        }

        void reset2()
        {
            Add_frm.Children.Remove(tbox1);
            Add_frm.Children.Remove(tbox2);
            Add_frm.Children.Remove(tbox3);
            Add_Item_type.Text = null;
            Add_Item.Visibility = Visibility.Hidden;
            Canvas.SetLeft(Add_Item, 374);
        }

        void reset3()
        {
            Sv_Top_Subject.Text = null;
            Sv_Top_Sdate.Text = null;
            Sv_Top_Edate.Text = null;
            Sv_Top_Open.Text = null;
            Sv_Top_Realname.Text = null;
            UpdateMode = false;
            Sv_Child_Grid.ItemsSource = null;
            ChildTable.Clear();
            ChildNo = 1;
        }

        void reset4()
        {
            Btn_SvChildAdd.IsEnabled = true;
            Btn_SvChildDel.IsEnabled = false;
            Btn_SvChildUpd.IsEnabled = false;
        }

        void reset5()
        {
            UpdateRow.Clear();
            DeleteRow.Clear();
            AddRow.Clear();
        }

        #region Tab1 그리드에서 설문조사 더블 클릭시
        private void sv_list_Grid_Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            Add_Item_Subject.Text = null;

            reset1();
            reset2();
            reset3();
            reset5();

            sv_tab.SelectedIndex = 1;
            e.Handled = true;

            var SelSv = sv_list_Grid.SelectedItem as DataRowView;

            Sv_Top_Subject.Text = SelSv.Row["SvSubject"].ToString();
            Sv_Top_Sdate.Text = SelSv.Row["SvSdate"].ToString();
            Sv_Top_Edate.Text = SelSv.Row["SvEdate"].ToString();
            Sv_Top_Realname.Text = SelSv.Row["SvRealname"].ToString();
            Sv_Top_Open.Text = SelSv.Row["SvOpen"].ToString();

            string view_svlist = "select * from sv_child where sv_no = @SelSvNo";
            if (rs != null) rs.Close();
            query = new MySqlCommand();
            query.Connection = conn;
            query.CommandText = view_svlist;
            query.Prepare();
            query.Parameters.AddWithValue("@SelSvNo", SelSv.Row["SvIndexNo"]);
            rs = query.ExecuteReader();

            ChildTable = new DataTable();

            ChildTable.Columns.Add("SvCNo", typeof(string));
            ChildTable.Columns.Add("SvCSubject", typeof(string));
            ChildTable.Columns.Add("SvCType", typeof(string));
            ChildTable.Columns.Add("SvCTypeCnt", typeof(string));
            ChildTable.Columns.Add("SvCTV1", typeof(string));
            ChildTable.Columns.Add("SvCTV2", typeof(string));
            ChildTable.Columns.Add("SvCTV3", typeof(string));
            ChildTable.Columns.Add("SvCTV4", typeof(string));
            ChildTable.Columns.Add("SvCTV5", typeof(string));

            while (rs.Read())
            {
                ChildTable.Rows.Add(
                rs["sc_no"].ToString(),
                rs["sc_subject"].ToString(),
                rs["sc_type"].ToString(),
                rs["sc_tcnt"].ToString(),
                rs["sc_tval1"].ToString(),
                rs["sc_tval2"].ToString(),
                rs["sc_tval3"].ToString(),
                rs["sc_tval4"].ToString(),
                rs["sc_tval5"].ToString()
                );
                ChildNo = Convert.ToInt32(rs["sc_no"].ToString()) + 1;
                Sv_Child_Grid.ItemsSource = ChildTable.DefaultView;
            }

            ReadSvIndex = Convert.ToInt16(SelSv.Row["SvIndexNo"]);
            UpdateMode = true;
        }
        #endregion

        #region 설문 등록 / 수정 취소시 컨트롤 초기화
        private void Btn_SvCancel_Click(object sender, RoutedEventArgs e)
        {
            Add_Item_Subject.Text = null;

            reset1();
            reset2();
            reset3();
            reset4();
            reset5();
        }
        #endregion

        #region 설문 항목 삭제 부분
        private void Btn_SvChildDel_Click(object sender, RoutedEventArgs e)
        {
            DataRowView drv = Sv_Child_Grid.SelectedItem as DataRowView;

            DeleteRow.Add(Convert.ToInt32(drv[0]));

            var itemSource = ChildTable as DataTable;

            itemSource.Rows[Sv_Child_Grid.SelectedIndex].Delete();

            Sv_Child_Grid.ItemsSource = ChildTable.DefaultView;

            reset4();
        }
        #endregion

        #region 항목 수정 부분
        private void Btn_SvChildUpd_Click(object sender, RoutedEventArgs e)
        {
            if (Add_Item_Subject.Text == "") { MessageBox.Show("항목 제목을 입력하시오."); return; }
            if (Add_Item_type.Text == "") { MessageBox.Show("항목 타입을 선택하시오."); return; }

            if (Add_Item_type.SelectedIndex >= 2)
            {
                if (Add_Item_Val_1.Text == "" || Add_Item_Val_1 == null) { MessageBox.Show("항목(1)을 입력해주세요."); return; }
                if (Add_Item_Val_2.Text == "" || Add_Item_Val_2 == null) { MessageBox.Show("항목(2)을 입력해주세요."); return; }

                if (Add_frm.Children.Contains(tbox1))
                {
                    if (Item_Val3 == "" || Item_Val3 == null) { MessageBox.Show("항목(3)을 입력해주세요."); return; }
                }
                if (Add_frm.Children.Contains(tbox2))
                {
                    if (Item_Val4 == "" || Item_Val4 == null) { MessageBox.Show("항목(4)을 입력해주세요."); return; }
                }
                if (Add_frm.Children.Contains(tbox3))
                {
                    if (Item_Val5 == "" || Item_Val5 == null) { MessageBox.Show("항목(5)을 입력해주세요."); return; }
                }
            }

            UpdateRow.Add(UpdClickRow);

            DataRowView sdr = Sv_Child_Grid.SelectedItem as DataRowView;

            sdr[1] = Add_Item_Subject.Text;
            sdr[2] = Add_Item_type.Text;
            sdr[3] = Add_frm.Children.Count - 3;
            sdr[4] = Add_Item_Val_1.Text;
            sdr[5] = Add_Item_Val_2.Text;
            sdr[6] = Item_Val3;
            sdr[7] = Item_Val4;
            sdr[8] = Item_Val5;

            Add_Item_Subject.Text = null;

            reset1();
            reset2();
            reset4();
        }
        #endregion

        #region 설문 항목 클릭 부분
        private void Sv_Child_Grid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            Add_Item_Subject.Text = null;

            reset1();
            reset2();

            DataRowView sdr = Sv_Child_Grid.SelectedItem as DataRowView;

            if (Sv_Child_Grid.SelectedIndex < 0) return;

            UpdClickRow = Convert.ToInt32(sdr[0]);

            Btn_SvChildAdd.IsEnabled = false;
            Btn_SvChildDel.IsEnabled = true;
            Btn_SvChildUpd.IsEnabled = true;

            Add_Item_Subject.Text = sdr[1].ToString();
            Add_Item_type.Text = sdr[2].ToString();

            if (sdr[2].ToString() == "라디오박스" || sdr[2].ToString() == "체크박스")
            {
                Add_Item_Val_1.Text = sdr[4].ToString();
                Add_Item_Val_2.Text = sdr[5].ToString();

                for (int i = 3; i <= Convert.ToInt32(sdr[3]); i++)
                {
                    int ValLength = Add_frm.Children.Count - 3;

                    switch (i)
                    {
                        case 3: tbox1 = new TextBox(); getObject = tbox1; Item_Val3 = sdr[i + 3].ToString(); break;
                        case 4: tbox2 = new TextBox(); getObject = tbox2; Item_Val4 = sdr[i + 3].ToString(); break;
                        case 5: tbox3 = new TextBox(); getObject = tbox3; Item_Val5 = sdr[i + 3].ToString(); Add_Item.Visibility = Visibility.Hidden; break;
                        default: break;
                    }

                    int Add_Left_Position = ValLength * 182 + 10;

                    getObject.Height = 41;
                    getObject.Width = 177;
                    getObject.Name = "Add_Item_Val_" + (ValLength + 1);
                    getObject.Text = sdr[i + 3].ToString();
                    getObject.TextChanged += new TextChangedEventHandler(TextBox_TextChanged);

                    Canvas.SetLeft(getObject, Add_Left_Position);
                    Canvas.SetTop(getObject, 68);

                    Add_frm.Children.Add(getObject);
                    Canvas.SetLeft(Add_Item, Add_Left_Position + 192);
                }
            }
        }
        #endregion

        #region 항목 수정 취소
        private void Btn_SvChildCancel_Click(object sender, RoutedEventArgs e)
        {
            Add_Item_Subject.Text = null;

            reset1();
            reset2();
            reset4();
        }
        #endregion

        private void Sv_Top_Edate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Sv_Top_Sdate.SelectedDate > Sv_Top_Edate.SelectedDate) { MessageBox.Show("종료 날짜가 시작 날짜보다 느립니다."); Sv_Top_Edate.Text = null; return; }
        }

        #region 버튼 [설문 저장] 부분
        private void Btn_SvSave_Click(object sender, RoutedEventArgs e)
        {
            if (Sv_Child_Grid.Items.Count <= 0) { MessageBox.Show("저장 할 항목이 존재하지 않습니다."); return; }

            //======================================================//
            //                       입력모드                       //
            //======================================================//
            if (UpdateMode == false)
            {
                // 1. 설문조사 탑 저장 구간
                string Save_SvTop = "insert into sv_top(sv_subject, sv_sdate, sv_edate, sv_writer, sv_realname, sv_open)" +
                    " values(@sv_subject, @sv_sdate, @sv_edate, @sv_writer, @sv_realname, @sv_open)";
                if (rs != null) rs.Close();
                query = new MySqlCommand();
                query.Connection = conn;
                query.CommandText = Save_SvTop;
                query.Prepare();
                query.Parameters.AddWithValue("@sv_subject", Sv_Top_Subject.Text);
                query.Parameters.AddWithValue("@sv_sdate", Sv_Top_Sdate.Text);
                query.Parameters.AddWithValue("@sv_edate", Sv_Top_Edate.Text);
                query.Parameters.AddWithValue("@sv_writer", "qkrrbgy");
                query.Parameters.AddWithValue("@sv_realname", Sv_Top_Realname.Text);
                query.Parameters.AddWithValue("@sv_open", Sv_Top_Open.Text);
                query.ExecuteNonQuery();

                // 2. 설문조사 인덱스 번호 가져오기
                string Read_SvTopIndex = "select sv_no from sv_top order by sv_no desc limit 1";
                if (rs != null) rs.Close();
                query = new MySqlCommand();
                query.Connection = conn;
                query.CommandText = Read_SvTopIndex;
                query.Prepare();
                rs = query.ExecuteReader();

                // 3. 설문 상세 항목 저장
                if (rs.Read())
                {
                    ReadSvIndex = (int)rs["sv_no"];

                    foreach (DataRowView drv in (DataView)Sv_Child_Grid.ItemsSource)
                    {
                        string Save_SvChild = "insert into sv_child(sv_no, sc_no, sc_subject, sc_type, sc_tcnt, sc_tval1, sc_tval2, sc_tval3, sc_tval4, sc_tval5)" +
                        " values(@sv_no, @sc_no, @sc_subject, @sc_type, @sc_tcnt, @sc_tval1, @sc_tval2, @sc_tval3, @sc_tval4, @sc_tval5)";
                        if (rs != null) rs.Close();
                        query = new MySqlCommand();
                        query.Connection = conn;
                        query.CommandText = Save_SvChild;
                        query.Prepare();
                        query.Parameters.AddWithValue("@sv_no", ReadSvIndex.ToString());
                        query.Parameters.AddWithValue("@sc_no", drv[0].ToString());
                        query.Parameters.AddWithValue("@sc_subject", drv[1].ToString());
                        query.Parameters.AddWithValue("@sc_type", drv[2].ToString());
                        query.Parameters.AddWithValue("@sc_tcnt", drv[3].ToString());
                        query.Parameters.AddWithValue("@sc_tval1", drv[4].ToString());
                        query.Parameters.AddWithValue("@sc_tval2", drv[5].ToString());
                        query.Parameters.AddWithValue("@sc_tval3", drv[6].ToString());
                        query.Parameters.AddWithValue("@sc_tval4", drv[7].ToString());
                        query.Parameters.AddWithValue("@sc_tval5", drv[8].ToString());
                        query.ExecuteNonQuery();
                    }
                }
                else
                {
                    MessageBox.Show("설문조사 [TOP] 번호를 불러오지 못했습니다.");
                    return;
                }
            }
            else
            {
                //======================================================//
                //                       수정모드                       //
                //======================================================//

                String Sv_Top_Update = "update sv_top set" +
                        " sv_subject = @sv_subject" +
                        ", sv_sdate = @sv_sdate" +
                        ", sv_edate = @sv_edate" +
                        ", sv_realname = @sv_realname" +
                        ", sv_open = @sv_open" +
                        " where sv_no = @sv_no";
                if (rs != null) rs.Close();
                query = new MySqlCommand();
                query.Connection = conn;
                query.CommandText = Sv_Top_Update;
                query.Prepare();
                query.Parameters.AddWithValue("@sv_subject", Sv_Top_Subject.Text);
                query.Parameters.AddWithValue("@sv_sdate", Sv_Top_Sdate.Text.Substring(0, 10));
                query.Parameters.AddWithValue("@sv_edate", Sv_Top_Edate.ToString().Substring(0, 10));
                query.Parameters.AddWithValue("@sv_realname", Sv_Top_Realname.Text);
                query.Parameters.AddWithValue("@sv_open", Sv_Top_Open.Text);
                query.Parameters.AddWithValue("@sv_no", ReadSvIndex.ToString());
                query.ExecuteNonQuery();
       
                foreach (DataRowView drg in (DataView)Sv_Child_Grid.ItemsSource)
                {
                    for (int i = 0; i < AddRow.Count; i++)
                    {
                        if (Convert.ToInt32(drg[0]) == AddRow[i])
                        {
                            Console.WriteLine("drg: " + Convert.ToInt32(drg[0]) + " AddRow: " + AddRow[i]);
                            Sv_Child_Grid.SelectedItem = drg;

                            DataRowView drv = Sv_Child_Grid.SelectedItem as DataRowView;

                            string Save_SvChild_Add = "insert into sv_child(sv_no, sc_no, sc_subject, sc_type, sc_tcnt, sc_tval1, sc_tval2, sc_tval3, sc_tval4, sc_tval5)" +
                                " values(@sv_no, @sc_no, @sc_subject, @sc_type, @sc_tcnt, @sc_tval1, @sc_tval2, @sc_tval3, @sc_tval4, @sc_tval5)";
                            if (rs != null) rs.Close();
                            query = new MySqlCommand();
                            query.Connection = conn;
                            query.CommandText = Save_SvChild_Add;
                            query.Prepare();
                            query.Parameters.AddWithValue("@sv_no", ReadSvIndex.ToString());
                            query.Parameters.AddWithValue("@sc_no", drv[0].ToString());
                            query.Parameters.AddWithValue("@sc_subject", drv[1].ToString());
                            query.Parameters.AddWithValue("@sc_type", drv[2].ToString());
                            query.Parameters.AddWithValue("@sc_tcnt", drv[3].ToString());
                            query.Parameters.AddWithValue("@sc_tval1", drv[4].ToString());
                            query.Parameters.AddWithValue("@sc_tval2", drv[5].ToString());
                            query.Parameters.AddWithValue("@sc_tval3", drv[6].ToString());
                            query.Parameters.AddWithValue("@sc_tval4", drv[7].ToString());
                            query.Parameters.AddWithValue("@sc_tval5", drv[8].ToString());
                            query.ExecuteNonQuery();
                        }
                    }
                }
                for (int i=0; i<UpdateRow.Count; i++)
                {
                    Sv_Child_Grid.SelectedIndex = i;

                    DataRowView drv = Sv_Child_Grid.SelectedItem as DataRowView;

                    String Sv_Child_Update = "update sv_child set" +
                        " sc_subject = @sc_subject" +
                        ", sc_type = @sc_type" +
                        ", sc_tcnt = @sc_tcnt" +
                        ", sc_tval1 = @sc_tval1" +
                        ", sc_tval2 = @sc_tval2" +
                        ", sc_tval3 = @sc_tval3" +
                        ", sc_tval4 = @sc_tval4" +
                        ", sc_tval5 = @sc_tval5" +
                        " where sv_no = @sv_no and sc_no = @sc_no";
                    if (rs != null) rs.Close();
                    query = new MySqlCommand();
                    query.Connection = conn;
                    query.CommandText = Sv_Child_Update;
                    query.Prepare();
                    query.Parameters.AddWithValue("@sc_subject", drv[1].ToString());
                    query.Parameters.AddWithValue("@sc_type", drv[2].ToString());
                    query.Parameters.AddWithValue("@sc_tcnt", drv[3].ToString());
                    query.Parameters.AddWithValue("@sc_tval1", drv[4].ToString());
                    query.Parameters.AddWithValue("@sc_tval2", drv[5].ToString());
                    query.Parameters.AddWithValue("@sc_tval3", drv[6].ToString());
                    query.Parameters.AddWithValue("@sc_tval4", drv[7].ToString());
                    query.Parameters.AddWithValue("@sc_tval5", drv[8].ToString());
                    query.Parameters.AddWithValue("@sv_no", ReadSvIndex.ToString());
                    query.Parameters.AddWithValue("@sc_no", drv[0].ToString());
                    query.ExecuteNonQuery();
                }

                for (int i = 0; i < DeleteRow.Count; i++)
                {
                    String Sv_Child_Update = "delete from sv_child where sv_no = @sv_no and sc_no = @sc_no";
                    if (rs != null) rs.Close();
                    query = new MySqlCommand();
                    query.Connection = conn;
                    query.CommandText = Sv_Child_Update;
                    query.Prepare();
                    query.Parameters.AddWithValue("@sv_no", ReadSvIndex.ToString());
                    query.Parameters.AddWithValue("@sc_no", DeleteRow[i].ToString());
                    query.ExecuteNonQuery();

                    Console.WriteLine(ReadSvIndex.ToString());
                    Console.WriteLine(DeleteRow[i].ToString());
                }
            }            

            MessageBox.Show("설문 저장 완료");

            reset3();
            reset5();
        }
        #endregion

        #region 체크박스 / 라디오박스 추가 부분
        private void Add_Item_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            int ValLength = Add_frm.Children.Count - 3;

            if (ValLength < 5)
            {
                int Add_Left_Position = ValLength * 182 + 10;

                if (ValLength == 2)
                {
                    tbox1 = new TextBox();
                    getObject = tbox1;
                }
                else if (ValLength == 3)
                {
                    tbox2 = new TextBox();
                    getObject = tbox2;
                }
                else if (ValLength == 4)
                {
                    tbox3 = new TextBox();
                    getObject = tbox3;
                    Add_Item.Visibility = Visibility.Hidden;
                }

                getObject.Height = 41;
                getObject.Width = 177;
                getObject.Name = "Add_Item_Val_" + (ValLength + 1);
                getObject.TextChanged += new TextChangedEventHandler(TextBox_TextChanged);

                Canvas.SetLeft(getObject, Add_Left_Position);
                Canvas.SetTop(getObject, 68);

                Add_frm.Children.Add(getObject);
                Canvas.SetLeft(Add_Item, Add_Left_Position + 192);
            }
            else
            {
                Add_Item.Visibility = Visibility.Hidden;
            }
            
        }
        #endregion


        void TextBox_TextChanged(object sender, RoutedEventArgs e)
        {
            get_txt_val(sender);
        }

        #region 자식 항목 등록 부분

        private void Btn_SvChildAdd_Click(object sender, RoutedEventArgs e)
        {
            if (Add_Item_Subject.Text == "") { MessageBox.Show("항목 제목을 입력하시오."); return; }
            if (Add_Item_type.Text == "" || Add_Item_type.Text == null) { MessageBox.Show("항목 타입을 선택하시오."); return; }

            if (Add_Item_type.SelectedIndex >= 2)
            {
                if (Add_Item_Val_1.Text == "" || Add_Item_Val_1 == null) { MessageBox.Show("항목(1)을 입력해주세요."); return; }
                if (Add_Item_Val_2.Text == "" || Add_Item_Val_2 == null) { MessageBox.Show("항목(2)을 입력해주세요."); return; }

                if (Add_frm.Children.Contains(tbox1) && (Item_Val3 == "" || Item_Val3 == null)) { MessageBox.Show("항목(3)을 입력해주세요."); return; }
                if (Add_frm.Children.Contains(tbox2) && (Item_Val4 == "" || Item_Val4 == null)) { MessageBox.Show("항목(3)을 입력해주세요."); return; }
                if (Add_frm.Children.Contains(tbox3) && (Item_Val5 == "" || Item_Val5 == null))  { MessageBox.Show("항목(3)을 입력해주세요."); return; }
            }

            ChildTable.Rows.Add(
                ChildNo,
                Add_Item_Subject.Text,
                Add_Item_type.Text,
                Add_frm.Children.Count - 3,
                Add_Item_Val_1.Text,
                Add_Item_Val_2.Text,
                Item_Val3,
                Item_Val4,
                Item_Val5
            );

            Sv_Child_Grid.ItemsSource = ChildTable.DefaultView;

            AddRow.Add(ChildNo);

            ChildNo++;

            Add_Item_Subject.Text = null;

            reset1();
            reset2();
        }
        #endregion

        void get_txt_val(object sender)
        {
            TextBox getVal = sender as TextBox;

            if (getVal.Name == "Add_Item_Val_3"){Item_Val3 = getVal.Text;}
            else if (getVal.Name == "Add_Item_Val_4"){Item_Val4 = getVal.Text;}
            else {Item_Val5 = getVal.Text;}
        }
    }
}
