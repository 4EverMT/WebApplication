using EmailHandling;
using Models;
using PhotosManager.Models;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Hosting;

namespace DAL
{
    public sealed class DB
    {
        #region singleton setup
        private static readonly DB instance = new DB();
        public static DB Instance { get { return instance; } }
        #endregion

        public static StudentsRepository Students { get; set; } = new StudentsRepository();

        public static TeachersRepository Teachers { get; set; } = new TeachersRepository();

        public static RegistrationsRepository Registrations { get; set; } = new RegistrationsRepository();
        public static AllocationsRepository Allocations { get; set; } = new AllocationsRepository();
        public static CoursesRepository Courses { get; set; } = new CoursesRepository();

        static public UsersRepository Users { get; set; }
            = new UsersRepository();

        static public NotificationsRepository Notifications { get; set; }
            = new NotificationsRepository();

        static public LoginsRepository Logins { get; set; }
            = new LoginsRepository();

        static public EventsRepository Events { get; set; }
            = new EventsRepository();

        static public Repository<UnverifiedEmail> UnverifiedEmails { get; set; }
            = new Repository<UnverifiedEmail>();

        static public Repository<RenewPasswordCommand> RenewPasswordCommands { get; set; }
            = new Repository<RenewPasswordCommand>();
    }
}