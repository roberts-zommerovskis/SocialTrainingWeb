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
    public class DTO
    {





        public static List<Employee> GetEmployees()
        {
            const string API_URL = "http://localhost:4567/api/karma/getEmployees";
            List<Employee> employees;
            var request = WebRequest.Create(API_URL);
            request.ContentType = "application/json; charset=utf-8";
            string text;
            var response = (HttpWebResponse)request.GetResponse();
            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                text = streamReader.ReadToEnd();
                employees = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<List<Employee>>(text);
            }

            return employees;
        }

    }
}
