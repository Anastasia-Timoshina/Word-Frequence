using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace IndTask
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Tree tree = new Tree("C:\\Users\\USER\\Desktop\\IndTask\\File.txt");

        List<char> word = new List<char>();
        Dictionary<string, double> dictionary = new Dictionary<string, double>();

        List<KeyValuePair<string, double>> storage = new List<KeyValuePair<string, double>>();
        public void Plot()
        {


            for (int i = 1; i <= storage.Count; i++)
            {
                string x = storage[i - 1].Key;
                double y = storage[i - 1].Value;
                chart.Invoke((MethodInvoker)(() => chart.Series[0].Points.AddXY(x, y)));
                richTextBox.Invoke((MethodInvoker)(() => richTextBox.Text += $"{storage[i - 1].Key} -------- {storage[i - 1].Value} -------- {(double)1 / i}\n"));
            }

        }
        static bool clicked = false;
        private void chart_Click(object sender, EventArgs e)
        {
            if (!clicked)
            {
                richTextBox.Text = "";
                dictionary.Clear();
                storage = storage.Skip(storage.Count).ToList();
                tree.Build();
                tree.Traversal(tree.root, word, dictionary);
                Console.WriteLine(storage.GetType());
                storage = dictionary.OrderBy(k => k.Key).OrderByDescending(d => d.Value).ToList();
                
                this.chart.Series[0].Points.Clear();


                this.chart.ChartAreas[0].AxisX.Interval = 20;
                this.chart.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
                this.chart.ChartAreas[0].AxisY.MajorGrid.Enabled = false;

                this.chart.ChartAreas[0].AxisX.Maximum = storage.Count;
                this.chart.ChartAreas[0].AxisY.Maximum = storage[0].Value;

                this.chart.ChartAreas[0].AxisX.LabelStyle.Angle = -90;

                this.chart.ChartAreas[0].CursorX.AutoScroll = true;
                this.chart.ChartAreas[0].AxisX.ScaleView.Zoomable = true;


                Thread t3 = new Thread(Plot);

                t3.Start();
                clicked = true;
            }
           
        }

            
    }
}

