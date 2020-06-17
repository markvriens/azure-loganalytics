using System;
using System.Collections.Generic;
using System.Text;

namespace Azure.LogAnalytics.Model
{
    public class LogItem
    {
        public string Name { get; set; }
        public string Categorie { get; set; }
        public string Location { get; set; }
        public string CurrentValue { get; set; }
        public int Limit { get; set; }
        public string SubscriptionName { get; set; }
        public string SubscriptionId { get; set; }
        public int UsagePercentage { get; set; }
    }
}
