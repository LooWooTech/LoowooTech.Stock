using System;
using System.Collections.Generic;
using System.Text;

namespace LoowooTech.Updater.Entities
{
    public class ProductFile
    {
        public string Address { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public string Hash { get; set; }

        public long Size { get; set; }
    }
}
