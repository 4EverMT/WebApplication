using DAL;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Models
{
    public class Course : Record
    {
        public string Code { get; set; }
        public string Title { get; set; }
        public int Session { get; set; }

        // Propriétés calculées
        [JsonIgnore] public string FullName => Code + " " + Title;
        [JsonIgnore] public string Caption => "[" + Session + "] " + Code + " " + Title;

        // Toutes les inscriptions pour ce cours
        [JsonIgnore]
        public List<Registration> Registrations => DB.Registrations.ToList()
            .Where(r => r.CourseId == Id).ToList();

        [JsonIgnore]
        public List<Allocation> Allocations => DB.Allocations.ToList()
            .Where(r => r.CourseId == Id).ToList();

        // Inscriptions de la prochaine session seulement
        [JsonIgnore]
        public List<Registration> NextSessionRegistrations => DB.Registrations.ToList()
            .Where(r => r.CourseId == Id && r.IsNextSession).ToList();

        // Étudiants inscrits (toutes sessions)
        [JsonIgnore]
        public List<Student> Students
        {
            get
            {
                var students = new List<Student>();
                foreach (var registration in Registrations.OrderBy(r => r.Student.LastName))
                {
                    students.Add(registration.Student);
                }
                return students;
            }
        }

        // Étudiants inscrits pour la prochaine session
        [JsonIgnore]
        public List<Student> NextSessionStudents
        {
            get
            {
                var students = new List<Student>();
                foreach (var registration in NextSessionRegistrations.OrderBy(r => r.Student.LastName))
                {
                    students.Add(registration.Student);
                }
                return students;
            }
        }

        // SelectList pour tous les étudiants inscrits
        [JsonIgnore]
        public SelectList StudentsSelectList =>
            SelectListUtilities<Student>.Convert(Students, "Caption");

        // SelectList pour la prochaine session seulement
        [JsonIgnore]
        public SelectList NextSessionStudentsToSelectList =>
            SelectListUtilities<Student>.Convert(NextSessionStudents, "Caption");

        // Allocation du prof pour une année donnée
        [JsonIgnore]
        public Teacher TeacherForNextSession
        {
            get
            {
                var allocation = DB.Allocations.ToList()
                    .FirstOrDefault(a => a.CourseId == Id && a.IsNextSession);
                return allocation?.Teacher;
            }
        }

        // Supprimer toutes les inscriptions du cours
        public void DeleteAllRegistrations()
        {
            foreach (Registration registration in Registrations)
                DB.Registrations.Delete(registration.Id);
        }

        public void DeleteAllAllocations()
        {
            foreach (Allocation allocation in Allocations)
                DB.Allocations.Delete(allocation.Id);
        }

        // Supprimer seulement les inscriptions de la prochaine session
        public void DeleteNextSessionRegistrations()
        {
            foreach (Registration registration in NextSessionRegistrations)
                DB.Registrations.Delete(registration.Id);
        }

        // Mettre à jour les inscriptions de la prochaine session
        public void UpdateRegistrations(List<int> selectedStudentsId)
        {
            DeleteNextSessionRegistrations();
            if (selectedStudentsId != null)
                foreach (int studentId in selectedStudentsId)
                {
                    DB.Registrations.Add(new Registration { CourseId = Id, StudentId = studentId });
                }
        }

        public bool IsValid()
        {
            if (Session < 1 || Session > 6
                || string.IsNullOrEmpty(Code.Trim())
                || string.IsNullOrEmpty(Title.Trim())
                || DB.Courses.ToList().Where(c => c.Code == Code && c.Id != Id).Count() > 0)
                return false;
            return true;
        }
    }
}