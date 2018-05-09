using System.Web;
using System.Web.Optimization;

namespace HomecareManagement
{
    public class BundleConfig
    {
        // 如需 Bundling 的詳細資訊，請造訪 http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            #region 使用CDN

            string CdnPath = "https://ajax.googleapis.com/ajax/libs/jquery/2.1.4/jquery.min.js";

            bundles.Add(new ScriptBundle("~/bundles/jquery", CdnPath).Include(
                       "~/Scripts/jquery-2.1.4.min.js"));
            #endregion

            // 使用開發版本的 Modernizr 進行開發並學習。然後，當您
            // 準備好實際執行時，請使用 http://modernizr.com 上的建置工具，只選擇您需要的測試。
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));
        }
    }
}