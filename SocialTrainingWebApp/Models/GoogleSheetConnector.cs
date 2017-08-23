using System.Web.Mvc;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;

namespace SocialTrainingWebApp.Models
{
    public class GoogleSheetConnector
    {
        static string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
        static string ApplicationName = "Google Sheets API .NET Employees";

        public static void ImportDataIntoDB()
        {
            UserCredential credential;

            string path = HttpContext.Current.Server.MapPath(@"~/client_secret.json");
            using (var stream =
                new FileStream(path, FileMode.Open, FileAccess.Read))
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
            }

            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            String spreadsheetId = "1m6k0YyMeVk9ykByZNfAK8sh7CAzXdbn_l8vDlPJF-9k";

            String range = "Sheet1!A2:I";
            SpreadsheetsResource.ValuesResource.GetRequest request =
                    service.Spreadsheets.Values.Get(spreadsheetId, range);


            ValueRange response = request.Execute();
            List<Employee> transferableEmployees = new List<Employee>();
            IList<IList<Object>> values = response.Values;
            if (values != null && values.Count > 0)
            {
                foreach (var row in values)
                {
                    if (row[2].ToString().Equals("2") || row[2].ToString().Equals("1"))
                    {
                        var employee = new Employee { FullName = row[1].ToString(), ImportId = long.Parse(row[0].ToString()), Sex = row[8].ToString(), Email = row[4].ToString() };
                        transferableEmployees.Add(employee);
                    }
                }
                List<long> existingEmployeeImportNums = new List<long>();
                using (var db = new AppDbContext())
                {
                    foreach (var employee in transferableEmployees)
                    {
                        existingEmployeeImportNums.Add(employee.ImportId);
                        var employeeInDb = db.Employee.Where(empDB => empDB.ImportId == employee.ImportId) // or whatever your key is
                .SingleOrDefault();
                        if (employeeInDb == null)
                            db.Employee.Add(employee);
                    }

                    foreach (var dbEmployee in db.Employee)
                    {
                        if (!existingEmployeeImportNums.Contains(dbEmployee.ImportId))
                        {
                            db.Employee.Remove(dbEmployee);
                        }
                    }
                    db.SaveChanges();
                }
            }
            else
            {
                //TODO: Cover case where data is not found
            }
        }

        public static List<EmployeeWrapper> AccessData()
        {
            List<EmployeeWrapper> employeeWrapperList = new List<EmployeeWrapper>();
            using (var db = new AppDbContext())
            {
                foreach (var employeeRecord in db.Employee.ToList<Employee>())
                {
                    employeeWrapperList.Add(new EmployeeWrapper { employee = employeeRecord, isUnguessed = true });
                }
            }
            return employeeWrapperList;
        }

        public static int GetEmployeeCount()
        {
            int employeeCountInDb;
            using (var db = new AppDbContext())
            {
                employeeCountInDb = db.Employee.Count();
            }
            return employeeCountInDb;
        }

    }
}