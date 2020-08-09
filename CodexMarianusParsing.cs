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
        public enum Gospels
        {
            Natthew = 0,
            Mark = 1,
            Luke = 2,
            John = 3
        }
        public CodexMarianusParsing()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
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
                codexMarianus.documentID = new DirectoryInfo(folderPath).GetFiles().Length.ToString();
                var units = Regex.Split(richTextBox1.Text, "\n\n");
                foreach (var unit in units)
                {
                    var strings = unit.Split('\n');
                }
            }
        }
    }
}
