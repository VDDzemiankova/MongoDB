using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using System.Reflection;

namespace insurance_agency
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            DataGridViewButtonColumn btn1 = new DataGridViewButtonColumn();
            btn1.Text = "Изменить";
            btn1.Tag = "Yes";
            btn1.UseColumnTextForButtonValue = true;
            btn1.Name = "Изменение";
            btn1.FlatStyle = FlatStyle.Popup;
            btn1.HeaderCell.Value = "Изменение";
            dataGridView1.Columns.Add(btn1);

            DataGridViewButtonColumn btn2 = new DataGridViewButtonColumn();
            btn2.Tag = "Yes";
            btn2.Text = "Удалить";
            btn2.UseColumnTextForButtonValue = true;
            btn2.Name = "Удаление";
            btn2.FlatStyle = FlatStyle.Popup;
            btn2.HeaderCell.Value = "Удаление";
            dataGridView1.Columns.Add(btn2);

            for (int i = 0; i < 8; i++)
            {
                dataGridView1.Columns[i].Visible =  true;
                checkedListBox1.SetItemChecked(i, true);
            }

            dataGridView1.Width = dataGridView1.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) + 10;
            groupBox1.Width = dataGridView1.Width;
            foreach (DataGridViewColumn col in dataGridView1.Columns)
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
            ClientSize = new Size(ClientSize.Width, dataGridView1.Bottom + 26);
            ClientSize = new Size(dataGridView1.Left + dataGridView1.Width + 26, ClientSize.Height);
        }

        private void добавитьНовуюToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.Owner = this;
            form2.act = 'I';
            form2.textBox8.Enabled = true;
            this.Visible = false;
            form2.Show();
        }

        public async Task getData()
        {
            if (dataGridView1.Rows.Count != 0)
                dataGridView1.Rows.Clear();
            string connectionString = ConfigurationManager.ConnectionStrings["MongoDb"].ConnectionString;
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("insurance_agency");
            var collection = database.GetCollection<Insurance>("insurance");
            var filter = getFilter();
            string sort = getSort();
            var insurances = (sort.Length == 0) ? await collection.Find(filter).ToListAsync(): await collection.Find(filter).Sort(sort).ToListAsync();
            foreach (Insurance ins in insurances)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dataGridView1);
                row.Cells[0].Value = ins.Id.ToString();
                row.Cells[1].Value = ins.Insured.Name;
                row.Cells[2].Value = ins.Insured.Adress;
                row.Cells[3].Value = ins.Type;
                row.Cells[4].Value = ins.Amount;
                row.Cells[5].Value = ins.Employee.Name;
                row.Cells[6].Value = ins.Employee.Adress;
                if (ins.Employee.Experience != 0)
                    row.Cells[7].Value = ins.Employee.Experience.ToString();
                dataGridView1.Rows.Add(row);
            }

            dataGridView1.Width = dataGridView1.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) + 10;
            groupBox1.Width = dataGridView1.Width;
            button1.Width = groupBox1.Width - 10;
            foreach (DataGridViewColumn col in dataGridView1.Columns)
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
            ClientSize = new Size(ClientSize.Width, dataGridView1.Bottom + 26);
            ClientSize = new Size(dataGridView1.Left + dataGridView1.Width + 26, ClientSize.Height);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 8; i++)
            {
                dataGridView1.Columns[i].Visible = checkedListBox1.GetItemCheckState(i).ToString() == "Checked" ? true : false;
            }

            getData().GetAwaiter();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 8 && e.RowIndex != -1)
            {
                int row_num = e.RowIndex;
                Form2 form2 = new Form2();
                form2.act = 'U';
                form2.Owner = this;
                form2.textBox8.Enabled = false;
                form2.textBox8.Text = dataGridView1[0, row_num].Value.ToString();
                form2.textBox1.Text = dataGridView1[1, row_num].Value.ToString();
                form2.textBox2.Text = dataGridView1[2, row_num].Value.ToString();
                form2.textBox3.Text = dataGridView1[5, row_num].Value.ToString();
                if (dataGridView1[6, row_num].Value != null) 
                    form2.textBox4.Text = dataGridView1[6, row_num].Value.ToString();
                if (dataGridView1[7, row_num].Value != null)
                    form2.textBox5.Text = dataGridView1[7, row_num].Value.ToString();
                form2.textBox6.Text = dataGridView1[3, row_num].Value.ToString();
                form2.textBox7.Text = dataGridView1[4, row_num].Value.ToString();
                this.Visible = false;
                form2.Show();
            }
            if (e.ColumnIndex == 9 && e.RowIndex != -1)
            {
                int num = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString());
                if (MessageBox.Show($"Вы точно хотите удалить страховку номер {num.ToString()}?", "Подтверждение удаления", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.No)
                {
                    MessageBox.Show($"Операция отменена", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                    return;
                }
                try
                {
                    DeleteDocs(num).GetAwaiter();
                }
                catch(Exception ee)
                {
                    MessageBox.Show(ee.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                }
                MessageBox.Show($"Страховка номер {num.ToString()} успешно удалена", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                getData().GetAwaiter();
            }
        }

        private static async Task DeleteDocs(int num)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MongoDb"].ConnectionString;
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("insurance_agency");
            var collection = database.GetCollection<Insurance>("insurance");
            await collection.DeleteOneAsync(new BsonDocument("_id", num));
        }

        private void Form1_VisibleChanged(object sender, EventArgs e)
        {
            getData().GetAwaiter();
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private BsonDocument getFilter()
        {
            int i;
            if (!Int32.TryParse(textBox1.Text.Trim(), out i) && textBox1.Text.Trim().Length != 0)
            {
                MessageBox.Show("Номер страховки должен быть целым числом", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                textBox1.Clear();
                textBox1.Focus();
                return new BsonDocument();
            }
            if (!Int32.TryParse(textBox4.Text.Trim(), out i) && textBox4.Text.Trim().Length != 0)
            {
                MessageBox.Show("Страховая сумма должна быть целым числом", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                textBox4.Clear();
                textBox4.Focus();
                return new BsonDocument();
            }
            if (!Int32.TryParse(textBox6.Text.Trim(), out i) && textBox6.Text.Trim().Length != 0)
            {
                MessageBox.Show("Страховая сумма должна быть целым числом", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                textBox6.Clear();
                textBox6.Focus();
                return new BsonDocument();
            }
            BsonArray filters = new BsonArray();
            if (Int32.TryParse(textBox1.Text.Trim(), out i) && textBox1.Text.Trim().Length != 0)
            {
                filters.Add(new BsonDocument("_id", i));
            }
            if (textBox2.Text.Trim() != "")
            {
                filters.Add(new BsonDocument("insured.name", textBox2.Text.Trim()));
            }
            if (textBox3.Text.Trim() != "")
            {
                filters.Add(new BsonDocument("type", textBox3.Text.Trim()));
            }
            if (textBox5.Text.Trim() != "")
            {
                filters.Add(new BsonDocument("employee.name", textBox5.Text.Trim()));
            }
            if (Int32.TryParse(textBox4.Text.Trim(), out i) && textBox4.Text.Trim().Length != 0)
            {
                filters.Add(new BsonDocument("amount", new BsonDocument("$gte", i)));
            }
            if (Int32.TryParse(textBox6.Text.Trim(), out i) && textBox6.Text.Trim().Length != 0)
            {
                filters.Add(new BsonDocument("amount", new BsonDocument("$lte", i)));
            }
            var filter = (filters.Count == 0) ? new BsonDocument() : ((filters.Count == 1) ? filters[0].AsBsonDocument : new BsonDocument("$and", filters));
            return filter;
        }

        private string getSort()
        {
            string sort = "";
            int asc = checkBox1.Checked ? -1 : 1;

            for (int i = 0; i < 8; i++)
            {
                if (checkedListBox2.GetItemCheckState(i).ToString() == "Checked")
                {
                    switch(i)
                    {
                        case 0:
                            sort += $"_id: {asc}, ";
                            break;
                        case 1:
                            sort += $" \"insured.name\": {asc}, ";
                            break;
                        case 2:
                            sort += $"\"insured.adress\": {asc}, ";
                            break;
                        case 3:
                            sort += $"type: {asc}, ";
                            break;
                        case 4:
                            sort += $"amount: {asc}, ";
                            break;
                        case 5:
                            sort += $"\"employee.name\": {asc}, ";
                            break;
                        case 6:
                            sort += $"\"employee.adress\": {asc}, ";
                            break;
                        case 7:
                            sort += $"\"employee.experience\": {asc}, ";
                            break;
                    }
                }
            }
            if (sort.Length != 0)
            {
                sort = sort.Substring(0, sort.Length - 2);
                sort = "{" + sort + "}";
            }
            return sort;
        }
    }
}
