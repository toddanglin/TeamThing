using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using TeamThing.Model;

namespace TeamThing.ModelBootstrapper
{
    class Program
    {
        static void Main(string[] args)
        {
            var script = DDLBuilder.GenerateScript<TeamThingContext>(Console.Out);

            ////log the script
            LogScript(script);

            TestContext();
            Console.WriteLine("Done, press any key to continue...");
            Console.ReadLine();
        }

        private static void TestContext()
        {
            Console.WriteLine("Running Tests");

            User newUser;
            User newUser2;
            Team newTeam;
            Thing thing;
            using (var context = new TeamThingContext())
            {
                //add some new data
                newUser = new User("UNIT TEST USER");
                newUser.FirstName = "Josh";
                newUser.LastName = "Holt";
                context.Add(newUser);
                context.SaveChanges();

                //add some new data
                newUser2 = new User("UNIT TEST USER 2");
                newUser2.FirstName = "Josh2";
                newUser2.LastName = "Holt2";
                context.Add(newUser2);
                context.SaveChanges();

                newTeam = new Team("UNIT TEST TEAM", newUser, false);
                newTeam.Members.Add(new TeamUser(newTeam, newUser2));
                context.Add(newTeam);
                context.SaveChanges();

                thing = new Thing(newTeam, newUser);
                thing.Description = "UNIT TEST THING";
                thing.AssignedTo.Add(new UserThing(thing, newUser2, newUser));
                thing.AssignedTo.Add(new UserThing(thing, newUser, newUser));



                context.Add(thing);
                context.SaveChanges();

                thing.UpdateStatus(newUser2, ThingStatus.Delayed);
                thing.Complete(newUser2);

                context.SaveChanges();
            }

            using (var context = new TeamThingContext())
            {
                newUser = context.GetAll<User>().FirstOrDefault(p => p.EmailAddress == newUser.EmailAddress);
                newUser2 = context.GetAll<User>().FirstOrDefault(p => p.EmailAddress == newUser2.EmailAddress);
                newTeam = context.GetAll<Team>().FirstOrDefault(p => p.Name == newTeam.Name);
                thing = context.GetAll<Thing>().FirstOrDefault(p => p.Description == thing.Description);

                Debug.Assert(newUser != null);
                Debug.Assert(newUser2 != null);
                Debug.Assert(newTeam != null);
                Debug.Assert(thing != null);

                Debug.Assert(thing.Owner != null);
                Debug.Assert(newTeam.Owner != null);

                Debug.Assert(newUser.Teams.Count > 0);

                Debug.Assert(newUser2.Teams.Count > 0);
                Debug.Assert(newUser2.Things.Count > 0);

                Debug.Assert(newTeam.Members.Count > 0);
                //Debug.Assert(newTeam.TeamThings.Count > 0);

                Debug.Assert(thing.AssignedTo.Count > 0);

                Debug.Assert(thing.History.Count == 2);


                Debug.Assert(thing.History[0].EditedBy != null);
                Debug.Assert(thing.History[0].EditedByUserId != 0);

                Debug.Assert(thing.History[0].Thing == thing);
                Debug.Assert(thing.History[0].ThingId == thing.Id);

                //        //clean up
                //        //Category is marked as dependent, so it will be removed automatically with the product
                context.Delete(thing);
                context.Delete(newTeam);
                context.Delete(newUser);
                context.Delete(newUser2);

                context.SaveChanges();
            }

            Console.WriteLine("All Tests Pass!");
        }

        private static void LogScript(string script)
        {
            //log the script
            if (!string.IsNullOrEmpty(script))
            {
                Console.Write(script);

                var fileName = string.Format("text-{0:yyyy-MM-dd_hh-mm-ss-tt}.sql", DateTime.Now);
                var path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

                path = Path.Combine(path, "Scripts");

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                path = Path.Combine(path, fileName);

                using (Stream stream = File.Open(path, FileMode.CreateNew))
                using (TextWriter writer = new StreamWriter(stream))
                {
                    writer.Write(script);
                }
            }
        }
    }
}
