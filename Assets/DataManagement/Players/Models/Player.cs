﻿using System.Collections.Generic;

namespace DataManagement.Players.Models
{
    [System.Serializable]
    public class Player
    {
        public string Name;
        public string Icon;
        
        public Dictionary<ResourcesId, int> Resources;
        public string[] Layout;
    }
}