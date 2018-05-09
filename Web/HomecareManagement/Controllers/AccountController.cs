using HomecareManagement.Models;
using HomecareManagement.Models.Web;
using HomecareManagement.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace HomecareManagement.Controllers
{
    public class AccountController : Controller
    {
        //
        // GET: /Account/
        AccountService db = new AccountService();

        public ActionResult Index()
        {
            return View();

        }

        #region Login

        /// <summary>驗證帳號密碼</summary>
        /// <param name="account"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logon(AccountModel account, String returnUrl)
        {
            String username = account.username;
            String password = account.password;
            if (!ModelState.IsValid)
            {
                return View("Index");
            }
            // 登入的密碼（以 SHA1 加密）
            password = FormsAuthentication.HashPasswordForStoringInConfigFile(password, "SHA1");
            AccountModel userdata = db.selectAccount(username, password);

            if (userdata.uid == 0)
            {
                ModelState.AddModelError("", "請輸入正確的帳號或密碼！");
                return View("Index");
            }
            else if (userdata.uid == -1)
            {
                ModelState.AddModelError("", "伺服器發生問題，請稍後在試。");
                return View("Index");
            }
            else
            {
                int uid = userdata.uid;
                String displayname = userdata.displayname;
                String level = userdata.level.ToString();
                // 登入時清空所有 Session 資料
                Session.RemoveAll();
                Response.Cookies.Clear();
                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1,
                  displayname,//你想要存放在 User.Identy.Name 的值，通常是使用者帳號
                  DateTime.Now,
                  DateTime.Now.AddMinutes(30),
                  false,//將管理者登入的 Cookie 設定成 Session Cookie
                  level,//userdata看你想存放啥
                  FormsAuthentication.FormsCookiePath
                  );
                String encTicket = FormsAuthentication.Encrypt(ticket);
                Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));
                HttpCookie cookie = new HttpCookie("uid", uid.ToString());
                cookie.Expires = DateTime.Now.AddMinutes(1440);
                Response.Cookies.Add(cookie);
                //導向到先前頁面
                if (returnUrl != null)
                {
                    return Redirect(FormsAuthentication.GetRedirectUrl(displayname, false));
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }

            }
        }

        /// <summary>登出</summary>
        /// <returns></returns>
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();

            //清除所有的 session
            Session.RemoveAll();

            //建立一個同名的 Cookie 來覆蓋原本的 Cookie
            HttpCookie cookie1 = new HttpCookie(FormsAuthentication.FormsCookieName, "");
            cookie1.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie1);

            //建立 ASP.NET 的 Session Cookie 同樣是為了覆蓋
            HttpCookie cookie2 = new HttpCookie("ASP.NET_SessionId", "");
            cookie2.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie2);

            //覆蓋原本username Cookie
            HttpCookie cookie3 = new HttpCookie("uid", "");
            cookie3.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie3);

            return RedirectToAction("Index", "Home");
        }

        #endregion

        #region Profile

        /// <summary>取得使用者資料</summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public ActionResult getUserInfo(String uid)
        {
            InfoModel info = null;
            if (uid != null)
            {
                info = db.selectInfo(int.Parse(uid));
            }
            else
            {
                Logout();
            }
            return Json(info);
        }

        /// <summary>編輯使用者資料</summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public ActionResult setUserInfo(InfoModel info)
        {
            int i = 0;
            String birthday = DateTime.Parse(info.birthday).ToString("yyyy/MM/dd");
            if (birthday.Contains("0001/"))
            {
                i = -1;
            }
            else
            {
                i = db.updateInfo(info);
            }
            return Json(i);
        }
        #endregion

    }
}
