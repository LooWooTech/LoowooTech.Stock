using System;
using System.Collections.Generic;
using System.Text;

namespace LoowooTech.Updater.Entities
{
    public class ProductVersion
    {
        public string ChangeLog { get; set; }

        /// <summary>
        /// Version Name
        /// </summary>
        public string Name { get; set; }
        
        public int Build { get; set; }

        public string ProductId { get; set; }

        public List<ProductFile> Files { get; set; }
    }
}
