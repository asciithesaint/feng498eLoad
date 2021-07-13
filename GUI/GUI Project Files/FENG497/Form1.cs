using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Windows.Forms.DataVisualization.Charting;

namespace FENG497
{
    public partial class Form1 : Form
    {
        float testTimer;
        string[] ports = SerialPort.GetPortNames();
        double time;
        string constantCurrentString;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {

            textBox1.Text = "0,00";
            constantCurrentString = textBox1.Text;



            time = 0f;

            foreach (string port in ports)
            {
                comboBox4.Items.Add(port);
                comboBox4.SelectedIndex = 0;
            }

            comboBox3.Items.Add("9600");
            comboBox3.Items.Add("14400");
            comboBox3.Items.Add("19200");
            comboBox3.Items.Add("28800");
            comboBox3.Items.Add("38400");
            comboBox3.Items.Add("57600");
            comboBox3.Items.Add("115200");
            comboBox3.SelectedIndex = 6;

            label3.ForeColor = Color.Red;
            label3.Text = "Disconnected";

            chart1.ChartAreas[0].AxisX.Title = "Time(s)";

            //series color definition
            chart1.Series["Ampere"].ChartType = SeriesChartType.FastLine;
            chart1.Series["Ampere"].Color = Color.LawnGreen;
            chart1.Series["Ampere"].BorderWidth = 3;

            chart1.Series["Voltage"].ChartType = SeriesChartType.FastLine;
            chart1.Series["Voltage"].Color = Color.Red;
            chart1.Series["Voltage"].BorderWidth = 3;

            chart1.Series["Power"].ChartType = SeriesChartType.FastLine;
            chart1.Series["Power"].Color = Color.Cyan;
            chart1.Series["Power"].BorderWidth = 3;

            chart1.Series["Series4"].ChartType = SeriesChartType.FastLine;
            chart1.Series["Series4"].Color = Color.Transparent;

        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                //read
                string reads = serialPort1.ReadLine();
                string[] values = reads.Split('/');
                serialPort1.DiscardInBuffer();
                //calculate
                double ampere = float.Parse(values[0]);
                double voltage = float.Parse(values[1]);
                ampere = ampere * 20f / 1023f;
                if (ampere > 1.36)
                {
                    double eqInC = (ampere * ampere * 0.434733819) - (3.930096977 * ampere)
                        + 12.91697891;
                    ampere = ampere * (eqInC + 100.0) / 100.0;
                }
                else if (ampere > 0.01 && ampere < 0.02)
                {
                    ampere = ampere * 5;
                }
                else if (ampere > 0.02 && ampere < 0.07)
                {
                    ampere = ampere * 3;
                }
                else if (ampere > 0.07 && ampere < 0.17)
                {
                    ampere = ampere * 1.7;
                }
                else if (ampere > 0.17 && ampere < 0.216)
                {
                    ampere = ampere * 1.43;
                }
                else if (ampere > 0.216 && ampere < 0.32)
                {
                    ampere = ampere * 1.35;
                }
                else if (ampere > 0.32 && ampere < 0.87)
                {
                    ampere = ampere * 1.19784005;
                }
                else if (ampere > 0.87 && ampere < 1.037)
                {
                    ampere = ampere * 1.0920181452;
                }
                else if (ampere > 1.037 && ampere < 1.36)
                {
                    ampere = ampere * 1.0743934586;
                }

                aGauge1.Value = Convert.ToInt32(ampere);

                voltage = voltage * 50f / 1023f * 1.031;

                double power = voltage * ampere;
                //write to form app
                label14.Text = ampere.ToString();
                label2.Text = voltage.ToString();
                label1.Text = power.ToString();

                //time of plot
                time += 0.030f;

                double ampered = 0;
                double voltaged = 0;
                double powered = 0;
                //plot
                if (plotA.Checked == true)
                {
                    chart1.Series["Ampere"].Points.AddXY(time, ampere);
                    ampered = ampere;
                }

                if (plotV.Checked == true)
                {
                    chart1.Series["Voltage"].Points.AddXY(time, voltage);
                    voltaged = voltage;
                }

                if (plotP.Checked == true)
                {
                    chart1.Series["Power"].Points.AddXY(time, power);
                    powered = power;
                }

                double maximus = Math.Max(ampered, voltaged);
                maximus = Math.Max(maximus, powered);
                chart1.Series["Series4"].Points.AddXY(time, maximus + 0.1f);

                if (freeTime.Checked == false)
                {//stopping test with timer
                    if (time >= testTimer)
                    {
                        timer2.Stop();
                        timer1.Stop();
                        time = 0f;
                        timeBox.Text = "";
                        serialPort1.WriteLine("0");
                    }
                }

            }
            catch (Exception ex1)
            {
                timer2.Stop();
                timer1.Stop();
                try
                {
                    serialPort1.WriteLine("0");
                }
                catch (Exception ex2)
                {
                    MessageBox.Show(ex2.Message + "\n\nSerial port may be disconnected! Please, " +
                    "\ndisconnect and connect the port again." +
                    "\n\nElse, the problem is sampling speed;" +
                    "\ntimer interval of application can be too fast" +
                    "\naccording to serial port baud rate." +
                    "\nOr the setted ampere value is greater than 20 Ampere.");
                }
                MessageBox.Show(ex1.Message + "\n\nSerial port may be disconnected! Please, " +
                    "\ndisconnect and connect the port again." +
                    "\n\nElse, the problem is sampling speed;" +
                    "\ntimer interval of application can be too fast" +
                    "\naccording to serial port baud rate." +
                    "\nOr the setted ampere value is greater than 20 Ampere.");
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            //constant current timer
            try
            {
                float ampereFloat = Convert.ToSingle(constantCurrentString);
                ampereFloat = ampereFloat / 20f * 4096f / 103.61f * 100f;
                int intToSendDAC = Convert.ToUInt16(ampereFloat);
                string strToSendDAC = Convert.ToString(intToSendDAC);
                serialPort1.WriteLine(strToSendDAC);
            }
            catch (Exception ex1)
            {
                timer1.Stop();
                timer2.Stop();
                try
                {
                    serialPort1.WriteLine("0");
                }
                catch (Exception ex2)
                {
                    MessageBox.Show(ex2.Message + "\n\nSerial port may be disconnected! Please, " +
                    "\ndisconnect and connect the port again." +
                    "\n\nElse, the problem is sampling speed;" +
                    "\ntimer interval of application can be too fast" +
                    "\naccording to serial port baud rate." +
                    "\nOr the setted ampere value is greater than 20 Ampere.");
                }
                MessageBox.Show(ex1.Message + "\n\nSerial port may be disconnected! Please, " +
                    "\ndisconnect and connect the port again." +
                    "\n\nElse, the problem is sampling speed;" +
                    "\ntimer interval of application can be too fast" +
                    "\naccording to serial port baud rate." +
                    "\nOr the setted ampere value is greater than 20 Ampere.");
            }

        }

        private void button6_Click_1(object sender, EventArgs e)
        {//constant current set button
            constantCurrentString = textBox1.Text;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            //reflesh ports button
            string[] ports = SerialPort.GetPortNames();

            foreach (string port in ports)
            {
                comboBox4.Items.Clear();
                comboBox4.Items.Add(port);
                comboBox4.SelectedIndex = 0;
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {//serial port connect button
            if (serialPort1.IsOpen == false)
            {
                if (comboBox4.Text == "")
                {
                    return;
                }
                else if (comboBox3.Text == "")
                {
                    return;
                }

                serialPort1.PortName = comboBox4.Text;
                serialPort1.BaudRate = Convert.ToInt32(comboBox3.Text);

                try
                {
                    serialPort1.Open();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                if (serialPort1.IsOpen == true)
                {
                    label3.ForeColor = Color.Green;
                    label3.Text = "Connected";
                }

            }

        }

        private void button9_Click(object sender, EventArgs e)
        {
            //serial port disconnect button
            timer1.Stop();
            timer2.Stop();
            serialPort1.Close();
            if (serialPort1.IsOpen == false)
            {
                label3.ForeColor = Color.Red;
                label3.Text = "Disconnected";
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            //test start button
            foreach (var series in chart1.Series)
            {
                series.Points.Clear();
            }

            if (plotA.Checked == false && plotV.Checked==false && plotP.Checked==false)
            {
                chart1.ChartAreas[0].AxisY.Title = "";
            }
            else if(plotA.Checked == false && plotV.Checked == false && plotP.Checked == true)
            {
                chart1.ChartAreas[0].AxisY.Title = "Power(W)";
            }
            else if (plotA.Checked == false && plotV.Checked == true && plotP.Checked == false)
            {
                chart1.ChartAreas[0].AxisY.Title = "Voltage(V)";
            }
            else if (plotA.Checked == false && plotV.Checked == true && plotP.Checked == true)
            {
                chart1.ChartAreas[0].AxisY.Title = "Voltage(V) / Power(W)";
            }
            else if (plotA.Checked == true && plotV.Checked == false && plotP.Checked == false)
            {
                chart1.ChartAreas[0].AxisY.Title = "Ampere(A)";
            }
            else if (plotA.Checked == true && plotV.Checked == false && plotP.Checked == true)
            {
                chart1.ChartAreas[0].AxisY.Title = "Ampere(A) / Power(W)";
            }
            else if (plotA.Checked == true && plotV.Checked == true && plotP.Checked == false)
            {
                chart1.ChartAreas[0].AxisY.Title = "Ampere(A) / Voltage(V)";
            }
            else
            {
                chart1.ChartAreas[0].AxisY.Title = "Ampere(A) / Voltage(V) / Power(W)";
            }


            if (freeTime.Checked == false && timeBox.Text != "")
            {
                testTimer = float.Parse(timeBox.Text);
            }
            else
            {
                freeTime.Checked = true;
            }
            timer1.Start();
            timer2.Start();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //stop test button
            timer1.Stop();
            timer2.Stop();
            time = 0f;
            timeBox.Text = "";
            byte[] zero = { 0x00 };
            try
            {
                serialPort1.Write(zero, 0, 1);
            }
            catch (Exception)
            {

            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //save image button
            string ffileName = imageName.Text;
            if (!ffileName.StartsWith(@"\"))
            {
                MessageBox.Show("Please write, file name as\nlike: "+ 
                    @"'\name.formatType'");
                return;
            }

            try
            {
                string fileName = imageName.Text;
                folderBrowserDialog1.ShowDialog();
                chart1.SaveImage(folderBrowserDialog1.SelectedPath + fileName, 
                    ChartImageFormat.Png);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message+"\nPlease write, file name as\nlike: "
                    + @"'\name.formatType'");
            }
            
        }

       













        ////////////////////////////////////////////////////////////////////////////
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }
        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {

        }
        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {

        }
        //chart
        private void chart1_Click(object sender, EventArgs e)
        {

        }
        //ampere label
        private void label9_Click(object sender, EventArgs e)
        {

        }
        //connection info label
        
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void folderBrowserDialog1_HelpRequest(object sender, EventArgs e)
        {

        }

        private void freeTime_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void timeBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void label13_Click(object sender, EventArgs e)
        {

        }

        private void label14_Click(object sender, EventArgs e)
        {

        }

        private void label17_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label15_Click(object sender, EventArgs e)
        {

        }

        private void label16_Click(object sender, EventArgs e)
        {

        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void aGauge1_ValueInRangeChanged(object sender, ValueInRangeChangedEventArgs e)
        {

        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
