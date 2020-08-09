using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using CorpusDraftCSharp;

namespace OldSlavonicCorpusPreprocessing
{
    public partial class CodexMarianusParsing : Form
    {
        public CodexMarianusParsing()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = "";
            var choice = openFileDialog1.ShowDialog();
            if (choice == DialogResult.OK)
            {
                string filePath = openFileDialog1.FileName;
                using (StreamReader r = new StreamReader(filePath))
                {
                    richTextBox1.Text += r.ReadToEnd();
                }
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            var choice = folderBrowserDialog1.ShowDialog();
            if (choice == DialogResult.OK)
            {
                Document codexMarianus = new Document();
                string folderPath = folderBrowserDialog1.SelectedPath;
                var units = Regex.Split(richTextBox1.Text, "\n\n");
            }
        }
    }
}
