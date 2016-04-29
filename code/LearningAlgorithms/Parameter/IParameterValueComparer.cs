using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LearningAlgorithms.Parameter
{
    public enum ParameterValueType
    {
        String,
        Integer,
        Real,
        NormalisedInteger,
        NormalisedReal
    };

    public interface IParameterValueComparer
    {
        bool isEqual(object value1, object value2);
    }
}
