using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MainProjectDMTS
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void tsmiWorkWithDBClick(object sender, EventArgs e)
        {
            SII.TaskForm taskForm = new SII.TaskForm();
            taskForm.MdiParent = this;
            this.Size = new Size(taskForm.Size.Width + 20, taskForm.Size.Height + 70);
            taskForm.StartPosition = FormStartPosition.Manual;
            taskForm.Location = new Point(0, 0);
            taskForm.Show();
            
        }

        private void tsmiNeuroNets_Click(object sender, EventArgs e)
        {
            NeuroWnd.NeuroNetsMainWindow neuroNetsWindow = new NeuroWnd.NeuroNetsMainWindow();
            neuroNetsWindow.MdiParent = this;
            this.Size = new Size(neuroNetsWindow.Size.Width + 20, neuroNetsWindow.Size.Height + 70);
            neuroNetsWindow.StartPosition = FormStartPosition.Manual;
            neuroNetsWindow.Location = new Point(0, 0);
            neuroNetsWindow.Show();
        }

        private void tsmiDesisionTrees_Click(object sender, EventArgs e)
        {
            DesisionTrees.DesisionTreeMainWindow treesWindow = new DesisionTrees.DesisionTreeMainWindow();
            treesWindow.MdiParent = this;
            this.Size = new Size(treesWindow.Size.Width + 20, treesWindow.Size.Height + 70);
            treesWindow.StartPosition = FormStartPosition.Manual;
            treesWindow.Location = new Point(0, 0);
            treesWindow.Show();
        }
    }
}
//To do : 1) юркановскую форму
  //      2) мою форму использования прикрутить параметры