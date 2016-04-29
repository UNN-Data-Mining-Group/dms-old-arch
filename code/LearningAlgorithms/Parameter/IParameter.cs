namespace NeuroWnd.Parameter
{
    public interface IParameter
    {
        string Type { get; }
        int CountNumbers { get; set; }

        string GetFromNormalized(int value);
        string GetFromNormalized(double value);
        double GetNormalizedDouble(string value);
        int GetNormalizedInt(string value);
    }
}
