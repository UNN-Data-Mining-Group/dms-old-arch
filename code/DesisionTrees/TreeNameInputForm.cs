using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DesisionTrees
{
    public partial class TreeNameInputForm : Form
    {
        

        public TreeNameInputForm()
        {
            
            InitializeComponent();
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            DesisionTreeMainWindow main = this.Owner as DesisionTreeMainWindow;
            main.newTreeName = txtbTreeName.Text;
            this.Close();
        }

        private void txtbTreeName_TextChanged(object sender, EventArgs e)
        {
            if (txtbTreeName.Text == "")
            {
                btnConfirm.Enabled = false;
            }
            else
            {
                btnConfirm.Enabled = true;
            }
        }
    }
}
