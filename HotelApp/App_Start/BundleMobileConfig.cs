using System.Web;
using System.Web.Optimization;

namespace HotelApp
{
    public class BundleMobileConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/Content/Mobile/css")
                .Include("~/Content/Site.Mobile.css",
                "~/Content/themes/mobileTheme/custom.css"));

            bundles.Add(new StyleBundle("~/Content/jquerymobile/css").Include(
                        "~/Content/jquery.mobile.icons-1.4.2.css",
                        "~/Content/jquery.mobile.structure-1.4.2.css"));

            bundles.Add(new ScriptBundle("~/bundles/jquerymobile")
                .Include("~/Scripts/jquery.mobile-{version}.js"));
        }
    }
}