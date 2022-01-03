using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Configuration;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace insurance_agency
{
    public partial class Form2 : Form
    {
        public char act;
        public Form2()
        {
            InitializeComponent();
            act = 'I';
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
            textBox6.Clear();
            textBox7.Clear();
            textBox8.Clear();
            textBox8.Focus();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            this.Owner.Show();
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (check(sender, e))
                return;
            Insurance insurance1 = new Insurance {
                Id = Convert.ToInt32(textBox8.Text),
                Insured = new Insured { 
                    Name = textBox1.Text,
                    Adress = textBox2.Text
                },
                Employee = new Employee {
                    Name = textBox3.Text,
                    Adress = textBox4.Text.Trim().Equals("") ? null : textBox4.Text,
                    Experience = textBox5.Text.Trim().Equals("") ? 0 : Convert.ToInt32(textBox5.Text),
                },
                Type = textBox6.Text,
                Amount = Convert.ToInt32(textBox7.Text)
            };
            if (act == 'I')
            {
                try
                {
                    SaveDocs(insurance1).GetAwaiter();
                }
                catch (Exception ee)
                {
                    MessageBox.Show(ee.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                }
                MessageBox.Show($"Запись успешно добавлена", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
            }
            else
            {
                try
                {
                    UpdateDocs(insurance1).GetAwaiter();
                }
                catch (Exception ee)
                {
                    MessageBox.Show(ee.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                }
                MessageBox.Show($"Запись успешно изменена", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
            }
            button3_Click(sender, e);
        }

        private static async Task SaveDocs(Insurance insurance1) {
            string connectionString = ConfigurationManager.ConnectionStrings["MongoDb"].ConnectionString;
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("insurance_agency");
            var collection = database.GetCollection<Insurance>("insurance");
            await collection.InsertOneAsync(insurance1);
        }

        private static async Task UpdateDocs(Insurance insurance1)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MongoDb"].ConnectionString;
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("insurance_agency");
            var collection = database.GetCollection<Insurance>("insurance");
            await collection.ReplaceOneAsync(new BsonDocument("_id", insurance1.Id), insurance1);
        }

        private bool check(object sender, EventArgs e)
        {
            int i;
            if (textBox1.Text.Trim().Equals(""))
            {
                MessageBox.Show("Введите ФИО застрахованного", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                textBox1.Focus();
                return true;
            }
            if (textBox2.Text.Trim().Equals(""))
            {
                MessageBox.Show("Введите адрес застрахованного", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                textBox2.Focus();
                return true;
            }
            if (textBox3.Text.Trim().Equals(""))
            {
                MessageBox.Show("Введите ФИО сотрудника", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                textBox3.Focus();
                return true;
            }
            if (!Int32.TryParse(textBox5.Text.Trim(), out i) && !textBox5.Text.Trim().Equals(""))
            {
                MessageBox.Show("Стаж должен быть целым числом", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                textBox5.Clear();
                textBox5.Focus();
                return true;
            }
            if (textBox6.Text.Trim().Equals(""))
            {
                MessageBox.Show("Введите тип страховки", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                textBox6.Focus();
                return true;
            }
            if (textBox7.Text.Trim().Equals(""))
            {
                MessageBox.Show("Введите страховую сумму", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                textBox7.Focus();
                return true;
            }
            if (!Int32.TryParse(textBox7.Text.Trim(), out i))
            {
                MessageBox.Show("Страховая сумма должна быть целым числом", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                textBox7.Clear();
                textBox7.Focus();
                return true;
            }
            if (textBox8.Text.Trim().Equals(""))
            {
                MessageBox.Show("Введите номер страховки", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                textBox8.Focus();
                return true;
            }
            if (!Int32.TryParse(textBox8.Text.Trim(), out i))
            {
                MessageBox.Show("Номер страховки должен быть целым числом", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                textBox8.Clear();
                textBox8.Focus();
                return true;
            }
            return false;
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
