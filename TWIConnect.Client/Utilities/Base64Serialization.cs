using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TWIConnect.Client.Utilities
{
  public static class Base64Serialization
  {
    public static string FromBase64String(this string value)
    {
      return System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(value));
    }

    public static string ToBase64String(this string value)
    {
      return (string.IsNullOrWhiteSpace(value))? string.Empty: System.Text.Encoding.UTF8.GetBytes(value).ToBase64String();
    }

    public static string ToBase64String(this byte[] buffer)
    {
      string output = System.Convert.ToBase64String(buffer);
      foreach (string stringToRemove in Constants.Encoding.Base64EncodingRemoveStrings)
      {
        output = output.Replace(stringToRemove, string.Empty);
      }
      return output;
    }
  }
}
