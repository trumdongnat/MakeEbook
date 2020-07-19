using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeEbook.Models
{
    public class Volumn
    {
        public int Index { get; set; }
        public string Name { get; set; }
        public List<Chapter> Chapters { get; set; }
    }
}
