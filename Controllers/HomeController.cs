using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static Controllers.AccessControl;

namespace WebApplication.Controllers
{

    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SetSearchString(string value)
        {
            Session["SearchString"] = "";
            if (value !=null)
                Session["SearchString"] = value.ToLower();

            return Content("");
        }

        public ActionResult ToggleSearch()
        {
            if (Session["Search"] == null) Session["Search"] = false;
            Session["Search"] = !(bool)Session["Search"];
            return Content("");
        }
        public ActionResult SetSession(int year, string session)
        {
            if (session == "Automne")
                NextSession.CurrentDate = new DateTime(year, 4, 1);
            if(session == "Hiver")
                NextSession.CurrentDate = new DateTime(year - 1,10,1);
            return Content("");
        }
        public ActionResult SessionSettings()
        {
            ViewBag.Year = Models.NextSession.Year;
            ViewBag.Session = Models.NextSession.ShortCaption;
            return View();
        }
        [HttpPost]
        public ActionResult SessionSettings(int year, string session)
        {
            if (session == "Automne")
                Models.NextSession.CurrentDate = new DateTime(year, 4, 1);
            if (session == "Hiver")
                Models.NextSession.CurrentDate = new DateTime(year - 1, 10, 1);
            return RedirectToAction("Index", "Students");
        }
    }
}