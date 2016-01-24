// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming
// ReSharper disable ClassNeverInstantiated.Global
using System.Collections.Generic;

namespace webjobParser.Dto
{
    public class Entry
    {
        public object executingJobRunId { get; set; }
        public string id { get; set; }
        public object functionId { get; set; }
        public object functionName { get; set; }
        public object functionFullName { get; set; }
        public string functionDisplayTitle { get; set; }
        public string status { get; set; }
        public string whenUtc { get; set; }
        public double duration { get; set; }
        public object exceptionMessage { get; set; }
        public object exceptionType { get; set; }
        public string hostInstanceId { get; set; }
        public object instanceQueueName { get; set; }
    }

    public class FunctionsInvokes
    {
        public List<Entry> entries { get; set; }
        public string continuationToken { get; set; }
        public bool isOldHost { get; set; }
    }
}
