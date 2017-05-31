using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Roc.Data
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ActionAttribute : Attribute
    {
        public ActionAttribute(ActionType type)
        {
            this.ActionType = type;
        }

        public ActionAttribute()
            : this(ActionType.ReadOrWrite)
        {

        }

        public ActionType ActionType { get; set; }
    }
}
