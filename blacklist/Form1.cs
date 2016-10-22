using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Data;
using System.Data.Entity;

namespace blacklist
{

    public partial class Form1 : Form
    {
        private db dbInstance = new db();        

        public Form1()
        {
            InitializeComponent();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string server = this.comboBox1.Text;
            string username = this.textBox1.Text;
            string reason = this.comboBox2.Text;
            string info = this.richTextBox1.Text;
            if (server == "" && username == "")
            {
                MessageBox.Show("服务器和ID请至少输入一个。");
                return;
            }

            if (reason == "")
            {
                MessageBox.Show("请至少输入一个黑名单理由。");
                return;
            }

            int userid = dbInstance.create(server, username, reason, info);
            if (userid <= 0)
            {
                MessageBox.Show("添加失败。");
                return;
            }
            else
            {                
                try
                {
                    PersonData[] result = dbInstance.queryID(userid);
                    if (result.Length <= 0)
                    {
                        MessageBox.Show("添加失败。");
                        return;
                    }

                    for(int i = 0; i < result.Length; i++)
                    {
                        DataGridViewRow row  = new DataGridViewRow();  
                        row.CreateCells(this.dataGridView1);
                        row.Cells[0].Value = result[i].id;
                        row.Cells[1].Value = result[i].server;
                        row.Cells[2].Value = result[i].username;
                        row.Cells[3].Value = result[i].reason;
                        row.Cells[4].Value = result[i].info;
                        this.dataGridView1.Rows.Add(row);
                    }
                   
                }
                catch
                {

                }

            }


        }

        private void button2_Click(object sender, EventArgs e)
        {
            string server = this.comboBox1.Text;
            string username = this.textBox1.Text;

            if (server == "" && username == "")
            {
                MessageBox.Show("服务器和ID请至少输入一个。");
                return;
            }

            this.dataGridView1.Rows.Clear();
            PersonData[] result = dbInstance.queryUser(server, username);
            if (result == null)
            {
                MessageBox.Show("无搜索结果");
                return;
            }

            if (result.Length == 0)
            {
                MessageBox.Show("无搜索结果");
                return;
            }

            
            for (int i = 0; i < result.Length; i++)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(this.dataGridView1);
                row.Cells[0].Value = result[i].id;
                row.Cells[1].Value = result[i].server;
                row.Cells[2].Value = result[i].username;
                row.Cells[3].Value = result[i].reason;                
                row.Cells[4].Value = result[i].info;
                this.dataGridView1.Rows.Add(row);
            }

        }

        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            MouseEventArgs a = (MouseEventArgs)e;

            if (a.Button == MouseButtons.Right)
            {
                if (dataGridView1.RowCount == 0)
                    return;

                if (MessageBox.Show("确定删除？", "", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                    return;

                foreach (var row in this.dataGridView1.SelectedRows)
                {
                    DataGridViewRow gridrow = (DataGridViewRow)row;
                    int id = (int)gridrow.Cells[0].Value;
                    if (0 <= dbInstance.delete(id))
                    {
                        this.dataGridView1.Rows.Remove(gridrow);
                    }
                }
            }
            else if(a.Button == MouseButtons.Right)
            {

            }
            
            
        }

    }


    public class db
    {
        SQLiteConnection conn = null;
        public db()
        {
            open();
            createTBFACE();
        }

        public void open()
        {
            conn = new SQLiteConnection();
            System.Data.SQLite.SQLiteConnectionStringBuilder connstr = new System.Data.SQLite.SQLiteConnectionStringBuilder();
            connstr.DataSource = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "blacklist.db";
            conn.ConnectionString = connstr.ToString();
            conn.Open();
        }

        public void close()
        {
            //1. close the connection string
            conn.Close();
        }


        private void createTBFACE()
        {
            string sql = "CREATE TABLE IF NOT EXISTS blacklist (id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, server TEXT(128), name TEXT(128), reason TEXT(128), info TEXT(128))";
            SQLiteCommand cmdCreateTable = new SQLiteCommand(sql, conn);
            cmdCreateTable.ExecuteNonQuery();//如果表不存在，创建数据表  
        }

        public int create(string server, string username, string reason, string info)
        {
            if (ConnectionState.Open != conn.State)
                return -1;

            //2. do the transaction
            SQLiteCommand cmd = new SQLiteCommand(conn);
            cmd.CommandText = "insert into blacklist (server, name, reason, info) values(@server, @name, @reason, @info)";//设置带参SQL语句  
            cmd.Parameters.AddRange(new[] {//添加参数  
                new SQLiteParameter("@server", server),  
                new SQLiteParameter("@name", username),  
                new SQLiteParameter("@reason", reason),  
                new SQLiteParameter("@info", info)
            });

            int id = 0;
            cmd.ExecuteScalar();
            cmd.CommandText = "SELECT last_insert_rowid()";
            var resultSet = cmd.ExecuteScalar();
            id = int.Parse(resultSet.ToString());
            if (id > 0)
                return id;
            else
                return -1;
        }

        public int delete(int id)
        {
            //1. get connection state
            if (ConnectionState.Open != conn.State)
                return -1;

            //2. do the transaction
            SQLiteCommand cmd = new SQLiteCommand(conn);
            cmd.CommandText = "delete from blacklist where id = @id";
            cmd.Parameters.AddRange(new[] {//添加参数  
                    new SQLiteParameter("@id", id)
                });

            cmd.ExecuteNonQuery();
            return 0;
        }

        public PersonData[] queryUser(string server, string name)
        {
            if (ConnectionState.Open != conn.State)
                return null;

            SQLiteCommand cmd = new SQLiteCommand(conn);
            if (server == "" && name != "")
                cmd.CommandText = "select * from blacklist where name = '" + name + "'";
            else if (name == "" && server != "")
                cmd.CommandText = "select * from blacklist where server = '" + server + "'";
            else if (name != "" && server != "")
                cmd.CommandText = "select * from blacklist where server = '" + server + "' and name = '" + name + "'";
            else
                return null;

            System.Data.SQLite.SQLiteDataReader reader = cmd.ExecuteReader();
            List<PersonData> personList = new List<PersonData>();

            while (reader.Read())
            {
                PersonData person = new PersonData();
                person.id = reader.GetInt16(0);
                person.server = reader.GetString(1);
                person.username = reader.GetString(2);
                person.reason = reader.GetString(3);
                person.info = reader.GetString(3);
                personList.Add(person);
            }

            return personList.ToArray();
        }

        public PersonData[] queryReason(string reason)
        {
            if (ConnectionState.Open != conn.State)
                return null;

            SQLiteCommand cmd = new SQLiteCommand(conn);
            cmd.CommandText = "select * from blacklist where reason = '" + reason + "'";
            System.Data.SQLite.SQLiteDataReader reader = cmd.ExecuteReader();
            List<PersonData> personList = new List<PersonData>();

            while (reader.Read())
            {
                PersonData person = new PersonData();
                person.id = reader.GetInt16(0);
                person.server = reader.GetString(1);
                person.username = reader.GetString(2);
                person.reason = reader.GetString(3);
                person.info = reader.GetString(3);
                personList.Add(person);
            }

            return personList.ToArray();
        }

        public PersonData[] queryID(int id)
        {
            if (ConnectionState.Open != conn.State)
                return null;

            SQLiteCommand cmd = new SQLiteCommand(conn);
            cmd.CommandText = "select * from blacklist where id = '" + id + "'";
            System.Data.SQLite.SQLiteDataReader reader = cmd.ExecuteReader();
            List<PersonData> personList = new List<PersonData>();
            while (reader.Read())
            {
                PersonData person = new PersonData();
                person.id = reader.GetInt16(0);
                person.server = reader.GetString(1);
                person.username = reader.GetString(2);
                person.reason = reader.GetString(3);
                person.info = reader.GetString(3);
                personList.Add(person);
            }

            return personList.ToArray();
        }
    }

    public struct PersonData
    {
        public int id;
        public string server;
        public string username;
        public string reason;
        public string info;
        public PersonData(int a)
        {
            id = 0;
            server = "";
            username = "";
            reason = "";
            info = "";
        }
    }

}
