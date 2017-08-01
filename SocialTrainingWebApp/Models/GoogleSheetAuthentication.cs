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

namespace SocialTrainingWebApp.Models
{
    public class GoogleSheetAuthentication
    {
        //public const string CLIENT_ID = "728310900206-5j6rn058m1djpk0hgs31mpj67von68lr.apps.googleusercontent.com";
        //public const string CLIENT_SECRET = "7kAsZKz98MRiHzr9GWve_h4q";
        //public const string SCOPE = "https://spreadsheets.google.com/feeds https://docs.google.com/feeds";
        //public const string REDIRECT_URI = "urn:ietf:wg:oauth:2.0:oob";

        static string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
        static string ApplicationName = "Google Sheets API .NET Quickstart";

        public static void ParameterCreator()
        {


            //OAuth2Parameters parameters = new OAuth2Parameters();
            //parameters.ClientId = CLIENT_ID;
            //parameters.ClientSecret = CLIENT_SECRET;
            //parameters.RedirectUri = REDIRECT_URI;
            //parameters.Scope = SCOPE;
            //string authorizationUrl = OAuthUtil.CreateOAuth2AuthorizationUrl(parameters);
            //Console.WriteLine(authorizationUrl);
            //Console.ReadLine();
            UserCredential credential;

            using (var stream =
                new FileStream(@"C:\Users\roberts.zommerovskis\documents\visual studio 2017\Projects\SocialTrainingWEB\SocialTrainingWebApp\client_secret.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = System.Environment.GetFolderPath(
                    System.Environment.SpecialFolder.Personal);
                credPath = Path.Combine(credPath, ".credentials/sheets.googleapis.com-dotnet-quickstart.json");

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Google Sheets API service.
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // Define request parameters.
            String spreadsheetId = "1-7koO8ufsz8yCQLu4eFnHLd1Qy5-trPYIgGKX7eE3Lo";
            String range = "Class Data!A2:E";
            SpreadsheetsResource.ValuesResource.GetRequest request =
                    service.Spreadsheets.Values.Get(spreadsheetId, range);

            // Prints the names and majors of students in a sample spreadsheet:
            // https://docs.google.com/spreadsheets/d/1BxiMVs0XRA5nFMdKvBdBZjgmUUqptlbs74OgvE2upms/edit
            ValueRange response = request.Execute();
            IList<IList<Object>> values = response.Values;
            if (values != null && values.Count > 0)
            {
                Console.WriteLine("Name, Major");
                foreach (var row in values)
                {
                    // Print columns A and E, which correspond to indices 0 and 4.
                    Console.WriteLine("{0}, {1}", row[0], row[4]);
                }
            }
            else
            {
                Console.WriteLine("No data found.");
            }
            Console.Read();



        }

    }
}
