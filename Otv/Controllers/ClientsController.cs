using DanMu.Controllers;
using Otv.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Otv.Controllers
{
    public class ClientsController : BaseController
    {
        //
        // GET: /Clients/

        public ActionResult Index()
        {
            ViewBag.stb = CuidUtils.stbUids.Count;
            ViewBag.phone = CuidUtils.phoneUids.Count;
            return View();
        }

        public JsonResult Refresh()
        {
            JsonResult res = new JsonResult();
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            res.Data = new { state = "ok", stb = CuidUtils.stbUids.Count, phone = CuidUtils.phoneUids.Count };
            return res;
        }

    }
}
