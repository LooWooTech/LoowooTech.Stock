using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Models
{
    public class GHYT
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string BM { get; set; }
        public List<GHYT> GHYTs { get; set; }
    }
}
