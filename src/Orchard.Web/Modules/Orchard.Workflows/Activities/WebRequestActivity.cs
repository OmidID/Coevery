﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using Orchard.Localization;
using Orchard.Workflows.Models;
using Orchard.Workflows.Services;

namespace Orchard.Workflows.Activities {
    public class WebRequestActivity : Task {
        public WebRequestActivity() {
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public override bool CanExecute(WorkflowContext workflowContext, ActivityContext activityContext) {
            var url = activityContext.GetState<string>("Url");
            return !string.IsNullOrWhiteSpace(url);
        }

        public override IEnumerable<LocalizedString> GetPossibleOutcomes(WorkflowContext workflowContext, ActivityContext activityContext) {
            yield return T("Error");
            yield return T("Success");
        }

        public override IEnumerable<LocalizedString> Execute(WorkflowContext workflowContext, ActivityContext activityContext) {
            var url = activityContext.GetState<string>("Url");
            var verb = (activityContext.GetState<string>("Verb") ?? "GET").ToUpper();
            var headers = activityContext.GetState<string>("Headers");
            var formValues = activityContext.GetState<string>("FormValues") ?? "";

            using (var httpClient = new HttpClient {BaseAddress = new Uri(url)}) {
                HttpResponseMessage response;

                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                if (!String.IsNullOrWhiteSpace(headers)) {
                    foreach (var header in ParseKeyValueString(headers)) {
                        httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }

                switch (verb) {
                    default:
                    case "GET":
                        response = httpClient.GetAsync("").Result;
                        break;
                    case "POST":
                        var format = activityContext.GetState<string>("FormFormat");

                        switch (format) {
                            default:
                            case "KeyValue":
                                var form = ParseKeyValueString(formValues);
                                response = httpClient.PostAsync("", new FormUrlEncodedContent(form)).Result;
                                break;
                            case "Json":
                                var json = formValues.Replace("((", "{").Replace("))", "}");
                                response = httpClient.PostAsync("", new StringContent(json, Encoding.UTF8, "application/json")).Result;
                                break;
                        }
                        
                        break;
                }
                
                workflowContext.SetState("WebRequestResponse", response.Content.ReadAsStringAsync().Result);

                if (response.IsSuccessStatusCode)
                    yield return T("Success");

                yield return T("Error");
            }
        }

        public override string Name {
            get { return "WebRequest"; }
        }

        public override LocalizedString Category {
            get { return T("HTTP"); }
        }

        public override LocalizedString Description {
            get { return T("Performs an HTTP GET or POST request on the specified URL and stores the response as part of the workflow instance."); }
        }

        public override string Form {
            get { return "WebRequestActivity"; }
        }

        private static IEnumerable<KeyValuePair<string, string>> ParseKeyValueString(string text) {
            return Regex.Split(text, "\n\r").Select(x => x.Split(new[] { '=' })).ToDictionary(x => x[0].Trim(), x => x[1].Trim());
        }
    }
}