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
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using Newtonsoft.Json;

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
                    try
                    {
                        Text currentText = new Text();
                        var strings = unit.Split('\n');
                        var textNameString = strings.Where((s) => s.Contains("source")).FirstOrDefault();
                        var textName = Regex.Split(textNameString, ", ")[1];
                        var textIDAndName = textName.Split(' ');
                        int preliminaryID = 0;
                        if (textIDAndName[0] == "Matthew")
                        {
                            preliminaryID = 100 + Convert.ToInt32(textIDAndName[1]);
                        }
                        else if (textIDAndName[0] == "Mark")
                        {
                            preliminaryID = 200 + Convert.ToInt32(textIDAndName[1]);
                        }
                        else if (textIDAndName[0] == "Luke")
                        {
                            preliminaryID = 300 + Convert.ToInt32(textIDAndName[1]);
                        }
                        else if (textIDAndName[0] == "John")
                        {
                            preliminaryID = 400 + Convert.ToInt32(textIDAndName[1]);
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
                            var parts = l.Split('\t');
                            var id = parts[0];
                            var lexeme = parts[1];
                            Realization currentRealization = new Realization(currentClause, id, lexeme, lexeme);
                            if (currentRealization.realizationFields == null)
                            {
                                currentRealization.realizationFields = new List<Dictionary<string, List<SimpleValue>>>();
                            }
                            currentRealization.realizationFields.Add(new Dictionary<string, List<SimpleValue>>());
                            var lemma = parts[2];
                            List<SimpleValue> lemmaList = new List<SimpleValue>();
                            lemmaList.Add(new SimpleValue(lemma));
                            currentRealization.realizationFields[0].Add("Lemma", lemmaList);
                            var partOfSpeech = parts[3];
                            List<SimpleValue> posList = new List<SimpleValue>();
                            posList.Add(new SimpleValue(partOfSpeech));
                            currentRealization.realizationFields[0].Add("PoS", posList);
                            List<Grapheme> letters(Realization word)
                            {
                                List<Grapheme> graphemes = new List<Grapheme>();
                                for (int i = 0; i < word.lexemeOne.Length; i++)
                                {
                                    graphemes.Add(new Grapheme(currentRealization, i.ToString(), word.lexemeOne[i].ToString()));
                                }
                                return graphemes;
                            }
                            currentRealization.letters = letters(currentRealization);
                            if (currentClause.realizations == null)
                            {
                                currentClause.realizations = new List<Realization>();
                            }
                            currentClause.realizations.Add(currentRealization);
                        }
                        if (currentText.clauses == null)
                        {
                            currentText.clauses = new List<Clause>();
                        }
                        currentText.clauses.Add(currentClause);
                        if (codexMarianus.texts == null)
                        {
                            codexMarianus.texts = new List<Text>();
                            codexMarianus.texts.Add(currentText);
                        }
                        else
                        {
                            if (codexMarianus.texts.Where((text) => text.textID == currentText.textID.ToString()).Count() > 0)
                            {
                                int index = codexMarianus.texts.FindIndex((text) => text.textID == currentText.textID);
                                codexMarianus.texts[index] = currentText;
                            }
                            else
                            {
                                codexMarianus.texts.Add(currentText);
                            }
                        }
                        progressBar1.PerformStep();
                    }
                    catch
                    {
                        continue;
                    }                    
                }
                using (StreamWriter w = new StreamWriter(Path.Combine(folderPath, "Codex_Marianus.json")))
                {
                    w.WriteLine(codexMarianus.Jsonize());
                }
                MessageBox.Show("Файл обработан!", "Сообщение системы", MessageBoxButtons.OK);
            }
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            var choice = openFileDialog1.ShowDialog();
            if (choice == DialogResult.OK)
            {
                string filePath = openFileDialog1.FileName;
                Document codexMarianus = new Document();
                using (StreamReader r = new StreamReader(filePath))
                {
                    codexMarianus = JsonConvert.DeserializeObject<Document>(r.ReadToEnd());
                }
                List<(string, string)> pairs = new List<(string, string)>();
                foreach (var text in codexMarianus.texts)
                {
                    foreach (var clause in text.clauses)
                    {
                        foreach(var realization in clause.realizations)
                        {
                            foreach (var fieldGroup in realization.realizationFields)
                            {
                                pairs.Add((realization.lexemeOne, fieldGroup["PoS"][0].name));
                            }
                        }
                    }
                }
                ScriptEngine engine = Python.CreateEngine();
                ScriptScope scope = engine.CreateScope();
                var paths = engine.GetSearchPaths();
                paths.Add(@"C:\Users\user\source\repos\OldSlavonicCorpusPreprocessing\OldSlavonicCorpusPreprocessing\Lib\");
                engine.SetSearchPaths(paths);
                openFileDialog1.ShowDialog();
                var pythonFilePath = openFileDialog1.FileName;
                engine.ExecuteFile(pythonFilePath, scope);
                dynamic function = scope.GetVariable("main");
                var result = function(pairs, 90, 0, 0);
                MessageBox.Show(result.ToString());
            }
        }
    }
}
