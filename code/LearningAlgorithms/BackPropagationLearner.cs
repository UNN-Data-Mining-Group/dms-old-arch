using System;
using System.Linq;
using System.Text;

namespace LearningAlgorithms
{
    public class BackPropagationLearner: LearningAlgorithm
    {
        private double speed;
        private INeuroNetLearning neuroNet;
        private DataSet train;
        public override string Name
        {
            get { return "Алгоритм обратного распространения ошибки"; }
        }

        public BackPropagationLearner()
        {
            speed = 0.0;
            neuroNet = null;
            train = null;
        }

        public BackPropagationLearner(INeuroNetLearning neuroNet, DataSet train)
        {
            this.neuroNet = neuroNet;
            this.train = train;
        }

        public double Speed
        {
            get { return speed; }
            set
            {
                if (value > 0.0)
                    speed = value;
                else
                    throw new ArgumentOutOfRangeException();
            }
        }

        public INeuroNetLearning NeuroNet
        {
            get { return neuroNet; }
            set
            {
                if (value != null)
                    neuroNet = value;
                else
                    throw new ArgumentOutOfRangeException();
            }
        }

        public DataSet Train
        {
            get { return train; }
            set
            {
                if (value != null)
                    train = value;
                else
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Learn(bool isUsingCurrentWeights)
        {
            if (train == null || neuroNet == null)
                throw new ArgumentNullException();

            if (!neuroNet.IsWithSmoothActivateFunctions || neuroNet.IsRecursive || speed <= 0.0)
                throw new ArgumentException();

            int neuronsCount = neuroNet.CountOfNeurons;
            bool[,] topologyMatrix = neuroNet.get_bool_links();
            double[,] weights;

            if (!isUsingCurrentWeights)
            {
                weights = initializeRandom(neuronsCount, topologyMatrix);
                neuroNet.set_links(weights);
            }

            double[] errByNeurons = new double[neuronsCount];
            for (int i = 0; i < neuronsCount; i++)
                errByNeurons[i] = 0.0;

            for (int curSample = 0; curSample < train.Size; curSample++)
            {
                weights = neuroNet.get_links();

                double[] outs = neuroNet.GetOutputsOfAllNeurons(train.GetX(curSample));
                int indexOfFirstOutputNeuron = neuroNet.CountOfNeurons - neuroNet.CountOutputNeurons;
                for (int i = indexOfFirstOutputNeuron; i < neuroNet.CountOfNeurons; i++)
                {
                    errByNeurons[i] = -neuroNet.GetAfDerivative(i, neuroNet.GetWeightedSum(i)) *
                                      (outs[i] - train.GetY(curSample));
                }

                for (int i = indexOfFirstOutputNeuron - 1; i >= 0; i--)
                {
                    int[] childs = neuroNet.GetChilds(i);
                    foreach (int childIndex in childs)
                    {
                        errByNeurons[i] += errByNeurons[childIndex] * weights[i, childIndex];
                    }
                    errByNeurons[i] *= neuroNet.GetAfDerivative(i, neuroNet.GetWeightedSum(i));
                }

                for (int i = 0; i < neuronsCount; i++)
                {
                    for (int j = 0; j < neuronsCount; j++)
                    {
                        if (topologyMatrix[i, j])
                            weights[i, j] += outs[i] * errByNeurons[j] * speed;
                    }
                }

                neuroNet.set_links(weights);
            }
        }

        private double[,] initializeRandom(int neuronsCount, bool[,] topologyMatrix)
        {
            Random rnd = new Random(13052016);
            double[,] weights = new double[neuronsCount, neuronsCount];

            for (int i = 0; i < neuronsCount; i++)
            {
                for (int j = 0; j < neuronsCount; j++)
                {
                    if (topologyMatrix[i, j])
                        weights[i, j] = (rnd.NextDouble() - 0.5) / 10.0;
                    else
                        weights[i, j] = 0.0;
                }
            }

            return weights;
        }
    }
}
