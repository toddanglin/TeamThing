using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mercury.Model
{
    public enum EmailType
    {
        General,
        EndOfDayReminder,
        StartOfDayReminder,
        TeamSummary,
        ErrorInformation,
        SuccessInformation
    }
}
