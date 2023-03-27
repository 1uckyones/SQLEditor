using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Crypto.Tls;

namespace SQLQueryEditor
{
    public partial class MainWin : Form
    {
        private DBManager db = DBManager.GetIstance();
        public MainWin()
        {

            InitializeComponent();


            treeView1.ExpandAll();
            treeView1.Nodes.Add(db.GetDBname());
            treeView1.Nodes[0].ExpandAll();
            treeView1.Nodes[0].Nodes.Add("Table");
        }


        private void exec_query_Click(object sender, EventArgs e)
        {
            info_label.ForeColor = Color.Black;
            info_label.Text = "Executing ...";
            
            Task.Run(() =>
            {
                try
                {
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();
                    Tuple<List<string>, List<string[]>> col_data = db.ExecuteQuery(query_box.Text);

                   

                    List<string> col = col_data.Item1;
                    List<string[]> res = col_data.Item2;
                    

                    visualizzatore_risultati.Invoke((MethodInvoker)delegate
                    {
                        visualizzatore_risultati.Clear();

                        foreach (string c in col)
                            visualizzatore_risultati.Columns.Add(c);

                        foreach (string[] r in res)
                        {
                            ListViewItem item = new ListViewItem(r);
                            visualizzatore_risultati.Items.Add(item);
                        }
                        stopwatch.Stop();
                    });


                    info_label.Invoke((MethodInvoker)delegate
                    {
                        info_label.Text = "Query executed. Time " + Convert.ToDouble(stopwatch.ElapsedMilliseconds)  / 1000 + " s";
                        info_label.ForeColor = Color.Black;
                    });

                }
                catch (Exception exc)
                {
                    if (exc is MySqlException || exc is ArgumentNullException
                        || exc is InvalidOperationException || exc is NullReferenceException)
                    {
                        info_label.Invoke((MethodInvoker) delegate
                        {
                            info_label.Text = exc.Message;
                            info_label.ForeColor = Color.Red;
                        });

                    }
                }
            });
        }

        private void new_connection_Click(object sender, EventArgs e)
        {
            SQLQueryEditor conn = new SQLQueryEditor();
            conn.Visible = true;
        }

        private void MainWin_FormClosing(object sender, FormClosingEventArgs e)
        {
            db.CloseConnection();
        }

        private void close_and_exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }


    }
}
