using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HomecareManagement.Models;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using System.Web.Security;

namespace HomecareManagement.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Default/

        public ActionResult Index()
        {
            return View();
        }

    }
}
