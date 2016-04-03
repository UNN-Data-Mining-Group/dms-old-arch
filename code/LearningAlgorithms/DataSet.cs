using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LearningAlgorithms
{
    public class DataSet
    {
        private List<double[]> xList = new List<double[]>();
        private List<double> yList = new List<double>(); 

        public int Size { get { return xList.Count; }}

        public void AddSample(double[] x, double y)
        {
            xList.Add(x);
            yList.Add(y);
        }

        public double[] GetX(int curSample)
        {
            return xList[curSample];
        }

        public double GetY(int curSample)
        {
            return yList[curSample];
        }
    }
}
