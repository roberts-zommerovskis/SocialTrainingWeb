using System.Collections.Generic;
using System.Linq;


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
