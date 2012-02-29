using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mercury.Data.Util
{
    public interface IDataItemContainerLite : INamingContainerLite
    {
            // Properties
            object DataItem { get; }
            int DataItemIndex { get; }
            int DisplayIndex { get; }
    }
}
