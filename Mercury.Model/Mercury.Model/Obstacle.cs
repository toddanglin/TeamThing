using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.OpenAccess;

namespace Mercury.Model
{
    [Serializable]
    [Telerik.OpenAccess.Persistent()]
    public class Obstacle : ThingBase
    {
        private bool isCleared;

        [FieldAlias("isCleared")]
        public bool IsCleared
        {
            get
            {
                return isCleared;
            }
            set
            {
                isCleared = value;
            }
        }
    }
}
