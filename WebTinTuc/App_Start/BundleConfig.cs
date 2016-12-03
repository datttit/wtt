using System.Web;
using System.Web.Optimization;

namespace WebTinTuc
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new ScriptBundle("~/bundles/dropzonescripts").Include(
                     "~/Scripts/dropzone/dropzone.js"));

            bundles.Add(new StyleBundle("~/Content/dropzonescss").Include(
                     "~/Scripts/dropzone/basic.css",
                     "~/Scripts/dropzone/dropzone.css"));

            bundles.Add(new ScriptBundle("~/bundles/datepickerjs").Include(
                    "~/Content/momentjs/moment.min.js",
                    "~/Content/bootstrap-datepicker/js/bootstrap-datepicker.min.js",
                    "~/Content/bootstrap-datepicker/locales/bootstrap-datepicker.vi.min.js"
                ));

            bundles.Add(new StyleBundle("~/Content/Default/mainstyle").Include(
                    "~/Content/Default/css/style.css",
                    "~/Content/Default/css/responsive.css"
                    ));

            bundles.Add(new StyleBundle("~/Content/datepickercss").Include(
                    "~/Content/bootstrap-datepicker/css/bootstrap-datepicker.min.css"
                ));

            BundleTable.EnableOptimizations = true;
        }
    }
}
