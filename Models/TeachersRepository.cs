using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Models
{
    public class TeachersRepository : Repository<Teacher>
    {
        public void Update(Teacher teacher, List<int> selectedCoursesId)
        {
            base.Update(teacher);

            DB.Teachers.Get(teacher.Id).UpdateAllocations(selectedCoursesId);
        }
    }
}