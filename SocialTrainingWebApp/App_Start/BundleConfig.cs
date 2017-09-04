using System.Web;
using System.Web.Optimization;

namespace SocialTrainingWebApp
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/guessPanelAdjust").Include(
          "~/Scripts/dynamic.guess.panel.js"));

            bundles.Add(new ScriptBundle("~/bundles/checkAnswers").Include(
           "~/Scripts/answer.guess.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                       "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/ncjs").Include(
          "~/Content/jquery/dist/jquery.min.js",
          "~/Content/bootstrap/dist/js/bootstrap.js",
          "~/Content/nc3/js/bootstrap-tabdrop.js",
          "~/Content/nc3/js/visma-additional.js"));



            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/css2").Include(
          "~/Content/bootstrap.css",
          "~/Content/site2.css",
          "~/Content/nc3/css/nc.css"));

            bundles.Add(new StyleBundle("~/Content/css3").Include(
          "~/Content/nc3/css/nc.css"));
        }
    }
}
