using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace OldSlavonicCorpusPreprocessing
{

    [Table(Name = "Text")]
    public class Text
    {        
        
        [Column(IsPrimaryKey = true)]
        public int Id { get; set; }
        [Column(Name = "TextName")]
        public string textName { get; set; }
        [Column(Name = "TextURL")]
        public string textURL { get; set; }
    }
    /*
    class Text
    {
        private string textContents;
        public int textURL { get; set; }
        public string textName { get; set; }
        private int textID;
        private string textName;
        private string textURL;
        public Text(string name, string URL, string contents, string servername, string catalogue)
        {
            textCount = SetCount();
            textID = textCount;
            textName = name;
            textURL = URL;
            textContents = contents;
            TextIntoDB(servername, catalogue);
        }

        private int SetCount()
        {
            int previousCount = GetCount();
            return ++previousCount;
        }

        private int GetCount()
        {
            return 0 | textCount;
        }

        public string OutputTextSettings()
        {
            return "В базе " + textCount + " текстов. ID текущего текста - " + textID + ". Имя текущего текста - " + textName + ". Текст находится по ссылке: " + textURL + ".";
        }

       private void TextIntoDB(string servername, string catalogue)
        {
            SqlConnection con;
            con = new SqlConnection("server=" + servername + "; Initial Catalog = " + catalogue + "; Integrated Security = SSPI");
            con.Open();
            string query = "INSERT INTO [dbo].[Text] (id, TextName, TextURL) VALUES (" + textID + ", '" + textName + "', '" + textURL + "')";
            // объект для выполнения SQL-запроса
            SqlCommand command = new SqlCommand(query, con);
            // выполняем запрос
            command.ExecuteNonQuery();
            // закрываем подключение к БД
            con.Close();
        }

    }*/
}
