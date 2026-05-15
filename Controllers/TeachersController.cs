using DAL;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static Controllers.AccessControl;
namespace WebApplication.Controllers
{
    [UserAccess(Access.View)]
    public class TeachersController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetTeachers(bool forceRefresh)
        {
            List<Teacher> result = DB.Teachers.ToList().OrderBy(t => t.LastName).ToList();

            if (Session["Search"] != null && (bool)Session["Search"])
            {
                if (Session["SearchString"] == null)
                    Session["SearchString"] = "";
                result = result.Where(c => c.FullName.ToLower().Contains((string)Session["SearchString"])).ToList();
            }
            if (DB.Teachers.HasChanged|| forceRefresh || (Session["Search"] != null && (bool)Session["Search"]))
                return PartialView(result);
            return Content("");
        }
        public ActionResult Details(int id)
        {
            Session["CurrentTeacherId"] = id;
            Teacher result = DB.Teachers.Get(id);
            if(result != null)
                return View(result);
            return RedirectToAction("Index");
        }
        public ActionResult GetDetails(bool forceRefresh = false)
        {
            if (Session["CurrentTeacherId"] == null)
                Session["CurrentTeacherId"] = 0;
            
            int id = (int)Session["CurrentTeacherId"];
            Teacher result = DB.Teachers.Get(id);

            if(DB.Teachers.HasChanged || forceRefresh)
                return PartialView(result);
            return Content("");
        }
        public ActionResult GetAllocations(bool forceRefresh = false)
        {
            if (Session["CurrentTeacherId"] == null)
                Session["CurrentTeacherId"] = 0;

            int TeacherId = (int)Session["CurrentTeacherId"];
            List<Allocation> allocations = DB.Teachers.Get(TeacherId).Allocations;

            if (DB.Teachers.HasChanged || forceRefresh || Session["TeachersAllocationsYearsList"] == null)
            {
                List<int> years = new List<int>();
                foreach (Allocation allocation in allocations.ToList())
                {
                    if (!years.Contains(allocation.Year))
                        years.Add(allocation.Year);
                }
                Session["TeachersAllocationsYearsList"] = years;
                return PartialView(DB.Teachers.Get(TeacherId).Allocations);
            }
            return Content("");
        }
        [UserAccess(Access.Write)]
        public ActionResult Delete()
        {
            if (Session["CurrentTeacherId"] != null)
            {
                int id = (int)Session["CurrentTeacherId"];
                DB.Teachers.Get(id).DeleteAllAllocations();
                DB.Teachers.Delete(id);
            }
            return RedirectToAction("Index");
        }
        [UserAccess(Access.Write)]
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [UserAccess(Access.Write)]
        public ActionResult Create(Teacher teacher)
        {
            while (DB.Teachers.ToList().Any(t => t.Code == teacher.Code))
                teacher.Code = "CLG-420-" + new Random().Next(10000, 99999);

            if (teacher.IsValid())
            {
                DB.Teachers.Add(teacher);
                return RedirectToAction("Index");
            }
            return Redirect("/Accounts/Login?message=Accès illégal! &success=false");
        }
        [UserAccess(Access.Admin)]
        public ActionResult Edit()
        {
            int id = (int)Session["CurrentTeacherId"];
            Teacher teacher = DB.Teachers.Get(id);
            if (teacher != null)
            {
                ViewBag.Allocations = teacher.NextSessionCoursesToSelectList;
                ViewBag.Courses = DB.Courses.NextSessionToSelectList();
                return View(DB.Teachers.Get(id));
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        [UserAccess(Access.Admin)]
        public ActionResult Edit(Teacher teacher, List<int> selectedCoursesId)
        {
            teacher.Id = (int)Session["CurrentTeacherId"];
            teacher.Code = DB.Teachers.Get(teacher.Id).Code;
            if (teacher.IsValid())
            {
                DB.Teachers.Update(teacher, selectedCoursesId);
                return RedirectToAction("Details", new { id = teacher.Id });
            }
            return Redirect("/Accounts/Login?message=Accès illégal! &success=false");
        }
    }
}