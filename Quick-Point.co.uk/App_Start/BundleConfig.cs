using System.Web;
using System.Web.Optimization;

namespace Quick_Point.co.uk
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
					  "~/Scripts/bootstrap.bundle.min.js",
					  "~/jquery.easing/jquery.easing.min.js",
					  //"~/assets/vendor/aos/aos.js",
					  "~/owl.carousel/owl.carousel.min.js",
					  "~/Scripts/jquery-sticky/jquery.sticky.js",
					  "~/Scripts/main.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
					  "~/Content/bootstrap.min.css",
					  //"~/assets/vendor/aos/aos.css",
					  "~/owl.carousel/assets/owl.carousel.min.css",
                      "~/Content/site.css"));
        }
    }
}
