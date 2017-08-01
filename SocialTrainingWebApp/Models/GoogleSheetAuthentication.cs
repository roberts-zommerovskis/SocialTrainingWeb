using Microsoft.Ajax.Utilities;
using Google.GData.Spreadsheets;
using Google.GData.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Auth.OAuth2;
using System.IO;
using System.Threading;
using Google.Apis.Util.Store;
using System.Net;

namespace SocialTrainingWebApp.Models
{
    public class GoogleSheetAuthentication
    {
        public const string API_URL = "728310900206-5j6rn058m1djpk0hgs31mpj67von68lr.apps.googleusercontent.com";




        public static void ParameterCreator()
        {
            var request = WebRequest.Create("http://localhost:4567/api/karma/getEmployees");
            request.ContentType = "application/json; charset=utf-8";
            string text;
            var response = (HttpWebResponse)request.GetResponse();

            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                text = streamReader.ReadToEnd();
                List<Employee> employees = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<List<Employee>>(text);
            }


        }

    }
}
