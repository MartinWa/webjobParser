using System;

namespace webjobParser.Dto
{
    class CustomEntry
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public EntryStatus Status { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Duration { get; set; }

        public override string ToString()
        {
            return $"https://{Secrets.AppName.ToLower()}.scm.azurewebsites.net/azurejobs/#/functions/invocations/{Id.ToLower()} {Title} {Date} {Duration}";
        }

        public string ToHtml()
        {
            return $@"<tr><td><a href=""https://{Secrets.AppName.ToLower()}.scm.azurewebsites.net/azurejobs/#/functions/invocations/{Id.ToLower()}"">{Id.ToLower()}</a></td><td>{Title}</td><td>{Date}</td><td>{Duration}</td></tr>";
        }

        public string TableDataString()
        {
            return @"<tr><td>";
        }
    }

    internal enum EntryStatus
    {
        CompletedSuccess,
        Running,
        CompletedFailed,
        NeverFinished
    }
}
