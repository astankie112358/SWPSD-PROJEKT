using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWPSD_PROJEKT
{
    public class Book
    {
        public int id { get; set; }
        public string title { get; set; }
        public string author { get; set; }
        public List<string> types = new List<string>();
        public string typesview { get; set; }
        public string description { get; set; }
        public Book(int id, string name, string author, string description)
        {
            this.id = id;
            this.title = name;
            this.author = author;
            this.description = description;
            typesview = " ";
        }

    }

}
