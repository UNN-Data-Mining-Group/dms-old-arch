using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
  
namespace LearningAlgorithms
{

    public class ReverseComparer : System.Collections.IComparer
    {
        // Call CaseInsensitiveComparer.Compare with the parameters reversed.
        public int Compare(Object x, Object y)
        {
            return (new System.Collections.CaseInsensitiveComparer()).Compare(y, x);
        }
    }

    class neronnet1
    {
        bool[,] topologi;
        double[,] weight;
        //double[] output;
        public neronnet1(bool[,] topologi_)
        {
            topologi = topologi_;
           /* output = new double[topologi.GetLength(0)];
            for (int i = 0; i < topologi.GetLength(0); i++)
                output[i] = -10000;*/
        }
        public void set_links(double[,] weight_)
        {
            weight = weight_;
        }
        public double get_res(double[] x)
        {
           // for (int i = 0; i < topologi.GetLength(0); i++)
            //    output[i] = -10000;
            double res = 0;
            res = start1(ref x, topologi.GetLength(0) - 1);
            return res;
        }
      /*  private double start(ref double[] x, int index)
        {
            double res = 0;
            bool is_in = true;
            if (output[index] == -10000)
            {
                for (int i = index - 1; i >= 0; i--)
                {
                    if (topologi[i, index])
                    {
                        is_in = false;
                        res += weight[i, index] * start(ref x, i);
                    }
                }
                if (is_in)
                {
                    res = x[index];
                }
                output[index] = 2 / (1 + Math.Exp(-1 * (res)));
                res = output[index];
            }
            else
                res = output[index];

            return res;
        }*/
        public double start1(ref double[] x,int zag)
        {
            bool is_in;
            double res = 0;
            double[] output = new double[topologi.GetLength(0)] ;
            for (int i = 0; i < topologi.GetLength(0); i++)
                output[i] = -10000;
            for (int k = 0; k < topologi.GetLength(0); k++ )
            {
                res = 0;
                is_in = true;
                for (int i = k - 1; i >= 0; i--)
                {
                    if (topologi[i, k])
                    {
                        is_in = false;
                        res += weight[i, k] * output[i];
                    }
                }
                if (is_in)
                {
                    
                    res = x[k];
                }
                output[k] = 2 / (1 + Math.Exp(-1 * (res)));
                res = output[k];
            }
            return res;
        }
    }
    class GeneticAlgorithm : LearningAlgorithm
    {
        double eps = 1E-008, coef_mutation = 1E-004, persent_train = 0.1, selection_pesent = 0.1;
        double coef_mut_for_adapt;
        int count_person = 100, step_train, max_learning_step = 10000;
        int step, train, best_population_num;

        public override string Name
        {
            get { return "Генетический алгоритм"; }
        }
        bool is_rep_2;
        double min_err;
        const int count_gen_in_hromosom = 30;
        person[] population;
        class person
        {
            public neronnet1 pers;
            //public INeuroNetLearning pers;
            public long[] population_weight;
            public double err, average_error;
            public ulong live_time;
            public int good_live;
            public double[][] training_X;
            private double[] training_Y;
            public double max = -1;
            public void set(ref double[][] training_X_, ref double[] training_Y_)
            {
                training_X = training_X_;
                training_Y = training_Y_;
            }
            public void run(int start, int stop)
            {
                if ((int)live_time < training_X.Length)
                {
                    double[] res = new double[stop - start];

                    Parallel.For(start, stop, j =>
                    {
                        res[j - start] = 1;
                        res[j - start] = Math.Abs(training_Y[j] - pers.get_res(training_X[j]));
                        //err += (res[j - start] * res[j - start]);

                        //err += Math.Pow(training_Y[j] - pers.get_res(training_X[j]), 2);
                    });

                    for (int j = start; j < stop; j++)
                    {
                        if (max < res[j - start])
                        {
                            max = res[j - start];
                        }
                        if (max == -1)
                        {
                            max = -1;
                        }
                        err += (res[j - start] * res[j - start]);
                    }
                }
            }
        }
        bool[,] topologi;
        double[][] training_X;
        double[] training_Y;
        Random rand = new Random();
        public int get_step()//сколько шагов потребовалось
        {
            return step;
        }
        public double get_min_err()//с какой точностью завершился
        {
            return population[best_population_num].average_error;
        }
        public int get_good_live()//соклько прожил хорошим
        {
            return population[best_population_num].good_live;
        }
        public void set_max_step(int max_step)//число повторений для обучения
        {
            max_learning_step = max_step;
        }
        public void set_eps(double e_)//точность
        {
            eps = e_;
        }
        public void set_selection_persent(double selec_)//какой процент скрещивать
        {
            selection_pesent = selec_;
        }
        public void set_coef_mut(double mut)//мутация
        {
            coef_mutation = mut;
            //coef_mut_for_adapt = mut;
        }
        public void set_count_popul(int popul)//количество особей
        {
            count_person = popul;
        }
        public void set_persent_train(double persent_train_)//какой процент выборки использовать до скрещивания
        {
            persent_train = persent_train_;
        }
        private double[,] long_to_doubl(int num_person)
        {
            int num_hromosom = 0;
            const int max_weight = 1 << count_gen_in_hromosom;
            double[,] res = new double[topologi.GetLength(0), topologi.GetLength(1)];
            for (int i = 0; i < topologi.GetLength(0); i++)
            {
                for (int j = 0; j < topologi.GetLength(1); j++)
                {
                    if (topologi[i, j])
                    {
                        double weight_to_double = population[num_person].population_weight[num_hromosom] * 4;
                        double max_weight_to_doulbe = max_weight;
                        res[i, j] = weight_to_double / max_weight_to_doulbe - 2;
                        num_hromosom++;
                    }
                    else
                    {
                        res[i, j] = 0;
                    }
                }
            }
            return res;
        }
        private void sort_person()
        {
            /* person tmp;
             for (int i = 0; i < population.Length; i++)
             {
                 for (int j = i; j < population.Length; j++)
                 {
                     if (population[i].average_error - population[j].average_error > 1E-030)
                     {
                         tmp = population[i];
                         population[i] = population[j];
                         population[j] = tmp;
                     }
                 }
             }*/
            Array.Sort(population, (a, b) => a.average_error.CompareTo(b.average_error));
        }
        System.Windows.Forms.ProgressBar pb;
        public void save_result()
        {
            using (StreamWriter stream = new StreamWriter("data\\matrix.txt"))
            {
                double[,] tmp = long_to_doubl(best_population_num);
                for (int i = 0; i < tmp.GetLength(0); i++)
                {
                    for (int j = 0; j < tmp.GetLength(1); j++)
                    {
                        stream.Write("{0} ", tmp[i, j]);
                    }
                    stream.WriteLine();
                }
            }

        }
        public void genom(INeuroNetLearning solver, double[,] training_set_, bool is_rep_2_)//начало работы генетического алгоритма
        {
            is_rep_2 = is_rep_2_;
            best_population_num = 0;
            training_X = new double[training_set_.GetLength(0)][];
            training_Y = new double[training_set_.GetLength(0)];
            train = Convert.ToInt32(selection_pesent * count_person);
            for (int i = 0; i < training_set_.GetLength(0); i++)
            {
                training_X[i] = new double[training_set_.GetLength(1) - 1];
                training_Y[i] = training_set_[i, training_set_.GetLength(1) - 1];
                for (int j = 0; j < training_set_.GetLength(1) - 1; j++)
                {
                    training_X[i][j] = training_set_[i, j];
                }
            }
            step_train = Convert.ToInt32(training_X.GetLength(0) * persent_train);
            topologi = solver.get_bool_links();//получение топологии
            population = new person[count_person];
            for (int i = 0; i < count_person; i++)
            {
                //population[i].pers = solver.copy();//копируем решатели
                population[i] = new person();
                population[i].pers = new neronnet1(topologi);
            }
            int count_hromosom = 0;
            for (int i = 0; i < topologi.GetLength(0); i++)
            {
                for (int j = 0; j < topologi.GetLength(1); j++)
                {
                    if (topologi[i, j])
                    {
                        count_hromosom++;
                    }
                }
            }

            set_default_weight(count_hromosom);
            coef_mut_for_adapt = coef_mutation;
            for (int i = 0; i < count_person; i++)
            {
                population[i].pers.set_links(long_to_doubl(i));//задаём новые веса, полная матрица весов
            }
            start_genom();
            solver.set_links(long_to_doubl(best_population_num));
        }
        delegate void MyDel(Stopwatch st, int step);
        MyDel del;
        Label label2, label3;
        TimeSpan ts;
        private void start_genom()
        {

            step = 0;
            Progres pg = new Progres();

            Thread t1 = new Thread(new ThreadStart(delegate
                {
                    pb = pg.get_pg();
                    pb.Maximum = max_learning_step;
                    del = new MyDel(pg.start);
                    label2 = pg.get_lb();
                    label3 = pg.get_lb3();
                    pg.ShowDialog();

                }));
            Thread t = new Thread(new ThreadStart(delegate
            {
                Stopwatch stopWatch = new Stopwatch();



                ////////////////////////////////////////////////////////////////
                ////////////////////////////////////////////////////////////////
                ////////////////////////////////////////////////////////////////
                ////////////////////////////////////////////////////////////////
                ////////////////////////////////////////////////////////////////







                stopWatch.Start();
                for (int i = 0; i < training_Y.Length / step_train; i++)
                {


                    //                      for (int j = i * step_train; j < step_train * (i + 1); j++)
                    //                    {
                    Parallel.For(0, count_person, k =>
                    {

                        population[k].run(i * step_train, step_train * (i + 1));
                        //  population[k].err += Math.Pow(training_Y[j] - population[k].pers.get_res(training_X[j]), 2);

                    });// Parallel.For

                    //                       }
                   /* for (int j = 0; j < Convert.ToInt32((1 - selection_pesent) * count_person) -1; j++)
                    {
                        population[j].live_time += Convert.ToUInt64(step_train);
                        population[j].average_error = population[j].err / population[i].live_time;
                    }*/
                    selection(step_train);
                }
                /*    for (int i = 0; i < training_Y.Length / step_train; i++)
                    {
                        for (int j = i * step_train; j < step_train * (i + 1); j++)
                        {
                            for (int k = 0; k < count_person; k++)
                            {
                                population[k].err += Math.Pow(training_Y[j] - population[k].pers.get_res(training_X[j]), 2);
                            }
                        }
                        selection(step_train);
                    }*/
                if (step_train * (Convert.ToInt32(training_Y.Length / step_train)) < training_Y.Length)
                {
                    Parallel.For(0, count_person, k =>
                    {

                        population[k].run(step_train * (Convert.ToInt32(training_Y.Length / step_train)), training_Y.Length);
                        //  population[k].err += Math.Pow(training_Y[j] - population[k].pers.get_res(training_X[j]), 2);

                    });// Parallel.For
                    /*for (int i = step_train * (Convert.ToInt32(training_Y.Length / step_train)); i < training_Y.Length; i++)
                    {
                        for (int k = 0; k < count_person; k++)
                        {
                            population[k].err += Math.Pow(training_Y[i] - population[k].pers.get_res(training_X[i]), 2);
                        }
                    }*/
                    //selection(training_Y.Length - Convert.ToInt32(persent_train * 100 * step_train));
                    selection(training_Y.Length - Convert.ToInt32(persent_train * step_train));
                }
                step++;
                stopWatch.Stop();
                //del.Invoke(stopWatch, max_learning_step - step);
                ts = new TimeSpan(stopWatch.Elapsed.Ticks * (max_learning_step - step));
                string elapsedTime1 = String.Format("{0:00}:{1:00}:{2:00}",
                       ts.Hours, ts.Minutes, ts.Seconds);
                if (label2.InvokeRequired)
                {
                    pb.BeginInvoke(new MethodInvoker(delegate
                    {
                        label2.Text = elapsedTime1.ToString();
                    }));
                }
                else
                {
                    label2.Text = elapsedTime1.ToString();
                }
                if (label3.InvokeRequired)
                {
                    pb.BeginInvoke(new MethodInvoker(delegate
                    {
                        label3.Text = "Error = " + population[0].average_error.ToString();
                    }));
                }
                else
                {
                    label3.Text = "Error = " + population[0].average_error.ToString();
                }
                stopWatch.Reset();
               

                if (pb.InvokeRequired)
                    pb.BeginInvoke(new MethodInvoker(delegate
                    {
                        pb.Value = step;
                    }));
                else
                    pb.Value = step;







                ////////////////////////////////////////////////////////////////
                ////////////////////////////////////////////////////////////////
                ////////////////////////////////////////////////////////////////
                ////////////////////////////////////////////////////////////////
                ////////////////////////////////////////////////////////////////
                ////////////////////////////////////////////////////////////////
                ////////////////////////////////////////////////////////////////






                do
                {
                    stopWatch.Start();
                    for (int i = 0; i < training_Y.Length / step_train; i++)
                    {


                        //                      for (int j = i * step_train; j < step_train * (i + 1); j++)
                        //                    {
                        Parallel.For(/*Convert.ToInt32((1 - selection_pesent) * count_person) - 1*/0, count_person, k =>
                    {

                        population[k].run(i * step_train, step_train * (i + 1));
                        //  population[k].err += Math.Pow(training_Y[j] - population[k].pers.get_res(training_X[j]), 2);

                    });// Parallel.For

                        //                       }
                        selection(step_train);
                    }
                    /*    for (int i = 0; i < training_Y.Length / step_train; i++)
                        {
                            for (int j = i * step_train; j < step_train * (i + 1); j++)
                            {
                                for (int k = 0; k < count_person; k++)
                                {
                                    population[k].err += Math.Pow(training_Y[j] - population[k].pers.get_res(training_X[j]), 2);
                                }
                            }
                            selection(step_train);
                        }*/
                    if (step_train * (Convert.ToInt32(training_Y.Length / step_train)) < training_Y.Length)
                    {
                        Parallel.For(0, count_person, k =>
                        {

                            population[k].run(step_train * (Convert.ToInt32(training_Y.Length / step_train)), training_Y.Length);
                            //  population[k].err += Math.Pow(training_Y[j] - population[k].pers.get_res(training_X[j]), 2);

                        });// Parallel.For
                        /*for (int i = step_train * (Convert.ToInt32(training_Y.Length / step_train)); i < training_Y.Length; i++)
                        {
                            for (int k = 0; k < count_person; k++)
                            {
                                population[k].err += Math.Pow(training_Y[i] - population[k].pers.get_res(training_X[i]), 2);
                            }
                        }*/
                        //selection(training_Y.Length - Convert.ToInt32(persent_train * 100 * step_train));
                        selection(training_Y.Length - Convert.ToInt32(persent_train * step_train));
                    }
                    step++;
                    stopWatch.Stop();
                    //del.Invoke(stopWatch, max_learning_step - step);
                    ts = new TimeSpan(stopWatch.Elapsed.Ticks * (max_learning_step - step));
                    string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}:{3:00}",
                          ts.Days, ts.Hours, ts.Minutes, ts.Seconds);
                    if (label2.InvokeRequired)
                    {
                        pb.BeginInvoke(new MethodInvoker(delegate
                        {
                            label2.Text = elapsedTime.ToString();
                        }));
                    }
                    else
                    {
                        label2.Text = elapsedTime.ToString();
                    }
                    if (label3.InvokeRequired)
                    {
                        pb.BeginInvoke(new MethodInvoker(delegate
                        {
                            label3.Text = "Error = " + population[0].average_error.ToString() + "\nMax = " + population[0].max.ToString();
                        }));
                    }
                    else
                    {
                        label3.Text = "Error = " + population[0].average_error.ToString() + "\nMax = " + population[0].max.ToString();
                    }
                    stopWatch.Reset();
                    if (!pg.Visible)
                    {
                        break;
                    }

                    if (pb.InvokeRequired)
                        pb.BeginInvoke(new MethodInvoker(delegate
                        {
                            pb.Value = step;
                        }));
                    else
                        pb.Value = step;

                    // pb.Value++;
                } while ((step < max_learning_step) && (is_not_stop()));
                if (pb.InvokeRequired)
                    pb.BeginInvoke(new MethodInvoker(delegate
                    {
                        pb.Dispose();
                    }));
                else
                    pb.Dispose();
            }));
            t1.Start();
            t.Start();
            t.Join();

            if (pg.InvokeRequired)
                pg.BeginInvoke(new MethodInvoker(delegate
                {
                    pg.Close();
                }));
            else
                pg.Close();
            t1.Join();



        }

        private void set_default_weight(int count_hromosom)
        {
            const int max_weight = 1 << count_gen_in_hromosom;
            for (int i = 0; i < count_person; i++)
            {
                population[i].population_weight = new long[count_hromosom];
                population[i].err = 0;
                population[i].live_time = 0;
                population[i].good_live = 0;
                population[i].set(ref training_X, ref training_Y);
                for (int j = 0; j < count_hromosom; j++)
                {
                    population[i].population_weight[j] = rand.Next(0, max_weight);
                }
            }
        }

        private bool is_not_stop()
        {
            int i = 0;
            while ((i < count_person))
            {
                if (population[i].average_error < eps)
                {

                    if (population[i].live_time >= Convert.ToUInt64(training_Y.Length * 2))
                    {
                        best_population_num = i;
                        return false;
                    }
                    i++;
                }
                else
                    break;
            }
            return true;
        }
        private void selection(int live_time)
        {
           /* for (int i = Convert.ToInt32((1 - selection_pesent) * count_person) - 1; i < population.Length; i++)
            {
                population[i].live_time += Convert.ToUInt64(live_time);
                population[i].average_error = population[i].err / population[i].live_time;
            }*/
            for (int i = 0; i < population.Length; i++)
            {
                if ((int)population[i].live_time < population[i].training_X.Length)
                {
                    population[i].live_time += Convert.ToUInt64(live_time);
                    population[i].average_error = population[i].err / population[i].live_time;
                }
            }
            //Array.Sort(population, new Comparison<person>((a, b) => a.average_error.CompareTo(b.average_error)));
            sort_person();
           // if (!is_rep_2)
            {
                Reproduction();
            }
           // else
            {
                //Reproduction_2();
            }

            for (int i = 0; i < train; i++)
            {
                population[i].good_live++;

            }
            for (int i = train; i < population.Length - train - 1; i++)
            {
                population[i].good_live = 0;
            }
            for (int i = population.Length - train + 1; i < population.Length; i++)
            {
                population[i].good_live = 0;
                population[i].live_time = 0;
                population[i].err = 0;
                population[i].max = -1;
            }
        }

        private void set_mut(int i, int j)
        {
            double res = 0;
            for(int k = 0; k < population[i].population_weight.Length; k++)
                for (int p = 0; p < count_gen_in_hromosom; p++)
                {
                    if ((population[i].population_weight[k] & (1 << p)) == (population[j].population_weight[k] & (1 << p)))
                    {
                        res += 1;
                    }
                }
            coef_mutation = (1 - Math.Pow(1 - res / (count_gen_in_hromosom* population[i].population_weight.Length), 1.0/8.0))/5.0;
        }

        private void Reproduction()
        {
            long tmp, tmp_;
            const int max_weight = 1 << count_gen_in_hromosom;
            Random r = new Random();
            for (int i = 0; i < train - 1; i += 2)
            {
                //double max_ = (Math.Pow(1.1, count_person - i - 2));
                int next_1 = (int)((double)(count_person - i - 2) * (1 - Math.Sqrt(1 - r.NextDouble())));
                //int next_1 = count_person -  Convert.ToInt32(Math.Log(tmp_next_1,1.1)) - 2 - i;
                //double tmp_next_2 = (r.NextDouble() * max_);
                //int next_2 =count_person - Convert.ToInt32(Math.Log(tmp_next_2,1.1)) - 2 - i;
                int next_2 = (int)((double)(count_person - i - 2) * (1 - Math.Sqrt(1 - r.NextDouble())));
                if(next_1 == next_2)
                {
                    if(next_2 > 1)
                    {
                        next_2--;
                    }
                    else
                    {
                        next_2++;
                    }
                }
               // next_1 = i;
                //next_2 = i + 1;
                if (is_rep_2)
                {
                    set_mut(next_1, next_2);
                }
                for (int j = 0; j < population[i].population_weight.Length; j++)
                {
                    tmp = rand.Next(0, max_weight);
                    tmp_ = tmp;
                    tmp = population[next_1].population_weight[j] & tmp_;
                    tmp |= (population[next_2].population_weight[j] & (~tmp_));
                    population[population.Length - 1 - i].population_weight[j] = tmp;
                    if (rand.NextDouble() < coef_mutation)
                    {
                        long mut = rand.Next(0, max_weight);

                        population[population.Length - 1 - i].population_weight[j] ^= mut;
                    }
                    tmp = population[next_1].population_weight[j] & (~tmp_);
                    tmp |= (population[next_2].population_weight[j] & tmp_);
                    population[population.Length - 2 - i].population_weight[j] = tmp;
                    if (rand.NextDouble() < coef_mutation)
                    {
                        long mut = rand.Next(0, max_weight);

                        population[population.Length - 2 - i].population_weight[j] ^= mut;
                    }

                }
                population[population.Length - 1 - i].pers.set_links(long_to_doubl(population.Length - 1 - i));
                population[population.Length - 2 - i].pers.set_links(long_to_doubl(population.Length - 2 - i));
            }
        }
        /*    private void Reproduction_2()
            {
                double tmp, tmp_,r;
                const int max_weight = 1 << count_gen_in_hromosom;
                for (int i = 0; i < train - 1; i += 2)
                {
                    for (int j = 0; j < population[i].population_weight.Length; j++)
                    {
                        r = (2 * rand.NextDouble() - 1);
                        tmp = population[i].population_weight[j] / max_weight - 1 / 2;
                        tmp_ = population[i + 1].population_weight[j] / max_weight - 1 / 2;
                        population[population.Length - 1 - i].population_weight[j] =BitConverter.DoubleToInt64Bits(tmp
                            + r*(tmp - tmp_));
                        if (rand.NextDouble() < coef_mutation)
                        {
                            population[population.Length - 1 - i].population_weight[j] = BitConverter.DoubleToInt64Bits(2 * rand.NextDouble() - 1);
                        }
                        population[population.Length - 2 - i].population_weight[j] = BitConverter.DoubleToInt64Bits(tmp_
                            + r * (tmp_ - tmp)); 
                        if (rand.NextDouble() < coef_mutation)
                        {
                            population[population.Length - 2 - i].population_weight[j] = BitConverter.DoubleToInt64Bits(2 * rand.NextDouble() - 1);
                        }                   
                    }
                    population[population.Length - 1 - i].pers.set_links(long_to_doubl(population.Length - 1 - i));
                    population[population.Length - 2 - i].pers.set_links(long_to_doubl(population.Length - 2 - i));
                }
            }*/
    }
}
