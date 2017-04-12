using DanMu.Filter;
using Otv.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DanMu.Controllers
{
    [LoginAuth]
    public class BaseController : Controller
    {
        protected UsersContext db = new UsersContext();

    }
}
