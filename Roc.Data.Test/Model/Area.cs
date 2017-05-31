using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Roc.Data.Test.Model
{
    [Table("T_Area")]
    public class Area
    {
        [Key(true)]
        public int Id { get; set; }

        [Key]
        public string AreaId { get; set; }

        [Key]
        public string ParentId { get; set; }

        public string AreaCode { get; set; }

        [Column("Area_Name")]
        public string AreaName { get; set; }

        public string QuickQuery { get; set; }

        [Ignore]
        public int Layer { get; set; }
    }
}
