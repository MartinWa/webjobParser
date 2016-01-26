using System;

namespace webjobParser.Dto
{
    class CustomEntry
    {
        public string Id { get; set; }
        public string FunctionDisplayTitle { get; set; }
        public EntryStatus Status { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Duration { get; set; }
        public object ExceptionMessage { get; set; }
        public object ExceptionType { get; set; }
    }

    internal enum EntryStatus
    {
        CompletedSuccess,
        Running,
        CompletedFailed,
        NeverFinished
    }
}
