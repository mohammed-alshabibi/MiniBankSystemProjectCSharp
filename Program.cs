using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MiniBankSystemProject
{
    internal class Program
    {
        const string UserLogInFile = "accounts.txt";
        static List<string> names = new List<string>();
        static List<string> nationalIds = new List<string>();
        static List<string> passwords = new List<string>();
        static List<double> balances = new List<double>();
        static List<string> accountNumbers = new List<string>();
        static Queue<string> createAccountRequests = new Queue<string>();
        static Stack<string> complaints = new Stack<string>();
        static int lastAccountNumber = 1000;

        static void Main(string[] args)
        {
            LoadAccounts();
            WelcomeMenu();
        }

        public static void WelcomeMenu()
        {
            Console.WriteLine("\n--- Welcome to the Mini Bank System ---");
            Console.WriteLine("1. Admin");
            Console.WriteLine("2. User");
            Console.WriteLine("3. Exit");
            Console.Write("Please select an option: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1": AdminMenu(); break;
                case "2": UserMenu(); break;
                case "3": SaveAccounts(); Environment.Exit(0); break;
                default: Console.WriteLine("Invalid choice, please try again."); WelcomeMenu(); break;
            }
        }

        public static void AdminMenu()
        {
            bool exit = true;
            while (exit) 
            {
                
                Console.WriteLine("\n--- Admin Menu ---");
                Console.WriteLine("1. Process Account Requests");
                Console.WriteLine("2. Search Account by Name/National ID");
                Console.WriteLine("3. Delete Account");
                Console.WriteLine("4. View All Accounts");
                Console.WriteLine("5. Show Top 3 Richest Customers");
                Console.WriteLine("6. Show Total Bank Balance");
                Console.WriteLine("7. Export All Accounts");
                Console.WriteLine("8. Back to Main Menu");
                Console.Write("Select an option: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": ProcessCreateBankAccountRequest(); break;
                    case "2": SearchAccount(); break;
                    case "3": DeleteAccount(); break;
                    case "4": ViewAllAccounts(); break;
                    case "5": ShowTop3Richest(); break;
                    case "6": ShowTotalBankBalance(); break;
                    case "7": ExportAccounts(); break;
                    case "8": WelcomeMenu(); break;
                    default: Console.WriteLine("Invalid choice."); AdminMenu();exit = false; break;
                }
              
            }
            
        }

        public static void UserMenu()
        {
            Console.WriteLine("\n--- User Menu ---");
            Console.WriteLine("1. Create Account Request");
            Console.WriteLine("2. Login");
            Console.WriteLine("3. Submit Complaint");
            Console.WriteLine("4. Undo Last Complaint");
            Console.WriteLine("5. Back to Main Menu");
            Console.Write("Select an option: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1": RequestCreateBankAccount(); break;
                case "2": LoginUserAccount(); break;
                case "3": SubmitComplaint(); break;
                case "4": UndoLastComplaint(); break;
                case "5": WelcomeMenu(); break;
                default: Console.WriteLine("Invalid choice."); UserMenu(); break;
            }
        }

        public static void CreateAccount(string name, string nationalId, string password, double balance)
        {
            lastAccountNumber++;
            names.Add(name);
            nationalIds.Add(nationalId);
            passwords.Add(password);
            balances.Add(balance);
            accountNumbers.Add(lastAccountNumber.ToString());
            Console.WriteLine($"Account created! Account Number: {lastAccountNumber}");
        }

        public static void RequestCreateBankAccount()
        {
            Console.Write("Enter your name: ");
            string name = Console.ReadLine();
            Console.Write("Enter your national ID: ");
            string nationalId = Console.ReadLine();
            Console.Write("Enter your password: ");
            string password = Console.ReadLine();
            Console.Write("Enter initial balance: ");

            if (!double.TryParse(Console.ReadLine(), out double initialBalance))
            {
                Console.WriteLine("Invalid balance.");
                return;
            }

            if (nationalIds.Contains(nationalId) || createAccountRequests.Any(r => r.Contains(nationalId)))
            {
                Console.WriteLine("Account already exists or request pending.");
                return;
            }

            createAccountRequests.Enqueue(name + "|" + nationalId + "|" + password + "|" + initialBalance);
            Console.WriteLine("Account request submitted!");
        }

        public static void ProcessCreateBankAccountRequest()
        {
            if (createAccountRequests.Count == 0)
            {
                Console.WriteLine("No requests to process.");
                return;
            }

            string request = createAccountRequests.Dequeue();
            var parts = request.Split('|');
            CreateAccount(parts[0], parts[1], parts[2], double.Parse(parts[3]));
        }

        public static void LoginUserAccount()
        {
            Console.Write("Enter National ID: ");
            string nationalId = Console.ReadLine();
            Console.Write("Enter password: ");
            string password = Console.ReadLine();

            int index = nationalIds.IndexOf(nationalId);
            if (index != -1 && passwords[index] == password)
            {
                UserBankMenu(index);
            }
            else
            {
                Console.WriteLine("Login failed.");
            }
        }

        public static void UserBankMenu(int index)
        {
            Console.WriteLine($"\n--- Welcome {names[index]} ---");
            Console.WriteLine("1. View Balance");
            Console.WriteLine("2. Deposit");
            Console.WriteLine("3. Withdraw");
            Console.WriteLine("4. Transfer");
            Console.WriteLine("5. Logout");
            Console.Write("Select an option: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.WriteLine($"Your balance is {balances[index]:C}");
                    break;
                case "2":
                    Console.Write("Enter deposit amount: ");
                    if (double.TryParse(Console.ReadLine(), out double deposit))
                    {
                        balances[index] += deposit;
                        PrintReceipt("Deposit", index, deposit);
                    }
                    break;
                case "3":
                    Console.Write("Enter withdraw amount: ");
                    if (double.TryParse(Console.ReadLine(), out double withdraw) && balances[index] >= withdraw)
                    {
                        balances[index] -= withdraw;
                        PrintReceipt("Withdraw", index, withdraw);
                    }
                    else
                    {
                        Console.WriteLine("Insufficient funds.");
                    }
                    break;
                case "4":
                    Console.Write("Enter recipient account number: ");
                    string recipient = Console.ReadLine();
                    int receiverIndex = accountNumbers.IndexOf(recipient);
                    if (receiverIndex != -1)
                    {
                        Console.Write("Enter amount to transfer: ");
                        if (double.TryParse(Console.ReadLine(), out double amount) && balances[index] >= amount)
                        {
                            balances[index] -= amount;
                            balances[receiverIndex] += amount;
                            Console.WriteLine("Transfer successful.");
                        }
                        else
                            Console.WriteLine("Invalid amount or insufficient funds.");
                    }
                    else
                        Console.WriteLine("Recipient not found.");
                    break;
                case "5": WelcomeMenu(); break;
                default: Console.WriteLine("Invalid option."); break;
            }
            UserBankMenu(index);
        }

        public static void SearchAccount()
        {
            Console.Write("Enter Name or National ID to search: ");
            string input = Console.ReadLine();
            int index = names.IndexOf(input);
            if (index == -1)
                index = nationalIds.IndexOf(input);

            if (index != -1)
                Console.WriteLine($"Account: {accountNumbers[index]}, Balance: {balances[index]:C}");
            else
                Console.WriteLine("Account not found.");
        }

        public static void DeleteAccount()
        {
            Console.Write("Enter Account Number to delete: ");
            string accNum = Console.ReadLine();
            int index = accountNumbers.IndexOf(accNum);
            if (index != -1)
            {
                names.RemoveAt(index);
                nationalIds.RemoveAt(index);
                passwords.RemoveAt(index);
                balances.RemoveAt(index);
                accountNumbers.RemoveAt(index);
                Console.WriteLine("Account deleted.");
            }
            else
                Console.WriteLine("Account not found.");
        }

        public static void ViewAllAccounts()
        {
            for (int i = 0; i < names.Count; i++)
            {
                Console.WriteLine($"{accountNumbers[i]} | {names[i]} | {balances[i]:C}");
            }
        }

        public static void ShowTop3Richest()
        {
            var top = balances.Select((bal, i) => new { bal, i }).OrderByDescending(x => x.bal).Take(3);
            foreach (var acc in top)
                Console.WriteLine($"{names[acc.i]} - {balances[acc.i]:C}");
        }

        public static void ShowTotalBankBalance()
        {
            double total = balances.Sum();
            Console.WriteLine($"Total Bank Balance: {total:C}");
        }

        public static void ExportAccounts()
        {
            using (StreamWriter sw = new StreamWriter("ExportedAccounts.txt"))
            {
                sw.WriteLine("AccountNumber,Name,NationalID,Balance");
                for (int i = 0; i < names.Count; i++)
                    sw.WriteLine($"{accountNumbers[i]},{names[i]},{nationalIds[i]},{balances[i]}");
            }
            Console.WriteLine("Exported successfully.");
        }

        public static void SubmitComplaint()
        {
            Console.Write("Enter complaint: ");
            complaints.Push(Console.ReadLine());
            Console.WriteLine("Complaint submitted.");
        }

        public static void UndoLastComplaint()
        {
            if (complaints.Count > 0)
            {
                complaints.Pop();
                Console.WriteLine("Last complaint undone.");
            }
            else
                Console.WriteLine("No complaints to undo.");
        }

        public static void PrintReceipt(string type, int index, double amount)
        {
            string receipt = $"{type} Receipt\nName: {names[index]}\nAccount#: {accountNumbers[index]}\nAmount: {amount:C}\nBalance: {balances[index]:C}\nDate: {DateTime.Now}";
            Console.WriteLine(receipt);
            File.AppendAllText("receipts.txt", receipt + "\n\n");
        }

        public static void SaveAccounts()
        {
            using (StreamWriter sw = new StreamWriter(UserLogInFile))
            {
                for (int i = 0; i < names.Count; i++)
                    sw.WriteLine($"{names[i]}|{nationalIds[i]}|{passwords[i]}|{balances[i]}|{accountNumbers[i]}");
            }
        }

        public static void LoadAccounts()
        {
            if (File.Exists(UserLogInFile))
            {
                foreach (var line in File.ReadAllLines(UserLogInFile))
                {
                    var parts = line.Split('|');
                    names.Add(parts[0]);
                    nationalIds.Add(parts[1]);
                    passwords.Add(parts[2]);
                    balances.Add(double.Parse(parts[3]));
                    accountNumbers.Add(parts[4]);
                }
            }
        }
    }
}
