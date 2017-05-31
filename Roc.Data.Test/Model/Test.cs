using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Roc.Data.Test
{
    public class Test
    {
        public Test()
        {
            F_DateTime = DateTime.Now;
        }
        /// <summary>
        ///Id
        /// </summary>
        [Key(true)]
        public int Id { get; set; }
        /// <summary>
        ///F_Byte
        /// </summary>
        public byte F_Byte { get; set; }
        /// <summary>
        ///F_Int16
        /// </summary>
        public int F_Int16 { get; set; }
        /// <summary>
        ///F_Int32
        /// </summary>
        public int F_Int32 { get; set; }
        /// <summary>
        ///F_Int64
        /// </summary>
        public long F_Int64 { get; set; }
        /// <summary>
        ///F_Double
        /// </summary>
        public double F_Double { get; set; }
        /// <summary>
        ///F_Float
        /// </summary>
        public float F_Float { get; set; }
        /// <summary>
        ///F_Decimal
        /// </summary>
        public decimal F_Decimal { get; set; }
        /// <summary>
        ///F_Bool
        /// </summary>
        public bool F_Bool { get; set; }
        /// <summary>
        ///F_DateTime
        /// </summary>
        public DateTime F_DateTime { get; set; }
        /// <summary>
        ///F_Guid
        /// </summary>
        public Guid F_Guid { get; set; }
        /// <summary>
        ///F_String
        /// </summary>
        public string F_String { get; set; }
    }
}
