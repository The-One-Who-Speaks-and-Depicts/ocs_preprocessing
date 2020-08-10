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
            Matthew = 0,
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
                string folderPath = folderBrowserDialog1.SelectedPath;
                Document codexMarianus = new Document(new DirectoryInfo(folderPath).GetFiles().Length.ToString(), "Codex Marianus",
                    new DirectoryInfo(folderPath).GetFiles().Length.ToString() + "_" + "Codex Marianus", "_");
                var units = Regex.Split(richTextBox1.Text, "\n\n");
                progressBar1.Value = 0;
                progressBar1.Step = 1;
                progressBar1.Maximum = units.Length;
                foreach (var unit in units)
                {
                    Text currentText = new Text();
                    var strings = unit.Split('\n');
                    var textNameString = strings.Where((s) => s.Contains("source")).FirstOrDefault();
                    var textName = Regex.Split(textNameString, ", ")[1];
                    var textIDAndName = textName.Split(' ');
                    int preliminaryID = 0;
                    if (textIDAndName[0] == "Matthew")
                    {
                        preliminaryID = Convert.ToInt32(Gospels.Matthew + textIDAndName[1]);
                    }
                    else if (textIDAndName[0] == "Mark")
                    {
                        preliminaryID = Convert.ToInt32(Gospels.Mark + textIDAndName[1]);
                    }
                    else if (textIDAndName[0] == "Luke")
                    {
                        preliminaryID = Convert.ToInt32(Gospels.Luke + textIDAndName[1]);
                    }
                    else if (textIDAndName[0] == "John")
                    {
                        preliminaryID = Convert.ToInt32(Gospels.John + textIDAndName[1]);
                    }
                    if (codexMarianus.texts != null)
                    {
                        if (codexMarianus.texts.Where((text) => text.textID == preliminaryID.ToString()).ToList().Count != 0)
                        {
                            currentText = codexMarianus.texts.Where((text) => text.textID == preliminaryID.ToString()).FirstOrDefault();
                        }
                        else
                        {
                            currentText = new Text(codexMarianus, preliminaryID.ToString(), textName);
                        }
                    }
                    else
                    {
                        currentText = new Text(codexMarianus, preliminaryID.ToString(), textName);
                    }
                    var clauseID = strings.Where((s) => s.Contains("text")).FirstOrDefault();
                    var clauseText = strings.Where((s) => s.Contains("text")).FirstOrDefault();
                    Clause currentClause = new Clause(currentText, clauseID, clauseText);
                    var lexemes = strings.Where((s) => Regex.IsMatch(s, @"^\d{1,}")).ToList();
                    foreach (var l in lexemes)
                    {
                        var parts = l.Split(' ');
                        var id = parts[0];
                        var lexeme = parts[1];
                        Realization currentRealization = new Realization(currentClause, id, lexeme, lexeme);
                        var lemma = parts[2];
                        // add lemma field
                        var partOfSpeech = parts[3];
                        // add pos field
                        Func<Realization, List<Grapheme>> letters = (word) =>
                        {
                            List<Grapheme> graphemes = new List<Grapheme>();
                            for (int i = 0; i < word.lexemeOne.Length; i++)
                            {
                                graphemes.Add(new Grapheme(currentRealization, i.ToString(), word.lexemeOne[i].ToString()));
                            }
                            return graphemes;
                        };
                        currentRealization.letters = letters.Invoke(currentRealization);
                        if (currentClause.realizations == null)
                        {
                            currentClause.realizations = new List<Realization>();
                        }
                        currentClause.realizations.Add(currentRealization);
                    }
                    progressBar1.PerformStep();
                }
            }
        }
    }
}
