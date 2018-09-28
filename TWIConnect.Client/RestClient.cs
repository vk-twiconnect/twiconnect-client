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

      var requestJson = JObject.FromObject(body).ToString();
      var resquestContent = new StringContent(requestJson);

      using (var http = new HttpClient())
      {
        //Disable certs validation hack!
        ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });

        //Post request
        var response = http.PostAsync(uri, resquestContent).Result;

        //Validate 200 response code
        response.EnsureSuccessStatusCode();

        //Parse the response
        var responseContent = response.Content.ReadAsStringAsync().Result;
        var responseObject = JsonConvert.DeserializeObject<T>(responseContent);

        //Log perfromance
        Utilities.Logger.Log(NLog.LogLevel.Trace, Resources.Messages.EndOfExecution, "RestClient.PostJson", Logger.GetTimeElapsed(stopWatch));

        //Return parsed object
        return responseObject;
      }
    }
  }
}
