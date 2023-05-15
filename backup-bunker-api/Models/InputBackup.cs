﻿using Google.Cloud.Firestore;

namespace backup_bunker_api.Models
{
    public class InputBackup
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public List<string> Paths_From { get; set; }
        public List<string> Paths_To { get; set; }
        public List<bool> OnDaysOfWeek { get; set; }
        public List<string> AtTimes { get; set; }
    }
}
