using Microsoft.IdentityModel.Protocols;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace IVRPOC.Constants
{
    public class Constants
    {
        #region

        internal static string ADClientId = ConfigurationManager.AppSettings["ADClientId"];

        internal static string ADClientSecret = ConfigurationManager.AppSettings["ADClientSecret"];

        internal static string apiBasePath = ConfigurationManager.AppSettings["apiBasePath"].ToLower();

        internal static string botId = ConfigurationManager.AppSettings["MicrosoftAppId"];

        #endregion
    }
}