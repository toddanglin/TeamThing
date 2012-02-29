using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mercury.Model;
using Mercury.Data;
using Telerik.OpenAccess;
using System.Configuration;
using Mercury.Data.Util;
using System.Runtime.InteropServices;
using Mercury.Data.Logic;
using System.Threading;
using System.Diagnostics;

namespace Mercury.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = new Arguments(args);

            if (input.IsTrue("s"))
            {
                Trace.WriteLine("Run TeamThing in silent mode");

                if (!input.Exists("action"))
                    throw new ApplicationException("Cannot run silently without an action. Please set -action before running.");

                var action = input.Single("action");
                Trace.WriteLine(String.Format("Action value is {0}", action));
                if (action == "3")
                {
                    Trace.WriteLine("Executing ProcessImap method.");
                    ProcessImap(true);
                }
                else if (action == "4")
                {
                    //Send start of day remdiner email
                    SendStartReminder(true);
                }
                else if (action == "5")
                {
                    //Send start of day remdiner email
                    SendEndReminder(true);
                }

                Trace.Write("Exiting slient mode execution.");
                Environment.Exit(0);
            }
            else
                ShowMainMenu();
        }

        private static void SelectTeam()
        {
            System.Console.WriteLine("Enter the team ID");

            var id = System.Console.ReadLine();

            IObjectScope scope = InitScope();

            var tr = new TeamRepository(scope);
            var team = tr.GetByID(Convert.ToInt32(id));

            System.Console.WriteLine("Team \"{0}\" selected ({1} team members).", team.TeamName, team.TeamMembers.Count());

            System.Console.WriteLine("Press 1 to add Team Member.\n\rPress 2 to return to main menu.");

            var nextStep = System.Console.ReadLine();

            if (nextStep == "2")
            {
                ShowMainMenu();
            }
            else if (nextStep == "1")
            {
                bool addAnother = true;
                while (addAnother)
                {
                    AddTeamMember(tr, team, scope);

                    System.Console.WriteLine("Add another Team Member? (True/False)");
                    addAnother = Convert.ToBoolean(System.Console.ReadLine());
                }

                ShowMainMenu();
            }
        }

        private static IObjectScope InitScope()
        {
            var scope = ObjectScopeProvider1.GetNewObjectScope();
            scope.TransactionProperties.AutomaticBegin = true;
            return scope;
        }

        private static void CreateTeam()
        {
            System.Console.WriteLine("Enter Team Name");
            var teamName = System.Console.ReadLine();

            var team = new Team() 
            { 
                TeamName = teamName,
                IsEnabled = true                   
            };

            var scope = ObjectScopeProvider1.GetNewObjectScope();
            scope.TransactionProperties.AutomaticBegin = true;

            var tr = new TeamRepository(scope);

            tr.Add(team);

            scope.Transaction.Commit();

            System.Console.WriteLine("{0} team ID is {1}", teamName, team.TeamId);

            ShowMainMenu();
        }

        private static void ShowMainMenu()
        {
            System.Console.Clear();

            System.Console.WriteLine("Press '1' to create Team");
            System.Console.WriteLine("Press '2' to select Team by ID");
            System.Console.WriteLine("Press '3' to process IMAP items");
            System.Console.WriteLine("Press '4' to process start of day reminder");
            System.Console.WriteLine("Press '5' to process end of day reminder");
            System.Console.WriteLine("Press '0' to EXIT");

            var input = System.Console.ReadLine();

            if (input == "1") CreateTeam();
            else if (input == "2") SelectTeam();
            else if (input == "3") ProcessImap(false);
            else if (input == "4") SendStartReminder(false);
            else if (input == "5") SendEndReminder(false);
            else if (input == "0") Environment.Exit(0);
        }

        private static void ProcessImap(bool isSilent)
        {

            var scope = InitScope();
            var settings = ConfigurationManager.AppSettings;
            var tc = new TaskCollector(settings[StringConstants.CONFIG_KEY_EMAIL_USERNAME],
                                        settings[StringConstants.CONFIG_KEY_EMAIL_PASS],
                                        settings[StringConstants.CONFIG_KEY_EMAIL_SERVER],
                                        Convert.ToInt32(settings[StringConstants.CONFIG_KEY_EMAIL_PORT]),
                                        scope);

            //Process IMAP items for all team members on all teams
            var teamRepository = new TeamRepository(scope);
            var teams = teamRepository.GetAll().Where(t => t.TeamMembers.Count > 0 && t.IsEnabled == true).Select(t => t).ToList();
            foreach (var team in teams)
            {
                var tm = team.TeamMembers.ToList();
                if (!isSilent)
                    System.Console.WriteLine("TeamMembers on selected team: {0}", tm.Count());

                tc.ParseTasks(tm);
            }

            //Don't exit until all email is sent
            int timeout = 0;
            while (Emailer.IsEmailSending && timeout < 60)
            {
                System.Console.WriteLine("Waiting on async email to complete...");
                Thread.Sleep(500); //Wait 1/2 second for email to finish

                timeout++; //Timeout and allow close after 30 seconds
            }

            scope.Transaction.Commit();

            if (!isSilent)
            {
                var taskCount = new TaskRepository(scope).GetAll().Count();
                System.Console.WriteLine("Total tasks in database: {0}\n\nPress any key to return to main menu.", taskCount);
                System.Console.ReadKey();

                ShowMainMenu();
            }

            scope.Dispose();
        }

        private static void SendStartReminder(bool isSilent)
        {
            var scope = InitScope();

            //Process IMAP items for all team members on all teams
            var teamRepository = new TeamRepository(scope);
            var teams = teamRepository.GetAll().Where(t => t.TeamMembers.Count > 0 && t.IsEnabled == true).Select(t => new { TeamId = t.TeamId }).ToList();
            foreach (var team in teams)
            {
                var count = Emailer.SendStartOfDayReminder(team.TeamId, scope);
                System.Console.WriteLine("{0} emails sent for team with ID {1}", count, team.TeamId);
            }

            //Don't exit until all email is sent
            int timeout = 0;
            while (Emailer.IsEmailSending && timeout < 60)
            {
                System.Console.WriteLine("Waiting on async email to complete...");
                Thread.Sleep(500); //Wait 1/2 second for email to finish

                timeout++; //Timeout and allow close after 30 seconds
            }


            if (!isSilent)
            {
                System.Console.WriteLine("Start of day email reminders sent.");
                System.Console.ReadKey();

                ShowMainMenu();
            }

            scope.Dispose();
        }

        private static void SendEndReminder(bool isSilent)
        {
            var scope = InitScope();

            //Process email reminders for all teams
            var teamRepository = new TeamRepository(scope);
            var teams = teamRepository.GetAll().Where(t => t.TeamMembers.Count > 0 && t.IsEnabled == true).Select(t => new { TeamId = t.TeamId }).ToList();
            foreach (var team in teams)
            {
                var count = Emailer.SendEndOfDayReminder(team.TeamId, scope);
                System.Console.WriteLine("{0} emails sent for team with ID {1}", count, team.TeamId);
            }

            //Don't exit until all email is sent
            int timeout = 0;
            while (Emailer.IsEmailSending && timeout < 60)
            {
                System.Console.WriteLine("Waiting on async email to complete...");
                Thread.Sleep(500); //Wait 1/2 second for email to finish

                timeout++; //Timeout and allow close after 30 seconds
            }


            if (!isSilent)
            {
                System.Console.WriteLine("End of day email reminders sent.");
                System.Console.ReadKey();

                ShowMainMenu();
            }

            scope.Dispose();
        }

        private static void AddTeamMember(TeamRepository tr, Team team, IObjectScope scope)
        {
            System.Console.WriteLine("Enter new Team Member FirstName, LastName, Email (separated by spaces)");

            var rawIn = System.Console.ReadLine();
            var info = rawIn.Split(' ');

            tr.AddTeamMember(team.TeamId, info[0], info[1], info[2]);
            scope.Transaction.Commit();

            var count = team.TeamMembers.Count();

            System.Console.WriteLine("Team Member \"{0}\" added ({1} total members)", info[0], count);
        }
    }
}
