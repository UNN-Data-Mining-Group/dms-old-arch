using System;
using System.Collections.Generic;

namespace NeuroWnd.Parameter
{
    public class RealParameter : IParameter
    {
        public string Type { get { return "Real"; } }
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

        public double MinRange { get; private set; }

        public RealParameter(List<string> values)
        {
            if (values == null || values.Count == 0)
                throw new ArgumentException("values must contain at least one element");

            minValue = maxValue = Convert.ToDouble(values[0].Replace(".", ","));
            List<double> numbers = new List<double>();
            foreach (string item in values)
            {
                double val = Convert.ToDouble(item.Replace(".", ","));

                if (!numbers.Contains(val))
                    numbers.Add(val);

                minValue = Math.Min(minValue, val);
                maxValue = Math.Max(maxValue, val);
            }
            numbers.Sort();
            MinRange = double.PositiveInfinity;
            for (int i = 0; i < numbers.Count - 1; i ++)
            {
                MinRange = Math.Min(MinRange, Math.Abs(numbers[i] - numbers[i+1]));
            }

            countNumbers = -Convert.ToInt32(Math.Log10(MinRange)) + 1;
        }

        public string GetFromNormalized(int value)
        {
            double dval = value / Math.Pow(10, countNumbers);
            return GetFromNormalized(dval);
        }

        public string GetFromNormalized(double value)
        {
            if (value < 0.0)
                value = 0.0;
            else if (value > 1.0)
                value = 1.0;

            double size = maxValue - minValue;
            return Convert.ToString(minValue + value * size);
        }

        public string Get(double value)
        {
            return Convert.ToString(value);
        }

        public double GetDouble(string value)
        {
            double temp = Convert.ToDouble(value.Replace(".", ","));

            if (temp < minValue || temp > maxValue)
                throw new ArgumentOutOfRangeException();

            return temp;
        }

        public double GetNormalizedDouble(string value)
        {
            double val = GetDouble(value);
            return (val - minValue) / (maxValue - minValue);
        }

        public int GetNormalizedInt(string value)
        {
            double val = GetNormalizedDouble(value);
            return Convert.ToInt32(val * Math.Pow(10, countNumbers));
        }

        private double minValue, maxValue;
        private int countNumbers;
    }
}
