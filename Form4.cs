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
    public partial class Form4 : Form
    {
        public Form4()
        {
            InitializeComponent();
            label1.Text = "Исходный текст не существует.\n Загрузите текст при помощи кнопки\n \"Загрузить текст со страницы\"";
            button1.Text = "ОК";
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
