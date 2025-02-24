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
using LAB2Nhom;
namespace LAB2Nhom
{
    public partial class Form2 : Form
    {
        RSA512 rsa512 = new RSA512();
        private string Username;
        private string Passwords;
        public Form2()
        {
            InitializeComponent();
        }

        public Form2(string Username, string Passwords)
        {
            this.Username = Username;
            this.Passwords = Passwords;
        }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int i;
            i = dataGridView1.CurrentRow.Index;
            txtnv.Text = dataGridView1.Rows[i].Cells[0].Value.ToString();
            txthoten.Text = dataGridView1.Rows[i].Cells[1].Value.ToString();
            txtemail.Text = dataGridView1.Rows[i].Cells[2].Value.ToString();
            string ma = dataGridView1.Rows[i].Cells[0].Value.ToString();
            var value = dataGridView1.Rows[i].Cells[3].Value;
            if (value != null && value.GetType() == typeof(byte[]))
            {
                var byteValue = (byte[])value;
                string stringValue = rsa512.Decrypt(byteValue, ma);
                value = stringValue;
            }
            txtluong.Text = value.ToString();
            txtdn.Text = dataGridView1.Rows[i].Cells[4].Value.ToString();
            //txtmk.Text = dataGridView1.Rows[i].Cells[5].Value.ToString();
            PUBKEY.Text = dataGridView1.Rows[i].Cells[6].Value.ToString();

        }

        private void Form2_Load(object sender, EventArgs e)
        {
            LoadData();
        }
        private void Cleartext()
        {
            txtnv.Text = "";
            txthoten.Text = "";
            txtemail.Text = "";
            txtluong.Text = "";
            txtdn.Text = "";
            txtmk.Text = "";
            PUBKEY.Text = "";
        }
        private void LoadData()
        {
            string connectionString = @"Data Source=ALJFM90\SQLEXPRESS;Initial Catalog=QLSINHVIEN;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("SELECT MANV, HOTEN, Email, LUONG,TENDN,MATKHAU,PUBKEY FROM NHANVIEN", connection);
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

        private void button2_Click(object sender, EventArgs e)
        {
            string connectionString = @"Data Source=ALJFM90\SQLEXPRESS;Initial Catalog=QLSINHVIEN;Integrated Security=True";
            string ma = txtnv.Text;
            string filePath = @"../../Keys/" + ma;
            string fileName = ma + ".txt";
            RSAParameters publickey = new RSAParameters();
            RSAParameters privatekey = new RSAParameters();
            string fullPath = filePath + "/" + fileName;
            string publicKeyXml1 = rsa512.GetPublicKey();
            string privateKeyXml1 = rsa512.GetPrivateKey();
            RSA512.ReadKeysFromFile(fullPath, out privatekey, out publickey);
            byte[] en_luong = rsa512.RSAEncrypt(txtluong.Text, publickey);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string storedProcedure = "SP_UPD_NHANVIEN";

                using (SqlCommand command = new SqlCommand(storedProcedure, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@MANV", SqlDbType.VarChar).Value = txtnv.Text;
                    command.Parameters.Add("@HOTEN", SqlDbType.NVarChar).Value = txthoten.Text;
                    command.Parameters.Add("@EMAIL", SqlDbType.VarChar).Value = txtemail.Text;
                    command.Parameters.Add("@LUONG", SqlDbType.VarBinary).Value = en_luong;
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

        private void button1_Click(object sender, EventArgs e)
        {
            string ma = txtnv.Text;
            string connectionString = @"Data Source=ALJFM90\SQLEXPRESS;Initial Catalog=QLSINHVIEN;Integrated Security=True";
            string filePath = @"../../Keys/" + ma;
            string fileName = ma + ".txt";
            RSAParameters publickey = new RSAParameters();
            RSAParameters privatekey = new RSAParameters();
            string fullPath = filePath + "/" + fileName;
            if (File.Exists(fullPath))
            {
                //MessageBox.Show("Nhân viên đã tồn tại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                string publicKeyXml1 = rsa512.GetPublicKey();
                string privateKeyXml1 = rsa512.GetPrivateKey();
                RSA512.ReadKeysFromFile(fullPath, out privatekey, out publickey);
            }
            else
            {
                string publicKeyXml1 = rsa512.GetPublicKey();
                string privateKeyXml1 = rsa512.GetPrivateKey();
                RSA512.WriteKeysToFile(filePath, fileName, privateKeyXml1, publicKeyXml1);
                RSA512.ReadKeysFromFile(fullPath, out privatekey, out publickey);
            }

            byte[] en_luong = rsa512.RSAEncrypt(txtluong.Text, publickey);


            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string storedProcedure = "SP_INS_NHANVIEN";

                using (SqlCommand command = new SqlCommand(storedProcedure, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@MANV", SqlDbType.VarChar).Value = txtnv.Text;
                    command.Parameters.Add("@HOTEN", SqlDbType.NVarChar).Value = txthoten.Text;
                    command.Parameters.Add("@EMAIL", SqlDbType.VarChar).Value = txtemail.Text;
                    command.Parameters.Add("@LUONG", SqlDbType.VarBinary).Value = en_luong;
                    command.Parameters.Add("@TENDN", SqlDbType.NVarChar).Value = txtdn.Text;
                    command.Parameters.Add("@MATKHAU", SqlDbType.NVarChar).Value = CalculateSHA1(txtmk.Text);
                    command.Parameters.Add("@PUBKEY", SqlDbType.VarChar).Value = PUBKEY.Text;


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
        private void button3_Click(object sender, EventArgs e)
        {
            string connectionString = @"Data Source=ALJFM90\SQLEXPRESS;Initial Catalog=QLSINHVIEN;Integrated Security=True";


            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string storedProcedure = "SP_DEL_NHANVIEN";

                using (SqlCommand command = new SqlCommand(storedProcedure, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@MANV", SqlDbType.VarChar).Value = txtnv.Text;
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

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            string ma = null;
            int col_manv = 0;  

            
            if (dataGridView1.Columns[e.ColumnIndex].DataPropertyName == "LUONG")
            {
                
                var manvCellValue = dataGridView1.Rows[e.RowIndex].Cells[col_manv].Value;
                if (manvCellValue != null)
                {
                    ma = manvCellValue.ToString();
                    var value = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                    if (value != null && value.GetType() == typeof(byte[]))
                    {
                        var byteValue = (byte[])value;
                        string stringValue = rsa512.Decrypt(byteValue, ma);
                        e.Value = stringValue;
                    }
                }
                else
                {
                    e.Value = "";
                }
            }    
        }
    }
}
