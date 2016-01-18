using System.Web;
using System.Web.Optimization;

namespace Cognite.Arb.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery-ui").Include(
                        "~/Scripts/jquery-ui.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jnotify").Include(
                        "~/Scripts/jnotify*"));

            bundles.Add(new ScriptBundle("~/bundles/magicsuggest").Include(
                        "~/Scripts/magicsuggest-min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr.js"));

            bundles.Add(new ScriptBundle("~/bundles/foundation").Include(
                        "~/Scripts/foundation.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/app").Include(
                      "~/Scripts/app.js"));

            bundles.Add(new ScriptBundle("~/bundles/notifications").Include(
                      "~/Scripts/usernotifications.js"));

            bundles.Add(new ScriptBundle("~/bundles/usermanagement").Include(
                      "~/Scripts/usermanagement*"));

            bundles.Add(new ScriptBundle("~/bundles/complaintslist").Include(
                      "~/Scripts/complaints.list.js"));

            bundles.Add(new ScriptBundle("~/bundles/schedule").Include(
                      "~/Scripts/schedule.update.js"));

            bundles.Add(new ScriptBundle("~/bundles/complaintedit").Include(
                      "~/Scripts/complaint.edit.js"));

            bundles.Add(new ScriptBundle("~/bundles/complaintview").Include(
                      "~/Scripts/complaint.view.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/app.css",
                      "~/Content/font-awesome.min.css",
                      "~/Content/jquery-ui.min.css",
                      "~/Content/jnotify.css",
                      "~/Content/magicsuggest.css"));

            // Set EnableOptimizations to false for debugging.
            BundleTable.EnableOptimizations = false;
        }
    }
}
