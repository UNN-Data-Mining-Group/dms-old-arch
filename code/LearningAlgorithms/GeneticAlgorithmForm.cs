using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using LearningAlgorithms.Parameter;

namespace LearningAlgorithms
{
    public class Table
    {
        int seed = 13052016;
        int training_size, test_size;
        double train_persent = 8.0 / 10.0;
        double[,] training_set, test_set;
        public void create_table(double[,] training_set_)
        {
            Random rand = new Random(seed);
            int tmp;           
            
            bool[] is_added = new bool[training_set_.GetLength(0)];
            for (int i = 0; i < training_set_.GetLength(0); i++)
            {
                is_added[i] = false;
            }
            training_size = Convert.ToInt32(Convert.ToDouble(training_set_.GetLength(0)) * train_persent);
            test_size = training_set_.GetLength(0) - training_size;
            training_set = new double[training_size, training_set_.GetLength(1)];
            test_set = new double[test_size, training_set_.GetLength(1)];
            for (int i = 0; i < training_size; )
            {
                tmp = rand.Next(0, training_set_.GetLength(0));
                if (!is_added[tmp])
                {
                    is_added[tmp] = true;

                    for (int j = 0; j < training_set.GetLength(1); j++)
                        training_set[i, j] = training_set_[tmp, j];
                    i++;
                }
            }
            tmp = 0;
            for (int i = 0; i < training_set_.GetLength(0); i++)
            {
                if (!is_added[i])
                {
                    for (int j = 0; j < test_set.GetLength(1); j++)
                        test_set[tmp, j] = training_set_[i, j];
                    tmp++;
                }
            }
            save_result();
        }
        public void save_result()
        {
            string name ="test\\" + (test_set.GetLength(0) + training_set.GetLength(0)).ToString() + ".txt";
            using (System.IO.StreamWriter stream = new System.IO.StreamWriter(name))
            {
                stream.Write("{0} ", training_set.GetLength(0));
                stream.Write("{0} ", test_set.GetLength(0));
                stream.Write("{0} ", test_set.GetLength(1));
                stream.WriteLine();
                for (int i = 0; i < training_set.GetLength(0); i++)
                {
                    for (int j = 0; j < training_set.GetLength(1); j++)
                    {
                        stream.Write("{0} ", training_set[i, j]);
                    }
                    stream.WriteLine();
                }
                for (int i = 0; i < test_set.GetLength(0); i++)
                {
                    for (int j = 0; j < test_set.GetLength(1); j++)
                    {
                        stream.Write("{0} ", test_set[i, j]);
                    }
                    stream.WriteLine();
                }
            }

        }
    }
    public partial class GeneticAlgorithmForm : Form
    {
        INeuroNetLearning solver;
        GeneticAlgorithm gen;
        double[,] training_set;
        double[,] test_set;
        double[,] all_set;
        double train_persent;
        int training_size, test_size;
        double EPS;
        IParameterValueComparer comparer;
        public GeneticAlgorithmForm(INeuroNetLearning solver_, double[,] training_set_, IParameterValueComparer comparer_)
        {
            InitializeComponent();
            comparer = comparer_;
            gen = new GeneticAlgorithm();
            solver = solver_;
            all_set = training_set_;
           // new Table().create_table(training_set_);
        }
        
        private void BT_learn_Click(object sender, EventArgs e)
        {
            string[] err = { "Неверное значение eps", "Неверное значение количества особей",
                               "Неверное значение коэффициента мутации",
                               "Неверное значение процента обучающей выборки",
                               "Неверное значение процента скрещивания",
                               "Неверное значение максимального числа шагов",
                               "Неверное значение процента тестовой выборки"};
            int i = 0;
            try
            {
                gen.set_eps(Convert.ToDouble(TB_eps.Text.ToString()));
                i++;
                gen.set_count_popul(Convert.ToInt32(TB_count_popul.Text.ToString()));
                i++;
                gen.set_coef_mut(Convert.ToDouble(TB_coef_mut.Text.ToString()));
                i++;
                gen.set_persent_train(Convert.ToDouble(TB_train_percent.Text.ToString()));
                i++;
                gen.set_selection_persent(Convert.ToDouble(TB_selection_percent.Text.ToString()));
                i++;
                gen.set_max_step(Convert.ToInt32(TB_max_step.Text.ToString()));
                i++;
                train_persent = Convert.ToDouble(TB_persent_train.Text.ToString());
                i++;
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(err[i]);
            }
            if (i == 7)
            {
                parser(all_set);
                BT_learn.Enabled = false;
                gen.genom(solver, training_set, CB_lin_repr.Checked);
                BT_learn.Enabled = true;
                BT_write.Enabled = true;
                TB_res_count_step.Text = gen.get_step().ToString();
                TB_res_eps.Text = gen.get_min_err().ToString();
                gen.save_result();
                        
                LB_err.Text = "Ошибка на обучающей выборке = " + get_error(training_set).ToString();
                LB_max_err.Text = "Ошибка на тестовой выборке = " + get_error(test_set).ToString();  
               
            }
        }

        private double get_error(double[,] set)
        {
            double[][] training_X = new double[set.GetLength(0)][];
            double[] training_Y = new double[set.GetLength(0)];

            for (int j = 0; j < set.GetLength(0); j++)
            {
                training_X[j] = new double[set.GetLength(1) - 1];
                training_Y[j] = set[j, training_set.GetLength(1) - 1];
                for (int k = 0; k < set.GetLength(1) - 1; k++)
                {
                    training_X[j][k] = set[j, k];
                }
            }       
            double error = 0;
            for (int j = 0; j < training_Y.Length; j++)
            {
                double res = solver.get_res(training_X[j]);
                //if( Math.Abs(training_Y[j] - res) > EPS)
                if (!comparer.isEqual(training_Y[j], res))
                {
                    error += 1;
                }                
            }
            error /= training_Y.Length;
            return error;
        }

        private void parser(double[,] training_set_)
        {
            Random rand = new Random(13052016);
            int tmp;
            double max,min;
            max = min = training_set_[0,training_set_.GetLength(1) - 1];
            for(int i = 0; i < training_set_.GetLength(0); i++)
            {
                if (max < training_set_[i, training_set_.GetLength(1) - 1])
                    max = training_set_[i, training_set_.GetLength(1) - 1];
                if (min > training_set_[i, training_set_.GetLength(1) - 1])
                    min = training_set_[i, training_set_.GetLength(1) - 1];
            }
            int p = Convert.ToInt32(Math.Log10(2 * (max - min))) + 1;
            EPS = Math.Pow(10, -p) / 2;
            bool[] is_added = new bool[training_set_.GetLength(0)];
            for (int i = 0; i < training_set_.GetLength(0); i++)
            {
                is_added[i] = false;
            }
            training_size = Convert.ToInt32(Convert.ToDouble(training_set_.GetLength(0)) * train_persent);
            test_size = training_set_.GetLength(0) - training_size;
            training_set = new double[training_size, training_set_.GetLength(1)]; 
            test_set = new double[test_size,training_set_.GetLength(1)];
            for(int i = 0; i < training_size; )
            {
                tmp = rand.Next(0, training_set_.GetLength(0));
                if(!is_added[tmp])
                {
                    is_added[tmp] = true;

                    for (int j = 0; j < training_set.GetLength(1); j++)
                        training_set[i, j] = training_set_[tmp, j];
                    i++;
                }
            }
            tmp = 0;
            for (int i = 0; i < training_set_.GetLength(0); i++)
            {
                if (!is_added[i])
                {
                    for (int j = 0; j < test_set.GetLength(1); j++)
                        test_set[tmp, j] = training_set_[i, j];
                    tmp++;
                }
            }
        }

        private void BT_write_Click(object sender, EventArgs e)
        {
            solver.write_result(gen.Name);
            MessageBox.Show("Обученные веса записаны в базу данных");
            this.Close();
        }
    }
}
