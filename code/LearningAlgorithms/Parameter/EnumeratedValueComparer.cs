using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NeuroWnd.Parameter;

namespace LearningAlgorithms.Parameter
{
    public class EnumeratedValueComparer: IParameterValueComparer
    {
        public EnumeratedValueComparer(EnumeratedParameter par, 
            ParameterValueType t1, ParameterValueType t2)
        {
            _parameter = par;
            _type1 = t1;
            _type2 = t2;
        }
        public bool isEqual(object value1, object value2)
        {
            string v1 = getFromValue(value1, _type1);
            string v2 = getFromValue(value2, _type2);

            return v1.Equals(v2);
        }

        private string getFromValue(object value, ParameterValueType type)
        {
            string res;
            switch (type)
            {
                case ParameterValueType.String:
                    res = Convert.ToString(value);
                    break;
                case ParameterValueType.Integer:
                    res = _parameter.Get(Convert.ToInt32(value));
                    break;
                case ParameterValueType.Real:
                    throw new ArgumentException("Enum can not be represented as real");
                case ParameterValueType.NormalisedInteger:
                    res = _parameter.GetFromNormalized(Convert.ToInt32(value));
                    break;
                case ParameterValueType.NormalisedReal:
                    res = _parameter.GetFromNormalized(Convert.ToDouble(value));
                    break;
                default:
                    throw new ArgumentException("Unexpected value type");
            }
            return res;
        }

        private readonly ParameterValueType _type1;
        private readonly ParameterValueType _type2;
        private readonly EnumeratedParameter _parameter;
    }
}
