using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Roc.Data
{
    internal enum SqlTextType
    {
        None = 0,
        Query = 100,
        QueryPage = 101,
        Insert = 200,
        Update = 300,
        Delete = 400,
        Truncate = 401
    }

    internal enum SqlPartType
    {
        None = 0,
        Section = 100,
        SectionUpdate = 101,
        Where = 200,
        OrderBy = 300,
        GroupBy = 400,
        Having = 500,
        Function = 600
    }

    internal enum SqlOrderByType
    {
        ASC = 1,
        DESC = 2
    }

    public enum SqlLikeType
    {
        StartsWith = 100,
        EndsWith,
        Contains,
        Equals
    }
}

namespace Roc.Data
{
    public enum SqlJoinType
    {
        LEFT = 1,
        RIGHT = 2,
        INNER = 3
    }

    public enum SqlFunctionType
    {
        NONE = 0,
        COUNT = 100,
        //DISTINCT,
        SUM,
        MIN,
        MAX,
        AVG
    }

    public enum SqlInType
    {
        IN = 1,
        NOTIN = 2
    }
}
