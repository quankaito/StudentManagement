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
namespace LAB2Nhom
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }
        private void Cleartext()
        {
            txtmalop.Text = "";
            txttenlop.Text = "";
            txtmnv.Text = "";
            
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            LoadData();
        }
        private void LoadData()
        {
            string connectionString = @"Data Source=ALJFM90\SQLEXPRESS;Initial Catalog=QLSINHVIEN;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("SELECT MALOP,TENLOP,MANV FROM LOP", connection);
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

        private void button1_Click(object sender, EventArgs e)
        {
            string connectionString = @"Data Source=ALJFM90\SQLEXPRESS;Initial Catalog=QLSINHVIEN;Integrated Security=True";


            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string storedProcedure = "SP_INS_LOP";

                using (SqlCommand command = new SqlCommand(storedProcedure, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@MALOP", SqlDbType.VarChar).Value = txtmalop.Text;
                    command.Parameters.Add("@TENLOP", SqlDbType.NVarChar).Value = txttenlop.Text;
                    command.Parameters.Add("@MANV", SqlDbType.VarChar).Value = txtmnv.Text;
                    
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

        private void button3_Click(object sender, EventArgs e)
        {
            string connectionString = @"Data Source=ALJFM90\SQLEXPRESS;Initial Catalog=QLSINHVIEN;Integrated Security=True";


            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string storedProcedure = "SP_DEL_LOP";

                using (SqlCommand command = new SqlCommand(storedProcedure, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@MALOP", SqlDbType.VarChar).Value = txtmalop.Text;
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

        private void button2_Click(object sender, EventArgs e)
        {
            string connectionString = @"Data Source=ALJFM90\SQLEXPRESS;Initial Catalog=QLSINHVIEN;Integrated Security=True";


            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string storedProcedure = "SP_UPD_LOP";

                using (SqlCommand command = new SqlCommand(storedProcedure, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@MALOP", SqlDbType.VarChar).Value = txtmalop.Text;
                    command.Parameters.Add("@TENLOP", SqlDbType.NVarChar).Value = txttenlop.Text;
                    command.Parameters.Add("@MANV", SqlDbType.VarChar).Value = txtmnv.Text;
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

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int i;
            i = dataGridView1.CurrentRow.Index;
            txtmalop.Text = dataGridView1.Rows[i].Cells[0].Value.ToString();
            txttenlop.Text = dataGridView1.Rows[i].Cells[1].Value.ToString();
            txtmnv.Text = dataGridView1.Rows[i].Cells[2].Value.ToString();
        }
        
    }
}
