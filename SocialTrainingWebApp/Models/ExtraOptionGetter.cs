using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialTrainingWebApp.Models
{
    public class ExtraOptionGetter
    {
        public static List<Employee> GetMoreEmployeeOptions(List<Employee> allEmployees)
        {
            List<Employee> extraChoices = DTO.GetEmployees();
            Random rndGenerator = new Random();
            int rndNumber = rndGenerator.Next(extraChoices.Count - 1);
            do
            {
                if (!(allEmployees.Contains(extraChoices[rndNumber])))
                {
                    allEmployees.Add(extraChoices[rndNumber]);
                    break;
                }
                else
                {
                    rndNumber = rndGenerator.Next(extraChoices.Count - 1);
                }

            } while (true);
            return allEmployees;
        }
    }
}
