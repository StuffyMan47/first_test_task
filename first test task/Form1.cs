using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Xml.Linq;
using System;
using System.Diagnostics;

namespace first_test_task
{
    public partial class Form1 : Form
    {
        private SqlConnection sqlConnection = null;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Подключение БД
            try
            {
                sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString);
                sqlConnection.Open();

            }
            catch
            {
                //Проверка подключения БД
                if (sqlConnection.State != ConnectionState.Open)
                {
                    MessageBox.Show("Ошибка в подключении БД");
                }
            }
            //ReadTxt txt = new ReadTxt("E:\\test tasks\\first test task\\first test task\\preps.txt");
            //MessageBox.Show(txt.Text[1]);

            //Заполнение DataGridView фильтрации данными из БД
            //Пример для 1 таблицы
            //SqlDataAdapter dataAdapter = new SqlDataAdapter("SELECT * FROM medication", sqlConnection);
            
            SqlDataAdapter dataAdapter = new SqlDataAdapter("SELECT Name, Dosage, Number, FabricatorName, CountryName FROM medicaments JOIN Fabricator ON medicaments.Fabricator_ID = Fabricator.Id JOIN Country ON medicaments.Country_ID=Country.Id", sqlConnection);
            DataSet db = new DataSet();
            dataAdapter.Fill(db);
            dataGridView1.DataSource = db.Tables[0];
            dataGridView2.DataSource = db.Tables[0];
            dataGridView3.DataSource = db.Tables[0];
            sqlConnection.Close();
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            sqlConnection.Open();
            //Проверка на наличие двойника
            //SqlCommand checkComa = new SqlCommand($"SELECT Name FROM medication WHERE Name='{textBox1.Text}' AND Dosage='{textBox2.Text}' AND Number='{textBox3.Text}' AND Fabricator='{textBox4.Text}' AND Country='{textBox5.Text}'", sqlConnection);
            SqlCommand checkComa = new SqlCommand($"SELECT Name FROM medicaments JOIN Fabricator ON medicaments.Fabricator_ID = Fabricator.Id JOIN Country ON medicaments.Country_ID=Country.Id WHERE Name='{textBox1.Text}' AND Dosage='{textBox2.Text}' AND Number='{textBox3.Text}' AND FabricatorName='{textBox4.Text}' AND CountryName='{textBox5.Text}'", sqlConnection);

            var data = checkComa.ExecuteReader();
            if (data.HasRows)
            {
                MessageBox.Show("Данное лекарство уже зарегестрировано в БД");
            }
            else
            {
                data.Close();

                //SqlCommand coma = new SqlCommand($"INSERT INTO [medication] (Name, Dosage, Number, Fabricator, Country) VALUES (N'{textBox1}', N'{textBox2}', N'{textBox3}', N'{textBox4}', N'{textBox5}')", sqlConnection);
                SqlCommand coma = new SqlCommand($"BEGIN TRANSACTION\r\n" +
                    $"DECLARE @FabricatorId INT, @CountryId INT\r\n" +
                    $"SELECT @FabricatorId = Id FROM Fabricator WHERE Fabricator.FabricatorName=N'{textBox4.Text}'\r\n" +
                    $"SELECT @CountryId = Id FROM Country WHERE Country.CountryName=N'{textBox5.Text}'\r\n" +
                    $"\r\nIF @FabricatorId > 0\r\n" +
                    $"\tBEGIN\r\n" +
                    $"\t\tPRINT N'Fabricator EXIST'\r\n" +
                    $"\t\tPRINT @FabricatorId\r\n" +
                    $"\tEND\r\n" +
                    $"ELSE\r\n" +
                    $"\tBEGIN\r\n" +
                    $"\t\tSELECT @FabricatorId = MAX(Id)+1 FROM Fabricator\r\n" +
                    $"\t\tINSERT INTO Fabricator (FabricatorName) VALUES (N'{textBox4.Text}')\r\n" +
                    $"\t\tPRINT N'Fabricator NOT EXIST'\r\n" +
                    $"\t\tPRINT @FabricatorId\r\n" +
                    $"\tEND\r\n" +
                    $"\r\nIF @CountryId > 0\r\n" +
                    $"\tBEGIN\r\n" +
                    $"\t\tPRINT N'Country EXIST'\r\n" +
                    $"\t\tPRINT @CountryId\r\n" +
                    $"\tEND\r\n" +
                    $"ELSE\r\n" +
                    $"\tBEGIN\r\n" +
                    $"\t\tSELECT @CountryId = MAX(Id)+1 FROM Country\r\n" +
                    $"\t\tINSERT INTO Country (CountryName) VALUES (N'{textBox5.Text}')\r\n" +
                    $"\t\tPRINT N'Country NOT EXIST'\r\n" +
                    $"\t\tPRINT @CountryId\r\n" +
                    $"\tEND\r\n" +
                    $"INSERT INTO medicaments (Name, Dosage, Number, Fabricator_ID, Country_ID) VALUES (N'{textBox1.Text}', N'{textBox2.Text}', N'{textBox3.Text}', @FabricatorId, @CountryId)" +
                    $"\r\nCOMMIT TRANSACTION\r\nGO", sqlConnection);
                coma.Parameters.AddWithValue("Name", textBox1.Text);
                coma.Parameters.AddWithValue("Dosage", textBox2.Text);
                coma.Parameters.AddWithValue("Number", textBox3.Text);
                coma.Parameters.AddWithValue("Fabricator", textBox4.Text);
                coma.Parameters.AddWithValue("Country", textBox5.Text);
                coma.ExecuteNonQuery();
                //Обновление таблицы в Data grid view
                SqlDataAdapter dataAdapter = new SqlDataAdapter("SELECT Name, Dosage, Number, FabricatorName, CountryName FROM medicaments JOIN Fabricator ON medicaments.Fabricator_ID = Fabricator.Id JOIN Country ON medicaments.Country_ID=Country.Id", sqlConnection);
                DataSet db = new DataSet();
                dataAdapter.Fill(db);
                dataGridView1.DataSource = db.Tables[0];
                dataGridView2.DataSource = db.Tables[0];
                dataGridView3.DataSource = db.Tables[0];
                MessageBox.Show("Лекарство было добавлено");
            }
            sqlConnection.Close();
        }


        //Фильрация
        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            (dataGridView1.DataSource as DataTable).DefaultView.RowFilter = string.Format($"Name LIKE '%{textBox6.Text}%' AND Dosage LIKE '%{textBox7.Text}%' AND Number LIKE '%{textBox8.Text}%' AND FabricatorName LIKE '%{textBox9.Text}%' AND CountryName LIKE '%{textBox10.Text}%'");
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            (dataGridView1.DataSource as DataTable).DefaultView.RowFilter = string.Format($"Name LIKE '%{textBox6.Text}%' AND Dosage LIKE '%{textBox7.Text}%' AND Number LIKE '%{textBox8.Text}%' AND FabricatorName LIKE '%{textBox9.Text}%' AND CountryName LIKE '%{textBox10.Text}%'");
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            (dataGridView1.DataSource as DataTable).DefaultView.RowFilter = string.Format($"Name LIKE '%{textBox6.Text}%' AND Dosage LIKE '%{textBox7.Text}%' AND Number LIKE '%{textBox8.Text}%' AND FabricatorName LIKE '%{textBox9.Text}%' AND CountryName LIKE '%{textBox10.Text}%'");
        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            (dataGridView1.DataSource as DataTable).DefaultView.RowFilter = string.Format($"Name LIKE '%{textBox6.Text}%' AND Dosage LIKE '%{textBox7.Text}%' AND Number LIKE '%{textBox8.Text}%' AND FabricatorName LIKE '%{textBox9.Text}%' AND CountryName LIKE '%{textBox10.Text}%'");
        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {
            (dataGridView1.DataSource as DataTable).DefaultView.RowFilter = string.Format($"Name LIKE '%{textBox6.Text}%' AND Dosage LIKE '%{textBox7.Text}%' AND Number LIKE '%{textBox8.Text}%' AND FabricatorName LIKE '%{textBox9.Text}%' AND CountryName LIKE '%{textBox10.Text}%'");
        }

        //Удаление записи из БД
        private void DeleteButton_Click(object sender, EventArgs e)
        {
            sqlConnection.Open();
            SqlCommand deleteCommand = new SqlCommand($"DELETE FROM medicaments WHERE Name='{textBox11.Text}' AND Dosage='{textBox12.Text}' AND Number='{textBox13.Text}'", sqlConnection);
            deleteCommand.ExecuteNonQuery();


            //Обновление таблиц в Data grid view после удаления элемента
            SqlDataAdapter dataAdapter = new SqlDataAdapter("SELECT Name, Dosage, Number, FabricatorName, CountryName FROM medicaments JOIN Fabricator ON medicaments.Fabricator_ID = Fabricator.Id JOIN Country ON medicaments.Country_ID=Country.Id", sqlConnection);
            DataSet db = new DataSet();
            dataAdapter.Fill(db);
            dataGridView1.DataSource = db.Tables[0];
            dataGridView2.DataSource = db.Tables[0];
            dataGridView3.DataSource = db.Tables[0];
            sqlConnection.Close();
        }

        //Экспорт xml файла
        private void ExportButton_Click(object sender, EventArgs e)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<Medication>), new XmlRootAttribute("medications"));

            var medication = new List<Medication>();
            //medication.Add(new Medication(dataGridView1[0, 0].Value.ToString(), dataGridView1[1, 0].Value.ToString(), dataGridView1[2, 0].Value.ToString(), dataGridView1[3, 0].Value.ToString(), dataGridView1[4, 0].Value.ToString()));
            for (int i = 0; i < dataGridView1.RowCount-1; i++)
            {
                medication.Add(new Medication(dataGridView1[0, i].Value.ToString(), dataGridView1[1, i].Value.ToString(), dataGridView1[2, i].Value.ToString(), dataGridView1[3, i].Value.ToString(), dataGridView1[4, i].Value.ToString()));
            }
            //XElement xElements = new XElement("medications", medication.Select(i=>new XElement("medication", i)));
            //MessageBox.Show(xElements.ToString());
            string path;
            using (FileStream fs = new FileStream("Medications.xml", FileMode.OpenOrCreate))
            {
                xmlSerializer.Serialize(fs, medication);
                MessageBox.Show("XML файл сгенерирован");

                //Открытие файла
                Process p = new Process();
                ProcessStartInfo pi = new ProcessStartInfo();
                pi.UseShellExecute = true;
                pi.FileName = @"Medications.xml";
                p.StartInfo = pi;
                p.Start();
            }

        }
    }
}