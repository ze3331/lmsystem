﻿using Application.Core.Data;

namespace LMPlatform.Models.BTS
{
    public class BugStatus : ModelBase
    {
        public string Name
        {
            get; 
            set;
        }

        public Bug Bug
        {
            get; 
            set;
        }
    }
}
