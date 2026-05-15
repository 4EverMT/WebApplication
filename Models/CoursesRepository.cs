using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Models
{
    public class CoursesRepository : Repository<Course>
    {
        public SelectList NextSessionToSelectList()
        {
            var courses = this.ToList().Where(c => NextSession.ValidSessions.Contains(c.Session)).ToList();
            return SelectListUtilities<Course>.Convert(courses, "Caption");
        }

        public void Update(Course course, List<int> selectedStudentId)
        {
            base.Update(course);

            DB.Courses.Get(course.Id).UpdateRegistrations(selectedStudentId);
        }
    }
}