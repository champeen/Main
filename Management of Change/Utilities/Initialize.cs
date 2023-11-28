using External_Website_v2.Provider;
using Management_of_Change.Data;
using Management_of_Change.Provider;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System.Diagnostics;

namespace Management_of_Change.Utilities
{
    public class Initialization : BaseController
    {
        private static Boolean Initialized = false;
        private readonly Management_of_ChangeContext _context;

        // Providers...
        public static TeamsProvider TeamsErrorProvider { get; private set; }
        public static EmailProvider EmailProviderSmtp { get; private set; }
        public static HttpClient HttpClient { get; private set; }

        // Teams Error Hooks...
        private static string teamsErrorUrl { get; set; }
        private static string teamsErrorUrlDev { get; set; }
        private static string teamsErrorUrlPrd { get; set; }

        // Setup Email Paramaters...
        private static string emailHost { get; set; }
        private static int emailPort { get; set; }
        private static string emailUser { get; set; }
        private static string emailPassword { get; set; }
        private static string emailFrom { get; set; }



        // Setup Global Static Variables based on environment....
        public static string WebsiteUrl { get; set; }
        public static string ConnectionString { get; set; }
        public static string AttachmentDirectory { get; set; }

        private static string registryPath = @"Software\\SKSiltron\\MoC";
        public static string EmailError { get; set; }

        //public Initialize(Management_of_ChangeContext context, WebApplicationBuilder builder) : base(context, builder)
        //{
        //    _context = context;
        //}

        public static void Initialize(WebApplicationBuilder builder)
        {
            if (Initialized)
                return;
            Initialized = true;

            // Setup Environment and Path to retrieve secrets
            //var environment = builder.Environment.EnvironmentName;
            string environment = GetRegistryKey(registryPath, "Environment");

            if (environment == "Production")
            {
                registryPath = Path.Combine(registryPath, "Prd");
                WebsiteUrl = @"http://bay1vprd-moc01/";
            }
            else if (environment == "Development")
            {
                registryPath = Path.Combine(registryPath, "Dev");
                WebsiteUrl = @"http://appdevbaub01/";
            }
            else
            {
                throw new NullReferenceException("ERROR: registryPath: " + registryPath + " Key: Environment: " + environment + " is not equal to 'Development' or 'Production'");
            }

            // get error email box....
            EmailError = GetRegistryKey(registryPath, "EmailError");

            // Create Email Provider
            //emailHost = builder.Configuration.GetValue<string>("EmailProvider:emailHost");
            emailHost = GetRegistryKey(registryPath, "EmailHost");

            emailPort = Int32.Parse(GetRegistryKey(registryPath, "EmailPort"));
            emailUser = GetRegistryKey(registryPath, "EmailUser");
            emailPassword = GetRegistryKey(registryPath, "EmailPassword");
            emailFrom = GetRegistryKey(registryPath, "EmailFrom");
            EmailProviderSmtp = new EmailProvider(emailHost, emailUser, emailPassword, emailPort, emailFrom);

            // Get Connection String
            ConnectionString = GetRegistryKey(registryPath, "ConnectionString");
            //ConnectionString = builder.Configuration.GetConnectionString("PostgreSQLprd");

            // Get Attachment Directory
            AttachmentDirectory = GetRegistryKey(registryPath, "AttachmentDirectory");

            // Teams Provider for Error Channel
            teamsErrorUrl = GetRegistryKey(registryPath, "TeamsErrorUrl");
            //teamsErrorUrl = builder.Configuration.GetValue<string>("TeamsHooks:TeamsErrorUrlPrd");
            HttpClient = new HttpClient();
            TeamsErrorProvider = new TeamsProvider(teamsErrorUrl, HttpClient);
        }

        private static string GetRegistryKey(string registryPath, string registryKey)
        {
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(registryPath))
            {
                if (key != null)
                {
                    Object o = key.GetValue(registryKey);
                    if (o != null)
                    {
                        return o.ToString();
                    }
                    else
                    {
                        throw (new Exception("Error retrieving key: <strong>" + registryKey +
                            "</strong> from the registry at path <strong>" + registryPath + "</strong>."));
                    }
                }
            }
            return "";
        }

        public void GetUserInfo()
        {
            ViewBag.IsAdmin = _isAdmin;
            ViewBag.UserName = _username;
        }
    }
}
