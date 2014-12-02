using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrowUp.IoC
{
    /// <summary>
    /// Регистрация типов в контейнера инверсии управления
    /// </summary>
    class RegistrationHelper
    {
        public static void Configure(Container container)
        {
            //container.Register<IProjectsService, Projects>();
            //container.Register<IProjectsRepository, ProjectsData>();

            //container.Register<ITasksService, Tasks>();
            //container.Register<ITasksRepository, TaskData>();

            //container.Register<IStatusService, Statuses>();
            //container.Register<IStatusRepository, StatusData>();

            //container.Register<IPersonsService, Persons>();
            //container.Register<IPersonsRepository, PersonsData>();

            //container.Register<DbHelper, DbHelper>();
        }
    }
}
