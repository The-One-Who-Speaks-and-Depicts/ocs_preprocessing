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
    public partial class StartWindow : Form
    {
        public StartWindow()
        {
            InitializeComponent();
            Text = "Начальное окно";
            button1.Text = "Создать новый документ в корпусе";
            button2.Text = "Обучить модель на CoNLL-U данных";
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            new Form1().ShowDialog(this);
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            new CodexMarianusParsing().ShowDialog(this);
        }
    }
}