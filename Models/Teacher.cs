using DAL;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Models
{
    public class Teacher : Record
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Code { get; set; } = "CLG-420-" + new Random().Next(10000, 99999).ToString();
        [JsonIgnore] public string Email => $"{FirstName}.{LastName}@clg.qc.ca";
        public DateTime StartDate { get; set; }
        public string Phone { get; set; }
        public string Avatar { get; set; }

        // Propriétés calculées
        [JsonIgnore] public string FullName => LastName + " " + FirstName;
        [JsonIgnore] public string Caption => Code + " " + LastName + " " + FirstName;

        // Ancienneté en années
        [JsonIgnore] public double Seniority => Math.Round((DateTime.Now - StartDate).TotalDays / 365.25, 1);

        // Toutes les allocations du prof
        [JsonIgnore]
        public List<Allocation> Allocations => DB.Allocations.ToList()
            .Where(a => a.TeacherId == Id).ToList();

        // Allocations de la prochaine session seulement
        [JsonIgnore]
        public List<Allocation> NextSessionAllocations => DB.Allocations.ToList()
            .Where(a => a.TeacherId == Id && a.IsNextSession).ToList();

        // Cours assignés (toutes sessions)
        [JsonIgnore]
        public List<Course> Courses
        {
            get
            {
                var courses = new List<Course>();
                foreach (var allocation in Allocations.OrderBy(a => a.Course.Code))
                {
                    courses.Add(allocation.Course);
                }
                return courses;
            }
        }

        // Cours assignés pour la prochaine session
        [JsonIgnore]
        public List<Course> NextSessionCourses
        {
            get
            {
                var courses = new List<Course>();
                foreach (var allocation in NextSessionAllocations.OrderBy(a => a.Course.Code))
                {
                    courses.Add(allocation.Course);
                }
                return courses;
            }
        }

        // SelectList pour toutes les allocations
        [JsonIgnore]
        public SelectList CoursesSelectList =>
            SelectListUtilities<Course>.Convert(Courses, "Caption");

        // SelectList pour la prochaine session seulement
        [JsonIgnore]
        public SelectList NextSessionCoursesToSelectList =>
            SelectListUtilities<Course>.Convert(NextSessionCourses, "Caption");

        // Supprimer toutes les allocations du prof
        public void DeleteAllAllocations()
        {
            foreach (Allocation allocation in Allocations)
                DB.Allocations.Delete(allocation.Id);
        }

        // Supprimer seulement les allocations de la prochaine session
        public void DeleteNextSessionAllocations()
        {
            foreach (Allocation allocation in NextSessionAllocations)
                DB.Allocations.Delete(allocation.Id);
        }

        // Mettre à jour les allocations de la prochaine session
        public void UpdateAllocations(List<int> selectedCoursesId)
        {
            DeleteNextSessionAllocations();
            if (selectedCoursesId != null)
                foreach (int courseId in selectedCoursesId)
                {
                    DB.Allocations.Add(new Allocation { TeacherId = Id, CourseId = courseId });
                }
        }
        public bool IsValid()
        {
            if (string.IsNullOrEmpty(FirstName.Trim()) || string.IsNullOrEmpty(LastName.Trim()) || StartDate == DateTime.MinValue || string.IsNullOrEmpty(Phone.Trim()) || string.IsNullOrEmpty(Code.Trim()) || DB.Teachers.ToList().Where(c => c.Code == Code && c.Id != Id).Count() > 0)
                return false;
            return true;
        }
    }
}