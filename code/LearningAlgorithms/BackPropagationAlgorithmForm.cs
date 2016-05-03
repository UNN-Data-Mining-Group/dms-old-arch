using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LearningAlgorithms.Parameter;
using NeuroWnd.Parameter;

namespace LearningAlgorithms
{
    public enum OutputAttributeMode
    {
        AsItIs,
        IntNormalised,
        DoubleNormalised
    };
    public partial class BackPropagationAlgorithmForm : Form
    {
        private INeuroNetLearning neuroNet;
        private DataSet dataSet;
        private bool isLearningCompleted = false;
        private IParameterValueComparer comparer;
        private BackgroundWorker worker = null;

        public BackPropagationAlgorithmForm(INeuroNetLearning solver, double[,] trainingSet, IParameterValueComparer comparer)
        {
            neuroNet = solver;
            this.comparer = comparer;

            dataSet = new DataSet();
            for (int i = 0; i < trainingSet.GetLength(0); i++)
            {
                double[] x = new double[trainingSet.GetLength(1) - 1];
                for (int j = 0; j < x.Length; j++)
                {
                    x[j] = trainingSet[i, j];
                }
                dataSet.AddSample(x, trainingSet[i, trainingSet.GetLength(1) - 1]);
            }

            InitializeComponent();

            btnWriteResult.Enabled = false;

            lbTestError.Text = "";
            lbTrainError.Text = "";
            lbCurrentIter.Text = "";
        }

        private void learn(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            
            try
            {
                int requiredError = 100 - Convert.ToInt32(tbMinTestAcc.Text);
                int iterations = Convert.ToInt32(tbIterationNumber.Text);
                double speed = Convert.ToDouble(tbSpeed.Text);
                double trainPersent = Convert.ToDouble(tbTrainPercent.Text);

                List<int> trainIndexes = new List<int>();
                List<int> testIndexes = new List<int>();

                int trainSize = Convert.ToInt32(dataSet.Size*trainPersent);
                Random rnd = new Random();

                for (int i = 0; i < trainSize; i++)
                {

                    int ind = 0;
                    do
                        ind = rnd.Next(dataSet.Size);
                    while (trainIndexes.Contains(ind));
                    trainIndexes.Add(ind);
                }

                for (int i = 0; i < dataSet.Size; i++)
                {
                    if(!trainIndexes.Contains(i))
                        testIndexes.Add(i);
                }

                DataSet train = new DataSet();
                foreach (int sampleIndex in trainIndexes)
                    train.AddSample(dataSet.GetX(sampleIndex), dataSet.GetY(sampleIndex));

                DataSet test = new DataSet();
                foreach (int sampleIndex in testIndexes)
                    test.AddSample(dataSet.GetX(sampleIndex), dataSet.GetY(sampleIndex));

                BackPropagationLearner learner = new BackPropagationLearner(neuroNet, train);
                learner.Speed = speed;

                int currentIteration = 0;
                bool isFirstIteration = true;

                double testError1 = 0.0, testError2 = 0.0;
                double trainError1 = 0.0, trainError2 = 0.0;

                do
                {
                    learner.Learn(!isFirstIteration);

                    isFirstIteration = false;
                    currentIteration++;

                    for (int i = 0; i < train.Size; i++)
                    {
                        double y = neuroNet.get_res(train.GetX(i));

                        if (!comparer.isEqual(y, train.GetY(i)))
                            trainError1++;
                        trainError2 += Math.Pow(y - train.GetY(i), 2.0);
                    }
                    trainError1 /= train.Size;
                    trainError2 /= 2.0;

                    for (int i = 0; i < test.Size; i++)
                    {
                        double y = neuroNet.get_res(test.GetX(i));

                        if (!comparer.isEqual(y, test.GetY(i)))
                           testError1++;
                        testError2 += Math.Pow(y - test.GetY(i), 2.0);
                    }

                    testError1 /= test.Size;
                    testError2 /= 2.0;

                    worker.ReportProgress(0, new List<double>{trainError1, trainError2,  testError1, testError2, currentIteration, iterations});
                }
                while (currentIteration < iterations && !worker.CancellationPending && requiredError <= Convert.ToInt32(testError1 * 100.0));
                save_result();
                isLearningCompleted = true;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                isLearningCompleted = false;
            }
            
        }
        private void save_result()
        {
            using (System.IO.StreamWriter stream = new System.IO.StreamWriter("data\\matrix_back.txt"))
            {
                double[,] tmp = neuroNet.get_links();
                for (int i = 0; i < tmp.GetLength(0); i++)
                {
                    for (int j = 0; j < tmp.GetLength(1); j++)
                    {
                        stream.Write("{0} ", tmp[i, j]);
                    }
                    stream.WriteLine();
                }
            }

        }
        private void btnLearn_Click(object sender, EventArgs e)
        {
            btnLearn.Enabled = false;

            worker = new BackgroundWorker();
            worker.DoWork += learn;
            worker.ProgressChanged += WorkerOnProgressChanged;
            worker.RunWorkerCompleted += WorkerOnRunWorkerCompleted;
            worker.WorkerSupportsCancellation = true;
            worker.WorkerReportsProgress = true;

            worker.RunWorkerAsync();
        }

        private void WorkerOnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs runWorkerCompletedEventArgs)
        {
            if (!isLearningCompleted)
            {
                lbTrainError.Text = lbTestError.Text = lbCurrentIter.Text = String.Empty;
            }

            btnWriteResult.Enabled = isLearningCompleted;
            btnLearn.Enabled = true;
        }

        private void WorkerOnProgressChanged(object sender, ProgressChangedEventArgs progressChangedEventArgs)
        {
            List<double> pars = progressChangedEventArgs.UserState as List<double>;

            lbTestError.Text = String.Format("Ошибка на тестовой выборке: {0}% ({1})", Convert.ToInt32(pars[2]*100), pars[3]);
            lbTrainError.Text = String.Format("Ошибка на обучающей выборке: {0}% ({1})", Convert.ToInt32(pars[0] * 100), pars[1]);
            lbCurrentIter.Text = String.Format("Итерация {0} из {1}", pars[4], pars[5]);
        }

        private void btnWriteResult_Click(object sender, EventArgs e)
        {
            neuroNet.write_result(new BackPropagationLearner().Name);
        }

        private void BackPropagationAlgorithmForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (worker != null)
                worker.CancelAsync();
        }
    }
}
