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
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
            label1.Text = "Вам нужно указать ссылку, по которой располагается\n текст, приготовленный для прочтения программой";
            button1.Text = "ОК";
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
