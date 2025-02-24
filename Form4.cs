using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.IO;
using System.Xml;

namespace LAB2Nhom
{
    public partial class Form4 : Form
    {
        string Username;
        public Form4(string username)
        {
            InitializeComponent();
            Username = username;
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            LoadData();
        }
        private void LoadData()
        {
            string connectionString = @"Data Source=ALJFM90\SQLEXPRESS;Initial Catalog=QLSINHVIEN;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("SP_SEL_SINHVIEN'" + Username + "'", connection);
                try
                {
                    connection.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    dataGridView1.DataSource = dataTable;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }
        static string CalculateSHA1(string input)
        {
            using (SHA1 sha1 = SHA1.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha1.ComputeHash(inputBytes);


                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtmasv.Text) || string.IsNullOrWhiteSpace(txthoten.Text) ||
                string.IsNullOrWhiteSpace(txtngaysinh.Text) || string.IsNullOrWhiteSpace(txtdiachi.Text) ||
                string.IsNullOrWhiteSpace(txtmalop.Text) || string.IsNullOrWhiteSpace(txttendn.Text) ||
                string.IsNullOrWhiteSpace(txtmk.Text))
            {
                MessageBox.Show("Please fill in all fields.");
                return;
            }

            string connectionString = @"Data Source=ALJFM90\SQLEXPRESS;Initial Catalog=QLSINHVIEN;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string storedProcedure = "SP_INS_SINHVIEN";

                using (SqlCommand command = new SqlCommand(storedProcedure, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@MASV", SqlDbType.NVarChar).Value = txtmasv.Text;
                    command.Parameters.Add("@HOTEN", SqlDbType.NVarChar).Value = txthoten.Text;
                    command.Parameters.Add("@NGAYSINH", SqlDbType.Date).Value = txtngaysinh.Text;
                    command.Parameters.Add("@DIACHI", SqlDbType.NVarChar).Value = txtdiachi.Text;
                    command.Parameters.Add("@MALOP", SqlDbType.VarChar).Value = txtmalop.Text;
                    command.Parameters.Add("@TENDN", SqlDbType.NVarChar).Value = txttendn.Text;
                    string mk = txtmk.Text;
                    string mksha = CalculateSHA1(mk);
                    command.Parameters.Add("@MATKHAU", SqlDbType.VarChar).Value = mksha;

                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                        MessageBox.Show("Thêm thành công");
                        LoadData();
                        Cleartext();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
            }
        }

        //private void button1_Click(object sender, EventArgs e)
        //{

        //    string connectionString = @"Data Source=ALJFM90\SQLEXPRESS;Initial Catalog=QLSINHVIEN;Integrated Security=True";


        //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    {
        //        string storedProcedure = "SP_INS_SINHVIEN";

        //        using (SqlCommand command = new SqlCommand(storedProcedure, connection))
        //        {
        //            command.CommandType = CommandType.StoredProcedure;
        //            command.Parameters.Add("@MASV", SqlDbType.NVarChar).Value = txtmasv.Text;
        //            command.Parameters.Add("@HOTEN", SqlDbType.NVarChar).Value = txthoten.Text;
        //            command.Parameters.Add("@NGAYSINH", SqlDbType.Date).Value = txtngaysinh.Text;
        //            command.Parameters.Add("@DIACHI", SqlDbType.NVarChar).Value = txtdiachi.Text;
        //            command.Parameters.Add("@MALOP", SqlDbType.VarChar).Value = txtmalop.Text;
        //            command.Parameters.Add("@TENDN", SqlDbType.NVarChar).Value = txttendn.Text;
        //            string mk = txtmk.Text;
        //            string mksha = CalculateSHA1(mk);
        //            command.Parameters.Add("@MATKHAU", SqlDbType.VarChar).Value =mksha;
        //            try
        //            {
        //                connection.Open();

        //                command.ExecuteNonQuery();

        //                MessageBox.Show("Thêm thành công");
        //                LoadData();
        //                //Cleartext();
        //            }
        //            catch (Exception ex)
        //            {
        //                Console.WriteLine("Error: " + ex.Message);
        //            }
        //        }
        //    }
        //}

        private void button3_Click(object sender, EventArgs e)
        {
            string connectionString = @"Data Source=ALJFM90\SQLEXPRESS;Initial Catalog=QLSINHVIEN;Integrated Security=True";


            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string storedProcedure = "SP_DEL_SINHVIEN";

                using (SqlCommand command = new SqlCommand(storedProcedure, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@MASV", SqlDbType.VarChar).Value = txtmasv.Text;
                    try
                    {
                        connection.Open();

                        command.ExecuteNonQuery();

                        MessageBox.Show("Xóa thành công");
                        LoadData();
                        Cleartext();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                    }
                }
            }
        }

        private void Cleartext()
        {
            txtmasv.Text = "";
            txthoten.Text = "";
            txtngaysinh.Text = "";
            txtdiachi.Text = "";
            txtmalop.Text = "";
            txttendn.Text = "";
            txtmk.Text = "";
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int i;
            i = dataGridView1.CurrentRow.Index;
            txtmasv.Text = dataGridView1.Rows[i].Cells[0].Value.ToString();
            txthoten.Text = dataGridView1.Rows[i].Cells[1].Value.ToString();
            txtngaysinh.Text = dataGridView1.Rows[i].Cells[2].Value.ToString();
            txtdiachi.Text = dataGridView1.Rows[i].Cells[3].Value.ToString();
            txtmalop.Text = dataGridView1.Rows[i].Cells[4].Value.ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string connectionString = @"Data Source=ALJFM90\SQLEXPRESS;Initial Catalog=QLSINHVIEN;Integrated Security=True";


            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string storedProcedure = "SP_UPD_SINHVIEN";

                using (SqlCommand command = new SqlCommand(storedProcedure, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@MASV", SqlDbType.NVarChar).Value = txtmasv.Text;
                    command.Parameters.Add("@HOTEN", SqlDbType.NVarChar).Value = txthoten.Text;
                    command.Parameters.Add("@NGAYSINH", SqlDbType.Date).Value = txtngaysinh.Text;
                    command.Parameters.Add("@DIACHI", SqlDbType.NVarChar).Value = txtdiachi.Text;
                    command.Parameters.Add("@MALOP", SqlDbType.VarChar).Value = txtmalop.Text;
                    //command.Parameters.Add("@TENDN", SqlDbType.NVarChar).Value = txttendn.Text;
                    //string mk = txtmk.Text;
                    //string mksha = CalculateSHA1(mk);
                    //command.Parameters.Add("@MATKHAU", SqlDbType.VarChar).Value = mksha;
                    try
                    {
                        connection.Open();

                        command.ExecuteNonQuery();

                        MessageBox.Show("Thêm thành công");
                        LoadData();
                        Cleartext();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                    }
                }
            }
        }
    }
}
