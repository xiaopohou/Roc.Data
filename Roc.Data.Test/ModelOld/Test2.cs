using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Roc.Data.Test
{
    [Table("aaa")]
    public class Test2
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string FullName { get; set; }

        public DateTime AddTime { get; set; }

        public string Other { get; set; }

        public Test2()
        {
            this.AddTime = DateTime.Now;
        }
    }
}
