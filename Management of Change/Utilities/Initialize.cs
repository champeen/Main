using External_Website_v2.Provider;
using Management_of_Change.Data;
using Management_of_Change.Provider;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;

namespace Management_of_Change.Utilities
{
    public class Initialization : BaseController
    {
        private readonly Management_of_ChangeContext _context;
        public static TeamsProvider TeamsErrorProvider { get; private set; }
        public static EmailProvider EmailProviderSmtp { get; private set; }
        public static HttpClient HttpClient { get; private set; }
        private static string teamsErrorUrl { get; set; }
        public static string connectionString { get; set; }
        public static string message { get; set; }
        private static string teamsErrorUrlDev { get; set; }
        private static string teamsErrorUrlPrd { get; set; }
        private static Boolean Initialized = false;
        private static string emailHost { get; set; }
        private static int emailPort { get; set; }
        private static string emailUser { get; set; }
        private static string emailPassword { get; set; }
        private static string emailFrom { get; set; } 
        //public Initialize(Management_of_ChangeContext context, WebApplicationBuilder builder) : base(context, builder)
        //{
        //    _context = context;
        //}

        public static void Initialize(WebApplicationBuilder builder)
        {
            if (Initialized)
                return;
            Initialized = true;

            var environment = builder.Environment.EnvironmentName;

            // Create Email Provider
            emailHost = builder.Configuration.GetValue<string>("EmailProvider:emailHost");
            emailPort = builder.Configuration.GetValue<int>("EmailProvider:emailPort");
            emailUser = builder.Configuration.GetValue<string>("EmailProvider:emailUser");
            emailPassword = builder.Configuration.GetValue<string>("EmailProvider:emailPassword");
            emailFrom = builder.Configuration.GetValue<string>("EmailProvider:emailFrom");
            EmailProviderSmtp = new EmailProvider(emailHost, emailUser, emailPassword, emailPort, emailFrom);

            //emailHost = GetRegistryKey(registryPath, "EmailHost");
            //emailPort = GetRegistryKey(registryPath, "EmailPort").ToInt();
            //emailUser = GetRegistryKey(registryPath, "EmailUser");
            //emailPassword = GetRegistryKey(registryPath, "EmailPassword");
            //emailFrom = GetRegistryKey(registryPath, "EmailFrom");
            //emailSalesTo = GetRegistryKey(registryPath, "EmailSalesTo");
            //emailProcurementTo = GetRegistryKey(registryPath, "EmailProcurementTo");
            //emailGeneralTo = GetRegistryKey(registryPath, "EmailGeneralTo");
            //EmailProviderSmtp = new EmailProvider(emailHost, emailUser, emailPassword, emailPort, emailFrom, emailProcurementTo, emailSalesTo, emailGeneralTo);

            // Create Teams Provider  // Get Teams Hook for Errors....
            HttpClient = new HttpClient();
            try
            {
                if (environment == "Production")
                {
                    teamsErrorUrl = builder.Configuration.GetValue<string>("TeamsHooks:TeamsErrorUrlPrd");
                    //teamsErrorUrl = GetRegistryKey(registryPath, "TeamsErrorUrlPrd");
                    //connectionString = GetRegistryKey(registryPath, "ConnectionStringDev");   //  <-- CHANGE THIS TO PRODUCTION ONCE SETUP MJW II 
                }
                else
                {
                    teamsErrorUrl = builder.Configuration.GetValue<string>("TeamsHooks:TeamsErrorUrlDev");
                    //teamsErrorUrl = GetRegistryKey(registryPath, "TeamsErrorUrlDev");
                    //connectionString = GetRegistryKey(registryPath, "ConnectionStringDev");
                }
            }
            catch
            {
                teamsErrorUrl = builder.Configuration.GetValue<string>("TeamsHooks:TeamsHookErrorDev");
                throw;
            }
            finally
            {
                TeamsErrorProvider = new TeamsProvider(teamsErrorUrl, HttpClient);
            }
        }

        //private static string GetRegistryKey(string registryPath, string registryKey)
        //{
        //    using (RegistryKey key = Registry.LocalMachine.OpenSubKey(registryPath))
        //    {
        //        if (key != null)
        //        {
        //            Object o = key.GetValue(registryKey);
        //            if (o != null)
        //            {
        //                return o.ToString();
        //            }
        //            else
        //            {
        //                throw (new Exception("Error retrieving key: <strong>" + registryKey +
        //                    "</strong> from the registry at path <strong>" + registryPath + "</strong>."));
        //            }
        //        }
        //    }
        //    return "";
        //}

        public void GetUserInfo()
        {
            ViewBag.IsAdmin = _isAdmin;
            ViewBag.UserName = _username;
        }
    }
}
