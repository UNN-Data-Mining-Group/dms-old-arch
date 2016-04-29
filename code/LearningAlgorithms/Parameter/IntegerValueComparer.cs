using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NeuroWnd.Parameter;

namespace LearningAlgorithms.Parameter
{
    public class IntegerValueComparer: IParameterValueComparer
    {
        public int ComparingRange
        {
            get { return _comparingRange; }
            set
            {
                if(value < 0)
                    throw new ArgumentException("Comparing range must be positive value");
                _comparingRange = value;
            }
        }

        public IntegerValueComparer(IntegerParameter par, 
            ParameterValueType t1, ParameterValueType t2)
        {
            _parameter = par;
            _type1 = t1;
            _type2 = t2;
            _comparingRange = 0;
        }

        public bool isEqual(object value1, object value2)
        {
            int v1 = getFromValue(value1, _type1);
            int v2 = getFromValue(value2, _type2);

            if (Math.Abs(v1 - v2) <= _comparingRange)
                return true;
            else
                return false;
        }

        private int getFromValue(object value, ParameterValueType type)
        {
            int res;
            switch (type)
            {
                case ParameterValueType.String:
                    res = _parameter.GetInt(Convert.ToString(value));
                    break;
                case ParameterValueType.Integer:
                    res = Convert.ToInt32(value);
                    break;
                case ParameterValueType.Real:
                    throw new ArgumentException("Integer can not be represented as real");
                case ParameterValueType.NormalisedInteger:
                    res = Convert.ToInt32(_parameter.GetFromNormalized(Convert.ToInt32(value)));
                    break;
                case ParameterValueType.NormalisedReal:
                    res = Convert.ToInt32(_parameter.GetFromNormalized(Convert.ToDouble(value)));
                    break;
                default:
                    throw new ArgumentException("Unexpected value type");
            }
            return res;
        }

        private int _comparingRange;
        private readonly ParameterValueType _type1;
        private readonly ParameterValueType _type2;
        private readonly IntegerParameter _parameter;
    }
}
