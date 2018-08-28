using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TWIConnect.Client.Constants
{
    struct Protocol
    {
        internal const string FormFieldContent = "content";
        internal const string MethodPost = "POST";
        internal const string ContentTypeForm = "application/x-www-form-urlencoded";
        internal static string[] Base64EncodingRemoveStrings = new string[] { "77u/" };
    }
}
