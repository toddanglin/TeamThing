using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.Web;
using Telerik.OpenAccess;
using Mercury.Data;
using Mercury.Model;

namespace Mercury.Dash.Mvc.Services
{
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class TaskService
    {
        [WebGet]
        [OperationContract]
        public List<TaskDto> GetTeamMemberTasks(Guid key)
        {
            //TODO:
            //1) Find team member based on key
            //2) Get their tasks for today (only)
            //3) Return list as JSON

            var scope = (IObjectScope)HttpContext.Current.Items[Mercury.Dash.Mvc.MvcApplication.SCOPE_KEY];
            if (scope == null)
                scope = ObjectScopeProvider1.GetPerRequestScope(HttpContext.Current);

            var teamRepository = new TeamRepository(scope);
            var tm = teamRepository.GetTeamMemberByKey(key);

            var taskRepostiory = new TaskRepository(scope);
            var tasks = taskRepostiory.GetTodayTasksByPerson(tm);

            var taskList = new List<TaskDto>();
            foreach (var t in tasks)
            {
                var dto = new TaskDto() 
                { 
                    Description = t.Description,
                    DateCreated = t.DateCreated,
                    FirstName = t.TeamMember.FirstName,
                    LastName = t.TeamMember.LastName,
                    ProfileColor = t.TeamMember.ProfileColor,
                    Status = t.Status.ToString()
                };

                taskList.Add(dto);
            }

            return taskList;
        }

        [WebGet]
        [OperationContract]
        public int UpdateTaskStatus(Guid key, string description, string newstatus)
        {
            //TODO:
            //1) Find team member based on key
            //2) Get their tasks for today (only)
            //3) Return list as JSON

            //Return a status code
            //200 = success
            //304 = success, not modified
            //404 = Task not found
            //403 = Team Member not found
            //500 = Bad status string

            var scope = (IObjectScope)HttpContext.Current.Items[Mercury.Dash.Mvc.MvcApplication.SCOPE_KEY];
            if (scope == null)
                scope = ObjectScopeProvider1.GetPerRequestScope(HttpContext.Current);
            scope.TransactionProperties.AutomaticBegin = true;            

            var teamRepository = new TeamRepository(scope);
            var tm = teamRepository.GetTeamMemberByKey(key);

            if (tm == null)
                return 403;
            
            //Decode description
            description = HttpContext.Current.Server.UrlDecode(description).Trim();

            var taskRepostiory = new TaskRepository(scope);
            var task = taskRepostiory.FindOriginalTask(description, tm);

            if(task == null)
                return 404;

            TaskStatus status;
            if(!Enum.TryParse<TaskStatus>(newstatus, out status))
                return 500;
            
            if (task.Status == status) //no change needed
                return 304;

            task.Status = status;

            scope.Transaction.Commit();

            return 200;
        }
    }

    [Serializable]
    public class TaskDto
    {
        private string description;
        private DateTime dateCreated;
        private string firstName;
        private string lastName;
        private string profileColor;
        private string status;

        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                description = value;
            }
        }

        public DateTime DateCreated
        {
            get
            {
                return dateCreated;
            }
            set
            {
                dateCreated = value;
            }
        }

        public string FirstName
        {
            get
            {
                return firstName;
            }
            set
            {
                firstName = value;
            }
        }

        public string LastName
        {
            get
            {
                return lastName;
            }
            set
            {
                lastName = value;
            }
        }

        public string ProfileColor
        {
            get
            {
                return profileColor;
            }
            set
            {
                profileColor = value;
            }
        }

        public string Status
        {
            get
            {
                return status;
            }
            set
            {
                status = value;
            }
        }

    }
}
