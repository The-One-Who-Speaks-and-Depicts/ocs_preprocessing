using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Data.Linq;
using System.Diagnostics;
using CorpusDraftCSharp;
using Newtonsoft.Json;

namespace OldSlavonicCorpusPreprocessing
{
    public partial class Form1 : Form
    {
        string processing_text;
        bool text_is_edited = false;
        bool text_is_preprocessed = false;
        bool text_is_tokenized = false;
        bool text_is_ready = false;
        public Form1()
        {
            InitializeComponent();
            listBox2.SetSelected(0, true);
            button1.Text = "Загрузить текст со страницы";
            label1.Text = "Источник";
            label2.Text = "Оригинальный текст";
            label3.Text = "Обработанный текст\nдля дальнейшего редактирования";
            label4.Text = "Введите название текста";
            label5.Text = "Укажите период создания текста:";
            button2.Text = "Выделить текст источника и метаданные";
            button3.Text = "ASCII => Unicode";
            button4.Text = "Разбить текст по знакам пунктуации";
            button7.Text = "Токенизировать текст";
            button5.Text = "Занести текст в базу";
            button6.Text = "Соединить текст";
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            string input = GetCode(textBox1.Text);
            richTextBox1.Text += input;
            processing_text = input;
            MessageBox.Show("Текст загружен", "Сообщение программы");
        }

        public static string[] StringArrayRetrieval(string text)
        {
            string[] textEdited = text.Split('\n');
            return textEdited;
        }
        public static string GetCode(string urlAddress)
        {
            string data = "";
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlAddress);

                Cookie cookie = new Cookie
                {
                    Name = "beget",
                    Value = "begetok"
                };

                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(new Uri(urlAddress), cookie);

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Stream receiveStream = response.GetResponseStream();
                    StreamReader readStream = null;
                    if (response.CharacterSet == null)
                    {
                        readStream = new StreamReader(receiveStream);
                    }
                    else
                    {
                        readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                    }
                    data = readStream.ReadToEnd();
                    response.Close();
                    readStream.Close();
                }
                return data;
            }
            catch
            {
                Form3 emptyLinkDialog = new Form3();
                emptyLinkDialog.ShowDialog();
                emptyLinkDialog.Dispose();

            }
            return data;
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            processing_text = richTextBox1.Text;
            if (processing_text != null)
            {
                progressBar1.Minimum = 1;
                progressBar1.Maximum = processing_text.Length - 1;
                progressBar1.Value = 1;
                progressBar1.Step = 1;
                string pattern_pre = @"<pre.*>(.*\n)*<\/pre>";
                foreach (Match match in Regex.Matches(processing_text, pattern_pre, RegexOptions.IgnoreCase))
                {
                    string output = Regex.Replace(processing_text, @"<.*?>", "");
                    output = Regex.Replace(output, @"[0-9]{4,7}\s", "");
                    output = Regex.Replace(output, @"&amp;", "&");
                    richTextBox2.Text += output;
                    richTextBox2.Text += "\n";
                }
                string pattern_num = @"^[0-9]{4,7}";
                foreach (Match match in Regex.Matches(processing_text, pattern_num, RegexOptions.IgnoreCase))
                {
                    string output = Regex.Replace(processing_text, @"<.*?>", "");
                    output = Regex.Replace(output, @"&amp;", "&");
                    output = Regex.Replace(output, @"^[0-9]{4,7}\s", "");
                    richTextBox2.Text += output;
                    richTextBox2.Text += "\n";
                }
                string pattern_percent = @"^%{4,7}"; // needs testing on codex supraslensis
                foreach (Match match in Regex.Matches(processing_text, pattern_percent, RegexOptions.IgnoreCase))
                {
                    string output = Regex.Replace(processing_text, @"<.*?>", "");
                    output = Regex.Replace(output, @"&amp;", "&");
                    richTextBox2.Text += output;
                    richTextBox2.Text += "\n";
                }
                progressBar1.PerformStep();
                MessageBox.Show("Предварительная обработка текста завершена", "Сообщение программы");
            }
            else
            {
                Form4 textAbsentDialog = new Form4();
                textAbsentDialog.ShowDialog();
                textAbsentDialog.Dispose();

            }

        }

        private void Button3_Click(object sender, EventArgs e)
        {
            if (!listBox1.GetSelected(1) && !listBox1.GetSelected(0))
            {
                Form2 letterChoiceEmptyDialog = new Form2();
                letterChoiceEmptyDialog.ShowDialog(this);
                letterChoiceEmptyDialog.Dispose();
            }
            else
            {
                progressBar1.Maximum = 48;
                progressBar1.Value = 1;
                progressBar1.Step = 1;
                string output = richTextBox2.Text;
                richTextBox2.ResetText();


                output = Regex.Replace(output, @"\*<~>", "<~>*");
                output = Regex.Replace(output, @"\*\(\(\(", "(((*");
                output = Regex.Replace(output, @"\*\(\(/", "((/*");
                output = Regex.Replace(output, @"\*\(//", "(//*");

                output = Regex.Replace(output, @"\*/\\", "/\\*"); // /\
                output = Regex.Replace(output, @"\*!~", "!~*"); // !~
                output = Regex.Replace(output, @"\*\[\*", "[*"); // [*
                output = Regex.Replace(output, @"\*//", "//*"); // //
                output = Regex.Replace(output, @"\*\(/", "(/*"); // (/
                output = Regex.Replace(output, @"\*\(\(", "((*"); // ((
                output = Regex.Replace(output, @"\*/\(", "/(*"); // /(
                output = Regex.Replace(output, @"\*\\/", "\\/*");
                output = Regex.Replace(output, @"\*!", "!*");

                output = Regex.Replace(output, @"\*!", "!*");
                output = Regex.Replace(output, @"\*\(", "(*");
                output = Regex.Replace(output, "\\*\"", "\"*");
                output = Regex.Replace(output, @"\*\)", ")*");
                output = Regex.Replace(output, @"\*\^", "^*");
                output = Regex.Replace(output, @"\*~", "~*");
                output = Regex.Replace(output, @"\*/", "/*");
                output = Regex.Replace(output, @"\*`", "`*");



                output = Regex.Replace(output, @"\*\*", "*");
                progressBar1.PerformStep();
                if (listBox1.GetSelected(0))
                {
                    output = Regex.Replace(output, @"\*v", "В");
                    output = Regex.Replace(output, @"v", "в");
                    progressBar1.PerformStep();
                    output = Regex.Replace(output, @"\*U", "Ѵ");
                    output = Regex.Replace(output, @"U", "ѵ");
                    progressBar1.PerformStep();
                }
                else
                {
                    output = Regex.Replace(output, @"\*U", "У");
                    output = Regex.Replace(output, @"U", "у");
                    progressBar1.PerformStep();
                    output = Regex.Replace(output, @"\*v", "В");
                    output = Regex.Replace(output, @"v", "в");
                    progressBar1.PerformStep();
                }
                output = Regex.Replace(output, @"\*ja", "Ꙗ");
                output = Regex.Replace(output, @"ja", "ꙗ");
                progressBar1.PerformStep();
                output = Regex.Replace(output, @"\*a", "А");
                output = Regex.Replace(output, @"a", "а");
                progressBar1.PerformStep();
                output = Regex.Replace(output, @"\*b", "Б");
                output = Regex.Replace(output, @"b", "б");
                progressBar1.PerformStep();
                output = Regex.Replace(output, @"\*g", "Г");
                output = Regex.Replace(output, @"g", "г");
                progressBar1.PerformStep();
                output = Regex.Replace(output, @"\*d", "Д");
                output = Regex.Replace(output, @"d", "д");
                progressBar1.PerformStep();
                output = Regex.Replace(output, @"\*je", "Ѥ");
                output = Regex.Replace(output, @"je", "ѥ");
                progressBar1.PerformStep();
                output = Regex.Replace(output, @"\*e", "Е");
                output = Regex.Replace(output, @"e", "е");
                progressBar1.PerformStep();
                output = Regex.Replace(output, @"\*i", "И");
                output = Regex.Replace(output, @"i", "и");
                progressBar1.PerformStep();
                output = Regex.Replace(output, @"\*k", "К");
                output = Regex.Replace(output, @"k", "к");
                progressBar1.PerformStep();
                output = Regex.Replace(output, @"\*l", "Л");
                output = Regex.Replace(output, @"l", "л");
                progressBar1.PerformStep();
                output = Regex.Replace(output, @"\*m", "М");
                output = Regex.Replace(output, @"m", "м");
                progressBar1.PerformStep();
                output = Regex.Replace(output, @"\*n", "Н");
                output = Regex.Replace(output, @"n", "н");
                progressBar1.PerformStep();
                output = Regex.Replace(output, @"\*o", "О");
                output = Regex.Replace(output, @"o", "о");
                progressBar1.PerformStep();
                output = Regex.Replace(output, @"\*p", "П");
                output = Regex.Replace(output, @"p", "п");
                progressBar1.PerformStep();
                output = Regex.Replace(output, @"\*r", "Р");
                output = Regex.Replace(output, @"r", "р");
                progressBar1.PerformStep();
                output = Regex.Replace(output, @"\*s", "С");
                output = Regex.Replace(output, @"s", "с");
                progressBar1.PerformStep();
                output = Regex.Replace(output, @"\*t", "Т");
                output = Regex.Replace(output, @"t", "т");
                progressBar1.PerformStep();
                output = Regex.Replace(output, @"\*ju", "Ю");
                output = Regex.Replace(output, @"ju", "ю");
                progressBar1.PerformStep();
                output = Regex.Replace(output, @"\*u", "Ѹ");
                output = Regex.Replace(output, @"u", "ѹ");
                progressBar1.PerformStep();
                output = Regex.Replace(output, @"\*F", "Ф");
                output = Regex.Replace(output, @"\*f", "Ф");
                output = Regex.Replace(output, @"F", "ф");
                output = Regex.Replace(output, @"f", "ф");
                progressBar1.PerformStep();
                output = Regex.Replace(output, @"\*x", "Х");
                output = Regex.Replace(output, @"x", "х");
                progressBar1.PerformStep();
                output = Regex.Replace(output, @"\*c", "Ц");
                output = Regex.Replace(output, @"c", "ц");
                progressBar1.PerformStep();
                output = Regex.Replace(output, @"\*y", "Ы");
                output = Regex.Replace(output, @"y", "ы");
                progressBar1.PerformStep();
                output = Regex.Replace(output, @"\*S", "Ш");
                output = Regex.Replace(output, @"S", "ш");
                progressBar1.PerformStep();
                output = Regex.Replace(output, @"\*D", "S");
                output = Regex.Replace(output, @"D", "s");
                progressBar1.PerformStep();
                output = Regex.Replace(output, @"\*Z", "Ж");
                output = Regex.Replace(output, @"Z", "ж");
                progressBar1.PerformStep();
                output = Regex.Replace(output, @"\*G", "Ꙉ");
                output = Regex.Replace(output, @"G", "Ꙉ");
                progressBar1.PerformStep();
                output = Regex.Replace(output, @"\*C", "Ч");
                output = Regex.Replace(output, @"C", "ч");
                progressBar1.PerformStep();
                output = Regex.Replace(output, @"\*&", "Ъ");
                output = Regex.Replace(output, @"&", "ъ");
                progressBar1.PerformStep();
                output = Regex.Replace(output, @"\*@", "Ѣ");
                output = Regex.Replace(output, @"@", "ѣ");
                progressBar1.PerformStep();
                output = Regex.Replace(output, @"\*\$", "Ь");
                output = Regex.Replace(output, @"\$", "ь");
                progressBar1.PerformStep();
                output = Regex.Replace(output, @"\*jE", "Ѩ");
                output = Regex.Replace(output, @"jE", "ѩ");
                progressBar1.PerformStep();
                output = Regex.Replace(output, @"\*E", "Ѧ");
                output = Regex.Replace(output, @"E", "ѧ");
                progressBar1.PerformStep();
                output = Regex.Replace(output, @"\*jO", "Ѭ");
                output = Regex.Replace(output, @"jO", "ѭ");
                progressBar1.PerformStep();
                output = Regex.Replace(output, @"\*O", "Ѫ");
                output = Regex.Replace(output, @"O", "ѫ");
                progressBar1.PerformStep();
                output = Regex.Replace(output, @"\*Y", "Ѵ");
                output = Regex.Replace(output, @"Y", "ѵ");
                progressBar1.PerformStep();
                output = Regex.Replace(output, @"\*J", "I");
                output = Regex.Replace(output, @"\*I", "I");
                output = Regex.Replace(output, @"J", "i");
                output = Regex.Replace(output, @"I", "i");
                progressBar1.PerformStep();
                output = Regex.Replace(output, @"\*z", "З");
                output = Regex.Replace(output, @"z", "з");
                progressBar1.PerformStep();
                output = Regex.Replace(output, @"\*T", "Ѳ");
                output = Regex.Replace(output, @"T", "ѳ");
                progressBar1.PerformStep();
                output = Regex.Replace(output, @"\*A", "Ꙙ");
                output = Regex.Replace(output, @"A", "Ꙙ");
                progressBar1.PerformStep();
                output = Regex.Replace(output, @"\*q", "Щ");
                output = Regex.Replace(output, @"q", "щ");
                progressBar1.PerformStep();
                output = Regex.Replace(output, @"\*X", "Ѯ");
                output = Regex.Replace(output, @"X", "ѯ");
                progressBar1.PerformStep();
                output = Regex.Replace(output, @"\*P", "Ѱ");
                output = Regex.Replace(output, @"P", "ѱ");
                progressBar1.PerformStep();
                output = Regex.Replace(output, @"\\", "∽");
                output = Regex.Replace(output, @"--", "∽");
                progressBar1.PerformStep();
                richTextBox2.Text += output;
                progressBar1.PerformStep();
                MessageBox.Show("Кодировка текста изменена", "Сообщение программы");
                text_is_edited = true;
            }
        }



        private void Button5_Click(object sender, EventArgs e)
        {
            if (text_is_ready)
            {
                bool res = false;
                do
                {
                    MessageBox.Show("Укажите папку, в которой размещается корпус", "Сообщение программы");
                    var choice = folderBrowserDialog1.ShowDialog();
                    if (choice == DialogResult.OK)
                    {
                        string folderPath = folderBrowserDialog1.SelectedPath;
                        Document serializedDoc = new Document(new DirectoryInfo(folderPath).GetFiles().Length.ToString(), textBox2.Text,
                            new DirectoryInfo(folderPath).GetFiles().Length.ToString() + "_" + textBox2.Text, "_");
                        var units = Regex.Split(richTextBox2.Text, "\n");
                        progressBar1.Minimum = 0;
                        progressBar1.Value = 0;
                        progressBar1.Step = 1;
                        progressBar1.Maximum = units.Length;
                        Text currentText = new Text(serializedDoc, "0", serializedDoc.documentName);
                        for (int i = 0; i < units.Length; i++)
                        {
                            try
                            {
                                Clause currentClause = new Clause(currentText, i.ToString(), units[i]);
                                var lexemes = currentClause.clauseText.Split(' ');
                                for (int j = 0; j < lexemes.Length; j++)
                                {
                                    Realization currentRealization;
                                    if (lexemes[j].StartsWith("!"))
                                    {
                                        if (Regex.Match(lexemes[j], @"[!]{1}[Ц|ц|W|w|Ѱ|ѱ|Х|х|Ф|ф|Ѹ|ѹ|У|у|Т|т|С|с|Р|р]{0,1}[Ч|ч|П|п|О|о|Ѯ|ѯ|Н|н|М|м|Л|л|К|к|I|i|И|и]{0,1}[Ѳ|ѳ|И|и|З|з|Е|е|Д|д|Г|г|В|в|Б|б|А|а]{0,1}$").Success)
                                        {
                                            currentRealization = new Realization(currentClause, j.ToString(), lexemes[j], "~" + Regex.Replace(lexemes[j], @"[\[\]\?\-\'\`\^\~\(\)\!\\]", "") + "~");
                                        }
                                        else
                                        {
                                            currentRealization = new Realization(currentClause, j.ToString(), lexemes[j], "~" + Regex.Replace(lexemes[j], @"[\[\]\?\-\'\`\^\~\(\)\!\\]", ""));
                                        }

                                    }
                                    else
                                    {
                                        currentRealization = new Realization(currentClause, j.ToString(), lexemes[j], Regex.Replace(lexemes[j], @"[\[\]\?\-\'\`\^\~\(\)\!\\]", ""));
                                    }
                                    if (currentRealization.realizationFields == null)
                                    {
                                        currentRealization.realizationFields = new List<Dictionary<string, List<IValue>>>();
                                    }
                                    currentRealization.realizationFields.Add(new Dictionary<string, List<IValue>>());
                                    List<Grapheme> letters(Realization word)
                                    {
                                        List<Grapheme> graphemes = new List<Grapheme>();
                                        for (int k = 0; k < word.lexemeOne.Length; k++)
                                        {
                                            graphemes.Add(new Grapheme(currentRealization, k.ToString(), word.lexemeOne[k].ToString()));
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

                                progressBar1.PerformStep();
                            }
                            catch (Exception r)
                            {
                                if (r is IOException)
                                {
                                    Console.WriteLine(r.Message);
                                }
                                continue;
                            }
                        }
                        serializedDoc.texts.Add(currentText);

                        using (StreamWriter sw = new StreamWriter(Path.Combine(folderPath, serializedDoc.documentName + ".json")))
                        {
                            sw.Write(serializedDoc.Jsonize());
                        }


                        DialogResult dialogResult = MessageBox.Show("Требуются ли частеречная разметка и лемматизация?", "Сообщение программы", MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.Yes)
                        {
                            SimpleValue period = new SimpleValue(listBox2.Text);
                            List<IValue> periodFieldValues = new List<IValue>();
                            periodFieldValues.Add(period);
                            SimpleValue tagged = new SimpleValue("Automatically_tagged");
                            List<IValue> taggedFieldValues = new List<IValue>();
                            taggedFieldValues.Add(tagged);
                            Dictionary<string, List<IValue>> fields = new Dictionary<string, List<IValue>>();
                            fields.Add("Period", periodFieldValues);
                            fields.Add("Tagged", taggedFieldValues);
                            serializedDoc.documentMetaData.Add(fields);
                            using (StreamWriter sw = new StreamWriter(Path.Combine(folderPath, serializedDoc.documentName + ".json")))
                            {
                                sw.Write(serializedDoc.Jsonize());
                            }
                            res = false;
                            string model_path = "";
                            do
                            {
                                MessageBox.Show("Укажите папку с моделью, обученной для частеречной разметки и файлом Python", "Сообщение системы", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
                                DialogResult result = folderBrowserDialog.ShowDialog();
                                if (result == DialogResult.OK)
                                {
                                    res = true;
                                    model_path = folderBrowserDialog.SelectedPath;
                                }
                            }
                            while (!res);

                            string command = "python " + Path.Combine(model_path, "main.py") + " --data " + Path.Combine(folderPath, serializedDoc.documentName + ".json") + " --modus prediction --folder " + model_path;
                            var processInfo = new ProcessStartInfo("cmd.exe", "/c " + command);
                            processInfo.CreateNoWindow = true;
                            processInfo.UseShellExecute = false;
                            processInfo.RedirectStandardError = true;
                            processInfo.RedirectStandardOutput = true;
                            var process = Process.Start(processInfo);
                            process.WaitForExit();
                            process.Close();

                            res = false;
                            model_path = "";
                            do
                            {
                                MessageBox.Show("Укажите папку с моделью, обученной для лемматизации и файлом Python", "Сообщение системы", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
                                DialogResult result = folderBrowserDialog.ShowDialog();
                                if (result == DialogResult.OK)
                                {
                                    res = true;
                                    model_path = folderBrowserDialog.SelectedPath;
                                }
                            }
                            while (!res);
                            command = "python " + Path.Combine(model_path, "main.py") + " --data " + Path.Combine(folderPath, serializedDoc.documentName + ".json") + " --modus prediction --folder " + model_path;
                            processInfo = new ProcessStartInfo("cmd.exe", "/c " + command);
                            processInfo.CreateNoWindow = true;
                            processInfo.UseShellExecute = false;
                            processInfo.RedirectStandardError = true;
                            processInfo.RedirectStandardOutput = true;
                            process = Process.Start(processInfo);
                            process.WaitForExit();
                            process.Close();
                            MessageBox.Show("Частеречная разметка и лемматизация проведены для текста", "Сообщение системы");
                            res = true;
                        }
                        else if (dialogResult == DialogResult.No)
                        {
                            SimpleValue period = new SimpleValue(listBox2.Text);
                            List<IValue> periodFieldValues = new List<IValue>();
                            periodFieldValues.Add(period);
                            SimpleValue tagged = new SimpleValue("Not_tagged");
                            List<IValue> taggedFieldValues = new List<IValue>();
                            taggedFieldValues.Add(tagged);
                            Dictionary<string, List<IValue>> fields = new Dictionary<string, List<IValue>>();
                            fields.Add("Period", periodFieldValues);
                            fields.Add("Tagged", taggedFieldValues);
                            serializedDoc.documentMetaData.Add(fields);
                            using (StreamWriter sw = new StreamWriter(Path.Combine(folderPath, serializedDoc.documentName + ".json")))
                            {
                                sw.Write(serializedDoc.Jsonize());
                            }
                            MessageBox.Show("Частеречная разметка и лемматизация не проведены для текста", "Сообщение системы");
                            res = true;
                        }
                        res = true;
                    }
                    res = true;
                    MessageBox.Show("Текст занесён в базу данных", "Сообщение системы", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                while (!res);
            }
            else
            {
                MessageBox.Show("Прежде чем заносить текст в базу данных, нужно разбить его по знакам пунктуации", "Сообщение программы", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }



        private void Button6_Click(object sender, EventArgs e)
        {
            if (!text_is_edited)
            {
                MessageBox.Show("Форматирование следует после того, как выполнена предварительная обработка текста", "Сообщение программы", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                text_is_edited = false;
                List<string> splittedText = richTextBox2.Text.Split('\n').ToList<string>();
                string editedText = "";
                progressBar1.Maximum = splittedText.Count - 1;
                progressBar1.Minimum = 1;
                progressBar1.Value = 1;
                for (int i = 0; i < splittedText.Count; i++)
                {
                    if (splittedText[i].EndsWith("-"))
                    {
                        editedText += Regex.Replace(splittedText[i], @"-\z", "");
                    }
                    else
                    {
                        editedText += splittedText[i];
                        editedText += " ";
                    }
                    progressBar1.PerformStep();
                }
                editedText = Regex.Replace(editedText, @"-\s", "");
                richTextBox2.Text = editedText;
                MessageBox.Show("Форматирование завершено", "Сообщение программы", MessageBoxButtons.OK);
                text_is_preprocessed = true;
            }
        }

        private void Button7_Click(object sender, EventArgs e)
        {
            if (!text_is_preprocessed)
            {
                MessageBox.Show("Прежде чем токенизировать текст, выполните предобработку", "Сообщение программы", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                // и поскольку nltk не работает, нам придётся делать это всё вручную
                List<string> splittedText = richTextBox2.Text.Split(' ').ToList<string>();
                richTextBox2.Clear();
                List<string> tokenizedText = new List<string>();
                progressBar1.Maximum = splittedText.Count - 1;
                progressBar1.Minimum = 1;
                progressBar1.Value = 1;
                foreach (var word in splittedText)
                {
                    try
                    {
                        if (!String.IsNullOrWhiteSpace(word) && !String.IsNullOrEmpty(word))
                        {
                            if (word[0] == '=')
                            {
                                var new_words = Regex.Split(word, @"([=]+)");
                                foreach (var new_word in new_words)
                                {
                                    if (!String.IsNullOrWhiteSpace(word) && !String.IsNullOrEmpty(word))
                                    {
                                        tokenizedText.Add(new_word);
                                    }
                                }
                            }
                            else
                            {
                                if (Regex.IsMatch(word, @"\A[^=]+[=]+[^=]+\z") || !(Regex.IsMatch(word, @"[=]{3,}")))
                                {
                                    tokenizedText.Add(word);
                                }
                                else
                                {
                                    var new_words = Regex.Split(word, @"([=]{3,})");
                                    foreach (var new_word in new_words)
                                    {
                                        if (!String.IsNullOrWhiteSpace(word) && !String.IsNullOrEmpty(word))
                                        {
                                            tokenizedText.Add(new_word);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch
                    {
                        //log enmpty string
                    }
                    finally
                    {
                        progressBar1.PerformStep();
                    }
                }

                foreach (var word in tokenizedText)
                {
                    richTextBox2.AppendText(word + " ");

                }
                MessageBox.Show("Токенизация завершена", "Сообщение программы", MessageBoxButtons.OK, MessageBoxIcon.Information);
                text_is_tokenized = true;
            }
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            if (text_is_tokenized)
            {
                var wordArray = richTextBox2.Text.Split(' ');
                richTextBox2.Clear();
                foreach (var word in wordArray)
                {
                    if (Regex.IsMatch(word, @"(\.|:\.|\.:\.|:\.\.\.:|\.,\.|∽|,|:\.:|: \.|: -|:-|::|:)"))
                    {
                        richTextBox2.AppendText(word + "\n");
                    }
                    else
                    {
                        richTextBox2.AppendText(word.Trim() + " ");
                    }
                }
                MessageBox.Show("Текст разбит на клаузы", "Сообщение программы", MessageBoxButtons.OK, MessageBoxIcon.Information);
                text_is_ready = true;
            }
            else
            {
                MessageBox.Show("Токенизация не завершена!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
    }
}