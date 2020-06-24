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
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            label1.Text = "Вам нужно выбрать, как используется буква U:\n для обозначения ижицы или ука как лигатуры.\n В противном случае,\n программа будет работать некорректно";
            button1.Text = "OK";
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Close();
        }

    }
}
