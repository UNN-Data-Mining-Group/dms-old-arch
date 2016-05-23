using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NeuroWnd.Neuron_definition;
using NeuroWnd.Activate_functions;
using LearningAlgorithms;

namespace NeuroWnd.Neuro_Nets
{
    public class NeuroNetLearningInterface : LearningAlgorithms.INeuroNetLearning
    {
        private DataBaseHandler dbHandler;
        private NeuroNet learned_net;
        private string netName;
        private string selectionName;

        public int CountLayers { get { return learned_net.NeuronsInLayers.GetLength(0); } }
        public bool IsRecursive { get; private set; }
        public int CountOfNeurons { get { return learned_net.NeuronsCount; } }
        public int CountInputNeurons { get { return learned_net.InputNeuronsCount; } }
        public int CountOutputNeurons { get { return learned_net.OutputNeuronsCount; } }
        public double[] GetOutputsOfAllNeurons(double[] x)
        {
            learned_net.Solve(x);
            double[] res = new double[learned_net.NeuronsCount];
            for (int i = 0; i < learned_net.NeuronsCount; i++)
            {
                Neuron n = learned_net.GetNeuron(i);
                res[i] = n.OutputValue;
            }
            return res;
        }

        public double GetAfDerivative(int neuronIndex, double x)
        {
            ActivateFunction af = learned_net.GetNeuron(neuronIndex).ActivateFunctionOfNeuron;
            return af.Derivative(x);
        }

        public double GetWeightedSum(int neuronIndex)
        {
            bool[,] topology = learned_net.ConnectionsOfNeurons;
            double[,] weights = learned_net.WeightsOfConnections;

            double res = 0.0;
            for (int i = 0; i < neuronIndex; i++)
            {
                if (topology[i, neuronIndex])
                    res += weights[i, neuronIndex]*learned_net.GetNeuron(i).OutputValue;
            }
            return res;
        }

        public int[] GetChilds(int neuronIndex)
        {
            List<int> childs = new List<int>();
            bool[,] topology = learned_net.ConnectionsOfNeurons;
            for (int i = neuronIndex + 1; i < learned_net.NeuronsCount; i++)
            {
                if (topology[neuronIndex, i])
                    childs.Add(i);
            }
            return childs.ToArray();
        }

        public bool IsIterationsFinished { get { return learned_net.IsIterationsFinished; } }
        public bool IsWaveCameToOutputNeuron { get { return learned_net.IsWaveCameToOutputNeuron; } }

        public bool[,] get_bool_links()
        {
            return learned_net.ConnectionsOfNeurons;
        }
        public double[,] get_links()
        {
            return learned_net.WeightsOfConnections;
        }
        public void set_links(double[,] links)
        {
            learned_net.WeightsOfConnections = links;
        }
        public double get_res(double[] X)
        {
            return learned_net.Solve(X)[0];
        }
        public INeuroNetLearning copy()
        {
            return new NeuroNetLearningInterface(this);
        }
        public void write_result(string algorithm)
        {
            LoadingWindow lw = new LoadingWindow();
            lw.MakeLoading(
                    () => dbHandler.WriteLearnedWeights(netName, selectionName,
                LearningAlgorithmsLibrary.GetNameOfTypeOfAlgoritm(algorithm),
                get_links(), get_bool_links(), lw),
                "Запись обученных весов в БД");
        }

        public bool IsWithSmoothActivateFunctions
        {
            get
            {
                for (int i = 0; i < learned_net.NeuronsCount; i++)
                {
                    if (!learned_net.GetNeuron(i).
                        ActivateFunctionOfNeuron.
                        HasContinuousDerivative)
                        return false;
                }
                return true;
            }
        }

        public NeuroNetLearningInterface(NeuroNet net, string _neuroNetName, string _selectionName, DataBaseHandler _dbh)
        {
            learned_net = net;
            netName = _neuroNetName;
            selectionName = _selectionName;
            dbHandler = _dbh;
        }
        public NeuroNetLearningInterface(NeuroNetLearningInterface inn)
        {
            dbHandler = inn.dbHandler;
            learned_net = new NeuroNet(inn.learned_net);
            netName = inn.netName;
            selectionName = inn.selectionName;
        }
    }
}
