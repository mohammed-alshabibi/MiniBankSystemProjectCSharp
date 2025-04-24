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
        static double initialBalance;
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
        // request to create bank account that enter bank account initial balance and link bank account with national id that enter user in create user account
        public static void RequestCreateBankAccount()
        {
            Console.Write("Enter your name with national ID: ");
            string UserNationalId = Console.ReadLine();
            Console.Write("Enter your initial balance: ");
            if (!double.TryParse(Console.ReadLine(), out initialBalance))
            {
                Console.WriteLine("Invalid balance, please try again.");
                return;
            }
            string request = UserNationalId + "|" + initialBalance.ToString();
            createAccountRequests.Enqueue(request);
            Console.WriteLine("Request to create bank account has been submitted.");
        }
        // proccess request to create bank account that check if user name with national id exist in user name list 
        public static void ProcessCreateBankAccountRequest()
        {
            if (createAccountRequests.Count == 0)
            {
                Console.WriteLine("No requests to process.");
                return;
            }
            string request = createAccountRequests.Dequeue();
            string[] requestParts = request.Split('|');
            string UserNationalId = requestParts[0];
            double initialBalance = double.Parse(requestParts[1]);
            if (UserNames.Contains(UserNationalId))
            {
                lastAccountNumber++;
                accountNumbers.Add(lastAccountNumber.ToString());
                balances.Add(initialBalance);
                Console.WriteLine($"Bank account created successfully! Your account number is {lastAccountNumber}.");
            }
            else
            {
                Console.WriteLine("User does not exist, cannot create bank account.");
            }
        }
        //deposit money to bank account first check if user name with national id exist in user name list and then check if account number exist in account number list and then check if admin accepted the request to create bank account
        public static void DepositMoney()
        {
            Console.Write("Enter your name with national ID: ");
            string UserNationalId = Console.ReadLine();
            Console.Write("Enter your account number: ");
            string accountNumber = Console.ReadLine();
            Console.Write("Enter the amount to deposit: ");
            if (!double.TryParse(Console.ReadLine(), out double depositAmount))
            {
                Console.WriteLine("Invalid amount, please try again.");
                return;
            }
            if (UserNames.Contains(UserNationalId) && accountNumbers.Contains(accountNumber))
            {
                int index = accountNumbers.IndexOf(accountNumber);
                balances[index] += depositAmount;
                Console.WriteLine($"Deposit successful! Your new balance is {balances[index]}.");
            }
            else
            {
                Console.WriteLine("Invalid user or account number, please try again.");
            }
        }
        // withdraw money from bank account first check if user name with national id exist in user name list and then check if account number exist in account number list and then check if admin accepted the request to create bank account
        public static void WithdrawMoney()
        {
            Console.Write("Enter your name with national ID: ");
            string UserNationalId = Console.ReadLine();
            Console.Write("Enter your account number: ");
            string accountNumber = Console.ReadLine();
            Console.Write("Enter the amount to withdraw: ");
            if (!double.TryParse(Console.ReadLine(), out double withdrawAmount))
            {
                Console.WriteLine("Invalid amount, please try again.");
                return;
            }
            if (UserNames.Contains(UserNationalId) && accountNumbers.Contains(accountNumber))
            {
                int index = accountNumbers.IndexOf(accountNumber);
                if (balances[index] >= withdrawAmount)
                {
                    balances[index] -= withdrawAmount;
                    Console.WriteLine($"Withdrawal successful! Your new balance is {balances[index]}.");
                }
                else
                {
                    Console.WriteLine("Insufficient funds, please try again.");
                }
            }
            else
            {
                Console.WriteLine("Invalid user or account number, please try again.");
            }
        }
        

    }

}
