using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using webjobParser.Dto;

namespace webjobParser
{
    static class Program
    {
        static void Main()
        {
            const string path = "webJobLogs.json";
            const string resultFile = "log.html";
            if (!File.Exists(path))
            {
                var client = new RestClient($"https://{Secrets.AppName}.scm.azurewebsites.net/")
                {
                    Authenticator = new HttpBasicAuthenticator($"{Secrets.UserName}", $"{Secrets.Password}")
                };
                const int limit = 100;
                var continuationToken = "";
                var invocationsRequest =
                    new RestRequest(
                        $"azurejobs/api/jobs/continuous/{Secrets.WebJobName}/functions?limit={limit}&continuationToken={continuationToken}",
                        Method.GET);
                var functionInvokes = client.Execute<FunctionsInvokes>(invocationsRequest);
                var data = functionInvokes.Data;
                var entries = new List<Entry>();
                entries.AddRange(data.entries);
                var count = limit;
                while (data.continuationToken != null)
                {
                    Console.WriteLine($"Count = {count}");
                    count += limit;
                    continuationToken = data.continuationToken;
                    invocationsRequest = new RestRequest($"azurejobs/api/jobs/continuous/{Secrets.WebJobName}/functions?limit={limit}&continuationToken={continuationToken}", Method.GET);
                    functionInvokes = client.Execute<FunctionsInvokes>(invocationsRequest);
                    data = functionInvokes.Data;
                    entries.AddRange(data.entries);
                }
                var json = JsonConvert.SerializeObject(entries);
                File.WriteAllText(path, json);
            }
            var jsonString = File.ReadAllText(path);
            var allEntries = JsonConvert.DeserializeObject<List<Entry>>(jsonString);
            var customEntries = allEntries.Select(a => new CustomEntry
            {
                Id = a.id,
                Title = a.functionDisplayTitle,
                Date = Convert.ToDateTime(a.whenUtc),
                Duration = TimeSpan.FromMilliseconds(a.duration),
                Status = (EntryStatus)Enum.Parse(typeof(EntryStatus), a.status),
            }).ToList();
            var result = new List<string>
            {
                "<!DOCTYPE html>",
                "<html>",
                "<head>",
                $"<title>Log from {Secrets.WebJobName}</title>",
                @"<link rel=""stylesheet"" href=""https://maxcdn.bootstrapcdn.com/bootstrap/3.3.6/css/bootstrap.min.css"" integrity=""sha384-1q8mTJOASx8j1Au+a5WDVnPi2lkFfwwEAa8hDDdjZlpLegxhjVME1fgjWPGmkzs7"" crossorigin=""anonymous"">",
                "</head>",
                "<body>",
                $@"<h1>Log from <a href=""https://{Secrets.AppName.ToLower()}.scm.azurewebsites.net/azurejobs/#/jobs/continuous/{Secrets.WebJobName.ToLower()}"">{Secrets.WebJobName}</a></h1>"
            };
            var running = customEntries.Where(r => r.Status == EntryStatus.Running).OrderByDescending(r => r.Duration);
            result.Add("<h2>Running</h2>");
            result.Add(@"<table class=""table table-striped"">");
            result.AddRange(running.Select(e => e.ToHtml()));
            result.Add("</table>");
            var completedFailed = customEntries.Where(r => r.Status == EntryStatus.CompletedFailed);
            result.Add("<h2>Failed</h2>");
            result.Add(@"<table class=""table table-striped"">");
            result.AddRange(completedFailed.Select(e => e.ToHtml()));
            result.Add("</table>");
            var neverFinished = customEntries.Where(r => r.Status == EntryStatus.NeverFinished);
            result.Add("<h2>Never finished</h2>");
            result.Add(@"<table class=""table table-striped"">");
            result.AddRange(neverFinished.Select(e => e.ToHtml()));
            result.Add("</table>");
            result.Add("</body>");
            result.Add("</html>");
            File.WriteAllText(resultFile, string.Join(Environment.NewLine,result));
        }
    }
}




//const string groupName = "portalId";
//var portalIdRegexString = $@"((?<{groupName}>\d+))";
//var portalIdRegex = new Regex(portalIdRegexString, RegexOptions.None);
//var logs = new List<WebJobLog>();
//foreach (var entry in entries)
//{
//    var id = entry.id;
//    var portalId = 0;
//    var match = portalIdRegex.Match(entry.functionDisplayTitle);
//    if (match.Success)
//    {
//        var portalIdString = match.Groups[groupName].Value;
//        int.TryParse(portalIdString, out portalId);
//    }
//    var date = Convert.ToDateTime(entry.whenUtc);
//    var duration = TimeSpan.FromMilliseconds(entry.duration);
//    var logRequest = new RestRequest($"azurejobs/api/log/output/{id}?start=0", Method.GET);
//    var log = client.Execute(logRequest);
//    logs.Add(new WebJobLog
//    {
//        Id = id,
//        PortalId = portalId,
//        Date = date,
//        Duration = duration,
//        Log = log.Content
//    });
//    Console.WriteLine($"Log id {id} portal id {portalId} date {date} duration {duration}");
//}