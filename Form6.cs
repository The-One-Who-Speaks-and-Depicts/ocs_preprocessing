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
    public partial class Form6 : Form
    {
        public Form6()
        {
            InitializeComponent();
            label1.Text = "Текст не подготовлен к занесению в базу.\n Проделайте все необходимые шаги:\n1) Скачайте текст\n2)Выделите текст и метаданные\n3) Выполните конвертацию из ASCII в Unicode";
            button1.Text = "OK";
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
