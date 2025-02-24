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

    public partial class Form5 : Form
    {
        string MA;
        string Username;
        string Password;
        RSA512 rsa512 = new RSA512();
        public Form5(string ma, string password, string username)
        {
            InitializeComponent();
            MA = ma;
            Username = username;
            Password = password;
        }

        private void Form5_Load(object sender, EventArgs e)
        {
            LoadData();
        }
        private void LoadData()
        {
            string connectionString = @"Data Source=ALJFM90\SQLEXPRESS;Initial Catalog=QLSINHVIEN;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("SP_SEL_BANGDIEM", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@TK", Username);
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

        private void Cleartext()
        {
            txtmasv.Text = "";
            txtmahp.Text = "";
            txtdiem.Text = "";         
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

            int i;
            i = dataGridView1.CurrentRow.Index;
            txtmasv.Text = dataGridView1.Rows[i].Cells[0].Value.ToString();
            txtmahp.Text = dataGridView1.Rows[i].Cells[1].Value.ToString();
            txtdiem.Text = dataGridView1.Rows[i].Cells[2].Value.ToString();
        }

            private void txtmasv_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string ma = MA;
            string connectionString = @"Data Source=ALJFM90\SQLEXPRESS;Initial Catalog=QLSINHVIEN;Integrated Security=True";
            string filePath = @"../../Keys/" + ma;
            string fileName = ma + ".txt";
            RSAParameters publickey = new RSAParameters();
            RSAParameters privatekey = new RSAParameters();
            string fullPath = filePath + "/" + fileName;
            string publicKeyXml1 = rsa512.GetPublicKey();
            string privateKeyXml1 = rsa512.GetPrivateKey();
            RSA512.ReadKeysFromFile(fullPath, out privatekey, out publickey);
            byte[] en_diem = rsa512.RSAEncrypt(txtdiem.Text, publickey);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string storedProcedure = "SP_INS_DIEM";
        
                using (SqlCommand command = new SqlCommand(storedProcedure, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@TK", SqlDbType.VarChar).Value = Username;
                    command.Parameters.Add("@MASV", SqlDbType.NVarChar).Value = txtmasv.Text;
                    command.Parameters.Add("@MAHP", SqlDbType.VarChar).Value = txtmahp.Text;
                    command.Parameters.Add("@DIEMTHI", SqlDbType.VarBinary).Value =en_diem;
                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                        MessageBox.Show("Thêm thành công");
                        LoadData();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                    }
                }
            }
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            string ma = MA;
            int col_manv = 0;


            if (dataGridView1.Columns[e.ColumnIndex].DataPropertyName == "DIEMTHI")
            {
                    ma = MA ;
                    var value = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                    if (value != null && value.GetType() == typeof(byte[]))
                    {
                        var byteValue = (byte[])value;
                        string stringValue = rsa512.Decrypt(byteValue, ma);
                        e.Value = stringValue;
                    }
                
                else
                {
                    e.Value = "";
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string ma = MA; // Assuming MA is a variable containing some identifier
            string connectionString = @"Data Source=ALJFM90\SQLEXPRESS;Initial Catalog=QLSINHVIEN;Integrated Security=True";
            string filePath = @"../../Keys/" + ma;
            string fileName = ma + ".txt";
            RSAParameters publickey = new RSAParameters();
            RSAParameters privatekey = new RSAParameters();
            string fullPath = filePath + "/" + fileName;
            string publicKeyXml1 = rsa512.GetPublicKey(); // Assuming rsa512 is an instance of some RSA class
            string privateKeyXml1 = rsa512.GetPrivateKey(); // Assuming rsa512 is an instance of some RSA class
            RSA512.ReadKeysFromFile(fullPath, out privatekey, out publickey);
            byte[] en_diem = rsa512.RSAEncrypt(txtdiem.Text, publickey); // Assuming txtdiem.Text contains the grade to be encrypted

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string storedProcedure = "SP_UPD_DIEM";

                using (SqlCommand command = new SqlCommand(storedProcedure, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@TK", SqlDbType.VarChar).Value = Username;
                    command.Parameters.Add("@MASV", SqlDbType.NVarChar).Value = txtmasv.Text;
                    command.Parameters.Add("@MAHP", SqlDbType.VarChar).Value = txtmahp.Text;
                    command.Parameters.Add("@DIEMTHI", SqlDbType.VarBinary).Value = en_diem;
                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                        MessageBox.Show("Cập nhật điểm thành công");
                        LoadData();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
            }           
        }
    }
}


