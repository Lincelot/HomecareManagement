using HomecareManagement.Models;
using HomecareManagement.Models.Web;
using HomecareManagement.Service;
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace HomecareManagement.Controllers
{
    public class AdminController : Controller
    {
        //
        // GET: /Admin/
        AdminService db = new AdminService();


        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        #region Service

        /// <summary>取得表格[服務項目]初始化用資料</summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public JsonResult getGridServiceInitData()
        {
            var data = db.selectGridServiceInitData();
            return Json(data);
        }

        /// <summary>取得服務類別</summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public JsonResult getService_ItemData()
        {
            var data = db.selectService_ItemData();
            return Json(data);
        }

        /// <summary>設定服務名稱&狀態</summary>
        /// <param name="uid"></param>
        /// <param name="service_Item"></param>
        /// <param name="serviceName"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public JsonResult setServiceNameAndStatus(int uid, int service_Item, String serviceName, Boolean status)
        {
            var data = db.updateServiceNameAndStatus(uid, service_Item, serviceName, status);
            return Json(data);
        }

        #endregion

        #region Equipment

        /// <summary>取得表格[裝置管理]初始化用資料</summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public JsonResult getGridEquipmentInitData()
        {
            var data = db.selectGridEquipmentInitData();
            return Json(data);
        }

        /// <summary>取得督導&照服員資料</summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public JsonResult getFormEquipmentUserData()
        {
            var data = db.selectFormEquipmentUserData();
            return Json(data);
        }

        /// <summary>設定新的裝置</summary>
        /// <param name="account_uid">使用者編號</param>
        /// <param name="mac">識別碼</param>
        /// <param name="type">類型</param>
        /// <param name="summary">備註</param>
        /// <param name="status">狀態</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public JsonResult setNewEquipment(int account_uid, String mac, int type, String summary, Boolean status)
        {
            if (type == 1)
            {
                mac = mac.Replace(":", "").ToUpper();
            }
            int i = db.insertNewEquipment(account_uid, mac, type, summary, status);
            return Json(i);
        }

        /// <summary>修改舊的裝置</summary>
        /// <param name="account_uid">使用者編號</param>
        /// <param name="mac">識別碼</param>
        /// <param name="type">類型</param>
        /// <param name="summary">備註</param>
        /// <param name="status">狀態</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public JsonResult setOldEquipment(int uid, int account_uid, String mac, int type, String summary, Boolean status)
        {
            int i = db.updateOldEquipment(uid, account_uid, mac, type, summary, status);
            return Json(i);
        }

        /// <summary>刪除舊的裝置</summary>
        /// <param name="uid">編號</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public JsonResult setDelEquipment(int uid)
        {
            int i = db.deleteOldEquipment(uid);
            return Json(i);
        }

        #endregion

        #region License

        /// <summary>取得表格[證照]初始化資料</summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public JsonResult getGridLicenseInitData()
        {
            var data = db.selectGridLicenseInitData();
            var list = from item in data.AsEnumerable()
                       select new GridLicenseModel
                       {
                           LicenseUID = item.Field<int>("uid"),
                           name = item.Field<String>("name"),
                           summary = item.Field<String>("summary"),
                           edit_time = item.Field<DateTime>("edit_time").ToString("yyyy/MM/dd HH:mm:ss")
                       };
            return Json(list);
        }

        /// <summary>修改證照</summary>
        /// <param name="uid"></param>
        /// <param name="name"></param>
        /// <param name="summary"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public JsonResult setOldLicense(int uid, String name, String summary)
        {
            int data = db.updateOldLicense(uid, name, summary);
            return Json(data);
        }

        /// <summary>新增證照</summary>
        /// <param name="name"></param>
        /// <param name="summary"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public JsonResult setNewLicense(String name, String summary)
        {
            int data = db.insertNewLicense(name, summary);
            return Json(data);
        }

        /// <summary>刪除證照</summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public JsonResult setDelLicense(int uid)
        {
            int i = db.deleteOldLicense(uid);
            return Json(i);
        }

        #endregion

    }
}
