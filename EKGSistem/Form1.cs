using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using signalreader;
using System.Media;
using System.Windows.Forms.DataVisualization.Charting;


namespace EKGSistem
{
    public partial class Form1 : Form
    {


        double t = 0;
        string inputfile;
        int BPM, i;
        double[] RR = new double[10];
        string tip;
        QRS detektor = new QRS();

        
        public Form1()
        {
            InitializeComponent();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            clearChart();

        }
        private void clearChart()
        {
            chart1.Series[0].Points.Clear();

            for (int i = 0; i < 250; i++)
            {
                chart1.Series[0].Points.AddY(0);

            }
        }




        private void timer1_Tick(object sender, EventArgs e)
        {
            double[] a = new double[15];
            UlazniBuffer.ReadMany(out a, 15);
            double x;

            if (tip == "dat")
                x = (a.Last() - 1024) * 0.005;
            else
                x = a.Last();
            t += 0.04;
            bool check = detektor.QRSCheck(x);

            if (chart1.Series[0].Points.Count < 250)
            {

                chart1.Series[0].Points.AddY(x);
                if (check)
                {

                    chart1.Series[0].Points.Last().Label = "*";
                    chart1.Series[0].Points.Last().LabelAngle = 0;
                    i++;
                   
                    RR[i] = t;
                    BPM = (int)(60 / (RR.Sum() / 9));
                    t = 0;
                    BPMMetar.Text = BPM.ToString() + " BPM";
                    if (i == 9)
                    {
                        i = 0;
                    }
                }
                
            }
            else
            {
                for (int i = 0; i < chart1.Series[0].Points.Count - 1; i++)
                {
                    chart1.Series[0].Points[i] = chart1.Series[0].Points[i + 1];

                }


                chart1.Series[0].Points.RemoveAt(249);
                chart1.Series[0].Points.AddY(x);
                if (check)
                {


                    chart1.Series[0].Points.Last().Label = "*";
                    chart1.Series[0].Points.Last().LabelAngle = 45;

                    
                    i++;
                 
                    RR[i] = t;
                    BPM = (int)(60 / (RR.Sum() / 9));
                    BPMMetar.Text = BPM.ToString() + " BPM";
                    t = 0;
                    if (i == 9)
                    {
                        i = 0;
                    }
                }
                


            }

            chart1.Update();

        }




        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            inputfile = openFileDialog1.FileName;

            optionsToolStripMenuItem.Enabled = true;
        }



        private void bindingSource1_CurrentChanged(object sender, EventArgs e)
        {

        }

       

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void pauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            

        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            

        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void enableSoundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void BPMMetar_Click(object sender, EventArgs e)
        {
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void manualToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void groupBox1_Enter_1(object sender, EventArgs e)
        {

        }

        private void chart1_Click(object sender, MouseEventArgs e)
        {






        }

        private void button2_Click(object sender, EventArgs e)
        {
            Int16 chan = Convert.ToInt16(1);
            if (radioButton2.Checked) chan = Convert.ToInt16(2);

            clearChart();
            tip = inputfile.Split('.').Last();
            UlazniBuffer.Close();
            if (tip == "txt")
                UlazniBuffer.Open(inputfile, chan, EKGFileType.TEXT);
            else
                UlazniBuffer.Open(inputfile, chan, EKGFileType.BINARY);
            timer1.Start();
            i = 0;
            t = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            UlazniBuffer.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            UlazniBuffer.Close();
        }

        private void checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {
            
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        List<ToolTip> tools = new List<ToolTip>();
        private void chart1_MouseClick(object sender, MouseEventArgs e)
        {
            
            ToolTip toolTip = new System.Windows.Forms.ToolTip();
            toolTip.Draw += new System.Windows.Forms.DrawToolTipEventHandler(this.toolTip_Draw);
            Point chartLocationOnForm = chart1.FindForm().PointToClient(chart1.Parent.PointToScreen(chart1.Location));
            double x = chart1.ChartAreas[0].CursorX.Position;
            double y = chart1.ChartAreas[0].CursorY.Position;

            toolTip.OwnerDraw = true;
            toolTip.ForeColor = System.Drawing.Color.Green;
            toolTip.BackColor = System.Drawing.Color.Black;

            toolTip.UseAnimation = false;
            toolTip.UseFading = false;

            string s = comboBox1.Text;
            toolTip.Show(s.Substring(0, 1), this.chart1, e.Location.X, e.Location.Y);
            tools.Add(toolTip);
        }

        private void toolTip_Draw(object sender, DrawToolTipEventArgs e)
        {
           
            e.DrawBackground();
            e.DrawBorder();
            e.DrawText();
        }

        void funkcija() {
            foreach (ToolTip tip in tools) {
                tip.RemoveAll();
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {

            funkcija();

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }
}
