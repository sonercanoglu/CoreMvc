using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreMvc.Models
{
    public class PaginatedList<T> : List<T>
    {
        public int _PageSize
        {
            get { return 1; }
            set { _PageSize = value; }
        }
        public int _PageIndex { get; set; }
        public int _TotalRecords { get; set; }
        public List<T> _items { get; set; }
    }
}