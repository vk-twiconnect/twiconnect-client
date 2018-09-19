using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using TWIConnect.Client.Utilities;
using System.Net.Security;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TWIConnect.Client
{
  public static class RestClient
  {
    public static T PostJson<T>(string uri, object body)
    {
      System.Diagnostics.Stopwatch stopWatch = System.Diagnostics.Stopwatch.StartNew();
      Utilities.Logger.Log(NLog.LogLevel.Trace, Resources.Messages.StartClientRequest, uri, JsonConvert.SerializeObject(body));

      var requestJObject = JObject.FromObject(body);
      using (var http = new HttpClient())
      {
        //Disable certs validation hack!
        ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });

        var response = (http.PostAsJsonAsync(uri, requestJObject)).Result;
        response.EnsureSuccessStatusCode();
        var responseBody = response.Content.ReadAsAsync<T>().Result;

        Utilities.Logger.Log(NLog.LogLevel.Trace, Resources.Messages.EndOfExecution, "RestClient.PostJson", Logger.GetTimeElapsed(stopWatch));
        return responseBody;
      }
    }
  }
}
