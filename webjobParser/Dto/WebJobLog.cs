using System;
using RestSharp;

namespace webjobParser.Dto
{
    public class WebJobLog
    {
        public string Id { get; set; }
        public int PortalId { get; set; }
        public DateTime Date { get; set; }
        public string Log { get; set; }
        public TimeSpan Duration { get; set; }
    }
}