using DAL;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Models
{
    public class StudentsRepository : Repository<Student>
    {
        public void Update(Student student, List<int> selectedCoursesId)
        {
            base.Update(student);

            DB.Students.Get(student.Id).UpdateRegistrations(selectedCoursesId);
        }

        public SelectList ToSelectList()
        {
            var students = this.ToList();
            return SelectListUtilities<Student>.Convert(students, "Caption");
        }
    }
}