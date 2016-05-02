using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NeuroWnd.Parameter;

namespace LearningAlgorithms.Parameter
{
    public class RealValueComparer : IParameterValueComparer
    {
        public double ComparingRange
        {
            get { return _comparingRange; }
            set
            {
                if (value < 0)
                    throw new ArgumentException("Comparing range must be positive value");
                _comparingRange = value;
            }
        }
        public RealValueComparer(RealParameter par, 
            ParameterValueType t1, ParameterValueType t2)
        {
            _parameter = par;
            _type1 = t1;
            _type2 = t2;
            _comparingRange = par.MinRange / 2.0;
        }

        public bool isEqual(object value1, object value2)
        {
            double v1 = getFromValue(value1, _type1);
            double v2 = getFromValue(value2, _type2);

            if (Math.Abs(v1 - v2) < _comparingRange)
                return true;
            else
                return false;
        }

        private double getFromValue(object value, ParameterValueType type)
        {
            double res;
            switch (type)
            {
                case ParameterValueType.String:
                    res = _parameter.GetDouble(Convert.ToString(value));
                    break;
                case ParameterValueType.Integer:
                    throw new ArgumentException("Real can not be represented as integer");
                case ParameterValueType.Real:
                    res = Convert.ToDouble(value);
                    break;
                case ParameterValueType.NormalisedInteger:
                    res = Convert.ToDouble(_parameter.GetFromNormalized(Convert.ToInt32(value)));
                    break;
                case ParameterValueType.NormalisedReal:
                    res = Convert.ToDouble(_parameter.GetFromNormalized(Convert.ToDouble(value)));
                    break;
                default:
                    throw new ArgumentException("Unexpected value type");
            }
            return res;
        }

        private double _comparingRange;
        private readonly ParameterValueType _type1;
        private readonly ParameterValueType _type2;
        private readonly RealParameter _parameter;
    }
}
