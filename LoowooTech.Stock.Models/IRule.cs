﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Stock.Models
{
    public interface IRule
    {
        string ID { get; set; }
        string RuleName { get; set; }
        void Check();
    }
}
