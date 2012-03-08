using System;
using System.IO;
using System.Linq;
using OpenAccessRuntime.product;
using TeamThing.Model;

namespace TeamThing.ModelBootstrapper
{
    class Program
    {
        static void Main(string[] args)
        {
            var script = DDLBuilder.GenerateAndExecuteScript<TeamThingContext>(Console.Out);

            ////log the script
            LogScript(script);

            TestContext();
            Console.WriteLine("Done, press any key to continue...");
            Console.ReadLine();
        }

        private static void TestContext()
        {
            Console.WriteLine("Running Tests");
            using (var context = new TeamThingContext())
            {
                //add some new data
                User newUser = new User("holt@telerik.com");
                newUser.FirstName = "Josh";
                newUser.LastName = "Holt";
                context.Add(newUser);
                context.SaveChanges();

                //add some new data
                User newUser2 = new User("jholt456@gmail.com");
                newUser2.FirstName = "Josh2";
                newUser2.LastName = "Holt2";
                context.Add(newUser2);
                context.SaveChanges();

                Team newTeam = new Team("Closed Team", newUser, false);
                context.Add(newTeam);
                context.SaveChanges();

                Thing thing = new Thing(newUser);
                thing.Description = "Test Thing";
                thing.AssignedTo.Add(new UserThing(thing, newUser2, newUser));
                context.Add(thing);
                context.SaveChanges();

                ////recall the saved items
                //IProduct retrievedProduct = context.Products.FirstOrDefault(p => p.Name == newProduct.Name);
                //ICategory retrievedCategory = context.Categories.FirstOrDefault(p => p.Name == newCategory.Name);
                //Debug.Assert(retrievedProduct != null);
                //Debug.Assert(retrievedCategory!= null);
                //Debug.Assert(retrievedProduct.Category == retrievedCategory);
                //Debug.Assert(retrievedCategory.Products.Contains(retrievedProduct));


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
