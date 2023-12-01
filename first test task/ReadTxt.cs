using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace first_test_task
{
    internal class ReadTxt
    {
        public string[] Text;
        public string[] Name;
        public string[] Dosage;
        public string[] Number;
        public string[] Fabricator;
        public string[] Country;

        public ReadTxt(string path)
        {
            using (FileStream fstream = File.OpenRead(path))
            {
                // выделяем массив для считывания данных из файла
                byte[] buffer = new byte[fstream.Length];
                // считываем данные
                fstream.ReadAsync(buffer, 0, buffer.Length);
                // декодируем байты в строку
                string textFromFile = Encoding.Default.GetString(buffer);
                Console.WriteLine($"Текст из файла: {textFromFile}");
                this.Text = new string[textFromFile.Length];
                this.Name = new string[textFromFile.Length];
                this.Dosage = new string[textFromFile.Length];
                this.Fabricator = new string[textFromFile.Length];
                this.Country = new string[textFromFile.Length];
                int i = 0;
                foreach (string line in textFromFile.Split("\n"))
                {
                    this.Text[i++] = line;
                    // Разбиваем строку по символам  
                    //string regex = "(.*%)|(.*),\\s((\\d.\\d*|\\d*) мг|мл)\\s(.\\s\\d*)\\s(.*\\sООО|ОАО|АО|РУП|ЛЛс|ГмбХ|ЗАО)\\s\\((.*)\\)";
                    //string[] result = Regex.Split(line, regex,RegexOptions.IgnoreCase);
                    //this.Name[i++] = result[i-1];
                }

            }
        }
    }
}
