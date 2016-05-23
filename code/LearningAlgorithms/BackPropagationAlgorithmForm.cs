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
        private IParameterValueConverter converter;
        private BackgroundWorker worker = null;

        public BackPropagationAlgorithmForm(INeuroNetLearning solver, double[,] trainingSet, IParameterValueComparer comparer, IParameterValueConverter converter)
        {
            neuroNet = solver;
            this.comparer = comparer;
            this.converter = converter;

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
                int seed = Convert.ToInt32(tbSeed.Text);

                List<int> trainIndexes = new List<int>();
                List<int> testIndexes = new List<int>();

                int trainSize = Convert.ToInt32(dataSet.Size*trainPersent);
                Random rnd = new Random(seed);

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

                BackPropagationLearner learner = new BackPropagationLearner(neuroNet, train) {Speed = speed};

                int currentIteration = 0;
                bool isFirstIteration = true;

                double testError1, testError2;
                double trainError1, trainError2;


                using (StreamWriter writer = new StreamWriter("stat_learning.txt"))
                {
                    writer.WriteLine("Iteration\tTrain%\tTrainF\tTest%\tTestF\t");
                    do
                    {
                        testError1 = testError2 = 0.0;
                        trainError1 = trainError2 = 0.0;

                        learner.Speed = speed;
                        learner.Learn(!isFirstIteration);

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

                        isFirstIteration = false;

                        for (int i = 0; i < test.Size; i++)
                        {
                            double y = neuroNet.get_res(test.GetX(i));

                            if (!comparer.isEqual(y, test.GetY(i)))
                                testError1++;
                            testError2 += Math.Pow(y - test.GetY(i), 2.0);
                        }

                        testError1 /= test.Size;
                        testError2 /= 2.0;

                        writer.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}", currentIteration, 
                            trainError1*100, trainError2, testError1*100, testError2);

                        worker.ReportProgress(0,
                            new List<double>
                            {
                                trainError1,
                                trainError2,
                                testError1,
                                testError2,
                                currentIteration,
                                iterations,
                                speed
                            });
                    } while (currentIteration < iterations && !worker.CancellationPending &&
                             requiredError <= Convert.ToInt32(testError1*100.0));
                }

                save_result();
                isLearningCompleted = true;
                double err = 0.0;

                List<double> s = new List<double>();
                List<double> a = new List<double>();
                
                using (StreamWriter writer = new StreamWriter("answersTrain.txt"))
                {
                    writer.WriteLine("Instance\tActual\tSolved");
                    for(int i = 0; i < train.Size; i++)
                    {
                        double actual = train.GetY(i);
                        double solved = learner.NeuroNet.get_res(train.GetX(i));

                        s.Add(solved);
                        a.Add(actual);

                        if (!comparer.isEqual(actual, solved))
                            err++;

                        writer.WriteLine("{0}\t{1}\t{2}", (i+1), converter.Get(actual), 
                            converter.Get(solved));
                    }
                    err /= train.Size;
                    err *= 100;
                    writer.WriteLine("Error: {0} persent", err);
                }

                using (StreamWriter writer = new StreamWriter("outTrain.txt"))
                {
                    for(int i = 0; i < a.Count; i++)
                    {
                        writer.WriteLine("{0} {1}", a[i], s[i]);
                    }
                }

                a.Clear();
                s.Clear();
                err = 0.0;

                using (StreamWriter writer = new StreamWriter("answersTest.txt"))
                {
                    writer.WriteLine("Instance\tActual\tSolved");
                    for (int i = 0; i < test.Size; i++)
                    {
                        double actual = test.GetY(i);
                        double solved = learner.NeuroNet.get_res(test.GetX(i));

                        s.Add(solved);
                        a.Add(actual);

                        if (!comparer.isEqual(actual, solved))
                            err++;

                        writer.WriteLine("{0}\t{1}\t{2}", (i + 1), converter.Get(actual),
                            converter.Get(solved));
                    }
                    err /= test.Size;
                    err *= 100;
                    writer.WriteLine("Error: {0} persent", err);
                }

                using (StreamWriter writer = new StreamWriter("outTest.txt"))
                {
                    for (int i = 0; i < a.Count; i++)
                    {
                        writer.WriteLine("{0} {1}", a[i], s[i]);
                    }
                }
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
            tbSpeed.Text = Convert.ToString(pars[6]);
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
