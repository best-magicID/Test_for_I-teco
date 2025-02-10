using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Mail;
using System.Net;
using System.IO;

namespace Test_for_I_teco
{
    public partial class FormTest : Form
    {

        public int timeLeft; //оставшееся время
        string email1;  //емайл с первой формы
        string fIO1;    // передача с 1 формы ФИО
        string[] text_iz_file1; // передача вопросов с 1 формы
        public string name_texta_for_test;
 

        public FormTest(string email, string fIO, string[] text_iz_file)
        {
            InitializeComponent();

            label2.Text = "";
            timer1.Start();
            int countMin = 10; // количество минут на решение теста
            timeLeft = countMin * 60; // перевод в секунды
            timer1.Interval = 1000;
            email1 = email;
            fIO1 = fIO;
            text_iz_file1 = text_iz_file;
        }


        //при загрузке формы загружает из файла текст и раскидывает по форме
        private void FormTest_Load(object sender, EventArgs e)
        {
            try
            {
                //название файла для чтения
                //name_texta_for_test = "text.txt";

                //активация кнопки, для чтения файла
                button2_Click(sender, e);

                //вывод оставшегося количества вопросов
                int осталось_вопросв = tabControl1.TabCount;
                label18.Text = "Осталось вопросов " + осталось_вопросв;


                //При запуске формы редактирует ширину и высоту label
                //перебор tabPage на tabControl1

                foreach (TabPage tabPage in tabControl1.TabPages.OfType<TabPage>())
                {

                    //перебор label на tabPage
                    foreach (Label label in tabPage.Controls.OfType<Label>())
                    {
                        // label.MaximumSize = new Size(650, 300);
                        label.MaximumSize = new Size(tabControl1.Width - 30, 500);
                    }
                }

            }
            catch(Exception ex )
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK);
            }
      
        }



        //настройки таймера
        private void timer1_Tick(object sender, EventArgs e)
        {

            //вывод таймера и его уменьшение
            timeLeft = timeLeft - 1;
            label2.Text = Convert.ToString(timeLeft);

            //цифры становятся красными, если меньше 100
            if (timeLeft == 100)
                label2.ForeColor = Color.Red;


            //завершение теста
            //отправление писем и вывод результата
            if (timeLeft < 1)
            {
                try
                {
                    timer1.Stop();

                    //чтение данныих
                    button1_Click(sender, e);


                    //сбор всех вопросов в одну переменную для вывода в почте
                    string только_вопросы = "";

                    for (int i = 0; i < lines_iz_file.Count; i++)
                    {
                        if (lines_iz_file.Find(x => x.Type_response == "вопрос " + i) == null)
                            break;

                        только_вопросы += i + 1 + ") " + lines_iz_file.Find(x => x.Type_response == "вопрос " + i.ToString()).Text_line_file + "\n";

                    }

                    //вывод в программе результат теста
                    MessageBox.Show(fIO1 + "\n\n" +
                        "Тест закончен.\n\nКоличество правильных ответов: "+ кол_прав_ответов + "" +
                        "\nКоличество неправильных ответов: " + кол_НеПрав_ответов + "\n ", 
                        "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    //Отправление письма

                    SmtpClient smtp = new SmtpClient("smtp.yandex.ru", 587);
                    smtp.Credentials = new NetworkCredential("test.irovshik@yandex.ru", "zowlsrqcdybpuhsv");

                    MailMessage mail = new MailMessage();
                    mail.From = new MailAddress("test.irovshik@yandex.ru", "I-teco");
                    mail.To.Add(new MailAddress("polchihin@yandex.ru"));
                    //доп адрес с 1 страницы
                    mail.To.Add(new MailAddress(email1));
                    mail.Subject = "Результат вашего тестирования";
                    mail.Body = "Добрый день!\n\n" +
                        "ФИО проходившего тестирование:\n" + fIO1 + 
                        "\n\nПочта пользователя, проходившего тестирование\n" + email1 + 
                        "\n\nКоличество правильных ответов: " + кол_прав_ответов + "" +
                        "\nКоличество неправильных ответов: " + кол_НеПрав_ответов + "\n\n" +
                        "Вопросы из теста:\n" + только_вопросы;

                    smtp.EnableSsl = true;

                    try
                    {
                        smtp.Send(mail);
                        MessageBox.Show("Письмо отправлено!", "Внимание", MessageBoxButtons.OK);

                        //выход из программы по завершению теста
                        Application.Exit();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
           
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                

            }


        }//end Void


        //подсчет количества правильных ответов
        public int кол_прав_ответов;
        public int кол_НеПрав_ответов;


        //кнопка, завершить тест
        private void button1_Click(object sender, EventArgs e)
        {
            timeLeft = 1; //сбрасывает таймер до нуля


            //количество выбранных ячеек
            int количеств_выбран_ответов = 0;

            List<выбранны_ответ> list_checkBoxs = new List<выбранны_ответ>();

            foreach (TabPage tabPage in tabControl1.Controls.OfType<TabPage>())
            {
                foreach (CheckBox checkBox in tabPage.Controls.OfType<CheckBox>())
                {
                    if(checkBox.Checked == true)
                    {
                        количеств_выбран_ответов++;

                        list_checkBoxs.Add(new выбранны_ответ { Id_line = количеств_выбран_ответов, Name_line = checkBox.Text });
                    }
                }
            }

            //подсчет кол-ва правильных ответов
            кол_прав_ответов = 0;

            for (int i = 0; i < lines_iz_file.Count; i++) // все элементы
            {
                for (int p = 0; p < list_checkBoxs.Count; p++) // только выбранные
                {
                    // var sdfg = "+" + list_checkBoxs[p].Name_line.Substring(1);

                    if (lines_iz_file[i].Text_line_file.Contains("+" + list_checkBoxs[p].Name_line.Substring(1)) == true) //сравнение всех элементов с выбранным
                    {
                        кол_прав_ответов++;
                    }
                }
            }

            кол_НеПрав_ответов = количеств_выбран_ответов - кол_прав_ответов;

        } //end Void


        //переменные для локации вопроса 
        public int label_x;
        public int label_y;

        //переменные для локации 1 ответа 
        public int checkBox_x1;
        public int checkBox_y1;
        public int checkBox_x2;
        public int checkBox_y2;
        public int checkBox_x3;
        public int checkBox_y3;

        //создание листа для добавления данных из текстового файлв
        public List<Line_iz_file> lines_iz_file = new List<Line_iz_file>();


        //чтение из файла
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                // * не использую, можно удалить (скачивает текстовый файл с информацией о HTML странице)
                ///скачивание файла из сети
                #region download
                //string remoteUri = "https://docviewer.yandex.ru/view/87073921/?*=SCMkYEGvNo3U%2BA7mpsP2muKUI%2BB7InVybCI6InlhLWRpc2s6Ly8vZGlzay%2FQkNC50YLQtdC60L4v0JLQtdGA0YHQuNC4INC%2F0YDQvtCz0YDQsNC80LzRiyBcItCi0LXRgdGC0LjRgNC%2B0LLQsNC90LjQtVwiL3RleHQudHh0IiwidGl0bGUiOiJ0ZXh0LnR4dCIsIm5vaWZyYW1lIjpmYWxzZSwidWlkIjoiODcwNzM5MjEiLCJ0cyI6MTY1NzIwMTQ5NjQ0MSwieXUiOiI1Nzg1OTA1ODUxNjQxNjgyMDMwIn0%3D/";
                //string fileName = "text.txt", myStringWebResource = null;

                //WebClient myWebClient = new WebClient();

                //myStringWebResource = remoteUri + fileName;


                //myWebClient.DownloadFile(myStringWebResource, fileName);
                //String text = File.ReadAllText(fileName);
                # endregion




                //расположение кнопок и ответов
                label_x = 14;       
                label_y = 22;

                checkBox_x1 = 18;   
                //checkBox_y1 = 54;
                checkBox_y1 = 84;

                checkBox_x2 = 18;   
                //checkBox_y2 = 104;
                checkBox_y2 = 114;

                checkBox_x3 = 18;   
                //checkBox_y3 = 114;
                checkBox_y3 = 144;

                //шрифт вопросов и ответов
                var Font = new Font("Microsoft Sans Serif", 12, FontStyle.Regular);


                if(File.Exists(name_texta_for_test) == true || text_iz_file1.Count() > 4)
                {
                    //в данном случае проверка . есть ли файл, вдруг он не удалился
                    //через секцию, будет считывание из массива, которые нам нужно
                    if (File.Exists(name_texta_for_test) == true)
                    {

                        //string x = Directory.GetCurrentDirectory(); // получение пути запускаемого файла * не используется

                        string[] lines = File.ReadAllLines(name_texta_for_test); // чтение из файла

                        int счетчик = 0;

                        //добавление в лист и очистка от пустых строк
                        foreach (string item in text_iz_file1)
                        {
                            if (item.Count() < 1)
                            {

                            }
                            else if (item.Count() > 4)
                            {
                                lines_iz_file.Add(new Line_iz_file() { Id_line_file = счетчик, Text_line_file = item });

                                счетчик++;
                            }
                            else
                            {
                                MessageBox.Show("Вопросы не загружены", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                break;
                            }
                        }
                    }
                    else
                    {
                        int счетчик = 0;

                        foreach (string item in text_iz_file1)
                        {
                            if (text_iz_file1.Count() >= 4)
                            {
                                lines_iz_file.Add(new Line_iz_file() { Id_line_file = счетчик, Text_line_file = item });

                                счетчик++;
                            }
                            else
                            {
                                MessageBox.Show("Вопросы не загружены", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                break;
                            }
                        }
                    }
            


                    int номер_вопроса = 0;

                    //поиск строк с 3 точками
                    for (int i = 0; i < lines_iz_file.Count; i++)
                    {

                        ///Важно: поиск строки с 3 звездочками, пример: ***

                        if (lines_iz_file[i].Text_line_file.ToString().Contains("***") == true)
                        {
                            
                            lines_iz_file[i + 1].Type_response = "вопрос " + номер_вопроса;

                            lines_iz_file[i + 2].Type_response = "ответ 1, вопрос " + номер_вопроса;
                            lines_iz_file[i + 3].Type_response = "ответ 2, вопрос " + номер_вопроса;
                            lines_iz_file[i + 4].Type_response = "ответ 3, вопрос " + номер_вопроса;

                            номер_вопроса++;
                        }
                    }

                    try
                    {
                        //удаление лишних tabpage
                        int колич_лишних_вкладок = tabControl1.TabPages.Count - номер_вопроса;

                        if (колич_лишних_вкладок > 1)
                        {
                            for (int i = 0; i < колич_лишних_вкладок; i++)
                            {
                                tabControl1.TabPages.RemoveAt(tabControl1.TabPages.Count - колич_лишних_вкладок);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString(), "Ошибка удаления вкладок", MessageBoxButtons.OK);
                    }



                    //Изменение текста на форме
                    int number_вопроса = 0;

                    foreach (TabPage tabPage in tabControl1.TabPages.OfType<TabPage>()) //перебор tabPage на tabControl1
                    {
                        
                        foreach (Label label in tabPage.Controls.OfType<Label>()) //перебор label на tabPage
                        {
                            //расположение вопроса
                            label.Location = new Point(label_x, label_y);

                            // label.MaximumSize = new Size(650, 300);
                            //label.MaximumSize = new Size(tabControl1.Width - 50, 300);
                            //label.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Left);

                            //шрифт вопроса
                            label.Font = Font;

                            //поиск теста вопроса
                            var line = lines_iz_file.Find(p => p.Type_response == "вопрос " + number_вопроса);
                            label.Text = line.Text_line_file;
                        }

                        int number_checkBox = 1;

                        foreach (CheckBox checkBox in tabPage.Controls.OfType<CheckBox>()) //перебор checkBox на tabPage
                        {
                            switch (number_checkBox)
                            {
                                //1 ответ
                                case 1:
                                    //расположение 1 ответа
                                    checkBox.Location = new Point(checkBox_x1, checkBox_y1);

                                    //стиль шрифта 1 ответа
                                    checkBox.Font = Font;


                                    var text_checkBox1 = lines_iz_file.Find(xx => xx.Type_response == "ответ 1, вопрос " + number_вопроса);
                                    checkBox.Text = text_checkBox1.Text_line_file.Replace('-', ' ').Replace('+', ' ');

                                    break;

                                //2 ответ
                                case 2:
                                    checkBox.Location = new Point(checkBox_x2, checkBox_y2);

                                    checkBox.Font = Font;

                                    var text_checkBox2 = lines_iz_file.Find(xx => xx.Type_response == "ответ 2, вопрос " + number_вопроса);
                                    checkBox.Text = text_checkBox2.Text_line_file.Replace('-', ' ').Replace('+', ' ');

                                    break;

                                //3 ответ
                                case 3:
                                    checkBox.Location = new Point(checkBox_x3, checkBox_y3);

                                    checkBox.Font = Font;

                                    //checkBox.MaximumSize = new Size(tabPage.Width - 20, 500);


                                    var text_checkBox3 = lines_iz_file.Find(xx => xx.Type_response == "ответ 3, вопрос " + number_вопроса);
                                    checkBox.Text = text_checkBox3.Text_line_file.Replace('-', ' ').Replace('+', ' ');

                                    number_checkBox = 1;
                                    break;
                            }

                            number_checkBox++;

                        }
                        number_вопроса++;
                    }

                    int number_question = 0;

                    //замена названий tabPage и нетолько
                    foreach (TabPage tabPage in tabControl1.TabPages)
                    {
                        tabPage.Text = number_question + 1  + " вопрос";
                        number_question++;

                    }

                }
    
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        } //конец Void

        

        //кнопка, следующий вопрос
        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                if (tabControl1.SelectedIndex == (tabControl1.TabPages.Count - 1))
                    tabControl1.SelectedTab = tabPage1;
                else
                    tabControl1.SelectedIndex++;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }


        //кнопка, предыдущий вопрос
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (tabControl1.SelectedTab == tabPage1)
                    tabControl1.SelectedIndex = tabControl1.TabPages.Count - 1;
                else
                    tabControl1.SelectedIndex--;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        //переключение стрелками * отключено
        private void tabControl1_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.KeyCode == Keys.Left)
            //    button3_Click(sender, e);

            //if (e.KeyCode == Keys.Right)
            //    button4_Click(sender, e);
        }


        //При закрытии формы, закрывается приложение
        //Если убрать, то после закрытия, формы с вопросами, приложение будет работать в фоне!!!
        private void FormTest_FormClosing(object sender, FormClosingEventArgs e)
        {

            Application.Exit();
        }


        //изменение размера текста в зависимости от размера окна
        private void tabControl1_Resize(object sender, EventArgs e)
        {
            try
            {
                int x = label3.Height + label3.Location.Y;

                //перебор tabPage на tabControl1
                foreach (TabPage tabPage in tabControl1.TabPages.OfType<TabPage>())
                {

                    //перебор label на tabPage
                    foreach (Label label in tabPage.Controls.OfType<Label>())
                    {
                        label.MaximumSize = new Size(tabControl1.Width - 30, 300);
                    }

                    //перебор label на tabPage для изменения местоположения
                    foreach (CheckBox checkBox in tabPage.Controls.OfType<CheckBox>())
                    {
                        //checkBox.Top = label1.Bottom + 80;
                        //checkBox.Top = checkBox.Top + x;
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK);
            }

            


        }//end void



        ///событие возникающее после смены вкладки
        ///Показывает, сколько осталось вопросов
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

            ///вывод оставшегося количества вопросов
            //изменение цвета заголовка, уже пройденных вопросов * не работает
            try
            {
                //лист с названиями tabPage где выделен ответ
                List<Cheched_tabPage> List_cheched_tabPage = new List<Cheched_tabPage>();


                int number_vkladki = 0;
                int осталось_вопросв = tabControl1.TabCount;


                foreach (TabPage tabPage in tabControl1.TabPages.OfType<TabPage>())
                {

                    foreach (CheckBox checkBox in tabPage.Controls.OfType<CheckBox>())
                    {
                        if (checkBox.Checked == true)
                        {


                            var nameTabPage = List_cheched_tabPage.Find(x => x.Name_tabPage.Contains("tabPage.Name"));

                            if (nameTabPage == null)
                            {
                                number_vkladki++;

                                List_cheched_tabPage.Add(new Cheched_tabPage { ID_tabPage = number_vkladki, Name_tabPage = tabPage.Name });

                                осталось_вопросв--;
                                label18.Text = "Осталось вопросов " + осталось_вопросв.ToString();

                                break;
                            }

                            //tabPage.ForeColor = Color.Green; //изменяет цвет текста
                            //tabPage.BackColor = Color.Green; // изменяет фон tabPage
                            //tabPage.Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold);
                        }
                    }
                }

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK);
            }
            
        }





    }
}//end code
