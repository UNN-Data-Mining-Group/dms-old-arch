using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SII
{
    public class Selection
    {
        public int ID;
        public int TaskID;
        public String Name;
        public int CountRows;

        public bool WithRes; //обучающая выборка или нет(результат известен или нет)

        private List<ValueParametr> ArrValueParameters;

        public List<ValueParametr> GetArrValueParameters()
        {
            return ArrValueParameters;
        }

        public void LoadArrValueParametersFromFile(String namefile, List<Parametr> arrParameters, System.Windows.Forms.ProgressBar progressBar = null)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();            
            ArrValueParameters = ValueParametr.GetArrValuesFromFile(namefile, arrParameters, ID, WithRes);
            sw.Stop();
            Console.WriteLine("затрачено времени на чтение файла:{0}", sw.Elapsed);
            sw = System.Diagnostics.Stopwatch.StartNew();
            AddValuesToDB(progressBar);
            sw.Stop();
            Console.WriteLine("затрачено времени на вставку в бд:{0}", sw.Elapsed);
            CountRows = ArrValueParameters.Count / arrParameters.Count;
        }

        private void AddValuesToDB(System.Windows.Forms.ProgressBar progressBar = null)
        {
            if (progressBar != null)
            {
                progressBar.Minimum = 0;
                progressBar.Maximum = ArrValueParameters.Count;
                progressBar.Step = 1;
            }
            SQLManager sqlManager = SQLManager.MainSQLManager;
            String sqlReqStr;
            sqlManager.StartTransaction();
            foreach (ValueParametr value in ArrValueParameters)
            {
                sqlReqStr = "INSERT INTO VALUE_PARAM (PARAM_ID, SELECTION_ID, VALUE, ROW_INDEX) " +
                "VALUES('" + value.ParametrID + "','" + value.SelectionID + "','" + value.Value + "','" + value.RowIndex + "');";
                int state = sqlManager.SendInsertRequestWithTransaction(sqlReqStr);
                if (progressBar != null)
                {
                    progressBar.PerformStep();
                }                
            }
            sqlManager.EndTransaction(true);
            int lastId = sqlManager.LastId();
            for (int i = ArrValueParameters.Count - 1; i >= 0; i--)
            {
                ValueParametr value = ArrValueParameters[i];
                value.ID = lastId;
                lastId--;
            }
        }
    }
}
