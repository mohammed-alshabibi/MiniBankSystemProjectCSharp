using System;

namespace MiniBankSystemProject
{
    internal class Program
    {
        const string UserLogInFile = "login.txt";
        static List<string> UserNames = new List<string>();
        static List<string> passwords = new List<string>();
        static List<string> accountNumbers = new List<string>();
        static List<double> balances = new List<double>();
        static Queue<string> createAccountRequests = new Queue<string>();
        static int lastAccountNumber;

        static void Main(string[] args)
        {

        }
        // welcome menu 
        public static void WelcomeMenu()
        {
            Console.WriteLine("Welcome to the Mini Bank System");
            Console.WriteLine("1.Admin");
            Console.WriteLine("2.User");
            Console.WriteLine("3. Exit");
            Console.Write("Please select an option: ");
            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    //AdminMenu();
                    break;
                case "2":
                    //UserMenu();
                    break;
                case "3":
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Invalid choice, please try again.");
                    WelcomeMenu();
                    break;
            }
        }
        //create user account that enter user name with national id and password
        public static void CreateUserAccount()
        {
            Console.Write("Enter your name: ");
            string name = Console.ReadLine();
            Console.Write("Enter your national ID: ");
            string nationalId = Console.ReadLine();
            Console.Write("Enter your password: ");
            string password = Console.ReadLine();
            if (UserNames.Contains(name))
            {
                Console.WriteLine("User already exists.");
                return;
            }
            UserNames.Add(name + nationalId);
            passwords.Add(password);
            lastAccountNumber++;
            accountNumbers.Add(lastAccountNumber.ToString());
            balances.Add(0.0);
            Console.WriteLine($"Account created successfully! Your account number is {lastAccountNumber}.");
        }
        // login user account that enter user name with national id and password
        public static void LoginUserAccount()
        {
            Console.Write("Enter your name with national ID: ");
            string name = Console.ReadLine();
            string nationalId = Console.ReadLine();
            Console.Write("Enter your password: ");
            string password = Console.ReadLine();
            string userId = name + nationalId;
            if (UserNames.Contains(userId) && passwords[UserNames.IndexOf(userId)] == password)
            {
                Console.WriteLine("Login successful!");
                // UserMenu();
            }
            else
            {
                Console.WriteLine("Invalid credentials, please try again.");
                LoginUserAccount();
            }

        }
        // create admin account that enter user name with national id and password
        public static void CreateAdminAccount()
        {
            Console.Write("Enter your national ID: ");
            string nationalId = Console.ReadLine();
            Console.Write("Enter your password: ");
            string password = Console.ReadLine();
            string AdminId = "Admin" + nationalId;
            if (UserNames.Contains(AdminId))
            {
                Console.WriteLine("Admin already exists.");
                return;
            }
            UserNames.Add(AdminId);
            passwords.Add(password);
            lastAccountNumber++;
            accountNumbers.Add(lastAccountNumber.ToString());
            balances.Add(0.0);
            Console.WriteLine($"Admin account created successfully! Your account number is {lastAccountNumber}.");
        }

        // login admin account that enter Admin+national id and password and check if admin or normal user
        public static void LoginAdminAccount()
        {
            Console.Write("Enter your national ID: ");
            string nationalId = Console.ReadLine();
            Console.Write("Enter your password: ");
            string password = Console.ReadLine();
            string AdminId = "Admin" + nationalId;
            if (UserNames.Contains(AdminId) && passwords[UserNames.IndexOf(AdminId)] == password)
            {
                Console.WriteLine("Login successful!");
                // AdminMenu();
            }
            else
            {
                Console.WriteLine("Invalid credentials, please try again.");
                LoginAdminAccount();
            }

        }

    }

}
