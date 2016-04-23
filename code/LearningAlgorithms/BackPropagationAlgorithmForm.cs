using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
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
        private bool isPercentError;
        private IParameter outParameter;
        private OutputAttributeMode mode;

        public BackPropagationAlgorithmForm(INeuroNetLearning solver, double[,] trainingSet, IParameter outPar, OutputAttributeMode outMode)
        {
            neuroNet = solver;
            outParameter = outPar;
            mode = outMode;

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

                do
                {
                    learner.Learn(!isFirstIteration);

                    isFirstIteration = false;
                    currentIteration++;
                    double testError = 0.0;
                    double trainError = 0.0;

                    for (int i = 0; i < train.Size; i++)
                    {
                        double y = neuroNet.get_res(train.GetX(i));

                        if (isPercentError)
                        {
                            if (outParameter.Type.Equals("Real"))
                            {
                                RealParameter p = outParameter as RealParameter;
                                double solvedY = 0.0;
                                double rightY = 0.0;
                                if (mode == OutputAttributeMode.AsItIs)
                                {
                                    solvedY = y;
                                    rightY = train.GetY(i);
                                }
                                else if (mode == OutputAttributeMode.DoubleNormalised)
                                {
                                    solvedY = Convert.ToDouble(p.GetFromNormalized(y));
                                    rightY = Convert.ToDouble(p.GetFromNormalized(train.GetY(i)));
                                }
                                else if (mode == OutputAttributeMode.IntNormalised)
                                {
                                    solvedY = Convert.ToDouble(p.GetFromNormalized(Convert.ToInt32(y)));
                                    rightY = Convert.ToDouble(p.GetFromNormalized(Convert.ToInt32(train.GetY(i))));
                                }
                                if (Math.Abs(solvedY - rightY) >= Math.Pow(10, -p.CountNumbers)/2)
                                    trainError += 1;
                            }
                            else if (outParameter.Type.Equals("Integer"))
                            {
                                IntegerParameter p = outParameter as IntegerParameter;
                                int solvedY = 0;
                                int rightY = 0;
                                if (mode == OutputAttributeMode.AsItIs)
                                {
                                    solvedY = Convert.ToInt32(y);
                                    rightY = Convert.ToInt32(train.GetY(i));
                                }
                                else if (mode == OutputAttributeMode.DoubleNormalised)
                                {
                                    solvedY = Convert.ToInt32(p.GetFromNormalized(y));
                                    rightY = Convert.ToInt32(p.GetFromNormalized(train.GetY(i)));
                                }
                                else if (mode == OutputAttributeMode.IntNormalised)
                                {
                                    solvedY = Convert.ToInt32(p.GetFromNormalized(Convert.ToInt32(y)));
                                    rightY = Convert.ToInt32(p.GetFromNormalized(Convert.ToInt32(train.GetY(i))));
                                }
                                if (Math.Abs(solvedY - rightY) > 2)
                                    trainError += 1;
                            }
                            else if (outParameter.Type.Equals("Enum"))
                            {
                                EnumeratedParameter p = outParameter as EnumeratedParameter;
                                string solvedY = "";
                                string rightY = "";
                                if (mode == OutputAttributeMode.AsItIs)
                                {
                                    solvedY = p.Get(Convert.ToInt32(y));
                                    rightY = p.Get(Convert.ToInt32(train.GetY(i)));
                                }
                                else if (mode == OutputAttributeMode.DoubleNormalised)
                                {
                                    solvedY = p.GetFromNormalized(y);
                                    rightY = p.GetFromNormalized(train.GetY(i));
                                }
                                else if (mode == OutputAttributeMode.IntNormalised)
                                {
                                    solvedY = p.GetFromNormalized(Convert.ToInt32(y));
                                    rightY = p.GetFromNormalized(Convert.ToInt32(train.GetY(i)));
                                }
                                if (!solvedY.Equals(rightY))
                                    trainError += 1;
                            }
                        }
                        else
                            trainError += Math.Pow(y - train.GetY(i), 2.0);
                    }
                    trainError /= train.Size;

                    for (int i = 0; i < test.Size; i++)
                    {
                        double y = neuroNet.get_res(test.GetX(i));

                        if (isPercentError)
                        {
                            if (outParameter.Type.Equals("Real"))
                            {
                                RealParameter p = outParameter as RealParameter;
                                double solvedY = 0.0;
                                double rightY = 0.0;
                                if (mode == OutputAttributeMode.AsItIs)
                                {
                                    solvedY = y;
                                    rightY = test.GetY(i);
                                }
                                else if (mode == OutputAttributeMode.DoubleNormalised)
                                {
                                    solvedY = Convert.ToDouble(p.GetFromNormalized(y));
                                    rightY = Convert.ToDouble(p.GetFromNormalized(test.GetY(i)));
                                }
                                else if (mode == OutputAttributeMode.IntNormalised)
                                {
                                    solvedY = Convert.ToDouble(p.GetFromNormalized(Convert.ToInt32(y)));
                                    rightY = Convert.ToDouble(p.GetFromNormalized(Convert.ToInt32(test.GetY(i))));
                                }
                                if (Math.Abs(solvedY - rightY) >= Math.Pow(10, -p.CountNumbers)/2)
                                    testError += 1;
                            }
                            else if (outParameter.Type.Equals("Integer"))
                            {
                                IntegerParameter p = outParameter as IntegerParameter;
                                int solvedY = 0;
                                int rightY = 0;
                                if (mode == OutputAttributeMode.AsItIs)
                                {
                                    solvedY = Convert.ToInt32(y);
                                    rightY = Convert.ToInt32(test.GetY(i));
                                }
                                else if (mode == OutputAttributeMode.DoubleNormalised)
                                {
                                    solvedY = Convert.ToInt32(p.GetFromNormalized(y));
                                    rightY = Convert.ToInt32(p.GetFromNormalized(test.GetY(i)));
                                }
                                else if (mode == OutputAttributeMode.IntNormalised)
                                {
                                    solvedY = Convert.ToInt32(p.GetFromNormalized(Convert.ToInt32(y)));
                                    rightY = Convert.ToInt32(p.GetFromNormalized(Convert.ToInt32(test.GetY(i))));
                                }
                                if (Math.Abs(solvedY - rightY) > 2)
                                    testError += 1;
                            }
                            else if (outParameter.Type.Equals("Enum"))
                            {
                                EnumeratedParameter p = outParameter as EnumeratedParameter;
                                string solvedY = "";
                                string rightY = "";
                                if (mode == OutputAttributeMode.AsItIs)
                                {
                                    solvedY = p.Get(Convert.ToInt32(y));
                                    rightY = p.Get(Convert.ToInt32(test.GetY(i)));
                                }
                                else if (mode == OutputAttributeMode.DoubleNormalised)
                                {
                                    solvedY = p.GetFromNormalized(y);
                                    rightY = p.GetFromNormalized(test.GetY(i));
                                }
                                else if (mode == OutputAttributeMode.IntNormalised)
                                {
                                    solvedY = p.GetFromNormalized(Convert.ToInt32(y));
                                    rightY = p.GetFromNormalized(Convert.ToInt32(test.GetY(i)));
                                }
                                if (!solvedY.Equals(rightY))
                                    testError += 1;
                            }
                        }
                        else
                            testError += Math.Pow(y - test.GetY(i), 2.0);
                    }
                    testError /= test.Size;

                    worker.ReportProgress(0, new List<double>{trainError, testError, currentIteration, iterations});
                } 
                while (currentIteration < iterations);

                isLearningCompleted = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                isLearningCompleted = false;
            }
            
        }

        private void btnLearn_Click(object sender, EventArgs e)
        {
            btnLearn.Enabled = false;

            isPercentError = chbErrorPercent.Checked;

            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += learn;
            worker.ProgressChanged += WorkerOnProgressChanged;
            worker.RunWorkerCompleted += WorkerOnRunWorkerCompleted;
            worker.WorkerSupportsCancellation = false;
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

            lbTestError.Text = String.Format("Ошибка на тестовой выборке: {0}", pars[1]);
            lbTrainError.Text = String.Format("Ошибка на обучающей выборке: {0}", pars[0]);
            lbCurrentIter.Text = String.Format("Итерация {0} из {1}", pars[2], pars[3]);
        }

        private void btnWriteResult_Click(object sender, EventArgs e)
        {
            neuroNet.write_result(new BackPropagationLearner().Name);
        }
    }
}
