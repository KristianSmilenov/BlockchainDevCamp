﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blockchain
{
    public class AppSettings
    {
        public int Difficulty { get; set; }
        public int MinerReward { get; set; }
        public string NodeName { get; set; }
        public string NodeUrl{ get; set; }
        public string FaucetAddress { get; set; }
    }
}
