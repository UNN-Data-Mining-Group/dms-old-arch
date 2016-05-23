using System;
using System.Collections.Generic;

namespace NeuroWnd.Parameter
{
    public class EnumeratedParameter: IParameter
    {
        public string Type { get { return "Enum"; } }
        public int CountNumbers
        {
            get { return countNumbers; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException();
                countNumbers = value;
            }
        }

        public EnumeratedParameter(List<string> values)
        {
            classes = new List<string>();

            foreach (string item in values)
            {
                if (!classes.Contains(item))
                    classes.Add(item);
            }
            countClasses = classes.Count;
            countNumbers = Convert.ToInt32(Math.Log10(2 * countClasses)) + 1;
        }

        public string GetFromNormalized(int value)
        {
            return GetFromNormalized(value / Math.Pow(10, countNumbers));
        }

        public string GetFromNormalized(double value)
        {
            double step = 1.0 / (2 * countClasses);

            if (value <= 0.0)
                value = step;
            else if (value >= 1.0)
                value = 1 - step;

            value -= step;
            value = value * countClasses;
            return classes[Convert.ToInt32(value)];
        }

        public string Get(int value)
        {
            if (value < 0)
                value = 0;
            else if (value > classes.Count - 1)
                value = classes.Count - 1;

            return classes[value];
        }

        public int GetInt(string value)
        {
            return classes.IndexOf(value);
        }

        public double GetNormalizedDouble(string value)
        {
            int val = GetInt(value);
            double step = 1.0 / countClasses;
            double temp = 0.0;
            for (int i = 0; i < countClasses; i++)
            {
                if (Math.Abs(val - temp) < 1e-10)
                {
                    return step / 2.0 + i * step;
                }
                temp++;
            }
            return Double.NaN;
        }

        public int GetNormalizedInt(string value)
        {
            double val = GetNormalizedDouble(value);
            return Convert.ToInt32(val * Math.Pow(10, countNumbers));
        }

        private readonly int countClasses;
        private int countNumbers;
        private readonly List<string> classes;
    }
}
