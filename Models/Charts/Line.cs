using System.Collections.Generic;

namespace NjordBooks.BLL.Models.Charts
{
    public class Line
    {
        public string Name { get; set; }

        // dates
        public List<string> Xcords { get; set; }

        // balances
        public List<decimal> Ycords { get; set; }
    }

    public class Chart
    {
        public List<string> Dates { get; set; }
        public List<Line> Lines { get; set; }
    }
}
