using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AsyncForm
{
    //Создать приложение, которое асинхронно скачивает картинки с любого сайта (пример http://demotivators.to/) Необходимо скачать 100 картинок и остановиться.
    //Максимальное число одновременных скачиваний 5
    //После загрузки картинки если ещё не 100, то начинать загрузку новой
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        const int countImg = 100;
        static MatchCollection Matches;

        #region  многопоточность не осилил
        class myThread
        {
            Thread thread;

            public myThread(string name, int pos)
            {
                thread = new Thread(this.PreRun);
                thread.Name = name;
                thread.Start(pos);
            }

            async void PreRun(object pos)
            {
                //await Run((int)pos); //Cannot access a non-static member of outer type 'Async Form.Form1' via nested type 'Async Form.Form1.myThread
            }
        }
        #endregion

        private async void button1_Click(object sender, EventArgs e)
        {          
            int thisCount = 0; // количество просмотренных картинок
            int page = 1; // страница сайта
            do
            {
                // Вытащим всю разметку веб страницы и через регулярное выражение все ссылки картинок
                string url = string.Empty;
                if (page == 1)                
                    url = "http://demotivators.to/";
                else
                    url = "http://demotivators.to/?page=" + page.ToString();
                string s = string.Empty;
                HttpWebRequest myRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
                StreamReader sr = new StreamReader(myResponse.GetResponseStream(), Encoding.GetEncoding(1251));
                s = sr.ReadToEnd();

                Matches = Regex.Matches(s, "<img.+?src=[\"'](.+?)[\"'].*?>", RegexOptions.IgnoreCase);

                for (int i = 0; i < Matches.Count; i++)
                {
                    if(!Matches[i].Groups[1].Value.Contains("/media/posters")) continue; // отсеем лишнее
                    if (thisCount > countImg) break;

                    #region  многопоточность не осилил
                    //myThread t1 = new myThread("Thread 1", i);
                    //thisCount++; i++;
                    //myThread t2 = new myThread("Thread 2", i);
                    //thisCount++; i++;
                    //myThread t3 = new myThread("Thread 3", i);
                    //thisCount++; i++;
                    //myThread t4 = new myThread("Thread 4", i);
                    //thisCount++; i++;
                    //myThread t5 = new myThread("Thread 5", i);
                    #endregion

                    thisCount++;
                    await Run((int)i);
                }
                page++;
            }
            while (thisCount < countImg);
        }

        private async Task Run(int pos)
        {
            await Task.Run(async () =>
            {
                pictureBox1.WaitOnLoad = false;
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                pictureBox1.LoadAsync(@"http://demotivators.to/" + Matches[pos].Groups[1].Value);
            });
            Task.Delay(500).Wait(); // Подождем
        }
        
    }
}
