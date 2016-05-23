using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LearningAlgorithms.Parameter;
using NeuroWnd.Neuro_Nets;

namespace NeuroWnd
{
    public partial class NeuroNetSolvingWindow : Form
    {
        private NeuroNet currentNet;
        private List<IParameterValueConverter> converters;
        private double[,] selection;
        private List<string> names;
        private List<string[]> solved;

        public NeuroNetSolvingWindow(NeuroNet net, double[,] convertedSelection, List<IParameterValueConverter> converters, List<string> names)
        {
            InitializeComponent();

            currentNet = net;
            selection = convertedSelection;
            this.converters = converters;
            this.names = names;
        }

        private void solve(object sender, DoWorkEventArgs doWorkEventArgs)
        {
            int percentage = 0;

            solved = new List<string[]>();

            for (int i = 0; i < selection.GetLength(0); i++)
            {
                double[] curX = new double[selection.GetLength(1) - 1];
                double[] curY = new double[1];

                for (int j = 0; j < selection.GetLength(1) - 1; j++)
                {
                    curX[j] = selection[i, j];
                }

                curY = currentNet.Solve(curX);

                string[] res = new string[selection.GetLength(1) + 1];

                res[0] = (i+1).ToString();
                int lastInd = selection.GetLength(1) - 1;
                for (int j = 0; j < lastInd; j++)
                {
                    res[j + 1] = converters[j].Get(curX[j]);
                }
                res[lastInd + 1] = converters[lastInd].Get(curY[0]);

                solved.Add(res);

                ((BackgroundWorker)sender).ReportProgress(100 * i / selection.GetLength(0));
            }
        }

        private void btnWrite_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName != string.Empty)
            {
                using (StreamWriter writer = new StreamWriter(saveFileDialog1.OpenFile()))
                {
                    for (int i = 0; i < selection.GetLength(0); i++)
                    {
                        for (int j = 0; j <= selection.GetLength(1); j++)
                        {
                            writer.Write(solved[i][j] + "\t");
                        }
                        writer.Write('\n');
                    }
                }
            }
        }

        private void start()
        {
            lbSolveStatus.Text = "Идет решение...";
            btnWrite.Enabled = false;
            dataGridView1.Rows.Clear();

            dataGridView1.Columns.Add("number", "Номер");
            for (int i = 0; i < selection.GetLength(1); i++)
            {
                dataGridView1.Columns.Add(i.ToString(), names[i]);
            }

            BackgroundWorker w = new BackgroundWorker();
            w.WorkerReportsProgress = true;
            w.WorkerSupportsCancellation = false;
            w.DoWork += solve;
            w.ProgressChanged += showProgress;
            w.RunWorkerCompleted += done;

            w.RunWorkerAsync();
        }

        private void done(object sender, RunWorkerCompletedEventArgs e)
        {
            for (int i = 0; i < selection.GetLength(0); i++)
            {
                dataGridView1.Rows.Add(solved[i]);
            }

            lbSolveStatus.Text = "Решено";
            btnWrite.Enabled = true;
            progressBar1.Value = 100;
        }

        private void showProgress(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void NeuroNetSolvingWindow_Shown(object sender, EventArgs e)
        {
            start();
        }
    }
}
