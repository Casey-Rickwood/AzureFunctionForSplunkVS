﻿using System.Text.RegularExpressions;

namespace AzureFunctionForSplunk
{
    public class AzureMonitorMessage
    {
        protected dynamic Message { get; set; }
        protected string ResourceId { get; set; }
        public string SubscriptionId { get; set; }
        public string ResourceType { get; set; }
        public string ResourceName { get; set; }
        public string ResourceGroup { get; set; }
        public string SplunkSourceType { get; set; }

        public AzureMonitorMessage()
        {
            SubscriptionId = "";
            ResourceGroup = "";
            ResourceName = "";
            ResourceType = "";
            SplunkSourceType = "";
        }

        public string GetSplunkEventFromMessage()
        {
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(Message);

            var s = "{";
            s += "\"sourcetype\": \"" + SplunkSourceType + "\",";
            s += "\"event\": " + json;
            s += "}";

            return s;

        }

        protected void GetStandardProperties()
        {
            var patternSubscriptionId = "SUBSCRIPTIONS\\/(.*?)\\/";
            var patternResourceGroup = "SUBSCRIPTIONS\\/(?:.*?)\\/RESOURCEGROUPS\\/(.*?)\\/";
            var patternResourceType = "PROVIDERS\\/(.*?\\/.*?)(?:\\/)";
            var patternResourceName = "PROVIDERS\\/(?:.*?\\/.*?\\/)(.*?)(?:\\/|$)";
            var patternDatabase = "PROVIDERS\\/(.*?\\/.*?)(?:\\/)(?:.*\\/)(.*DATABASES)";

            Match m = Regex.Match(ResourceId, patternSubscriptionId);
            SubscriptionId = m.Groups[1].Value;

            m = Regex.Match(ResourceId, patternResourceGroup);
            ResourceGroup = m.Groups[1].Value;

            m = Regex.Match(ResourceId, patternResourceName);
            ResourceName = m.Groups[1].Value;

            m = Regex.Match(ResourceId, patternDatabase);
            var group1 = m.Groups[1].Value;
            var group2 = m.Groups[2].Value;
            if (group2 == "DATABASES")
            {
                ResourceType = group1 + "/" + group2;
            } else
            {
                m = Regex.Match(ResourceId, patternResourceType);
                ResourceType = m.Groups[1].Value;
            }
        }
    }

    public class MetricMessage : AzureMonitorMessage
    {
        public MetricMessage(string resourceId, dynamic message)
        {
            ResourceId = resourceId;
            Message = message;

            GetStandardProperties();

            AddStandardProperties();

        }

        private void AddStandardProperties()
        {
            if (SubscriptionId != "")
            {
                Message.amm_SubscriptionId = SubscriptionId;
            }
            if (ResourceGroup != "")
            {
                Message.amm_ResourceGroup = ResourceGroup;
            }
            if (ResourceType != "")
            {
                Message.amm_ResourceType = ResourceType;
            }
            if (ResourceName != "")
            {
                Message.amm_ResourceName = ResourceName;
            }
        }
    }

    public class ActivityLogMessage : AzureMonitorMessage
    {

        public ActivityLogMessage(string resourceId, string sourceType, dynamic message)
        {
            ResourceId = resourceId;
            SplunkSourceType = sourceType;
            Message = message;

            GetStandardProperties();

            AddStandardProperties();

        }

        private void AddStandardProperties()
        {
            if (SubscriptionId != "")
            {
                Message.amal_SubscriptionId = SubscriptionId;
            }
            if (ResourceGroup != "")
            {
                Message.amal_ResourceGroup = ResourceGroup;
            }
            if (ResourceType != "")
            {
                Message.amal_ResourceType = ResourceType;
            }
            if (ResourceName != "")
            {
                Message.amal_ResourceName =ResourceName;
            }
        }
    }

    public class DiagnosticLogMessage : AzureMonitorMessage
    {

        public DiagnosticLogMessage(string resourceId, dynamic message)
        {
            ResourceId = resourceId;
            Message = message;

            GetStandardProperties();

            AddStandardProperties();

        }

        public void SetSourceType(string sourceType)
        {
            SplunkSourceType = sourceType;
        }

        private void AddStandardProperties()
        {
            if (SubscriptionId != "")
            {
                Message.amdl_SubscriptionId = SubscriptionId;
            }
            if (ResourceGroup != "")
            {
                Message.amdl_ResourceGroup = ResourceGroup;
            }
            if (ResourceType != "")
            {
                Message.amdl_ResourceType = ResourceType;
            }
            if (ResourceName != "")
            {
                Message.amdl_ResourceName = ResourceName;
            }
        }
    }

}
