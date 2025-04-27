using System;
using System.Collections.Generic;
using System.IO;


namespace MiniBankSystemProject
{
    internal class Program
    {
        // File to store user login information and account details
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
            try
            {
                // Load existing accounts and complaints from files
                LoadAccounts();
                LoadComplaints();
                WelcomeMenu();
            }catch(Exception ex)
            {
                Console.WriteLine($"Error during initialization: {ex.Message}");
            }
            
        }
        // Display the welcome menu and prompt user for selection
        public static void WelcomeMenu()
        {
            try
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
                    case "3": SaveAccounts(); SaveComplaints(); Environment.Exit(0); break;
                    default: Console.WriteLine("Invalid choice, please try again."); WelcomeMenu(); break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Welcome Menu: {ex.Message}");
            }

        }
        // Display the admin menu and prompt user for selection
        public static void AdminMenu()
        {
            try
            {
                bool exit = true;
                while (exit)
                {

                    Console.WriteLine("\n--- Admin Menu ---");
                    Console.WriteLine("1. Process Account Requests");
                    Console.WriteLine("2. Search Account by Name/National ID");
                    Console.WriteLine("3. Delete Account");
                    Console.WriteLine("4. View All Accounts");
                    Console.WriteLine("5. Show Total Bank Balance");
                    Console.WriteLine("6. Export All Accounts");
                    Console.WriteLine("7. View All Complaints");
                    Console.WriteLine("8. Back to Main Menu");
                    Console.Write("Select an option: ");
                    string choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1": ProcessCreateBankAccountRequest(); break;
                        case "2": SearchAccount(); break;
                        case "3": DeleteAccount(); break;
                        case "4": ViewAllAccounts(); break;
                        case "5": ShowTotalBankBalance(); break;
                        case "6": ExportAccounts(); break;
                        case "7": ViewAllComplaints(); break;
                        case "8": WelcomeMenu(); break;
                        default: Console.WriteLine("Invalid choice."); AdminMenu(); exit = false; break;
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Admin Menu: {ex.Message}");
            }


            }
        // Display the user menu and prompt user for selection
        public static void UserMenu()
        {
            try
            {
                bool flag = true;
                while (flag)
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
                        default: Console.WriteLine("Invalid choice."); UserMenu(); flag = false; break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in User Menu: {ex.Message}");
            }


            }
        // Create a new account and add it to the lists
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
        // Request to create a new bank account
        public static void RequestCreateBankAccount()
        {
            try
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
                // Check if the account already exists or if a request is pending
                if (nationalIds.Contains(nationalId) || createAccountRequests.Any(r => r.Contains(nationalId)))
                {
                    Console.WriteLine("Account already exists or request pending.");
                    return;
                }

                createAccountRequests.Enqueue(name + "|" + nationalId + "|" + password + "|" + initialBalance);
                Console.WriteLine("Account request submitted!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in RequestCreateBankAccount: {ex.Message}");

            }
        }
        // Process the account creation request
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
        // Login to an existing user account
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
        // Display the user bank menu and prompt user for selection
        public static void UserBankMenu(int index)
        {
            try
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
            }catch (Exception ex)
            {
                Console.WriteLine($"Error in UserBankMenu: {ex.Message}");
            }

            UserBankMenu(index);
        }
        // Search for an account by national ID
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
        // Delete an account
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
        // View all accounts
        public static void ViewAllAccounts()
        {
            for (int i = 0; i < names.Count; i++)
            {
                Console.WriteLine($"{accountNumbers[i]} | {names[i]} | {balances[i]:C}");
            }
        }
        // Show the total bank balance
        public static void ShowTotalBankBalance()
        {
            double total = balances.Sum();
            Console.WriteLine($"Total Bank Balance: {total:C}");
        }
        // Export all accounts to a file
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
        // Submit a complaint
        public static void SubmitComplaint()
        {
            Console.Write("Enter complaint: ");
            complaints.Push(Console.ReadLine());
            Console.WriteLine("Complaint submitted.");
        }
        // Undo the last complaint
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
        // Print a receipt for transactions
        public static void PrintReceipt(string type, int index, double amount)
        {
            string receipt = $"{type} Receipt\nName: {names[index]}\nAccount#: {accountNumbers[index]}\nAmount: {amount:C}\nBalance: {balances[index]:C}\nDate: {DateTime.Now}";
            Console.WriteLine(receipt);
            File.AppendAllText("receipts.txt", receipt + "\n\n");
        }
        // save accounts in to login file
        public static void SaveAccounts()
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(UserLogInFile))
                {
                    for (int i = 0; i < names.Count; i++)
                        sw.WriteLine($"{names[i]}|{nationalIds[i]}|{passwords[i]}|{balances[i]}|{accountNumbers[i]}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving accounts: {ex.Message}");
            }

        }
        // load accounts from file to list
        public static void LoadAccounts()
        {
            try
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
                    if (accountNumbers.Count > 0)
                    {
                        lastAccountNumber = accountNumbers.Max(x => int.Parse(x));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading accounts: {ex.Message}");
            }

        }
        // save complaints into file
        public static void SaveComplaints()
        {
            try
            {
                using (StreamWriter sw = new StreamWriter("complaints.txt"))
                {
                    foreach (var complaint in complaints)
                    {
                        sw.WriteLine(complaint);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving complaints: {ex.Message}");
            }

        }
        // load complaints from file to list
        public static void LoadComplaints()
        {
            try
            {
                if (File.Exists("complaints.txt"))
                {
                    foreach (var line in File.ReadAllLines("complaints.txt"))
                    {
                        complaints.Push(line);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading complaints: {ex.Message}");
            }

        }
        // admin view all complaints from user
        public static void ViewAllComplaints()
        {
            try
            {
                if (complaints.Count == 0)
                {
                    Console.WriteLine("No complaints available.");
                }
                else
                {
                    Console.WriteLine("\n--- All User Complaints ---");
                    foreach (var complaint in complaints)
                    {
                        Console.WriteLine($"- {complaint}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error viewing complaints: {ex.Message}");
            }

            AdminMenu();
        }


    }
}
