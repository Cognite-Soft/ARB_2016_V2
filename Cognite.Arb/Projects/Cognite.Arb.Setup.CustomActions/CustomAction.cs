using System;
using System.Data.SqlClient;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Windows.Forms;
using Microsoft.Deployment.WindowsInstaller;

namespace Cognite.Arb.Setup.CustomActions
{
    public class CustomActions
    {
        [CustomAction]
        public static ActionResult TestDatabaseConnectionAction(Session session)
        {
            session.Log("Begin CustomAction1");

            const string datasourceName = "Master";
            string datasourceServer = session["DATABASE_SERVER"];
            string datasourceUser = session["DATABASE_USER"];
            string datasourcePassword = session["DATABASE_PASSWORD"];

            TestDatabaseConnection(session,
                datasourceServer,
                datasourceName,
                datasourceUser,
                datasourcePassword);

            return ActionResult.Success;
        }

        public static void TestDatabaseConnection(Session session,
            string datasourceServer,
            string datasourceName,
            string datasourceUser,
            string datasourcePassword)
        {
            string dialogCaption = session["TestConnectionDialogCaption"];
            string succeededMessage = session["TestConnectionDialogSuccceeded"];

            try
            {
                var connectionBuilder = new SqlConnectionStringBuilder
                {
                    DataSource = datasourceServer,
                    InitialCatalog = datasourceName,
                    UserID = datasourceUser,
                    Password = datasourcePassword
                };

                using (var connection = new SqlConnection(connectionBuilder.ConnectionString))
                {
                    connection.Open();
                    connection.Close();
                }

                MessageBox.Show(succeededMessage,
                    dialogCaption,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information,
                    MessageBoxDefaultButton.Button1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,
                    dialogCaption,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1);
            }
        }

        [CustomAction]
        public static ActionResult CreateDefaultUserAction(Session session)
        {
            session.Log("Begin CreateDefaultUser custom action");
            string address = session["WEBAPI_ADDRESS"];
            string password = session["DEFAULT_USER_PASSWORD"];
            string phrase = session["DEFAULT_USER_PHRASE"];
            try
            {
                var result = CreateDefaultUser(address, password, phrase);
                if (result) session.Log("Default user has been created");
            }
            catch (Exception ex)
            {
                session.Log("Exception in CreateDefaultUserAction = " + ex.Message);
                return ActionResult.Failure;
            }
            session.Log("End CreateDefaultUser custom action");
            return ActionResult.Success;
        }

        private static bool CreateDefaultUser(string address, string password, string phrase)
        {
            var client = new HttpClient { BaseAddress = new Uri(address) };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            string uri = string.Format("users/createdefaultuser?password={0}&phrase={1}", password, phrase);
            var response = client.GetAsync(uri).Result;
            if (response.StatusCode == HttpStatusCode.OK) return true;
            if (response.StatusCode == HttpStatusCode.Forbidden) throw new Exception("Forbidden");
            throw new Exception("Create Default User Unknown Error");
        }
    }
}