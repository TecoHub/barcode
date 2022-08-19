using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Collections;

namespace Barcode
{
    public partial class Form1 : Form
    {

        ConnectionDB con = new ConnectionDB();

        //arraylist to getter and setter data  
        private static ArrayList Product_Code = new ArrayList();
        private static ArrayList Product_Name = new ArrayList();
        private static ArrayList P_UnitPrice = new ArrayList();


        public Form1()
        {
            InitializeComponent();
        }

        private void GetData()
        {
            try
            {
                con.Open();
                string query = "select * from Product_New";

                //MySqlDataReader row;  
                MySqlDataReader row;
                row = con.ExecuteReader(query);
                if (row.HasRows)
                {
                    while (row.Read())
                    {
                        Product_Code.Add(row["Product_Code"].ToString());
                        Product_Name.Add(row["Product_Name"].ToString());
                        P_UnitPrice.Add(row["P_UnitPrice"].ToString());

                    }
                }
                else
                {
                    MessageBox.Show("Data not found");
                }

                con.Close();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString());
            }
        }

        private void updateDatagrid()
        {
            maingrid.Rows.Clear();
            
            for (int i = 0; i < Product_Code.Count; i++)
            {
                //data
                DataGridViewRow newRow = new DataGridViewRow();
                maingrid.ColumnCount = 2;
                newRow.CreateCells(maingrid);
                newRow.Cells[0].Value = Product_Code[i];
                newRow.Cells[1].Value = Product_Name[i];
                maingrid.Rows.Add(newRow);

                //styles
                maingrid.Columns[1].Width = 386;
                maingrid.Columns[0].HeaderText = "商品コード";
                maingrid.Columns[1].HeaderText = "商品名";

            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            GetData();
            if (Product_Code.Count > 0)
            {
                updateDatagrid();
            }
            else
            {
                MessageBox.Show("Data not found");
            }
        }

        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView3_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            
            if (checkBox1.Checked)
            {
                maingrid.Rows.Clear();

                for (int i = 0; i < Product_Code.Count; i++)
                {
                    //data
                    DataGridViewRow newRow = new DataGridViewRow();
                    maingrid.ColumnCount = 3;
                    newRow.CreateCells(maingrid);
                    newRow.Cells[0].Value = Product_Code[i];
                    newRow.Cells[1].Value = Product_Name[i];
                    newRow.Cells[2].Value = P_UnitPrice[i];
                    maingrid.Rows.Add(newRow);

                    //styles
                    maingrid.Columns[1].Width = 326;
                    maingrid.Columns[2].Width = 60;
                    maingrid.Columns[0].HeaderText = "商品コード";
                    maingrid.Columns[1].HeaderText = "商品名";
                    maingrid.Columns[2].HeaderText = "価格";
                }

            } else
            {
                maingrid.Rows.Clear();

                for (int i = 0; i < Product_Code.Count; i++)
                {
                    //data
                    DataGridViewRow newRow = new DataGridViewRow();
                    maingrid.ColumnCount = 2;
                    newRow.CreateCells(maingrid);
                    newRow.Cells[0].Value = Product_Code[i];
                    newRow.Cells[1].Value = Product_Name[i];



                    maingrid.Rows.Add(newRow);

                    //styles
                    maingrid.Columns[1].Width = 386;
                    maingrid.Columns[0].HeaderText = "商品コード";
                    maingrid.Columns[1].HeaderText = "商品名";

                }
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            string textboxvalue = textBox2.Text;

                maingrid.Rows.Clear();
                try
                {
                    con.Open();
                    string query = "select * from Product_New where Product_Name like \'%" + textboxvalue + "%\'";

                    //MySqlDataReader row;  
                    MySqlDataReader row;
                    row = con.ExecuteReader(query);
                    if (row.HasRows)
                    {
                        while (row.Read())
                        {
                            Product_Code.Add(row["Product_Code"].ToString());
                            Product_Name.Add(row["Product_Name"].ToString());
                            P_UnitPrice.Add(row["P_UnitPrice"].ToString());
                        }
                    }
                    else
                    {
                        MessageBox.Show("Data not found");
                    }

                    con.Close();
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.ToString());
                }
            }
        }
    }
