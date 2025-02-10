using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YandexDisk.Client.Http;
using YandexDisk.Client.Protocol;
using YandexDisk.Client.Clients;

namespace Test_for_I_teco
{
    public partial class StartForm : Form
    {
        public StartForm()
        {
            InitializeComponent();

            //создание элементов на вкладке
            //    CheckBox checkBox = new CheckBox();
            //    checkBox.Name = "checkBox1";
            //    checkBox.Text = "Первый вопрос";
            //    checkBox.Location = new Point(50, 50);
            //    checkBox.Visible = true;

            //    Controls.Add(checkBox);
        }


        //кнопка, запуск теста
        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                //Проверка на минимальное кол-вао симоволов
                if (textBox2.Text == "" || textBox2.Text.Count() < 5 || textBox2.Text == "Введите ваше ФИО: Кудрин Илья Сергеевич")
                {
                    MessageBox.Show("Вы не ввели ФИО!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }


                ///
                ///после проверки при запуске, загружает выбранный файл
                ///api указывается при запуске программы
                ///
                ///Для работы нужно:
                //using YandexDisk.Client.Http;
                //using YandexDisk.Client.Protocol;
                //using YandexDisk.Client.Clients;

                //загрузка списка файлов на яндексе диске, в указанной папке
                var dataFolder = await api.MetaInfo.GetInfoAsync(new ResourceRequest
                {
                    Path = "/i-teco/"
                });



                //название файла 
                string name_file = "text.txt";

                ///Разделение, если пользователь выбрал название файла с тестом в окне
                if (textBox3.SelectedText == "")
                {
                    name_file = "text.txt";
                }
                else
                {
                    //разделитель для копирования названий в массив
                    string[] separator = { "\r\n" };

                    //добавление в массив названий из TextBox
                    string[] arr_lines = textBox3.Text.Split(separator, System.StringSplitOptions.RemoveEmptyEntries);

                    //выделенный текст
                    //в начале текса остается \n, его убирать не нужно, так как в массиве он такой
                    string select_iz_textBox = textBox3.SelectedText.Replace("\n\r", "");

                    foreach (var item in arr_lines)
                    {
                        if (select_iz_textBox == item)
                        {
                            name_file = item.ToString();
                            break;
                        }
                    }

                }


                //путь для сохранения файла
                var distDir = Path.Combine(Environment.CurrentDirectory, "");

                ///Проверяет, есть ли файл на ПК
                ///Если он есть, удаляет, во избежания подмены файла
                if (File.Exists(name_file) == true)
                {
                    //путь для удаления файла
                    string new_path1 = distDir + "\\" + name_file;

                    //удаление файла
                    File.Delete(new_path1);

                    //text_iz_file = File.ReadAllLines(name_file); // чтение из файла и запись данных в массив
                }

                //если фйла нет, то скачивает его из облака
                if (!File.Exists(name_file))
                {
                    //скачивание файлов из списка с Яндекс диска
                    //await api.Files.DownloadFileAsync(path: dataFolder.Path, Path.Combine(distDir, name_file));

                    //скачивание файлов из списка с Яндекс диска * скачивает все файлы из папки, не подходит
                    foreach (var item in dataFolder.Embedded.Items)
                    {
                        if(item.Name == name_file)
                        {
                            await api.Files.DownloadFileAsync(path: item.Path, Path.Combine(distDir, item.Name));
                            break;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Файл не найден", "Ошибка", MessageBoxButtons.OK);
                    //return;
                }

    


                //проверка почты на минимальное кол-вао символов
                int countSimbol = textBox1.Text.Count();
                if (countSimbol <= 5)
                {
                    MessageBox.Show("Введите почту.\nМинимальное количество символов " + 5 + ".", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                    MessageBox.Show("Если вы закроете окно с вопросами, \nтест автоматически закроется и текущий результат отправится вам на почту.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                    text_iz_file = File.ReadAllLines(name_file);

                    string email = textBox1.Text;
                    string fIO = textBox2.Text;

                    //открытие формы с тестом и скрытие старой формы
                    Form FormTest = new FormTest(email, fIO, text_iz_file);
                    this.Hide();
                    FormTest.ShowDialog();

                }

                //путь для удаления файла
                string new_path = distDir + "\\" + name_file;

                //удаление файла
                File.Delete(new_path);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);


            }
     
        }//end Void


        //выпадающее меню, кнопка о программе
        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Разработал данную программу:\nПолчихин Андрей А.\nООО \"Витте Про\"\n\nВерсия 1.8\n\n                         2022 год", "Разработчик", MessageBoxButtons.OK);
        }


        //Кнопка, проверка почты
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                // проверка корректности введеной почты
                string email = textBox1.Text;

                var addr = new System.Net.Mail.MailAddress(email);
                MessageBox.Show("Введен корректный E-mail", "Информация", MessageBoxButtons.OK);
            }
            catch
            {
                MessageBox.Show("Введен некорректный E-mail", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            //return addr.Address == email;
        }


        //очистка поля ввода ФИО
        private void textBox2_MouseClick(object sender, MouseEventArgs e)
        {
            if (textBox2.Text == "Введите ваше ФИО: Кудрин Илья Сергеевич")
            {
                textBox2.Text = "";
            }
        }


        //проверка емаил
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                button2_Click(sender, e);

        }


        //при нажатии Enter, фокус переходит на кнопку старт 
        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                button_start.Focus();
        }


        //массив для записи вопросов из файла
        public string[] text_iz_file;

        ///API это токен с помощью которого подключается к яндекс диску
        ///создается через сайт
        public DiskHttpApi api;


        ///при запуске программма будет 
        ///проверяет, есть ли файл конфиг * не сделано
        ///скачивает файл с тестом и удаляет его после прочтения
        private async void StartForm_Load(object sender, EventArgs e)
        {
            try
            {

                //токен, регистрируется на сайте
                api = new DiskHttpApi("AQAAAAA21AM2AAgLWLS8bkNUTEYUhH5yULFY_wc");

                //сканирует папку и загружает данные в переменную
                var dataFolder = await api.MetaInfo.GetInfoAsync(new ResourceRequest
                {
                    Path = "/i-teco/"
                });

                //вывод списка файлов из облака
                foreach (var item in dataFolder.Embedded.Items)
                {
                    //textBox3.Text += "\n" + item.Name + "\t" + item.Type + "\t" + item.MimeType + "\n";
                    textBox3.Text += item.Name + "\r\n";
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Ошибка", MessageBoxButtons.OK);
            }


        }


        //Вывод информации о некорректности ввода информации
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (textBox2.Text.Length < 5)
            {
                label4.Visible = true;
                label4.Text = "Минимальная длинна текста 5 символов";
            }
            else
            {
                label4.Visible = false;
            }
        }
    }
}
