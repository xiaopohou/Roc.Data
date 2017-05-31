using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Roc.Data.Test
{
    public class Item
    {
        public string F_Id { get; set; }

        public string F_ParentId { get; set; }

        public string F_EnCode { get; set; }

        [Column("F_FullName")]
        public string FullName { get; set; }

        public IEnumerable<ItemDetail> Details { get; set; }

        public Item()
        {
            //this.Details = new List<ItemDetail>();
        }
    }

    public class ItemDetail
    {
        public string F_Id { get; set; }

        public string F_ItemId { get; set; }

        public string F_ParentId { get; set; }

        public string F_ItemCode { get; set; }

        public string F_ItemName { get; set; }
    }
}
