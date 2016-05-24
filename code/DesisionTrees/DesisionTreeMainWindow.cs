using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SII;
using NeuroWnd;
using System.Globalization;

namespace DesisionTrees
{

    public partial class DesisionTreeMainWindow : Form
    {
        CultureInfo UsCulture = new CultureInfo("en-US");
        public int usedTreeRootID = 0;
        private SQLManagerForTrees sqlManager = new SQLManagerForTrees();
        private DataBaseHandler dbHandler;
        private DataContainer<List<string[]>> desTreeInfo;
        private List<Parametr> arrParams;
        public String newTreeName;

        public int node_id_in_table;  //for save tree into DB

        private bool isDesTreeSelected;
        private bool isSelectionSelected;

        private void LoadInformationForUsingDesTrees()//переделать
        {
            desTreeInfo.Clear();
            tvTaskSelections.Nodes.Clear();

            dgwTrees.Rows.Clear();

            List<Tuple<int, string, int>> ls = dbHandler.SelectAllTasks();
            foreach (Tuple<int, string, int> item in ls)
            {
                List<string> bf = dbHandler.SelectAllNeuroNetsByTask(item.Item2);// переделать
                List<string> selections = dbHandler.SelectSelectionsNames(item.Item2);
                List<string[]> itemContainer = new List<string[]>();
                foreach (string name in bf)
                {
                    string[] str = new string[4];
                    string[] wideStr = dbHandler.SelectNeuroNetDefinitionByName(name);
                    str[0] = wideStr[0];
                    str[1] = wideStr[1];
                    str[2] = wideStr[3];
                    str[3] = wideStr[4];
                    itemContainer.Add(str);
                }
                desTreeInfo.AddData(item.Item2, itemContainer);
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
            lbTreeSelected.Text = "Не выбрано";

            isDesTreeSelected = false;
            isSelectionSelected = false;

            btnUse.Enabled = false;
        }

        //private void FillLearningAlgorithmsTable(string NeuroNetName, string SelectionName)//переделать под деревья
        //{
        //    dgwLA.Rows.Clear();
        //    string[] names = LearningAlgorithmsLibrary.GetAllNamesOfAlgorithms();
        //    for (int i = 0; i < LearningAlgorithmsLibrary.CountAlgorithms; i++)
        //    {
        //        dgwLA.Rows.Add(names[i]);
        //    }

        //    List<string> ls = learningInfo.FindData(NeuroNetName).FindData(SelectionName);

        //    for (int i = 0; i < dgwLA.Rows.Count; i++)
        //    {
        //        string laName = dgwLA.Rows[i].Cells[0].Value.ToString();
        //        string laType = LearningAlgorithmsLibrary.GetNameOfTypeOfAlgoritm(laName);

        //        bool isConsist = false;
        //        foreach (string item in ls)
        //        {
        //            if (String.Compare(item, laType) == 0)
        //            {
        //                isConsist = true;
        //                break;
        //            }
        //        }

        //        if (isConsist)
        //        {
        //            dgwLA.Rows[i].Cells[1].Value = "Обучена";
        //        }
        //        else
        //        {
        //            dgwLA.Rows[i].Cells[1].Value = "Не обучена";
        //        }
        //    }

        //    dgwLA.AutoResizeColumns();
        //}

        private void FillDesTreeUsingTable()
        {
            dgwTrees.ColumnHeadersDefaultCellStyle.Font = new Font("Book Antiqua", 9);
            dgwTrees.Rows.Clear();
            List<Tree> tree_list = new List<Tree>();
            if(tvTaskSelections.SelectedNode.Parent != null) 
            {
                tree_list = sqlManager.GetTreeWithRequest("SELECT * FROM TREE WHERE TASK_ID = (SELECT ID FROM TASK WHERE NAME =  '" + tvTaskSelections.SelectedNode.Parent.Text + "')");
            }
            dgwTrees.Columns.Clear();


            dgwTrees.Columns.Add("TREE_NAME", "Имя дерева");
            dgwTrees.Columns.Add("TASK_NAME", "Задача");
            dgwTrees.Columns.Add("SELECTION_NAME", "Выборка");
            for (int i = 0; i < dgwTrees.Columns.Count; i++)
            {
                DataGridViewColumn column = dgwTrees.Columns[i];
                column.Width = dgwTrees.Size.Width / 3 - 17;
            }

            foreach (Tree tree in tree_list)
            {
                string[] Row = new string[3];
                Row[0] = tree.TREE_NAME;
                Row[1] = sqlManager.GetTasksWithRequest("SELECT * FROM TASK WHERE ID = " + tree.TASK_ID)[0].Name;
                Row[2] = sqlManager.GetOneSelectionWithRequest("SELECT * FROM SELECTION WHERE ID =" + tree.SELECTION_ID).Name;
                dgwTrees.Rows.Add(Row);
            }

        }

        public DesisionTreeMainWindow()
        {
            InitializeComponent();
            dbHandler = new DataBaseHandler();
            desTreeInfo = new DataContainer<List<string[]>>();
            // learningInfo = new DataContainer<DataContainer<List<string>>>();
            LoadInformationForUsingDesTrees();
        }

        private void tvTaskSelections_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (tvTaskSelections.SelectedNode.Level == 0)
            {
                if (String.Compare(lbTaskSelected.Text, tvTaskSelections.SelectedNode.Text) != 0)
                {
                    lbTaskSelected.Text = tvTaskSelections.SelectedNode.Text;
                    lbTreeSelected.Text = "Не выбрано";

                    isDesTreeSelected = false;
                }
                lbSelSelected.Text = "Не выбрана";
                isSelectionSelected = false;


                FillDesTreeUsingTable();
                btnUse.Enabled = false;
            }
            else
            {
                if (String.Compare(lbTaskSelected.Text, tvTaskSelections.SelectedNode.Parent.Text) != 0)
                {
                    lbTaskSelected.Text = tvTaskSelections.SelectedNode.Parent.Text;
                    lbTreeSelected.Text = "Не выбрано";

                    isDesTreeSelected = false;
                }
                lbSelSelected.Text = tvTaskSelections.SelectedNode.Text;
                isSelectionSelected = true;

                if (isDesTreeSelected)
                {
                    //FillLearningAlgorithmsTable(lbTreeSelected.Text, lbSelSelected.Text);
                    btnUse.Enabled = false;
                }
                else
                {
                    btnUse.Enabled = false;
                    FillDesTreeUsingTable();
                }
            }
        }

        public VeryfiedClassInfo[] ClassInfoInit()
        {
            Parametr param = sqlManager.GetOneParametrWithRequest("SELECT * FROM PARAM WHERE( NUMBER = '0' AND TASK_ID = (SELECT TASK_ID FROM SELECTION WHERE NAME =  '" + tvTaskSelections.SelectedNode.Text + "'))");
            int number_of_out_par = 1;
            for (int i = 0; i < param.Range.Length; i++)
            {
                if (param.Range[i] == '|')
                    number_of_out_par++;
            }

            VeryfiedClassInfo[] newClassInfo = new VeryfiedClassInfo[number_of_out_par];
            for (int i = 0; i < number_of_out_par; i++)
            {
                newClassInfo[i] = new VeryfiedClassInfo();
            }
            for (int i = 0, j = 0; i < param.Range.Length; i++)
            {
                if (param.Range[i] == '|')
                {
                    j++;
                    continue;
                }
                if (param.Range[i] != ' ')
                    newClassInfo[j].class_name += param.Range[i];
            }
            return newClassInfo;
        }

        public VeryfiedClassInfo[] ClassInfoInit2(EducationTable educationTable)
        {
            Parametr sortParameter = sqlManager.GetOneParametrWithRequest("SELECT * FROM PARAM WHERE( NUMBER = '0' AND TASK_ID = (SELECT TASK_ID FROM SELECTION WHERE NAME =  '" + tvTaskSelections.SelectedNode.Text + "'))");
            List<String> paramValues = new List<String>();

            //educationTable.BubbleSortByParam(0, sortParameter);
            educationTable.QuickSortByParam(0, educationTable.Rows.Count - 1, 0, sortParameter);
            String param = educationTable.Rows[0][0].Value;
            paramValues.Add(param);

            int number_of_out_par = 1;
            for (int i = 0; i < educationTable.Rows.Count; i++)
            {
                if (param != educationTable.Rows[i][0].Value)
                {
                    number_of_out_par++;
                    param = educationTable.Rows[i][0].Value;
                    paramValues.Add(param);
                }
            }

            VeryfiedClassInfo[] newClassInfo = new VeryfiedClassInfo[number_of_out_par];
            for (int i = 0; i < number_of_out_par; i++)
            {
                newClassInfo[i] = new VeryfiedClassInfo();
            }
            for (int i = 0; i < number_of_out_par; i++)
            {
                newClassInfo[i].class_name = paramValues[i];
            }
            return newClassInfo;
        }

        public double GiniSplitCalc(VeryfiedClassInfo[] leftClassInf, VeryfiedClassInfo[] rightClassInf)
        {
            double res = 0;
            int examplCntLeft = 0, examplCntRight = 0;
            for (int i = 0; i < leftClassInf.Length; i++)
            {
                examplCntLeft += leftClassInf[i].number_of_checked;
                examplCntRight += rightClassInf[i].number_of_checked;
            }

            for (int i = 0; i < leftClassInf.Length; i++)
            {
                res += ((Math.Pow(leftClassInf[i].number_of_checked, 2) / examplCntLeft) + (Math.Pow(rightClassInf[i].number_of_checked, 2) / examplCntRight));
            }
            return res;
        }

        public void FindBetterParametr(EducationTable education_table, ref int index_of_parametr, ref string best_value_for_split, ref Parametr _param)
        {
            index_of_parametr = 0;
            best_value_for_split = "";
            VeryfiedClassInfo[] leftClassInf = ClassInfoInit();//ClassInfoInit2(education_table);
            VeryfiedClassInfo[] rightClassInf = ClassInfoInit();//ClassInfoInit2(education_table);
            Parametr param;
            double giniValue = -100000;
            for (int index = 1; index < education_table.ParameterCount; index++)
            {
                param = sqlManager.GetOneParametrWithRequest("SELECT * FROM PARAM WHERE ID ='" + education_table.Rows.ElementAt(1)[index].ParametrID + "'");
                //education_table.BubbleSortByParam(index, param);
                education_table.QuickSortByParam(0, education_table.Rows.Count - 1, index, param);
                if ((param.Type == TypeParametr.Real) || (param.Type == TypeParametr.Int))
                {
                    double average = 0;
                    for (int prevRowInd = 0, nextRowInd = 1; nextRowInd < education_table.Rows.Count; prevRowInd++, nextRowInd++)
                    {
                        average = (Convert.ToDouble(education_table.Rows.ElementAt(prevRowInd)[index].Value, UsCulture) + Convert.ToDouble(education_table.Rows.ElementAt(nextRowInd)[index].Value, UsCulture)) / 2.0;
                        for (int i = 0; i < education_table.Rows.Count; i++)
                        {
                            if (Convert.ToDouble(education_table.Rows.ElementAt(i)[index].Value, UsCulture) <= average)
                            {
                                foreach (VeryfiedClassInfo clinf in rightClassInf)
                                {
                                    if (clinf.class_name == education_table.Rows.ElementAt(i)[0].Value)
                                    {
                                        clinf.number_of_checked++;
                                    }
                                }
                            }
                            else
                            {
                                foreach (VeryfiedClassInfo clinf in leftClassInf)
                                {
                                    if (clinf.class_name == education_table.Rows.ElementAt(i)[0].Value)
                                    {
                                        clinf.number_of_checked++;
                                    }
                                }
                            }
                        }
                        double newGiniValue = GiniSplitCalc(leftClassInf, rightClassInf);
                        if (newGiniValue > giniValue)
                        {
                            giniValue = newGiniValue;
                            index_of_parametr = index;
                            best_value_for_split = average.ToString(UsCulture);
                            _param = param;
                        }
                        for (int i = 0; i < leftClassInf.Length; i++)
                        {
                            leftClassInf[i].number_of_checked = 0;
                            rightClassInf[i].number_of_checked = 0;
                        }
                    }

                }
                else
                {
                    //param.Range
                    int number_of_var = 0;
                    for (int i = 0; i < param.Range.Length; i++)
                    {
                        if (param.Range[i] == '|')
                            number_of_var++;
                    }

                    String[] variables = new String[number_of_var + 1];
                    for (int i = 0, j = 0; i < param.Range.Length; i++)
                    {
                        if (param.Range[i] == '|')
                        {
                            j++;
                            continue;
                        }
                        if (param.Range[i] != ' ')
                            variables[j] += param.Range[i];
                    }

                    for (int j = 0; j < number_of_var; j++)
                    {
                        for (int i = 0; i < education_table.Rows.Count; i++)
                        {
                            if (education_table.Rows.ElementAt(i)[index].Value == variables[j])
                            {
                                foreach (VeryfiedClassInfo clinf in rightClassInf)
                                {
                                    if (clinf.class_name == education_table.Rows.ElementAt(i)[0].Value)
                                    {
                                        clinf.number_of_checked++;
                                    }
                                }
                            }
                            else
                            {
                                foreach (VeryfiedClassInfo clinf in leftClassInf)
                                {
                                    if (clinf.class_name == education_table.Rows.ElementAt(i)[0].Value)
                                    {
                                        clinf.number_of_checked++;
                                    }
                                }
                            }
                        }
                        double newGiniValue = GiniSplitCalc(leftClassInf, rightClassInf);
                        if (newGiniValue > giniValue)
                        {
                            giniValue = newGiniValue;
                            index_of_parametr = index;
                            best_value_for_split = variables[j];
                            _param = param;
                        }
                        for (int i = 0; i < leftClassInf.Length; i++)
                        {
                            leftClassInf[i].number_of_checked = 0;
                            rightClassInf[i].number_of_checked = 0;
                        }
                    }
                }
            }
        }

        public void SplitEducationTable(EducationTable education_table, Rule split_rule, Parametr param, ref EducationTable left_table, ref EducationTable right_table)
        {
            if ((param.Type == TypeParametr.Real) || (param.Type == TypeParametr.Int))
            {
                for (int i = 0; i < education_table.Rows.Count; i++)
                {
                    double curVal = Convert.ToDouble(education_table.Rows.ElementAt(i)[split_rule.index_of_param].Value, UsCulture);
                    double splitVal = Convert.ToDouble(split_rule.value, UsCulture);
                    if (curVal <= splitVal)
                    {
                        right_table.Rows.Add(education_table.Rows.ElementAt(i));
                    }
                    else
                    {
                        left_table.Rows.Add(education_table.Rows.ElementAt(i));
                    }
                }
                right_table.ParameterCount = education_table.ParameterCount;
                left_table.ParameterCount = education_table.ParameterCount;
            }
            else
            {
                for (int i = 0; i < education_table.Rows.Count; i++)
                {
                    String curVal = education_table.Rows.ElementAt(i)[split_rule.index_of_param].Value;
                    String splitVal = split_rule.value;
                    if (curVal == splitVal)
                    {
                        right_table.Rows.Add(education_table.Rows.ElementAt(i));
                    }
                    else
                    {
                        left_table.Rows.Add(education_table.Rows.ElementAt(i));
                    }
                }
                right_table.ParameterCount = education_table.ParameterCount;
                left_table.ParameterCount = education_table.ParameterCount;
            }
        }

        public void treeBuilding(EducationTable education_table, TreeNode tree_node)
        {
            VeryfiedClassInfo[] thisClassInfo = ClassInfoInit();//ClassInfoInit2(education_table);
            for (int i = 0; i < education_table.Rows.Count; i++)
            {
                foreach (VeryfiedClassInfo clinf in thisClassInfo)
                {
                    if (clinf.class_name == education_table.Rows.ElementAt(i)[0].Value)
                    {
                        clinf.number_of_checked++;
                    }
                }
            }
            int k = 0;
            foreach (VeryfiedClassInfo clinf in thisClassInfo)
            {
                if (clinf.number_of_checked >= 1)
                {
                    k++;
                }
            }

            if (k >= 2)
            {
                tree_node.is_leaf = false;
                int index_of_parametr = 0;
                string best_value_for_split = "";
                Parametr param = new Parametr();
                FindBetterParametr(education_table, ref index_of_parametr, ref best_value_for_split, ref param);
                tree_node.rule = new Rule();
                tree_node.rule.index_of_param = index_of_parametr;
                tree_node.rule.value = best_value_for_split;
                tree_node.left_child = new TreeNode();
                tree_node.right_child = new TreeNode();
                EducationTable left_table = new EducationTable();
                EducationTable right_table = new EducationTable();
                SplitEducationTable(education_table, tree_node.rule, param, ref left_table, ref right_table);
                treeBuilding(left_table, tree_node.left_child);
                treeBuilding(right_table, tree_node.right_child);


            }
            else
            {
                tree_node.is_leaf = true;
                tree_node.rule = new Rule();
                foreach (VeryfiedClassInfo clinf in thisClassInfo)
                {
                    if (clinf.number_of_checked > 0)
                    {
                        tree_node.rule.value = clinf.class_name;
                    }
                }

            }




        }

        private void SaveRuleToDB(Rule rule)
        {
            String sqlReqStr = "INSERT INTO RULE (PARAM_INDEX, VALUE) " +
                "VALUES('" + rule.index_of_param + "','" + rule.value + "');";
            int state = sqlManager.SendInsertRequest(sqlReqStr);
            if (state == 0)
                Console.WriteLine("error");
        }

        private void SaveNodeToDB(TreeNode node, int node_id)
        {
            if (node.is_leaf == false)
            {
                SaveRuleToDB(node.rule);
                int left_child_id = node_id_in_table + 1;
                int right_child_id = node_id_in_table + 2;
                int rule_id = sqlManager.GetMaxFeature("SELECT MAX(ID) FROM RULE", "MAX(ID)");
                String sqlReqStr = "INSERT INTO NODE (ID, LEFT_CHILD_ID, RIGHT_CHILD_ID, RULE_ID, IS_LEAF) " +
                "VALUES('" + node_id + "','" + left_child_id + "','" + right_child_id + "','" + rule_id + "','0');";
                int state = sqlManager.SendInsertRequest(sqlReqStr);
                if (state == 0)
                    Console.WriteLine("error");
                node_id_in_table = node_id_in_table + 2;
                SaveNodeToDB(node.left_child, left_child_id);
                SaveNodeToDB(node.right_child, right_child_id);
            }
            else
            {
                SaveRuleToDB(node.rule);
                int rule_id = sqlManager.GetMaxFeature("SELECT MAX(ID) FROM RULE", "MAX(ID)");
                String sqlReqStr = "INSERT INTO NODE (ID, LEFT_CHILD_ID, RIGHT_CHILD_ID, RULE_ID, IS_LEAF) " +
                "VALUES('" + node_id + "','0','0','" + rule_id + "','1');";
                int state = sqlManager.SendInsertRequest(sqlReqStr);
                if (state == 0)
                    Console.WriteLine("error");
            }
        }


        private void SaveTreeToDB(TreeNode root, String tree_name, int root_id, int cur_task_id, int cur_selection_id)
        {
            node_id_in_table = root_id;
            SaveNodeToDB(root, root_id);


            String sqlReqStr = "INSERT INTO TREE (TREE_NAME, TASK_ID, SELECTION_ID, ROOT_ID) " +
                "VALUES('" + tree_name + "','" + cur_task_id + "','" + cur_selection_id + "','" + root_id + "');";
            int state = sqlManager.SendInsertRequest(sqlReqStr);
            if (state == 0)
                Console.WriteLine("error");
        }




        private void btnBuildTree_Click(object sender, EventArgs e)
        {

            TreeNameInputForm treeNameForm = new TreeNameInputForm();
            treeNameForm.Owner = this;
            treeNameForm.ShowDialog();
            //treeNameForm.

            Selection selection = sqlManager.GetOneSelectionWithRequest("SELECT * FROM SELECTION WHERE NAME ='" + tvTaskSelections.SelectedNode.Text + "'");
            arrParams = sqlManager.GetParamsWithRequest("SELECT * FROM PARAM WHERE TASK_ID = (SELECT TASK_ID FROM SELECTION WHERE NAME =  '" + tvTaskSelections.SelectedNode.Text + "')");
            List<ValueParametr> arrValues = sqlManager.GetValuesWithRequest("select * from VALUE_PARAM where SELECTION_ID=(select ID from SELECTION where NAME ='" + tvTaskSelections.SelectedNode.Text + "')");
            EducationTable educatTable = new EducationTable(arrValues, arrParams);
            TreeNode root = new TreeNode();
            int root_id = 1 + sqlManager.GetMaxFeature("SELECT COUNT(1) FROM NODE", "COUNT(1)");

            for (int i = 0; i < educatTable.Rows.Count; i++)
            {
                var tmp = educatTable.Rows[i];
                for (int j = i + 1; j < educatTable.Rows.Count; j++)
                {
                    bool flag = true;
                    for (int j1 = 1; j1 < educatTable.Rows[j].Length; j1++)
                    {
                        if (tmp[j1].Value != educatTable.Rows[j][j1].Value)
                        {
                            flag = false;
                        }
                    }
                    if (flag == true)
                    {
                        educatTable.Rows.RemoveAt(j);
                    }
                }
            }
            for (int i = 0; i < educatTable.Rows.Count; i++)
            {
                var tmp = educatTable.Rows[i];
                for (int j = i + 1; j < educatTable.Rows.Count; j++)
                {
                    bool flag = true;
                    for (int j1 = 1; j1 < educatTable.Rows[j].Length; j1++)
                    {
                        if (tmp[j1].Value != educatTable.Rows[j][j1].Value)
                        {
                            flag = false;
                        }
                    }
                    if (flag == true)
                    {
                        educatTable.Rows.RemoveAt(j);
                    }
                }
            }
            
            treeBuilding(educatTable, root);

            SaveTreeToDB(root, newTreeName, root_id, selection.TaskID, selection.ID);

            FillDesTreeUsingTable();
        }

        private void dgwTrees_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if ((e.RowIndex != -1) && (e.ColumnIndex == -1))
            {
                lbTreeSelected.Text = dgwTrees.Rows[e.RowIndex].Cells["TREE_NAME"].Value.ToString();
                btnUse.Enabled = true;
                usedTreeRootID = sqlManager.GetTreeWithRequest("SELECT * FROM TREE WHERE TREE_NAME = '" + lbTreeSelected.Text + "'")[0].ROOT_ID;
                //usedTreeRootID = Convert.ToInt32(dgwTrees.Rows[e.RowIndex].Cells["ROOT_ID"].Value);
            }
        }

        private void btnUse_Click(object sender, EventArgs e)
        {
            arrParams = sqlManager.GetParamsWithRequest("SELECT * FROM PARAM WHERE TASK_ID = (SELECT TASK_ID FROM SELECTION WHERE NAME =  '" + tvTaskSelections.SelectedNode.Text + "')");
            TreeUsingForm treeUsingFrm = new TreeUsingForm(arrParams, usedTreeRootID);
            treeUsingFrm.Show();
        }

        private void btnTest_Click(object sender, EventArgs e)
        {

            Selection selection = sqlManager.GetOneSelectionWithRequest("SELECT * FROM SELECTION WHERE NAME ='" + tvTaskSelections.SelectedNode.Text + "'");
            arrParams = sqlManager.GetParamsWithRequest("SELECT * FROM PARAM WHERE TASK_ID = (SELECT TASK_ID FROM SELECTION WHERE NAME =  '" + tvTaskSelections.SelectedNode.Text + "')");
            List<ValueParametr> arrValues = sqlManager.GetValuesWithRequest("select * from VALUE_PARAM where SELECTION_ID=(select ID from SELECTION where NAME ='" + tvTaskSelections.SelectedNode.Text + "')");
            List<ValueParametr> learningSelection = new List<ValueParametr>();
            List<ValueParametr> testSelection = new List<ValueParametr>();

            //int k = 0, m = 0;
            //for (int i = 0; i < arrValues.Count; i++)
            //{
            //    if (k == 5)
            //    {
            //        testSelection.Add(arrValues[i]);
            //    }
            //    else
            //    {
            //        learningSelection.Add(arrValues[i]);
            //    }
            //    m++;
            //    if (m == arrParams.Count)
            //    {
            //        if (k == 5)
            //        {
            //            k = 0;
            //        }
            //        k++;
            //        m = 0;                    
            //    }
            //}

            EducationTable educatTable = new EducationTable(arrValues, arrParams);
            EducationTable testTable = new EducationTable();
            TreeNode root = new TreeNode();
            int root_id = 1 + sqlManager.GetMaxFeature("SELECT COUNT(1) FROM NODE", "COUNT(1)");

            for (int i = 0; i < educatTable.Rows.Count; i++)
            {
                var tmp = educatTable.Rows[i];
                for (int j = i + 1; j < educatTable.Rows.Count; j++)
                {
                    bool flag = true;
                    for (int j1 = 1; j1 < educatTable.Rows[j].Length; j1++)
                    {
                        if (tmp[j1].Value != educatTable.Rows[j][j1].Value)
                        {
                            flag = false;
                        }
                    }
                    if (flag == true)
                    {
                        educatTable.Rows.RemoveAt(j);
                    }
                }
            }
            for (int i = 0; i < educatTable.Rows.Count; i++)
            {
                var tmp = educatTable.Rows[i];
                for (int j = i + 1; j < educatTable.Rows.Count; j++)
                {
                    bool flag = true;
                    for (int j1 = 1; j1 < educatTable.Rows[j].Length; j1++)
                    {
                        if (tmp[j1].Value != educatTable.Rows[j][j1].Value)
                        {
                            flag = false;
                        }
                    }
                    if (flag == true)
                    {
                        educatTable.Rows.RemoveAt(j);
                    }
                }
            }
            int k = 0;
            for (int i = 0; i < educatTable.Rows.Count; i++)
            {
                if (k == 4)
                {
                    k = 0;
                    testTable.Rows.Add(educatTable.Rows[i]);
                    educatTable.Rows.RemoveAt(i);
                }
                k++;
            }


            //VeryfiedClassInfo[] ClassInf = ClassInfoInit2(educatTable);
            //String str = "";
            //for (int i = 0; i < ClassInf.Length; i++)
            //{
            //    str += ClassInf[i].class_name.ToString() + '|';
            //}

            treeBuilding(educatTable, root);

            double learningMistake = 0;
            int testMistake = 0;
            String answer = "";
            String[] input_row = new String[arrParams.Count - 1];
            for (int i = 0; i < educatTable.Rows.Count; i++)
            {
                for (int j = 0; j < arrParams.Count - 1; j++)
                {
                    input_row[j] = educatTable.Rows[i][j + 1].Value.ToString(); //dgwInputVal.Rows[0].Cells[i].Value.ToString();
                }
                answer = TreeUsing(input_row, root, arrParams);
                if (answer == educatTable.Rows[i][0].Value.ToString())
                {
                    learningMistake = learningMistake + 1;
                }
            }
            learningMistake = 100 - 100 * learningMistake / educatTable.Rows.Count;

            for (int i = 0; i < testTable.Rows.Count; i++)
            {
                for (int j = 0; j < arrParams.Count - 1; j++)
                {
                    input_row[j] = testTable.Rows[i][j + 1].Value.ToString(); //dgwInputVal.Rows[0].Cells[i].Value.ToString();
                }
                answer = TreeUsing(input_row, root, arrParams);
                if (answer == testTable.Rows[i][0].Value.ToString())
                {
                    testMistake = testMistake + 1;
                }
            }
            testMistake = 100 - 100 * testMistake / testTable.Rows.Count;
            lblTestResult.Text = "test = " + testMistake + "  learn = " + learningMistake;

        }

        public String TreeUsing(String[] param_row, TreeNode curNode, List<Parametr> arr_Params)
        {
            while (curNode.is_leaf != true)
            {

                if ((arr_Params[curNode.rule.index_of_param - 1].Type == TypeParametr.Real) || (arr_Params[curNode.rule.index_of_param - 1].Type == TypeParametr.Int))
                {
                    if (Convert.ToDouble(param_row[curNode.rule.index_of_param - 1], UsCulture) <= Convert.ToDouble(curNode.rule.value, UsCulture))
                    {
                        curNode = curNode.right_child;
                    }
                    else
                    {
                        curNode = curNode.left_child;
                    }
                }
                else
                {
                    if (param_row[curNode.rule.index_of_param - 1] == curNode.rule.value)
                    {
                        curNode = curNode.right_child;
                    }
                    else
                    {
                        curNode = curNode.left_child;
                    }
                }
            }
            return curNode.rule.value;
        }

    }
}
