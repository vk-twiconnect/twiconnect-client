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

namespace TWIConnect.Client.Utilities
{
  public static class RestClient
  {
    public static T PostJson<T>(string uri, object body)
    {
      System.Diagnostics.Stopwatch stopWatch = System.Diagnostics.Stopwatch.StartNew();
      try
      {
        Utilities.Logger.Log(NLog.LogLevel.Trace, Resources.Messages.StartClientRequest, uri, JsonConvert.SerializeObject(body));

        var requestJson = JObject.FromObject(body).ToString();
        var requestContent = new StringContent(requestJson);

        using (var http = new HttpClient())
        {
          //Disable certs validation hack!
          ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });

          //Set Content-Type
          const string contentType = "Content-Type";
          requestContent.Headers.Remove(contentType);
          requestContent.Headers.Add(contentType, "application/json");

          //Post request
          var response = http.PostAsync(uri, requestContent).Result;

          //Validate 200 response code
          response.EnsureSuccessStatusCode();

          //Parse the response
          var responseContent = response.Content.ReadAsStringAsync().Result;
          var responseObject = JsonConvert.DeserializeObject<T>(responseContent);

          //Log Response
          Utilities.Logger.Log(
            NLog.LogLevel.Trace, 
            Resources.Messages.EndClientRequest, 
            JsonConvert.SerializeObject(response), 
            JsonConvert.SerializeObject(responseObject)
          );


          //Log perfromance
          Utilities.Logger.Log(NLog.LogLevel.Trace, Resources.Messages.EndOfExecution, "RestClient.PostJson", Logger.GetTimeElapsed(stopWatch));

          //Return parsed object
          return responseObject;
        }
      }
      catch (Exception ex)
      {
        Utilities.Logger.Log(NLog.LogLevel.Error, ex);
        Utilities.Logger.Log(NLog.LogLevel.Error, "RestClient.PostJson", Logger.GetTimeElapsed(stopWatch));
        throw;
      }
    }
  }
}
