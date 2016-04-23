using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SII
{
    public partial class ProgressBarDialog : Form
    {
        public ProgressBarDialog(Selection selection, String fileName, List<Parametr> arrParams)
        {
            InitializeComponent();
            this.Show();
            selection.LoadArrValueParametersFromFile(fileName, arrParams, this.progressBar);
            this.Hide();
        }
    }
}
