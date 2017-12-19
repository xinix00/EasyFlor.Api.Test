using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            string username;
            string password;
            string database;
            string test;

            var tests = @"
1.One time data, only to start syncing
   - All purchases, creditinvoices, debetinvoices, packagings and payments
2.Daily data
   - All articles, groups, employees, suppliers, debtors, lists and listgroups
3.Incremental data
   - 3 days of creditinvoices, debetinvoices, payments, packagings and purchases
4.Speedtest
    - Tests speed of realtime endpoints(getting prices for articles) ";

            if (args.Length == 0)
            {
                Console.WriteLine("-------------------------------------------");
                Console.WriteLine("           EasyFlor Export test");
                Console.WriteLine("-------------------------------------------");
                Console.WriteLine("* press enter after each command.");
                Console.WriteLine("Username: ");
                username = Console.ReadLine();
                Console.WriteLine("Password: ");
                password = Console.ReadLine();
                Console.WriteLine("Database: ");
                database = Console.ReadLine();

                Console.WriteLine(@"What test to run?" + tests);
                test = Console.ReadLine();
            }
            else if(args.Length != 4)
            {
                Console.WriteLine("Api.Test.exe <username> <password> <database> <test>");
                Console.WriteLine("Tests:" + tests);
                Console.ReadLine();
                return;
            }
            else
            {
                username = args[0];
                password = args[1];
                database = args[2];
                test = args[3];
            }

            using (var item = new ApiTester())
            {
                if(test == "1")
                {
                    item.OneTimeTest(database, username, password);
                }
                else if(test == "2")
                {
                    item.DailyTest(database, username, password);
                }
                else if (test == "3")
                {
                    item.IncrementalTest(database, username, password);
                }
                else if (test == "4")
                {
                    item.SpeedTest(database, username, password);
                }
            }
        }
    }
}
