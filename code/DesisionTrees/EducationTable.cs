using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SII;
using System.Globalization;
namespace DesisionTrees
{
    public class EducationTable:Selection
    {
        CultureInfo UsCulture = new CultureInfo("en-US");
        public List<ValueParametr[]> Rows;
        public int ParameterCount;

        public EducationTable()
        {
            Rows = new List<ValueParametr[]>();
            ParameterCount = 0; 
        }

        public EducationTable(List<ValueParametr> arrValues, List<Parametr> arrParams)
        {
            Rows = new List<ValueParametr[]>();
            ParameterCount = arrParams.Count();
            if (arrValues != null)
            {
                int prevIndex = arrValues[0].RowIndex;
                int indexInRow = 0;
                ValueParametr[] row = new ValueParametr[arrParams.Count()];               
                foreach (ValueParametr valueParam in arrValues)
                {                    
                    if (valueParam.RowIndex != prevIndex)
                    {
                        Rows.Add(row);
                        indexInRow = 0;
                        row = new ValueParametr[arrParams.Count()];                        
                    }
                    row[indexInRow] = valueParam;
                    indexInRow++;
                    prevIndex = valueParam.RowIndex;
                }
            }
            
        }

        public void BubbleSortByParam(int index_of_param, Parametr param)
        {
            if ((param.Type == TypeParametr.Real) || (param.Type == TypeParametr.Int))
            {
                for (int i = 0; i < Rows.Count; i++)
                {
                    for (int j = i + 1; j < Rows.Count; j++)
                    {
                        if (Convert.ToDouble(Rows.ElementAt(j)[index_of_param].Value, UsCulture) < Convert.ToDouble(Rows.ElementAt(i)[index_of_param].Value, UsCulture))
                        {
                            var temp = Rows[i];
                            Rows[i] = Rows[j];
                            Rows[j] = temp;
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < Rows.Count; i++)
                {
                    for (int j = i + 1; j < Rows.Count; j++)
                    {
                        if (String.Compare(Rows.ElementAt(j)[index_of_param].Value,Rows.ElementAt(i)[index_of_param].Value) < 0)
                        {
                            var temp = Rows[i];
                            Rows[i] = Rows[j];
                            Rows[j] = temp;
                        }
                    }
                }
            }
        }

        public void QuickSortByParam(int first, int last, int index_of_param, Parametr param)
        {
            if ((param.Type == TypeParametr.Real) || (param.Type == TypeParametr.Int))
            {
                int i = first, j = last;
                var x = Convert.ToDouble(Rows[(first + last) / 2][index_of_param].Value, UsCulture);//  a[(first + last) / 2];
                do
                {
                    while (Convert.ToDouble(Rows[i][index_of_param].Value, UsCulture) < x) i++;
                    while (Convert.ToDouble(Rows[j][index_of_param].Value, UsCulture) > x) j--;

                    if (i <= j)
                    {
                        if (i < j)
                        {
                            var temp = Rows[i];
                            Rows[i] = Rows[j];
                            Rows[j] = temp;
                        }
                        i++;
                        j--;
                    }
                } while (i <= j);
                if (i < last)
                    QuickSortByParam(i, last, index_of_param, param);
                if (first < j)
                    QuickSortByParam(first, j, index_of_param, param);
            }
            else
            {
                for (int i = 0; i < Rows.Count; i++)
                {
                    for (int j = i + 1; j < Rows.Count; j++)
                    {
                        if (String.Compare(Rows.ElementAt(j)[index_of_param].Value, Rows.ElementAt(i)[index_of_param].Value) < 0)
                        {
                            var temp = Rows[i];
                            Rows[i] = Rows[j];
                            Rows[j] = temp;
                        }
                    }
                }

            }
        }
        
    }
}
