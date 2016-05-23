using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SQLite;
using NeuroWnd.Neuro_Nets;
using NeuroWnd.Activate_functions;
using System.Reflection;
using LearningAlgorithms;
using LearningAlgorithms.Parameter;
using NeuroWnd.Parameter;

namespace NeuroWnd
{
    public partial class NeuroNetsMainWindow : Form
    {
        private DataBaseHandler dbHandler;
        private DataContainer<List<string[]>> neuroNetsInfo;
        private DataContainer<DataContainer<List<string>>> learningInfo;

        private bool isNeuroNetSelected;
        private bool isSelectionSelected;

        private void LoadInformationForUsingNeuroNets()
        {
            cbInt.SelectedIndex = cbEnum.SelectedIndex = cbReal.SelectedIndex = 2;

            neuroNetsInfo.Clear();
            learningInfo.Clear();
            tvTaskSelections.Nodes.Clear();
            
            dgwNets.Rows.Clear();
            dgwLA.Rows.Clear();

            List<Tuple<int, string, int>> ls = dbHandler.SelectAllTasks();
            foreach (Tuple<int, string, int> item in ls)
            {
                List<string> bf = dbHandler.SelectAllNeuroNetsByTask(item.Item2);
                List<string> selections = dbHandler.SelectSelectionsNames(item.Item2);
                List<string[]> itemContainer = new List<string[]>();
                foreach (string name in bf)
                {
                    learningInfo.AddData(name, new DataContainer<List<string>>());
                    foreach (string sel in selections)
                    {
                        learningInfo.FindData(name).AddData(sel, dbHandler.SelectLearningStatistics(name, sel));
                    }

                    string[] str = new string[4];
                    string[] wideStr = dbHandler.SelectNeuroNetDefinitionByName(name);
                    str[0] = wideStr[0];
                    str[1] = wideStr[1];
                    str[2] = wideStr[3];
                    str[3] = wideStr[4];
                    itemContainer.Add(str);
                }
                neuroNetsInfo.AddData(item.Item2, itemContainer);
            }

            for (int i = 0; i < ls.Count; i++)
            {
                tvTaskSelections.Nodes.Add(ls[i].Item2);
                tvTaskSelections.Nodes[i].NodeFont = new Font(new FontFamily("Book Antiqua"), 12);
                List<Tuple<string, int>> it = dbHandler.SelectAllSelections(ls[i].Item1);
                for (int j = 0; j < it.Count; j++)
                {
                    tvTaskSelections.Nodes[i].Nodes.Add(it[j].Item1);
                    tvTaskSelections.Nodes[i].Nodes[j].NodeFont = new Font(new FontFamily("Book Antiqua"), 11, FontStyle.Italic);
                }
            }

            lbTaskSelected.Text = "Не выбрана";
            lbSelSelected.Text = "Не выбрана";
            lbNetSelected.Text = "Не выбрана";
            lbLASelected.Text = "Не выбран";

            isNeuroNetSelected = false;
            isSelectionSelected = false;

            btnLearn.Enabled = false;
            btnUse.Enabled = false;
        }
        private void FillLearningAlgorithmsTable(string NeuroNetName, string SelectionName)
        {
            dgwLA.Rows.Clear();
            string[] names = LearningAlgorithmsLibrary.GetAllNamesOfAlgorithms();
            for (int i = 0; i < LearningAlgorithmsLibrary.CountAlgorithms; i++)
            {
                dgwLA.Rows.Add(names[i]);
            }

            List<string> ls = learningInfo.FindData(NeuroNetName).FindData(SelectionName);

            for (int i = 0; i < dgwLA.Rows.Count; i++)
            {
                string laName = dgwLA.Rows[i].Cells[0].Value.ToString();
                string laType = LearningAlgorithmsLibrary.GetNameOfTypeOfAlgoritm(laName);

                bool isConsist = false;
                foreach (string item in ls)
                {
                    if (String.Compare(item, laType) == 0)
                    {
                        isConsist = true;
                        break;
                    }
                }

                if (isConsist)
                {
                    dgwLA.Rows[i].Cells[1].Value = "Обучена";
                }
                else
                {
                    dgwLA.Rows[i].Cells[1].Value = "Не обучена";
                }
            }

            dgwLA.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgwLA.AutoResizeColumns();
        }
        private void FillNeuroNetChangingTable()
        {
            dgwNeuroNets.Rows.Clear();

            List<string[]> list = dbHandler.SelectNeuroNetDefinitions();
            for (int i = 0; i < list.Count; i++)
            {
                dgwNeuroNets.Rows.Add(list[i]);
            }

            dgwNeuroNets.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgwNeuroNets.AutoResizeColumns();
        }
        private void FillNeuroNetUsingTable(string TaskName)
        {
            dgwNets.Rows.Clear();

            List<string[]> ls = neuroNetsInfo.FindData(TaskName);
            foreach (string[] item in ls)
            {
                dgwNets.Rows.Add(item);
            }

            dgwNets.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgwNets.AutoResizeColumns();
        }

        public NeuroNetsMainWindow()
        {
            InitializeComponent();

            Type ourtype = typeof(ActivateFunction); // Базовый тип
            Assembly ast = Assembly.GetAssembly(ourtype);
            Type[] types = ast.GetTypes();
            IEnumerable<Type> list = Assembly.GetAssembly(ourtype).GetTypes().Where(type => type.IsSubclassOf(ourtype));  // using System.Linq
            List<Type> aa = new List<Type>(list);

            foreach (Type itm in aa)
            {
                ActivateFunction af1 = (ActivateFunction)Activator.CreateInstance(itm);
                string name = af1.GetType().Name;
            }

            dbHandler = new DataBaseHandler();
            neuroNetsInfo = new DataContainer<List<string[]>>();
            learningInfo = new DataContainer<DataContainer<List<string>>>();

            dgwNeuroNets.ColumnHeadersDefaultCellStyle.Font = new Font("Book Antiqua", 9);
            dgwNeuroNets.Columns.Add("Name", "Имя");
            dgwNeuroNets.Columns.Add("TopologyTypeName", "Топология");
            dgwNeuroNets.Columns.Add("Task", "Задача");
            dgwNeuroNets.Columns.Add("NeuronCount", "Количество нейронов");
            dgwNeuroNets.Columns.Add("LayerCount", "Количество слоев");
            dgwNeuroNets.Columns.Add("ActivateFunction", "Активационная функция");

            dgwNeuronsInLayers.Columns.Add("Layer", "Слой");
            dgwNeuronsInLayers.Columns.Add("Neurons", "Число нейронов");

            dgwParamsAF.Columns.Add("Parameter", "Параметр");
            dgwParamsAF.Columns.Add("Value", "Значение");

            dgwNets.ColumnHeadersDefaultCellStyle.Font = new Font("Book Antiqua", 9);
            dgwNets.Columns.Add("NeuroNet", "Нейронная сеть");
            dgwNets.Columns.Add("TopologyTypeName", "Топология");
            dgwNets.Columns.Add("NeuronCount", "Количество нейронов");
            dgwNets.Columns.Add("LayerCount", "Количество слоев");

            dgwLA.ColumnHeadersDefaultCellStyle.Font = new Font("Book Antiqua", 9);
            dgwLA.Columns.Add("LearningAlgorithm", "Алгоритм обучения");
            dgwLA.Columns.Add("LearningStatus", "Статус обучения");

            LoadInformationForUsingNeuroNets();
            FillNeuroNetChangingTable();
        }

        private void btnAddNeuroNet_Click(object sender, EventArgs e)
        {
            AddChangeNeuroNetDialog dialog = new AddChangeNeuroNetDialog(dbHandler);
            dialog.ShowDialog();
            LoadInformationForUsingNeuroNets();
            FillNeuroNetChangingTable();
            btnChangeNeuroNet.Enabled = false;
            btnDeleteNeuroNet.Enabled = false;
        }

        private void btnChangeNeuroNet_Click(object sender, EventArgs e)
        {
            string[] line = dbHandler.SelectNeuroNetDefinitionByName(dgwNeuroNets.SelectedRows[0].Cells[0].Value.ToString());

            NeuroNetDefinition ndef = new NeuroNetDefinition(line);
            AddChangeNeuroNetDialog dialog = new AddChangeNeuroNetDialog(dbHandler, ndef);
            dialog.ShowDialog();
            LoadInformationForUsingNeuroNets();
            FillNeuroNetChangingTable();
            btnChangeNeuroNet.Enabled = false;
            btnDeleteNeuroNet.Enabled = false;

            dgwNeuronsInLayers.Rows.Clear();
            dgwParamsAF.Rows.Clear();
        }

        private void dgwNeuroNets_MouseUp(object sender, MouseEventArgs e)
        {
            if (dgwNeuroNets.SelectedRows.Count == 1)
            {
                btnChangeNeuroNet.Enabled = true;
                btnDeleteNeuroNet.Enabled = true;

                dgwNeuronsInLayers.Rows.Clear();
                dgwParamsAF.Rows.Clear();

                List<string[]> neuronsInLayers = dbHandler.SelectNeuronsInLayers(dgwNeuroNets.SelectedRows[0].Cells[0].Value.ToString(),
                    Convert.ToInt32(dgwNeuroNets.SelectedRows[0].Cells[3].Value),
                    dgwNeuroNets.SelectedRows[0].Cells[5].Value.ToString());

                List<string[]> parametersAF = dbHandler.SelectParametersOfAF(dgwNeuroNets.SelectedRows[0].Cells[0].Value.ToString(),
                    Convert.ToInt32(dgwNeuroNets.SelectedRows[0].Cells[3].Value),
                    dgwNeuroNets.SelectedRows[0].Cells[5].Value.ToString());

                foreach (string[] item in neuronsInLayers)
                {
                    dgwNeuronsInLayers.Rows.Add(item);
                }

                foreach (string[] item in parametersAF)
                {
                    dgwParamsAF.Rows.Add(item);
                }
            }
            else
            {
                btnChangeNeuroNet.Enabled = false;
                btnDeleteNeuroNet.Enabled = false;

                dgwNeuronsInLayers.Rows.Clear();
                dgwParamsAF.Rows.Clear();
            }
        }

        private void btnDeleteNeuroNet_Click(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show("После удаления все данные об нейронной сети, включая обученные веса, будут потеряны. Хотите продолжить?", "Предупреждение потери данных", MessageBoxButtons.OKCancel);

            if (res == DialogResult.OK)
            {
                LoadingWindow loadingWindow = new LoadingWindow();
                loadingWindow.MakeLoading(
                    () => dbHandler.DeleteTopologyAndWeightsMatrix(dgwNeuroNets.SelectedRows[0].Cells[0].Value.ToString(), loadingWindow),
                "Удаление нейронной сети из БД");
                dbHandler.DeleteNeuroNet(dgwNeuroNets.SelectedRows[0].Cells[0].Value.ToString());

                LoadInformationForUsingNeuroNets();
                FillNeuroNetChangingTable();
                btnChangeNeuroNet.Enabled = false;
                btnDeleteNeuroNet.Enabled = false;

                dgwNeuronsInLayers.Rows.Clear();
                dgwParamsAF.Rows.Clear();
            }
        }

        private void tvTaskSelections_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (tvTaskSelections.SelectedNode.Level == 0)
            {
                if (String.Compare(lbTaskSelected.Text, tvTaskSelections.SelectedNode.Text) != 0)
                {
                    lbTaskSelected.Text = tvTaskSelections.SelectedNode.Text;
                    lbNetSelected.Text = "Не выбрана";
                    
                    isNeuroNetSelected = false;
                }
                lbSelSelected.Text = "Не выбрана";
                isSelectionSelected = false;

                dgwLA.Rows.Clear();

                FillNeuroNetUsingTable(tvTaskSelections.SelectedNode.Text);
                btnUse.Enabled = false;
                btnDelete.Enabled = false;
                btnLearn.Enabled = false;
                lbLASelected.Text = "Не выбран";
            }
            else
            {
                if (String.Compare(lbTaskSelected.Text, tvTaskSelections.SelectedNode.Parent.Text) != 0)
                {
                    lbTaskSelected.Text = tvTaskSelections.SelectedNode.Parent.Text;
                    lbNetSelected.Text = "Не выбрана";

                    isNeuroNetSelected = false;
                }
                lbSelSelected.Text = tvTaskSelections.SelectedNode.Text;
                isSelectionSelected = true;

                if (isNeuroNetSelected)
                {
                    FillLearningAlgorithmsTable(lbNetSelected.Text, lbSelSelected.Text);
                    btnUse.Enabled = false;
                    btnDelete.Enabled = false;
                    btnLearn.Enabled = false;
                    lbLASelected.Text = "Не выбран";
                }
                else
                {
                    dgwLA.Rows.Clear();
                    btnUse.Enabled = false;
                    btnDelete.Enabled = false;
                    btnLearn.Enabled = false;
                    FillNeuroNetUsingTable(tvTaskSelections.SelectedNode.Parent.Text);
                    lbLASelected.Text = "Не выбран";
                }
            }
        }

        private void dgwNets_MouseUp(object sender, MouseEventArgs e)
        {
            if (dgwNets.SelectedRows.Count == 1)
            {
                lbNetSelected.Text = dgwNets.SelectedRows[0].Cells[0].Value.ToString();
                isNeuroNetSelected = true;

                if (isSelectionSelected)
                {
                    FillLearningAlgorithmsTable(lbNetSelected.Text, lbSelSelected.Text);
                    btnUse.Enabled = false;
                    btnDelete.Enabled = false;
                    btnLearn.Enabled = false;
                    lbLASelected.Text = "Не выбран";
                }
            }
            else
            {
                lbNetSelected.Text = "Не выбрана";
                lbLASelected.Text = "Не выбран";
                isNeuroNetSelected = false;
                dgwLA.Rows.Clear();
                btnUse.Enabled = false;
                btnDelete.Enabled = false;
                btnLearn.Enabled = false;
            }
        }

        private void dgwLA_MouseUp(object sender, MouseEventArgs e)
        {
            if (dgwLA.SelectedRows.Count == 1)
            {
                if (dgwLA.SelectedRows[0].Cells[1].Value.ToString() == "Обучена")
                {
                    btnUse.Enabled = true;
                    btnDelete.Enabled = true;
                    btnLearn.Enabled = false;
                }
                else if (dgwLA.SelectedRows[0].Cells[1].Value.ToString() == "Не обучена")
                {
                    btnUse.Enabled = false;
                    btnDelete.Enabled = false;
                    btnLearn.Enabled = true;
                }

                lbLASelected.Text = dgwLA.SelectedRows[0].Cells[0].Value.ToString();
            }
            else
            {
                btnUse.Enabled = false;
                btnDelete.Enabled = false;
                btnLearn.Enabled = false;
                lbLASelected.Text = "Не выбран";
            }
        }

        private void btnUse_Click(object sender, EventArgs e)
        {
            int countInputNeurons = dbHandler.SelectCountInputParametersInTask(lbTaskSelected.Text);
            int countOutputNeurons = 1;
            ActivateFunction af = LibraryOfActivateFunctions.
                GetActivateFunction(dbHandler.SelectActivateFunctionTypeByNeuroNet(lbNetSelected.Text), 
                LibraryOfActivateFunctions.GetterParameter.TypeOfActivateFunctionName);
            List<double> valuesOfParametersAF = dbHandler.SelectValuesOfParametersOfAF(lbNetSelected.Text);
            int k = 0;
            foreach (double item in valuesOfParametersAF)
            {
                af.SetValueOfParameter(k, item);
                k++;
            }

            int countNeurons = dbHandler.SelectCountNeuronsInNet(lbNetSelected.Text);
            bool[,] connections = new bool[countNeurons, countNeurons];
            double[,] weights = new double[countNeurons, countNeurons];
            List<Tuple<int, int, double>> ls = dbHandler.SelectLearnedTopology(lbNetSelected.Text, 
                lbSelSelected.Text, LearningAlgorithmsLibrary.GetNameOfTypeOfAlgoritm(lbLASelected.Text));
            for (int i = 0; i < countNeurons; i++)
            {
                for (int j = 0; j < countNeurons; j++)
                {
                    connections[i, j] = false;
                    weights[i, j] = 0.0;
                }
            }
            foreach (Tuple<int, int, double> item in ls)
            {
                connections[item.Item2, item.Item1] = true;
                weights[item.Item2, item.Item1] = item.Item3;
            }
            int[] neuronsInLayers = dbHandler.SelectNeuronsInLayers(lbNetSelected.Text);
            NeuroNet net = new NeuroNet(countInputNeurons, countOutputNeurons, neuronsInLayers, connections, weights, af);

            string[,] selection = dbHandler.SelectLearningSelection(lbTaskSelected.Text, lbSelSelected.Text);
            List<string> types = dbHandler.GetParameterTypesOfSelection(lbTaskSelected.Text);
            List<string> names = dbHandler.GetParameterNamesOfSelection(lbTaskSelected.Text);

            List<IParameter> pars = new List<IParameter>();
            double[,] convertedSelection = new double[selection.GetLength(0), selection.GetLength(1)];
            for (int par = 0; par < types.Count; par++)
            {
                if (types[par].Equals("Int"))
                {
                    List<string> v = new List<string>();
                    for (int i = 0; i < selection.GetLength(0); i++)
                        v.Add(selection[i, par]);
                    IntegerParameter ip = new IntegerParameter(v);
                    pars.Add(ip);

                    for (int i = 0; i < selection.GetLength(0); i++)
                    {
                        if (cbInt.SelectedIndex == 0)
                            convertedSelection[i, par] = Convert.ToDouble(ip.GetInt(selection[i, par]));
                        else if (cbInt.SelectedIndex == 1)
                            convertedSelection[i, par] = Convert.ToDouble(ip.GetNormalizedInt(selection[i, par]));
                        else if (cbInt.SelectedIndex == 2)
                            convertedSelection[i, par] = Convert.ToDouble(ip.GetNormalizedDouble(selection[i, par]));
                    }
                }
                else if (types[par].Equals("Real"))
                {
                    List<string> v = new List<string>();
                    for (int i = 0; i < selection.GetLength(0); i++)
                        v.Add(selection[i, par]);
                    RealParameter ip = new RealParameter(v);

                    pars.Add(ip);

                    for (int i = 0; i < selection.GetLength(0); i++)
                    {
                        if (cbReal.SelectedIndex == 0)
                            convertedSelection[i, par] = Convert.ToDouble(ip.GetDouble(selection[i, par]));
                        else if (cbReal.SelectedIndex == 1)
                            convertedSelection[i, par] = Convert.ToDouble(ip.GetNormalizedInt(selection[i, par]));
                        else if (cbReal.SelectedIndex == 2)
                            convertedSelection[i, par] = Convert.ToDouble(ip.GetNormalizedDouble(selection[i, par]));
                    }
                }
                else if ((types[par].Equals("Enum")))
                {
                    List<string> v = new List<string>();
                    for (int i = 0; i < selection.GetLength(0); i++)
                        v.Add(selection[i, par]);
                    EnumeratedParameter ip = new EnumeratedParameter(v);
                    pars.Add(ip);

                    for (int i = 0; i < selection.GetLength(0); i++)
                    {
                        if (cbEnum.SelectedIndex == 0)
                            convertedSelection[i, par] = Convert.ToDouble(ip.GetInt(selection[i, par]));
                        else if (cbEnum.SelectedIndex == 1)
                            convertedSelection[i, par] = Convert.ToDouble(ip.GetNormalizedInt(selection[i, par]));
                        else if (cbEnum.SelectedIndex == 2)
                            convertedSelection[i, par] = Convert.ToDouble(ip.GetNormalizedDouble(selection[i, par]));
                    }
                }
            }

            List<IParameterValueConverter> converters = new List<IParameterValueConverter>();

            for (int i = 0; i < types.Count; i++)
            {
                if (types[i] == "Real")
                {
                    ParameterValueType type = ParameterValueType.String;
                    switch (cbReal.SelectedIndex)
                    {
                        case 0:
                            type = ParameterValueType.Real;
                            break;
                        case 1:
                            type = ParameterValueType.NormalisedInteger;
                            break;
                        case 2:
                            type = ParameterValueType.NormalisedReal;
                            break;
                    }
                    converters.Add(new RealParameterConverter(type, (RealParameter) pars[i]));
                }
                else if (types[i] == "Int")
                {
                    ParameterValueType type = ParameterValueType.String;
                    switch (cbInt.SelectedIndex)
                    {
                        case 0:
                            type = ParameterValueType.Integer;
                            break;
                        case 1:
                            type = ParameterValueType.NormalisedInteger;
                            break;
                        case 2:
                            type = ParameterValueType.NormalisedReal;
                            break;
                    }
                    converters.Add(new IntegerParameterConverter(type, (IntegerParameter) pars[i]));
                }
                else if (types[i] == "Enum")
                {
                    ParameterValueType type = ParameterValueType.String;
                    switch (cbEnum.SelectedIndex)
                    {
                        case 0:
                            type = ParameterValueType.Integer;
                            break;
                        case 1:
                            type = ParameterValueType.NormalisedInteger;
                            break;
                        case 2:
                            type = ParameterValueType.NormalisedReal;
                            break;
                    }
                    converters.Add(new EnumeratedParameterConverter(type, (EnumeratedParameter) pars[i]));
                }
            }

            NeuroNetSolvingWindow solvingWnd = new NeuroNetSolvingWindow(net, convertedSelection, converters, names);
            solvingWnd.Show();
        }

        private void btnLearn_Click(object sender, EventArgs e)
        {
            int countInputNeurons = dbHandler.SelectCountInputParametersInTask(lbTaskSelected.Text);
            int countOutputNeurons = 1;
            ActivateFunction af = LibraryOfActivateFunctions.
                GetActivateFunction(dbHandler.SelectActivateFunctionTypeByNeuroNet(lbNetSelected.Text),
                LibraryOfActivateFunctions.GetterParameter.TypeOfActivateFunctionName);
            List<double> valuesOfParametersAF = dbHandler.SelectValuesOfParametersOfAF(lbNetSelected.Text);
            int k = 0;
            foreach (double item in valuesOfParametersAF)
            {
                af.SetValueOfParameter(k, item);
                k++;
            }

            int countNeurons = dbHandler.SelectCountNeuronsInNet(lbNetSelected.Text);
            bool[,] connections = new bool[countNeurons, countNeurons];
            double[,] weights = new double[countNeurons, countNeurons];
            List<Tuple<int, int>> ls = dbHandler.SelectUnlearnedTopology(lbNetSelected.Text);
            for (int i = 0; i < countNeurons; i++)
            {
                for (int j = 0; j < countNeurons; j++)
                {
                    connections[i, j] = false;
                    weights[i, j] = 0.0;
                }
            }
            foreach (Tuple<int, int> item in ls)
            {
                connections[item.Item2, item.Item1] = true;
            }
            int[] neuronsInLayers = dbHandler.SelectNeuronsInLayers(lbNetSelected.Text);
            NeuroNet net = new NeuroNet(countInputNeurons, countOutputNeurons, neuronsInLayers, connections, weights, af);
            NeuroNetLearningInterface Inn = new NeuroNetLearningInterface(net, lbNetSelected.Text,
                lbSelSelected.Text, dbHandler);

            string[,] selection = dbHandler.SelectLearningSelection(lbTaskSelected.Text, lbSelSelected.Text);
            List<string> types = dbHandler.GetParameterTypesOfSelection(lbTaskSelected.Text);

            IParameter outParameter = null;
            double[,] convertedSelection = new double[selection.GetLength(0), selection.GetLength(1)];
            for (int par = 0; par < types.Count; par++)
            {
                if (types[par].Equals("Int"))
                {
                    List<string> v = new List<string>();
                    for (int i = 0; i < selection.GetLength(0); i++)
                        v.Add(selection[i, par]);
                    IntegerParameter ip = new IntegerParameter(v);
                    if (par == types.Count - 1)
                        outParameter = ip;
                   
                    for (int i = 0; i < selection.GetLength(0); i++)
                    {
                        if (cbInt.SelectedIndex == 0)
                            convertedSelection[i, par] = Convert.ToDouble(ip.GetInt(selection[i, par]));
                        else if (cbInt.SelectedIndex == 1)
                            convertedSelection[i, par] = Convert.ToDouble(ip.GetNormalizedInt(selection[i, par]));
                        else if (cbInt.SelectedIndex == 2)
                            convertedSelection[i, par] = Convert.ToDouble(ip.GetNormalizedDouble(selection[i, par]));
                    }
                }
                else if (types[par].Equals("Real"))
                {
                    List<string> v = new List<string>();
                    for (int i = 0; i < selection.GetLength(0); i++)
                        v.Add(selection[i, par]);
                    RealParameter ip = new RealParameter(v);
                    
                    if (par == types.Count - 1)
                        outParameter = ip;

                    for (int i = 0; i < selection.GetLength(0); i++)
                    {
                        if (cbReal.SelectedIndex == 0)
                            convertedSelection[i, par] = Convert.ToDouble(ip.GetDouble(selection[i, par]));
                        else if (cbReal.SelectedIndex == 1)
                            convertedSelection[i, par] = Convert.ToDouble(ip.GetNormalizedInt(selection[i, par]));
                        else if (cbReal.SelectedIndex == 2)
                            convertedSelection[i, par] = Convert.ToDouble(ip.GetNormalizedDouble(selection[i, par]));
                    }
                }
                else if ((types[par].Equals("Enum")))
                {
                    List<string> v = new List<string>();
                    for (int i = 0; i < selection.GetLength(0); i++)
                        v.Add(selection[i, par]);
                    EnumeratedParameter ip = new EnumeratedParameter(v);
                    if (par == types.Count - 1)
                        outParameter = ip;

                    for (int i = 0; i < selection.GetLength(0); i++)
                    {
                        if (cbEnum.SelectedIndex == 0)
                            convertedSelection[i, par] = Convert.ToDouble(ip.GetInt(selection[i, par]));
                        else if (cbEnum.SelectedIndex == 1)
                            convertedSelection[i, par] = Convert.ToDouble(ip.GetNormalizedInt(selection[i, par]));
                        else if (cbEnum.SelectedIndex == 2)
                            convertedSelection[i, par] = Convert.ToDouble(ip.GetNormalizedDouble(selection[i, par]));
                    }
                }
            }

            IParameterValueComparer comparer = null;
            IParameterValueConverter converter = null;

            if (types[types.Count - 1] == "Real")
            {
                ParameterValueType type = ParameterValueType.String;
                switch (cbReal.SelectedIndex)
                {
                    case 0:
                        type = ParameterValueType.Real;
                        break;
                    case 1:
                        type = ParameterValueType.NormalisedInteger;
                        break;
                    case 2:
                        type = ParameterValueType.NormalisedReal;
                        break;
                }
                comparer = new RealValueComparer((RealParameter)outParameter, type, type);
                converter = new RealParameterConverter(type, (RealParameter)outParameter);
            }
            else if (types[types.Count - 1] == "Int")
            {
                ParameterValueType type = ParameterValueType.String;
                switch (cbInt.SelectedIndex)
                {
                    case 0:
                        type = ParameterValueType.Integer;
                        break;
                    case 1:
                        type = ParameterValueType.NormalisedInteger;
                        break;
                    case 2:
                        type = ParameterValueType.NormalisedReal;
                        break;
                }
                comparer = new IntegerValueComparer((IntegerParameter)outParameter, type, type);
                converter = new IntegerParameterConverter(type, (IntegerParameter)outParameter);
            }
            else if (types[types.Count - 1] == "Enum")
            {
                ParameterValueType type = ParameterValueType.String;
                switch (cbEnum.SelectedIndex)
                {
                    case 0:
                        type = ParameterValueType.Integer;
                        break;
                    case 1:
                        type = ParameterValueType.NormalisedInteger;
                        break;
                    case 2:
                        type = ParameterValueType.NormalisedReal;
                        break;
                }
                comparer = new EnumeratedValueComparer((EnumeratedParameter)outParameter, type, type);
                converter = new EnumeratedParameterConverter(type, (EnumeratedParameter)outParameter);
            }

            if (lbLASelected.Text.Equals("Генетический алгоритм"))
            {
                GeneticAlgorithmForm fm = new GeneticAlgorithmForm(Inn, convertedSelection, comparer);
                fm.ShowDialog();
            }
            else if (lbLASelected.Text.Equals("Алгоритм обратного распространения ошибки"))
            {
                BackPropagationAlgorithmForm fm = new BackPropagationAlgorithmForm(Inn, convertedSelection, comparer, converter);
                fm.ShowDialog();
            }
            
            LoadInformationForUsingNeuroNets();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show("После удаления обученные веса, полученные данным алгоритмом обучения, будут потеряны. Хотите продолжить?", "Предупреждение потери данных", MessageBoxButtons.OKCancel);

            if (res == DialogResult.OK)
            {
                LoadingWindow loadingWindow = new LoadingWindow();
                loadingWindow.MakeLoading(
                    () => dbHandler.DeleteWeightsMatrix(lbNetSelected.Text, lbSelSelected.Text, lbLASelected.Text, loadingWindow),
                "Удаление обученных весов из БД");
            }
            LoadInformationForUsingNeuroNets();
        }
    }
}
