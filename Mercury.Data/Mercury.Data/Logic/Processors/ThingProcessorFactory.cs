using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mercury.Data.Logic
{
    public static class ThingProcessorFactory
    {
        public static IThingProcessor GetProcessor(string key)
        {
            key = key.ToLower();
            if (key.Contains("add"))
                return new AddThingProcessor();
            else if (key == "remove" || key == "delete" || key == "r")
                return new RemoveThingProcessor();
            else if (key == "done" || key == "finished" || key == "complete" || key == "d")
                return new DoneThingProcessor();
            else if (key == "undo" || key == "u")
                return new UndoThingProcessor();
            else if (key.Contains("delay"))
                return new DelayThingProcessor();
            else
                return null;
        }
    }
}
