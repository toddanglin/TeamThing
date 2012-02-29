using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.OpenAccess;

namespace Mercury.Model
{
    [Serializable]
    [Telerik.OpenAccess.Persistent()]
    public class Task : ThingBase
    {        
        private TaskStatus status;
        private TaskType taskType;

        [FieldAlias("status")]
        public TaskStatus Status
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

        [FieldAlias("taskType")]
        public TaskType TaskType
        {
            get
            {
                return taskType;
            }
            set
            {
                taskType = value;
            }
        }
    }
}
