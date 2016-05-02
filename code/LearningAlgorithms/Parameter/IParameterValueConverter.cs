using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NeuroWnd.Parameter;

namespace LearningAlgorithms.Parameter
{
    public interface IParameterValueConverter
    {
        string Get(double value);
    }

    public class EnumeratedParameterConverter : IParameterValueConverter
    {
        private ParameterValueType type;
        private EnumeratedParameter par;
        public EnumeratedParameterConverter(ParameterValueType _type, EnumeratedParameter _par)
        {
            type = _type;
            par = _par;
        }
        public string Get(double value)
        {
            switch (type)
            {
                case ParameterValueType.Integer:
                    return par.Get(Convert.ToInt32(value));
                case ParameterValueType.NormalisedInteger:
                    return par.GetFromNormalized(Convert.ToInt32(value));
                case ParameterValueType.NormalisedReal:
                    return par.GetFromNormalized(value);
                default:
                    return string.Empty;
            }
        }
    }

    public class IntegerParameterConverter : IParameterValueConverter
    {
        private ParameterValueType type;
        private IntegerParameter par;
        public IntegerParameterConverter(ParameterValueType _type, IntegerParameter _par)
        {
            type = _type;
            par = _par;
        }
        public string Get(double value)
        {
            switch (type)
            {
                case ParameterValueType.Integer:
                    return par.Get(Convert.ToInt32(value));
                case ParameterValueType.NormalisedInteger:
                    return par.GetFromNormalized(Convert.ToInt32(value));
                case ParameterValueType.NormalisedReal:
                    return par.GetFromNormalized(value);
                default:
                    return string.Empty;
            }
        }
    }

    public class RealParameterConverter : IParameterValueConverter
    {
        private ParameterValueType type;
        private RealParameter par;
        public RealParameterConverter(ParameterValueType _type, RealParameter _par)
        {
            type = _type;
            par = _par;
        }
        public string Get(double value)
        {
            switch (type)
            {
                case ParameterValueType.Real:
                    return par.Get(value);
                case ParameterValueType.NormalisedInteger:
                    return par.GetFromNormalized(Convert.ToInt32(value));
                case ParameterValueType.NormalisedReal:
                    return par.GetFromNormalized(value);
                default:
                    return string.Empty;
            }
        }
    }
}
