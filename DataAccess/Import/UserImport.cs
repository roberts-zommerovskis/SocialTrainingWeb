using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;
using DataModel;
using DataModel.DataContext;
using DataModel.DataModel;
using Google.Apis.Drive.v3;
using Google.Apis.Http;
using Google.Apis.Services;

namespace DataAccess.Import
{
    // TODO Error logging!
    public class UserImport : IDisposable
    {
        private const string ApplicationName = "Social Training";

        private const string DefaultGoogleDocFileId = "1b4DVgHZjpqxmTWMhxp3Fz6vFx2gd6a5WH78bRelakiE";

        private const string Domain = "visma.com";

        private const string GoogleDocBaseUri = "https://docs.google.com/spreadsheets/d/";

        private readonly string[] AllLevels = new string[] { "I", "R", "J", "M", "S" };

        private GoogleDoc defaultGoogleDoc = null;

        private object lockObject = new object();

        private SocialTrainingContext db = null;

        public UserImport(SocialTrainingContext context)
        {
            db = context;
            EnsureInit();
        }

        public void ImportAllUsers(IConfigurableHttpClientInitializer credentials, GoogleDoc doc = null)
        {
            DriveService googleDriveService = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credentials,
                ApplicationName = ApplicationName,
            });

            FilesResource.ExportRequest request = googleDriveService.Files.Export(doc.GoogleFileId, "text/csv");
            using (MemoryStream stream = new MemoryStream())
            {
                request.Download(stream);
                ImportAllUsers(stream, doc);
            }
        }

        public void ImportAllUsers(string fileName, GoogleDoc doc = null)
        {
            using (FileStream stream = File.OpenRead(fileName))
            {
                ImportAllUsers(stream, doc);
            }
        }

        public void ImportAllUsers(Stream stream, GoogleDoc doc = null)
        {
            if (doc == null)
                doc = defaultGoogleDoc;

            using (StreamReader reader = new StreamReader(stream))
            {
                CsvReader csv = new CsvReader(reader);
                DateTime now = DateTime.Now;

                while (csv.Read())
                {
                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            long id = csv.GetField<long>("ID");
                            string email = GetEmail(csv);

                            User user = db.Users.FirstOrDefault(x => x.ImportId == id);

                            if (user == null && !string.IsNullOrWhiteSpace(email))
                                user = db.Users.FirstOrDefault(x => x.Email == email);

                            bool create = false;
                            if (user == null)
                            {
                                user = new User();
                                create = true;
                            }

                            user.GoogleDoc = doc;
                            ExtractUser(csv, user);

                            user.ImportDate = now;
                            if (user.JoinDate == null)
                                user.JoinDate = now;

                            if (create)
                                db.Users.Add(user);

                            db.SaveChanges();
                            dbContextTransaction.Commit();
                        }
                        catch (Exception)
                        {
                            dbContextTransaction.Rollback();
                        }
                    }
                }
            }
        }

        private void EnsureInit()
        {
            bool create = false;
            lock (lockObject)
            {
                if (defaultGoogleDoc == null)
                {
                    create = true;
                }
            }

            if (create)
            {
                defaultGoogleDoc = db.GoogleDocs.FirstOrDefault(x => x.GoogleFileId == DefaultGoogleDocFileId);
                if (defaultGoogleDoc == null)
                {
                    defaultGoogleDoc = new GoogleDoc()
                    {
                        GoogleFileId = DefaultGoogleDocFileId,
                        Uri = GoogleDocBaseUri + DefaultGoogleDocFileId
                    };

                    db.GoogleDocs.Add(defaultGoogleDoc);
                    db.SaveChanges();
                }
            }
        }

        private void ExtractUser(CsvReader csv, User user)
        {
            ExtractUserCommon(csv, user);
            ExtractUserTeam(csv, user);
            ExtractUserProject(csv, user);
            ExtractUserSkills(csv, user);
        }

        private void ExtractUserSkills(CsvReader csv, User user)
        {
            for (int i = 1; i < csv.CurrentRecord.Length; i++)
            {
                // 0-th cell is ID, thus starting with 1.

                string[] userLevels = csv.GetField<string>(i).Split(' ', '/');
                bool areSkills = userLevels.All(x => AllLevels.Contains(x));

                if (areSkills)
                {
                    string technologyName = csv.FieldHeaders[i];

                    Technology technology = db.Technologies.FirstOrDefault(x => x.TechnologyName == technologyName);
                    if (technology == null)
                    {
                        technology = new Technology()
                        {
                            TechnologyName = technologyName
                        };

                        db.Technologies.Add(technology);
                    }

                    foreach (string s in userLevels)
                    {
                        int index = AllLevels.Select((x, idx) => new { x, idx }).FirstOrDefault(o => o.x == s).idx;

                        Skill skill = new Skill()
                        {
                            User = user,
                            Level = (SkillLevel)index,
                            Technology = technology
                        };

                        db.Skills.Add(skill);

                        user.Skills.Add(skill);
                    }
                }
            }
        }

        private string GetEmail(CsvReader csv)
        {
            string result = csv.GetField<string>("Email").Trim();
            if (string.IsNullOrWhiteSpace(result))
            {
                result = string.Join(".", RemoveDiactrics(csv.GetField<string>("Name"))
                    .Trim()
                    .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                    .ToLower() + "@" + Domain;
            }
            return result;
        }

        private void ExtractUserCommon(CsvReader csv, User user)
        {
            user.ImportId = csv.GetField<long>("ID");
            user.FullName = csv.GetField<string>("Name");
            user.Status = (UserStatus)csv.GetField<int>("Status");
            user.JoinDate = csv.GetField<DateTime?>("Join Date");
            user.PrimaryClientContact = csv.GetField<string>("Primary Client Contact");
            user.SecondaryClientContact = csv.GetField<string>("Secondary Client contact");
            user.Organization = csv.GetField<string>("Chapter (Organization)");
            user.Country = csv.GetField<string>("Country ");
            string email = GetEmail(csv);
            if (string.IsNullOrWhiteSpace(user.Email) || !string.IsNullOrWhiteSpace(csv.GetField<string>("Email")))
                user.Email = email;
        }

        private void ExtractUserProject(CsvReader csv, User user)
        {
            string projectTitle = csv.GetField<string>("Squads (Projects)");
            Project project = db.Projects.FirstOrDefault(x => x.ProjectTitle == projectTitle);

            if (project == null)
            {
                project = new Project()
                {
                    ProjectTitle = projectTitle
                };

                db.Projects.Add(project);
            }

            user.Project = project;
        }

        private void ExtractUserTeam(CsvReader csv, User user)
        {
            string teamName = csv.GetField<string>("Tribe (Team)");
            Team team = db.Teams.FirstOrDefault(x => x.TeamName == teamName);

            if (team == null)
            {
                string organizationName = csv.GetField<string>("Chapter (Organization)");
                Organization organization = db.Organizations.FirstOrDefault(x => x.OrganizationName == organizationName);

                if (organization == null)
                {
                    organization = new Organization()
                    {
                        OrganizationName = organizationName
                    };

                    db.Organizations.Add(organization);
                }

                team = new Team()
                {
                    TeamName = teamName,
                    Organization = organization
                };

                db.Teams.Add(team);
            }
            user.Team = team;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        private static string RemoveDiactrics(string x)
        {
            string normalized = x.Normalize(NormalizationForm.FormD);
            StringBuilder builder = new StringBuilder();

            foreach (char c in normalized)
            {
                var category = CharUnicodeInfo.GetUnicodeCategory(c);
                if (category != UnicodeCategory.NonSpacingMark)
                    builder.Append(c);
            }

            return builder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
