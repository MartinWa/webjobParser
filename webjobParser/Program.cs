﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
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
                const string groupName = "portalId";
                var portalIdRegexString = $@"((?<{groupName}>\d+))";
                var portalIdRegex = new Regex(portalIdRegexString, RegexOptions.None);
                var logs = new List<WebJobLog>();
                foreach (var entry in entries)
                {
                    var id = entry.id;
                    var portalId = 0;
                    var match = portalIdRegex.Match(entry.functionDisplayTitle);
                    if (match.Success)
                    {
                        var portalIdString = match.Groups[groupName].Value;
                        int.TryParse(portalIdString, out portalId);
                    }
                    var date = Convert.ToDateTime(entry.whenUtc);
                    var duration = TimeSpan.FromMilliseconds(entry.duration);
                    var logRequest = new RestRequest($"azurejobs/api/log/output/{id}?start=0", Method.GET);
                    var log = client.Execute(logRequest);
                    logs.Add(new WebJobLog
                    {
                        Id = id,
                        PortalId = portalId,
                        Date = date,
                        Duration = duration,
                        Log = log.Content
                    });
                    Console.WriteLine($"Log id {id} portal id {portalId} date {date} duration {duration}");
                }
                var json = JsonConvert.SerializeObject(logs);
                File.WriteAllText(path, json);
            }
            var jsonString = File.ReadAllText(path);
            var allLogs = JsonConvert.DeserializeObject<List<WebJobLog>>(jsonString);
            var portalsWithWarnings = allLogs.Where(log => log.Log.Contains("Warning")).OrderBy(l => l.PortalId).ToList();
            var storageAccount = CloudStorageAccount.Parse(Secrets.StorageAccountConnectionString);
            var queueClient = storageAccount.CreateCloudQueueClient();
            var queue = queueClient.GetQueueReference(Secrets.QueueName);
            foreach (var log in portalsWithWarnings)
            {
                queue.AddMessage(new CloudQueueMessage(log.PortalId.ToString()));
            }
        }
    }
}

