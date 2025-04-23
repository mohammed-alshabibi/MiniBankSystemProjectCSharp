namespace MiniBankSystemProject
{
    internal class Program
    {
        const string UserLogInFile = "login.txt";
        static List<string> names = new List<string>();
        static List<string> passwords = new List<string>();
        static List<string> accountNumbers = new List<string>();
        static List<double> balances = new List<double>();
        static Queue<string> createAccountRequests = new Queue<string>();
        static void Main(string[] args)
        {
            
        }
        //Function to create user account
        public static void CreateAccount()
        {
            Console.WriteLine("Enter your uesr name:");
            string name = Console.ReadLine();
            Console.WriteLine("Enter your password:");
            string password = Console.ReadLine();
            Console.WriteLine("Enter your account number:");
            string accountNumber = Console.ReadLine();
            Console.WriteLine("Enter your balance:");
            double balance =double.Parse( Console.ReadLine());
            string accountDetails = name+"|"+password + "|" + accountNumber + "|" + balance;
            createAccountRequests.Enqueue(accountDetails);
            Console.WriteLine("Account creation request sent to admin.");
        }
        // Function Log in user 
        public static void LogIn()
        {
            Console.WriteLine("Enter your user name:");
            string name = Console.ReadLine();
            Console.WriteLine("Enter your password:");
            string password = Console.ReadLine();
            if (names.Contains(name) && passwords.Contains(password))
            {
                int index = names.IndexOf(name);
                Console.WriteLine($"Welcome {name}, your account number is {accountNumbers[index]} and your balance is {balances[index]}");
            }
            else
            {
                Console.WriteLine("Invalid username or password.");
            }
        }

    }
}
