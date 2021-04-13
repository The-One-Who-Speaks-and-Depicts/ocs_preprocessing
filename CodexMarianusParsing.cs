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
using System.Diagnostics;
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
            Text = "Обучение на CoNLL-U данных";
        }


        private void Button3_Click(object sender, EventArgs e)
        {
            bool res = false;
            string train_path = "";
            do
            {
                MessageBox.Show("Укажите тренировочный файл для модели, проводящей частеречную разметку", "Сообщение программы");
                var train_open_dialogue = new OpenFileDialog();
                DialogResult train_open_dialogue_result = train_open_dialogue.ShowDialog();
                if (train_open_dialogue_result == DialogResult.OK)
                {
                    train_path = train_open_dialogue.FileName;
                    res = true;
                }
            }
            while (!res);

            res = false;
            string file_path = "";
            do
            {
                MessageBox.Show("Укажите файл с кодом Python, нацеленный на обучение модели, проводящей частеречную разметку", "Сообщение программы");
                var py_open_dialogue = new OpenFileDialog();
                DialogResult py_open_dialogue_result = py_open_dialogue.ShowDialog();
                if (py_open_dialogue_result == DialogResult.OK)
                {
                    file_path = py_open_dialogue.FileName;
                    res = true;
                }
            }
            while (!res);

            MessageBox.Show("Начато обучение НММ-модели.", "Сообщение программы");
            string command = "python " + file_path + " --data " + train_path;
            var processInfo = new ProcessStartInfo("cmd.exe", "/c " + command);
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            processInfo.RedirectStandardError = true;
            processInfo.RedirectStandardOutput = true;
            var process = Process.Start(processInfo);
            process.WaitForExit();
            process.Close();
            MessageBox.Show("НММ-модель обучена и сохранена в директории Python-файла. Идёт обучение n-gram-модели.", "Сообщение программы");


            command = "python " + file_path + " --data " + train_path + " --method grams";
            processInfo = new ProcessStartInfo("cmd.exe", "/c " + command);
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            processInfo.RedirectStandardError = true;
            processInfo.RedirectStandardOutput = true;
            process = Process.Start(processInfo);
            process.WaitForExit();
            process.Close();
            MessageBox.Show("N-gram-модель обучена и сохранена в директории Python-файла", "Сообщение программы");
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            bool res = false;
            string train_path = "";
            do
            {
                MessageBox.Show("Укажите тренировочный файл для модели, проводящей лемматизацию", "Сообщение программы");
                var train_open_dialogue = new OpenFileDialog();
                DialogResult train_open_dialogue_result = train_open_dialogue.ShowDialog();
                if (train_open_dialogue_result == DialogResult.OK)
                {
                    train_path = train_open_dialogue.FileName;
                    res = true;
                }
            }
            while (!res);

            res = false;
            string file_path = "";
            do
            {
                MessageBox.Show("Укажите файл с кодом Python для обучения лемматизирующей модели", "Сообщение программы");
                var py_open_dialogue = new OpenFileDialog();
                DialogResult py_open_dialogue_result = py_open_dialogue.ShowDialog();
                if (py_open_dialogue_result == DialogResult.OK)
                {
                    file_path = py_open_dialogue.FileName;
                    res = true;
                }
            }
            while (!res);

            string command = "python " + file_path + " --data " + train_path;
            var processInfo = new ProcessStartInfo("cmd.exe", "/c " + command);
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            processInfo.RedirectStandardError = true;
            processInfo.RedirectStandardOutput = true;
            var process = Process.Start(processInfo);
            process.WaitForExit();
            process.Close();
            MessageBox.Show("Модель обучена и сохранена в директории Python-файла", "Сообщение программы");
        }
    }
}