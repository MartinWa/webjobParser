// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming
// ReSharper disable ClassNeverInstantiated.Global
using System.Collections.Generic;

namespace webjobParser.Dto
{
    public class InvocationDetails
    {
        public Invocation invocation { get; set; }
        public List<Parameter> parameters { get; set; }
        public object childrenIds { get; set; }
        public object ancestor { get; set; }
        public TriggerReason triggerReason { get; set; }
        public string trigger { get; set; }
        public bool isAborted { get; set; }
    }

    public class ExecutingJobRunId
    {
        public string jobType { get; set; }
        public string jobName { get; set; }
        public string runId { get; set; }
    }

    public class Invocation
    {
        public ExecutingJobRunId executingJobRunId { get; set; }
        public string id { get; set; }
        public string functionId { get; set; }
        public string functionName { get; set; }
        public string functionFullName { get; set; }
        public string functionDisplayTitle { get; set; }
        public string status { get; set; }
        public string whenUtc { get; set; }
        public double duration { get; set; }
        public object exceptionMessage { get; set; }
        public object exceptionType { get; set; }
        public string hostInstanceId { get; set; }
        public string instanceQueueName { get; set; }
    }

    public class Parameter
    {
        public string name { get; set; }
        public object description { get; set; }
        public string argInvokeString { get; set; }
        public object extendedBlobModel { get; set; }
        public object status { get; set; }
    }

    public class TriggerReason
    {
        public string parentGuid { get; set; }
        public string childGuid { get; set; }
    }
}