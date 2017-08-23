using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;


namespace SocialTrainingWebApp.Models
{
    public class LoggedInUserData
    {
        public string LoggedInUser { get; set; }


        public LoggedInUserData(string loggedInUserEmail)
        {
            if (loggedInUserEmail != null)
            {
                using (var db = new AppDbContext())
                {
                    List<Employee> employeesWithSuchEmail = db.Employee.Where(employeeElements => employeeElements.Email == loggedInUserEmail).ToList();
                    if (employeesWithSuchEmail.Count > 0)
                    {
                        string nameToSubstring = employeesWithSuchEmail.First().FullName;
                        LoggedInUser = nameToSubstring.Substring(0, nameToSubstring.IndexOf(" "));
                    }
                    else
                    {
                        LoggedInUser = loggedInUserEmail;
                    }
                }
            }
            else
            {
                LoggedInUser = "";
            }
        }


    }
}
