using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parsnet
{
    public partial class Users
    {
        public int TotalSites { get; set; }
        public int TotalScores { get; set; }
    }

    public partial class Scores
    {
        public static int Count { get; set; }
    }
}
