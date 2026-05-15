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
    public class CoursesController : Controller
    {
        // GET: Courses
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetCourses(bool forceRefresh)
        {
            List<Course> result = DB.Courses.ToList();

            if (Session["Search"] != null && (bool)Session["Search"])
            {
                if (Session["SearchString"] == null)
                    Session["SearchString"] = "";
                result = result.Where(c => c.FullName.ToLower().Contains((string)Session["SearchString"])).ToList();

            }

            List<int> session = new List<int>();
            foreach (Course course in result.ToList())
            {
                if (!session.Contains(course.Session))
                    session.Add(course.Session);
            }
            Session["CoursesSessionList"] = session;
            if (DB.Courses.HasChanged || forceRefresh || (Session["Search"] != null && (bool)Session["Search"]))
            {



                return PartialView(result);
            }


            return Content("");

        }


        public ActionResult Details(int id)
        {
            Session["CurrentCourseId"] = id;
            Session["UserCanEditCurrentCourse"] = false;
            Course course = DB.Courses.Get(id);

            if (course != null)
            {
                Session["UserCanEditCurrentCourse"] = Models.User.ConnectedUser.IsAdmin || Models.User.ConnectedUser.CanWrite;
                return View(course);
            }
            return RedirectToAction("Index");
        }

        public ActionResult GetInscriptions(bool forceRefresh)
        {
            if (Session["CurrentCourseId"] == null)
                Session["CurrentCourseId"] = 0;

            int courseId = (int)Session["CurrentCourseId"];
            List<Registration> registrations = DB.Courses.Get(courseId).Registrations;

            if (DB.Courses.HasChanged || forceRefresh || Session["CoursesInscriptionsYearsList"] == null)
            {
                List<int> years = new List<int>();
                foreach (Registration registration in registrations.ToList())
                {
                    if (!years.Contains(registration.Year))
                        years.Add(registration.Year);
                }
                Session["CoursesInscriptionsYearsList"] = years;
                return PartialView(DB.Courses.Get(courseId).Registrations);
            }
            return Content("");
        }

        public ActionResult GetDetails(bool forceRefresh)
        {
            if (Session["CurrentCourseId"] == null)
                Session["CurrentCourseId"] = 0;

            int courseId = (int)Session["CurrentCourseId"];
            Course course = DB.Courses.Get(courseId);
            if (DB.Courses.HasChanged || forceRefresh)
            {
                return PartialView(course);
            }
            return Content("");
        }

        [UserAccess(Access.Write)]
        public ActionResult Delete()
        {
            if (Session["CurrentCourseId"] != null)
            {
                int id = (int)Session["CurrentCourseId"];
                DB.Courses.Get(id).DeleteAllAllocations();
                DB.Courses.Get(id).DeleteAllRegistrations();
                DB.Courses.Delete(id);

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
        public ActionResult Create(Course course)
        {
            if (course.IsValid())
            {
                DB.Courses.Add(course);
                return RedirectToAction("Index");
            }
            return Redirect("/Accounts/Login?message=Accès illégal! &success=false");
        }

        [UserAccess(Access.Admin)]
        public ActionResult Edit()
        {
            int id = (int)Session["CurrentCourseId"];
            Course course = DB.Courses.Get(id);
            if (course != null)
            {
                ViewBag.Registrations = course.NextSessionStudentsToSelectList;
                ViewBag.Students = DB.Students.ToSelectList();
                return View(DB.Courses.Get(id));
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        [UserAccess(Access.Admin)]
        public ActionResult Edit(Course course, List<int> selectedStudentsId)
        {
            course.Id = (int)Session["CurrentCourseId"];
            if (course.IsValid())
            {
                DB.Courses.Update(course, selectedStudentsId);
                return RedirectToAction("Details", new { id = course.Id });
            }
            return Redirect("/Accounts/Login?message=Accès illégal! &success=false");
        }
    }
}