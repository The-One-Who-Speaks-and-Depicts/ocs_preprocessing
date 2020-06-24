using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OldSlavonicCorpusPreprocessing
{
    public partial class Form5 : Form
    {
        public Form5()
        {
            InitializeComponent();
            label1.Text = "Имя текста не введено";
            button1.Text = "OK";
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
