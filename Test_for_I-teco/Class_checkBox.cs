using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_for_I_teco
{
    public class выбранны_ответ
    {
        public int Id_line { get; set; }

        public string Name_line { get; set; }

    }

    public class Line_iz_file
    {
        public int Id_line_file { get; set; }

        public string Type_response { get; set; }

        public string Text_line_file { get; set; }

    }

    public class Cheched_tabPage
    {
        public int ID_tabPage { get; set; }

        public string Name_tabPage { get; set; }
    }

    public class Строка_из_Excel
    {
        public int Id_строки { get; set; }
        public string Text_вопроса { get; set; }
        public string Ответ_1 { get; set; }
        public string Ответ_2 { get; set; }
        public string Ответ_3 { get; set; }
    }

}//end class
