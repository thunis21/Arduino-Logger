using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Arduino_Datalogger
{
    public partial class Form1 : Form
    {
        ObservableCollection<SampleData> data = new ObservableCollection<SampleData>();
        public Form1()
        {
            InitializeComponent();
        }

        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
           
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            serialPort1.Open();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            char[] delimiterChars = { ':', '\t', '\r' };
            //DATA:31.40:250

            if (serialPort1.IsOpen)
            {
               var input= serialPort1.ReadLine();
                if (input.StartsWith("DATA"))
                {
                    string[] words = input.Split(delimiterChars);
                    data.Add(new SampleData {Temp =words[1], Speed=words[2]});

                    listBox1.Items.Add( String.Format("Temp:{0} Speed:{1}", words[1], words[2]));

                if (chkStorToDB.Checked)
                {
                    StoreToDB(words[1],words[2]);
                }  
                }

               
            }
            //dataGridView1.DataSource = data;
            //dataGridView1.Refresh();
        }

        private void StoreToDB(string Temp, string Speed)
        {
            string constring = "Server=DES-LAP03;Database=ArduinoDataLogger;Trusted_Connection=True;";
            using (SqlConnection con = new SqlConnection(constring))
            {
                if (con.State == ConnectionState.Closed) con.Open();
                string sql = "Insert Into DataLog (Temp,Speed,DateTimeRecieved) " +
                    "VALUES(@Temp,@Speed,@DateTimeRecieved)";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@Temp", Temp);
                cmd.Parameters.AddWithValue("@Speed", Speed);
                cmd.Parameters.AddWithValue("@DateTimeRecieved", DateTime.Now);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
