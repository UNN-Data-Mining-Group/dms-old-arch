using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LearningAlgorithms
{
    public partial class BackPropagationAlgorithmForm : Form
    {
        private INeuroNetLearning neuroNet;
        private DataSet dataSet;
        public BackPropagationAlgorithmForm(INeuroNetLearning solver, double[,] trainingSet)
        {
            neuroNet = solver;

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

            lbTestError.Text = "";
            lbTrainError.Text = "";
            lbCurrentIter.Text = "";
        }

        private void btnLearn_Click(object sender, EventArgs e)
        {
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
                        //trainError += Math.Pow(y - train.GetY(i), 2.0);
                        trainError += Convert.ToDouble(Convert.ToInt32(y) != Convert.ToInt32(train.GetY(i)));
                    }
                    trainError /= train.Size;

                    for (int i = 0; i < test.Size; i++)
                    {
                        double y = neuroNet.get_res(test.GetX(i));
                        //testError += Math.Pow(y - test.GetY(i), 2.0);
                        testError += Convert.ToDouble(Convert.ToInt32(y) != Convert.ToInt32(test.GetY(i)));
                    }
                    testError /= test.Size;

                    lbTestError.Text = String.Format("Ошибка на тестовой выборке: {0}", testError);
                    lbTrainError.Text = String.Format("Ошибка на обучающей выборке: {0}", trainError);
                    lbCurrentIter.Text = String.Format("Итерация {0} из {1}", currentIteration, iterations);
                } 
                while (currentIteration < iterations);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
