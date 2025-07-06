using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Transactions;


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
        static List<string> transferHistory = new List<string>();
        static int lastAccountNumber = 1000;
        static List<int> failedLoginAttempts = new List<int>();
        static List<bool> isLocked = new List<bool>();
        static List<List<string>> allTransactions = new List<List<string>>();
        const string TransactionFile = "transactions.txt";
        static List<double> loanAmounts = new List<double>();
        static List<double> loanInterestRates = new List<double>();
        static List<string> loanStatus = new List<string>(); // None, Pending, Approved, Rejected
        static List<List<int>> feedbackScores = new List<List<int>>();
        const double USD_TO_OMR = 3.8;
        const double EUR_TO_OMR = 4.1;
        static Queue<string> appointmentQueue = new Queue<string>();
        static List<bool> hasAppointment = new List<bool>(); // parallel to users
        const string AppointmentFile = "appointments.txt";

        static void Main(string[] args)
        {
            try
            {
                // Load existing accounts and complaints from files
                LoadAccounts();
                LoadComplaints();
                LoadTransferHistory();
                LoadTransactions();
                LoadAppointments();
                WelcomeMenu();
            }catch(Exception ex)
            {
                Console.WriteLine($"Error during initialization: {ex.Message}");
            }
            
        }
        public static void PrintColored(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ResetColor();
        }
        public static void PrintBoxedMenu(string title, string[] options, ConsoleColor titleColor = ConsoleColor.Yellow)
        {
            Console.Clear();
            int width = options.Max(o => o.Length);
            width = Math.Max(width, title.Length);
            width += 8; // padding

            string top = $"╭{new string('─', width)}╮";
            string bottom = $"╰{new string('─', width)}╯";

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(top);

            string paddedTitle = $"│{new string(' ', (width - title.Length) / 2)}{title}{new string(' ', (width - title.Length + 1) / 2)}│";
            Console.ForegroundColor = titleColor;
            Console.WriteLine(paddedTitle);
            Console.ForegroundColor = ConsoleColor.Cyan;

            Console.WriteLine($"├{new string('─', width)}┤");

            foreach (var option in options)
            {
                string line = $"│  {option.PadRight(width - 2)}│";
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(line);
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(bottom);
            Console.ResetColor();
        }
        public static void PrintUserMenu(string title, string[] options, ConsoleColor titleColor = ConsoleColor.Yellow)
        {
            Console.Clear();
            int width = options.Max(o => o.Length);
            width = Math.Max(width, title.Length);
            width += 8; // padding

            string top = $"╭{new string('─', width)}╮";
            string bottom = $"╰{new string('─', width)}╯";

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(top);

            string paddedTitle = $"│{new string(' ', (width - title.Length) / 2)}{title}{new string(' ', (width - title.Length + 1) / 2)}│";
            Console.ForegroundColor = titleColor;
            Console.WriteLine(paddedTitle);
            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine($"├{new string('─', width)}┤");

            foreach (var option in options)
            {
                string line = $"│  {option.PadRight(width - 2)}│";
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(line);
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(bottom);
            Console.ResetColor();
        }
        // Display the welcome menu and prompt user for selection
        public static void WelcomeMenu()
        {
            try
            {
                string[] options = new string[]
                {
            "1. Admin Login",
            "2. Login to Your Account",
            "3. Exit"
                };

                PrintBoxedMenu("🌟 Mini Bank System 🌟", options);

                Console.Write("\nSelect an option (1-3): ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        if (AdminLogin())
                            AdminMenu();
                        else
                            WelcomeMenu();
                        break;

                    case "2":
                        UserMenu();
                        break;

                    case "3":
                        SaveAccounts();
                        SaveComplaints();
                        SaveTransferHistory();
                        SaveTransactions();
                        SaveAppointments();

                        Console.Write("Would you like to create a backup file? (y/n): ");
                        string input = Console.ReadLine().ToLower();
                        if (input == "y")
                        {
                            BackupAllData();
                        }

                        Environment.Exit(0);
                        break;

                    default:
                        Console.WriteLine("Invalid choice, please try again.");
                        Console.ReadKey();
                        WelcomeMenu();
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Welcome Menu: {ex.Message}");
            }

            Console.WriteLine("\nPlease press any key to continue...");
            Console.ReadKey();
        }

        // Display the admin menu and prompt user for selection
        public static void AdminMenu()
        {
            // handle any exceptions that may occur during the admin menu
            try
            {
                bool exit = true;
                while (exit)
                {
                    Console.Clear();
                    PrintColored("====================================", ConsoleColor.DarkBlue);
                    PrintColored("            Admin Menu            ", ConsoleColor.White);
                    PrintColored("====================================", ConsoleColor.DarkBlue);
                    PrintColored("1. Process Account Requests", ConsoleColor.Green);
                    PrintColored("2. Search Account by Name/National ID", ConsoleColor.White);
                    PrintColored("3. Delete Account", ConsoleColor.Red);
                    PrintColored("4. View All Accounts", ConsoleColor.White);
                    PrintColored("5. Show Total Bank Balance", ConsoleColor.White);
                    PrintColored("6. Export All Accounts", ConsoleColor.White);
                    PrintColored("7. View All Complaints", ConsoleColor.White);
                    PrintColored("8. Process Loan Requests", ConsoleColor.White);
                    PrintColored("9. View Feedback Summary", ConsoleColor.White);
                    PrintColored("10. View Appointments", ConsoleColor.White);
                    PrintColored("12. Unlock Locked Account", ConsoleColor.White);
                    PrintColored("12. Show Top 3 Richest Customers", ConsoleColor.White);
                    PrintColored("13. Back to Main Menu", ConsoleColor.Red);
                    PrintColored("------------------------------------", ConsoleColor.DarkGray);
                    Console.Write("Select an option (1-13): ");
                    string choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1": ProcessCreateBankAccountRequest(); break;
                        case "2": SearchAccount(); break;
                        case "3": DeleteAccount(); break;
                        case "4":
                            Console.WriteLine($"DEBUG: Loaded Accounts = {names.Count}");
                            
                            ViewAllAccounts();
                            Console.WriteLine("Press Any Key to continue...");
                            Console.ReadKey();
                            break;
                        case "5": ShowTotalBankBalance();
                            Console.WriteLine("Press Any Key to continue...");
                            Console.ReadKey(); break;
                        case "6": ExportAccounts(); break;
                        case "7": ViewAllComplaints(); 
                            Console.WriteLine("Press Any Key to continue...");
                            Console.ReadKey(); break;
                        
                        case "8":
                            ProcessLoanRequests();
                            break;
                        case "9":
                            ViewFeedbackSummary();
                            break;
                        case "10":
                            ViewAppointments();
                            break;
                        case "11":
                            UnlockAccount();
                            break;
                        case "12":
                            ShowTop3RichestCustomers(); 
                            break;
                        case "13":
                            WelcomeMenu();
                            break;

                        default: Console.WriteLine("Invalid choice."); AdminMenu(); exit = false; break;
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Admin Mlenu: {ex.Message}");
            }
            Console.WriteLine("Please press any key to countune");
            Console.ReadKey();
            

            }
        // Display the user menu and prompt user for selection
        public static void UserMenu()
        {
            try
            {
                bool flag = true;
                while (flag)
                {
                    string[] options = new string[]
                    {
                "1. Create Account Request",
                "2. Login",
                "3. Back to Main Menu"
                    };

                    PrintUserMenu("👤 User Menu", options);

                    Console.Write("\nSelect an option (1-3): ");
                    string choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1":
                            RequestCreateBankAccount();
                            break;
                        case "2":
                            LoginUserAccount();
                            break;
                        case "3":
                            WelcomeMenu();
                            return; // exit current loop
                        default:
                            Console.WriteLine("Invalid choice.");
                            Console.ReadKey();
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in User Menu: {ex.Message}");
            }

            Console.WriteLine("Please press any key to continue...");
            Console.ReadKey();
        }
        // Display the user bank menu and prompt user for selection
        public static void UserBankMenu(int index)
        {
            try
            {
                Console.Clear();
                PrintColored("====================================", ConsoleColor.Cyan);
                PrintColored($" Welcome, {names[index]}!", ConsoleColor.Yellow);
                PrintColored("====================================", ConsoleColor.Cyan);
                PrintColored("1. View Balance", ConsoleColor.White);
                PrintColored("2. Deposit", ConsoleColor.White);
                PrintColored("3. Withdraw", ConsoleColor.White);
                PrintColored("4. Transfer", ConsoleColor.White);
                PrintColored("5. Generate Monthly Statement", ConsoleColor.White);
                PrintColored("6. Request Loan", ConsoleColor.White);
                PrintColored("7. Filter My Transactions", ConsoleColor.White);
                PrintColored("8. Book Appointment", ConsoleColor.White);
                PrintColored("9. Submit Complaint", ConsoleColor.White);
                PrintColored("10. Undo Last Complaint", ConsoleColor.White);
                PrintColored("11. View Transfer History", ConsoleColor.White);
                PrintColored("12. Logout", ConsoleColor.Red);
                PrintColored("------------------------------------", ConsoleColor.DarkGray);
                Console.Write("Select an option (1-12): ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.WriteLine($"Your balance is {balances[index]:C}");
                        break;

                    case "2":
                        Console.WriteLine("Select currency for deposit:");
                        Console.WriteLine("1. OMR (default)");
                        Console.WriteLine("2. USD");
                        Console.WriteLine("3. EUR");
                        Console.Write("Choice: ");
                        string currencyChoice = Console.ReadLine();

                        Console.Write("Enter deposit amount: ");
                        if (!double.TryParse(Console.ReadLine(), out double originalAmount) || originalAmount <= 0)
                        {
                            PrintColored("Invalid amount.", ConsoleColor.Red);
                            break;
                        }

                        // Exchange rates (approx)
                        double USD_TO_OMR = 0.385;
                        double EUR_TO_OMR = 0.41;

                        double convertedAmount = originalAmount;
                        string currencyUsed = "OMR";

                        switch (currencyChoice)
                        {
                            case "2":
                                currencyUsed = "USD";
                                convertedAmount = originalAmount * USD_TO_OMR;
                                break;
                            case "3":
                                currencyUsed = "EUR";
                                convertedAmount = originalAmount * EUR_TO_OMR;
                                break;
                        }

                        balances[index] += convertedAmount;
                        string transaction = $"Deposit ({currencyUsed})|{originalAmount}|{DateTime.Now:yyyy-MM-dd HH:mm}|{balances[index]}|Converted: {convertedAmount:F2} OMR";
                        allTransactions[index].Add(transaction);
                        PrintReceipt("Deposit", index, convertedAmount, "OMR");
                        AskForFeedback(index);
                        break;

                    case "3":
                        Console.Write("Enter withdraw amount: ");
                        if (double.TryParse(Console.ReadLine(), out double withdraw) && balances[index] >= withdraw)
                        {
                            balances[index] -= withdraw;
                            string withdrawTransaction = $"Withdraw|{withdraw}|{DateTime.Now:yyyy-MM-dd HH:mm}|{balances[index]}";
                            allTransactions[index].Add(withdrawTransaction);
                            PrintReceipt("Withdraw", index, withdraw);
                            AskForFeedback(index);
                        }
                        else
                        {
                            PrintColored("Insufficient funds.", ConsoleColor.Yellow);
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
                                string outTx = $"TransferTo:{accountNumbers[receiverIndex]}|{amount}|{DateTime.Now:yyyy-MM-dd HH:mm}|{balances[index]}";
                                string inTx = $"TransferFrom:{accountNumbers[index]}|{amount}|{DateTime.Now:yyyy-MM-dd HH:mm}|{balances[receiverIndex]}";
                                allTransactions[index].Add(outTx);
                                allTransactions[receiverIndex].Add(inTx);
                                // Save to transfer history
                                RecordTransfer(accountNumbers[index], accountNumbers[receiverIndex], amount);
                                SaveTransferHistory();
                                PrintColored("Transfer successful.", ConsoleColor.Green);
                                AskForFeedback(index);
                            }
                            else PrintColored("Invalid amount or insufficient funds.", ConsoleColor.Red);
                        }
                        else PrintColored("Recipient not found.", ConsoleColor.Yellow);
                        break;

                    case "5":
                        GenerateMonthlyStatement(index);
                        break;
                    case "6":
                        RequestLoan(index);
                        break;
                    case "7":
                        FilterTransactions(index);
                        break;
                    case "8":
                        BookAppointment(index);
                        break;
                    case "9":
                        SubmitComplaint(index);
                        break;
                    case "10":
                        UndoLastComplaint(index);
                        break;
                    case "11":
                        ViewTransferHistory(index);
                        break;

                    case "12":
                        WelcomeMenu();
                        return;
                    default:
                        PrintColored("Invalid option.", ConsoleColor.Red);
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UserBankMenu: {ex.Message}");
            }

            Console.WriteLine("Press any key to return to menu...");
            Console.ReadKey();
            UserBankMenu(index);
        }

        public static bool AdminLogin()
        {
            const string adminId = "admin";
            const string adminPassword = "admin123"; // You can also hash this if you want

            Console.Write("Enter Admin ID: ");
            string inputId = Console.ReadLine();

            Console.Write("Enter Admin Password: ");
            string inputPassword = ReadPassword();

            if (inputId == adminId && inputPassword == adminPassword)
            {
                Console.WriteLine("Admin login successful.");
                return true;
            }
            else
            {
                Console.WriteLine("Invalid admin credentials.");
                return false;
            }
        }

        // Create a new account and add it to the lists
        public static void CreateAccount(string name, string nationalId, string password, double balance)
        {
            // store the account details in the lists
            lastAccountNumber++;
            names.Add(name);
            nationalIds.Add(nationalId);
            passwords.Add(password);
            balances.Add(balance);
            accountNumbers.Add(lastAccountNumber.ToString());
            failedLoginAttempts.Add(0);
            isLocked.Add(false);
            loanAmounts.Add(0);
            loanInterestRates.Add(0);
            loanStatus.Add("None");
            feedbackScores.Add(new List<int>());
            allTransactions.Add(new List<string>());
            hasAppointment.Add(false);
            Console.WriteLine();
            PrintColored($"Account created! Account Number: {lastAccountNumber}", ConsoleColor.Green);
            Console.WriteLine("Please press any key to countune");
            Console.ReadKey();
            UserMenu();
        }
        public static void AskForFeedback(int index)
        {
            Console.Write("Rate our service (1–5): ");
            if (int.TryParse(Console.ReadLine(), out int rating) && rating >= 1 && rating <= 5)
            {
                feedbackScores[index].Add(rating);
                Console.WriteLine("Thank you for your feedback!");
            }
            else
            {
                Console.WriteLine("Invalid rating.");
            }
        }

        public static void RequestLoan(int index)
        {
            if (loanStatus[index] != "None")
            {
                Console.WriteLine("You already have an active or pending loan.");
                return;
            }

            if (balances[index] < 5000)
            {
                Console.WriteLine("You must have at least 5000 balance to request a loan.");
                return;
            }

            Console.Write("Enter loan amount: ");
            if (double.TryParse(Console.ReadLine(), out double loanAmount) && loanAmount > 0)
            {
                loanAmounts[index] = loanAmount;
                loanInterestRates[index] = 0.05; // 5% fixed rate for example
                loanStatus[index] = "Pending";

                Console.WriteLine("Loan request submitted and pending approval.");
            }
            else
            {
                Console.WriteLine("Invalid loan amount.");
            }
        }
        public static void ProcessLoanRequests()
        {
            bool found = false;

            for (int i = 0; i < names.Count; i++)
            {
                if (loanStatus[i] == "Pending")
                {
                    found = true;
                    Console.WriteLine($"\nLoan Request from {names[i]} (Account: {accountNumbers[i]})");
                    Console.WriteLine($"Requested: {loanAmounts[i]} | Rate: {loanInterestRates[i] * 100}%");

                    Console.Write("Approve (A) / Reject (R): ");
                    string input = Console.ReadLine().ToUpper();

                    if (input == "A")
                    {
                        balances[i] += loanAmounts[i];
                        loanStatus[i] = "Approved";
                        Console.WriteLine("Loan approved and amount added to account.");
                    }
                    else if (input == "R")
                    {
                        loanStatus[i] = "Rejected";
                        loanAmounts[i] = 0;
                        loanInterestRates[i] = 0;
                        Console.WriteLine("Loan rejected.");
                    }
                    else
                    {
                        Console.WriteLine("Invalid input. Skipped.");
                    }
                }
            }

            if (!found)
                Console.WriteLine("No pending loan requests.");

            Console.WriteLine("Press any key to return to Admin Menu...");
            Console.ReadKey();
            AdminMenu();
        }
        public static void BookAppointment(int index)
        {
            if (hasAppointment[index])
            {
                Console.WriteLine("You already have a pending appointment.");
                return;
            }

            Console.Write("Enter appointment purpose (e.g., Loan Consultation): ");
            string purpose = Console.ReadLine();

            DateTime date = DateTime.Now.AddDays(1); // For simplicity: next day
            string appointment = $"{accountNumbers[index]} | {names[index]} | {date:yyyy-MM-dd HH:mm} | {purpose}";

            appointmentQueue.Enqueue(appointment);
            hasAppointment[index] = true;
            SaveAppointments(); // optional — saves immediately
            Console.WriteLine("Appointment booked for: " + date.ToString("yyyy-MM-dd HH:mm"));
        }
        public static void SaveAppointments()
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(AppointmentFile))
                {
                    foreach (var app in appointmentQueue)
                    {
                        sw.WriteLine(app);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving appointments: " + ex.Message);
            }
        }
        public static void LoadAppointments()
        {
            try
            {
                appointmentQueue.Clear();

                if (File.Exists(AppointmentFile))
                {
                    foreach (var line in File.ReadAllLines(AppointmentFile))
                    {
                        appointmentQueue.Enqueue(line);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading appointments: " + ex.Message);
            }
        }

        public static void ViewAppointments()
        {
            if (appointmentQueue.Count == 0)
            {
                Console.WriteLine("No appointments scheduled.");
            }
            else
            {
                Console.WriteLine("--- Appointment Queue ---");
                foreach (var app in appointmentQueue)
                {
                    Console.WriteLine(app);
                }
            }

            Console.WriteLine("Press any key to return...");
            Console.ReadKey();
            AdminMenu();
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
                string rawPassword = ReadPassword(); // masked input
                string hashedPassword = HashPassword(rawPassword); //  secure hash

                Console.Write("Enter initial balance: ");
                if (!double.TryParse(Console.ReadLine(), out double initialBalance))
                {
                    Console.WriteLine("Invalid balance.");
                    Console.WriteLine("Please press any key to countune");
                    Console.ReadKey();
                    return;
                }

                // Check if the account already exists or if a request is pending
                if (nationalIds.Contains(nationalId) || createAccountRequests.Any(r => r.Contains(nationalId)))
                {
                    Console.WriteLine("Account already exists or request pending.");
                    Console.WriteLine("Please press any key to countune");
                    Console.ReadKey();
                    return;
                }

                // Save the hashed password instead of the raw one
                createAccountRequests.Enqueue($"{name}|{nationalId}|{hashedPassword}|{initialBalance}");
                Console.WriteLine("Account request submitted!");
                Console.WriteLine("Please press any key to countune");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in RequestCreateBankAccount: {ex.Message}");
            }
        }
        public static void UnlockAccount()
        {
            Console.Write("Enter account number to unlock: ");
            string accNum = Console.ReadLine();

            int index = accountNumbers.IndexOf(accNum);
            if (index == -1)
            {
                Console.WriteLine("Account not found.");
            }
            else if (!isLocked[index])
            {
                Console.WriteLine("Account is not locked.");
            }
            else
            {
                isLocked[index] = false;
                failedLoginAttempts[index] = 0;
                Console.WriteLine("Account unlocked successfully.");
            }

            Console.WriteLine("Press any key to return to Admin Menu...");
            Console.ReadKey();
            AdminMenu();
        }

        public static string ReadPassword()
        {
            StringBuilder password = new StringBuilder();
            ConsoleKeyInfo key;
            do
            {
                key = Console.ReadKey(true);
                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    password.Append(key.KeyChar);
                    Console.Write("*");
                }
                else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password.Remove(password.Length - 1, 1);
                    Console.Write("\b \b");
                }
            } while (key.Key != ConsoleKey.Enter);
            Console.WriteLine();
            return password.ToString();
        }

        public static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hash);
            }
        }

        // Process the account creation request
        public static void ProcessCreateBankAccountRequest()
        {
            if (createAccountRequests.Count == 0)
            {
               
                PrintColored("No requests to process.", ConsoleColor.Red);
                Console.WriteLine("Please press any key to continue...");
                Console.ReadKey();
                AdminMenu();
                return;
            }

            string request = createAccountRequests.Dequeue();
            var parts = request.Split('|');

            string name = parts[0];
            string nationalId = parts[1];
            string password = parts[2];
            double balance = double.Parse(parts[3]);

            Console.WriteLine("\n--- Account Creation Request ---");
            Console.WriteLine($"Name       : {name}");
            Console.WriteLine($"National ID: {nationalId}");
            Console.WriteLine($"Balance    : {balance:C}");
            Console.Write("\nApprove this request? (y/n): ");
            string choice = Console.ReadLine().ToLower();

            if (choice == "y")
            {
                CreateAccount(name, nationalId, password, balance);
                
                PrintColored("Account request approved and created.", ConsoleColor.Green);
                
            }
            else
            {
                
                PrintColored("Account request rejected.", ConsoleColor.Red);
            }

            Console.WriteLine("Press any key to process the next request or return...");
            Console.ReadKey();

            // Loop back if more requests remain
            if (createAccountRequests.Count > 0)
                ProcessCreateBankAccountRequest();
            else
                AdminMenu();
        }

        // Login to an existing user account
        public static void LoginUserAccount()
        {
            Console.Write("Enter National ID: ");
            string nationalId = Console.ReadLine();

            int index = nationalIds.IndexOf(nationalId);
            if (index == -1)
            {
                
                PrintColored("Account not found.", ConsoleColor.Red);
                Console.ReadKey();
                return;
            }

            if (isLocked[index])
            {
                
                PrintColored("Account is locked. Contact admin.", ConsoleColor.Red);
                Console.ReadKey();
                return;
            }

            int maxAttempts = 3;
            int passwordAttempts = 0;

            while (passwordAttempts < maxAttempts)
            {
                Console.Write("Enter password: ");
                string enteredPassword = ReadPassword();
                string hashedInput = HashPassword(enteredPassword);

                if (passwords[index] == hashedInput)
                {
                    PrintColored("Login successful!", ConsoleColor.Green);
                   
                    failedLoginAttempts[index] = 0;
                    UserBankMenu(index);
                    return;
                }
                else
                {
                    passwordAttempts++;
                    failedLoginAttempts[index]++;

                    Console.WriteLine($"Incorrect password. Attempt {passwordAttempts} of {maxAttempts}.");

                    if (failedLoginAttempts[index] >= 3)
                    {
                        isLocked[index] = true;
                        PrintColored("Account locked due to 3 failed login attempts.", ConsoleColor.Red);
                        SaveAccounts();
                        Console.ReadKey();
                        return;
                    }
                }
            }

            PrintColored("Too many incorrect password attempts.", ConsoleColor.Red);
            Console.ReadKey();
        }

       
        public static void GenerateMonthlyStatement(int index)
        {
            Console.Write("Enter month (1–12): ");
            int month = int.Parse(Console.ReadLine());
            Console.Write("Enter year (e.g. 2025): ");
            int year = int.Parse(Console.ReadLine());

            var filtered = allTransactions[index]
                .Where(t => DateTime.Parse(t.Split('|')[2]).Month == month &&
                            DateTime.Parse(t.Split('|')[2]).Year == year)
                .ToList();

            if (filtered.Count == 0)
            {
                PrintColored("No transactions found for that period.", ConsoleColor.Yellow);
                return;
            }

            string fileName = $"Statement_{accountNumbers[index]}_{year}-{month:00}.txt";
            using (StreamWriter sw = new StreamWriter(fileName))
            {
                sw.WriteLine($"Mini Bank Monthly Statement for {names[index]} - {month:00}/{year}");
                sw.WriteLine("--------------------------------------------------");
                sw.WriteLine("Type\t\tAmount\t\tDate\t\t\tBalance");
                foreach (var tx in filtered)
                {
                    var parts = tx.Split('|');
                    sw.WriteLine($"{parts[0]}\t\t{parts[1]}\t\t{parts[2]}\t\t{parts[3]}");
                }
            }

            Console.WriteLine($"Statement saved to file: {fileName}");
        }
        public static void ViewFeedbackSummary()
        {
            Console.WriteLine("\n--- Feedback Summary ---");

            for (int i = 0; i < names.Count; i++)
            {
                if (feedbackScores[i].Count > 0)
                {
                    double average = feedbackScores[i].Average();
                    Console.WriteLine($"{names[i]} ({accountNumbers[i]}): Avg Rating = {average:F2}");
                }
                else
                {
                    Console.WriteLine($"{names[i]} ({accountNumbers[i]}): No ratings yet.");
                }
            }

            Console.WriteLine("Press any key to return to Admin Menu...");
            Console.ReadKey();
            AdminMenu();
        }
        public static void BackupAllData()
        {
            string fileName = $"Backup_{DateTime.Now:yyyy-MM-dd_HHmm}.txt";

            using (StreamWriter sw = new StreamWriter(fileName))
            {
                sw.WriteLine("===== Accounts =====");
                for (int i = 0; i < names.Count; i++)
                {
                    sw.WriteLine($"{accountNumbers[i]} | {names[i]} | {nationalIds[i]} | Balance: {balances[i]:F2} | Locked: {isLocked[i]}");
                }

                sw.WriteLine("\n===== Transactions =====");
                for (int i = 0; i < allTransactions.Count; i++)
                {
                    sw.WriteLine($"--- {names[i]} ({accountNumbers[i]}) ---");
                    foreach (var tx in allTransactions[i])
                    {
                        sw.WriteLine(tx);
                    }
                }

                sw.WriteLine("\n===== Complaints =====");
                foreach (var complaint in complaints)
                {
                    sw.WriteLine($"- {complaint}");
                }

                sw.WriteLine("\n===== Feedback Summary =====");
                for (int i = 0; i < names.Count; i++)
                {
                    var scores = feedbackScores[i];
                    double avg = scores.Count > 0 ? scores.Average() : 0;
                    sw.WriteLine($"{names[i]} | Ratings: {scores.Count} | Avg: {avg:F2}");
                }

                sw.WriteLine("\n===== Loan Requests =====");
                for (int i = 0; i < names.Count; i++)
                {
                    if (loanStatus[i] != "None")
                    {
                        sw.WriteLine($"{names[i]} | Amount: {loanAmounts[i]} | Rate: {loanInterestRates[i]} | Status: {loanStatus[i]}");
                    }
                }

                sw.WriteLine("\n===== End of Backup =====");
            }

            Console.WriteLine($"Backup saved to {fileName}");
        }

        // Search for an account by national ID
        public static void SearchAccount()
        {
            Console.Write("Enter Name or National ID: ");
            string input = Console.ReadLine();

            var result = names
                .Select((name, i) => new { Name = name, NationalID = nationalIds[i], Account = accountNumbers[i], Balance = balances[i] })
                .FirstOrDefault(a => a.Name == input || a.NationalID == input);

            if (result != null)
                Console.WriteLine($"Account: {result.Account}, Balance: {result.Balance:C}");
            else
                Console.WriteLine("Account not found.");

            Console.WriteLine("Press any key to return...");
            Console.ReadKey();
            AdminMenu();
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
                PrintColored("Account deleted.", ConsoleColor.Red);
                
            }
            else
                PrintColored("Account not found.", ConsoleColor.Yellow);
            Console.WriteLine("Please press any key to countune");
            Console.ReadKey();
            AdminMenu();
        }
        // View all accounts
        public static void ViewAllAccounts()
        {
            

            if (names.Count == 0)
            {
                PrintColored("No accounts to display.", ConsoleColor.Yellow);
                Console.WriteLine("Please press any key to countune");
                Console.ReadKey ();

                return;
            }

            var allAccounts = names
                .Select((name, i) => new
                {
                    AccountNumber = accountNumbers[i],
                    Name = name,
                    Balance = balances[i]
                })
                .OrderByDescending(acc => acc.Balance); // optional sorting

            Console.WriteLine("--- All Accounts ---");
            foreach (var acc in allAccounts)
            {
                Console.WriteLine($"{acc.AccountNumber} | {acc.Name} | {acc.Balance:C}");
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
            PrintColored("Exported successfully.", ConsoleColor.Green);
            Console.WriteLine("Please press any key to countune");
            Console.ReadKey();
            AdminMenu();
        }
        // Submit a complaint
        public static void SubmitComplaint(int index)
        {
            Console.Write("Enter your complaint: ");
            string complaint = Console.ReadLine();
            complaints.Push(complaint);
            Console.WriteLine("Complaint submitted.");

            Console.WriteLine("Press any key to return to your menu...");
            Console.ReadKey();
            UserBankMenu(index);
        }

        // Undo the last complaint
        public static void UndoLastComplaint(int index)
        {
            if (complaints.Count > 0)
            {
                complaints.Pop();
                Console.WriteLine("Last complaint undone.");
            }
            else
                Console.WriteLine("No complaints to undo.");
            Console.WriteLine("Please press any key to countune");
            Console.ReadKey();
            UserBankMenu(index);
        }
        // Print a receipt for transactions
        public static void PrintReceipt(string type, int index, double amount, string currency = "OMR")
        {
            string receipt = $"{type} Receipt\n" +
                             $"Name: {names[index]}\n" +
                             $"Account#: {accountNumbers[index]}\n" +
                             $"Amount: {amount.ToString("C", CultureInfo.CreateSpecificCulture("en-OM"))} ({currency})\n" +
                             $"Balance: {balances[index].ToString("C", CultureInfo.CreateSpecificCulture("en-OM"))}\n" +
                             $"Date: {DateTime.Now}";

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
                    {
                        sw.WriteLine($"{names[i]}|{nationalIds[i]}|{passwords[i]}|{balances[i]}|{accountNumbers[i]}|{failedLoginAttempts[i]}|{isLocked[i]}|{loanAmounts[i]}|{loanInterestRates[i]}|{loanStatus[i]}|{hasAppointment[i]}|{string.Join(",", feedbackScores[i])}");
                    }
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
                    

                    var lines = File.ReadAllLines(UserLogInFile);
                    foreach (var line in lines)
                    {
                        Console.WriteLine($"Loading line: {line}");
                        var parts = line.Split('|');

                        // Must have at least 5 parts (basic info)
                        if (parts.Length < 5)
                        {
                            Console.WriteLine($"[Skipped] Invalid line: {line}");
                            continue;
                        }

                        try
                        {
                            names.Add(parts[0]);
                            nationalIds.Add(parts[1]);
                            passwords.Add(parts[2]);
                            balances.Add(double.Parse(parts[3]));
                            accountNumbers.Add(parts[4]);

                            // Optional fields (length > 11 expected for full details)
                            if (parts.Length > 11)
                            {
                                failedLoginAttempts.Add(int.Parse(parts[5]));
                                isLocked.Add(bool.Parse(parts[6]));
                                loanAmounts.Add(double.Parse(parts[7]));
                                loanInterestRates.Add(double.Parse(parts[8]));
                                loanStatus.Add(parts[9]);
                                hasAppointment.Add(bool.Parse(parts[10]));

                                var scores = parts[11]
                                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                                    .Select(int.Parse)
                                    .ToList();

                                feedbackScores.Add(scores);
                            }
                            else
                            {
                                // Fill defaults for missing data
                                failedLoginAttempts.Add(0);
                                isLocked.Add(false);
                                loanAmounts.Add(0);
                                loanInterestRates.Add(0);
                                loanStatus.Add("None");
                                hasAppointment.Add(false);
                                feedbackScores.Add(new List<int>());
                            }
                        }
                        catch (Exception exLine)
                        {
                            Console.WriteLine($"[Error] Parsing line failed: {line}\nReason: {exLine.Message}");

                            // Rollback partial entries to keep lists consistent
                            int rollbackIndex = names.Count - 1;
                            if (rollbackIndex >= 0)
                            {
                                names.RemoveAt(rollbackIndex);
                                nationalIds.RemoveAt(rollbackIndex);
                                passwords.RemoveAt(rollbackIndex);
                                balances.RemoveAt(rollbackIndex);
                                accountNumbers.RemoveAt(rollbackIndex);
                            }
                        }
                    }

                    // Update last account number
                    if (accountNumbers.Count > 0)
                    {
                        lastAccountNumber = accountNumbers.Max(x => int.Parse(x));
                    }

                    Console.WriteLine($"✔ Loaded {names.Count} accounts successfully.");
                }
                else
                {
                    Console.WriteLine("⚠ No user login file found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Fatal Error] Loading accounts: {ex.Message}");
            }
        }

        public static void FilterTransactions(int index)
        {
            Console.WriteLine("\n--- Transaction Filter Menu ---");
            Console.WriteLine("1. Show Last N Transactions");
            Console.WriteLine("2. Show Transactions After Date");
            Console.Write("Choose an option: ");
            string choice = Console.ReadLine();

            if (index >= allTransactions.Count || allTransactions[index].Count == 0)
            {
                Console.WriteLine("No transactions found.");
                return;
            }

            switch (choice)
            {
                case "1":
                    Console.Write("Enter N (number of recent transactions to show): ");
                    if (int.TryParse(Console.ReadLine(), out int n))
                    {
                        Console.WriteLine($"\nLast {n} transactions:");
                        var lastN = allTransactions[index].TakeLast(n);
                        foreach (var tx in lastN)
                            Console.WriteLine(tx);
                    }
                    else
                    {
                        Console.WriteLine("Invalid number.");
                    }
                    break;

                case "2":
                    Console.Write("Enter date (yyyy-MM-dd): ");
                    if (DateTime.TryParse(Console.ReadLine(), out DateTime filterDate))
                    {
                        Console.WriteLine($"\nTransactions after {filterDate:yyyy-MM-dd}:");
                        var filtered = allTransactions[index]
                            .Where(tx => DateTime.Parse(tx.Split('|')[2]) > filterDate);

                        foreach (var tx in filtered)
                            Console.WriteLine(tx);
                    }
                    else
                    {
                        Console.WriteLine("Invalid date format.");
                    }
                    break;

                default:
                    Console.WriteLine("Invalid option.");
                    break;
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
                Console.WriteLine($"DEBUG: Complaints Loaded = {complaints.Count}");

                if (complaints.Count == 0)
                {
                    Console.WriteLine("No complaints available.");
                    Console.WriteLine("Please press any key to countune");
                    Console.ReadKey();
                    AdminMenu();
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

          
        }

        public static void ViewTransferHistory(int index)
        {
            if (transferHistory.Count == 0)
            {
                Console.WriteLine("No transfer history available.");
                Console.WriteLine("Please press any key to countune");
                Console.ReadKey();
                UserBankMenu(index);
            }
            else
            {
                Console.WriteLine("\n--- Transfer History ---");
                foreach (var record in transferHistory)
                {
                    Console.WriteLine(record);
                }
            }
        }

        public static void RecordTransfer(string fromAccount, string toAccount, double amount)
        {
            string transferRecord = $"From: {fromAccount} To: {toAccount} Amount: {amount:C} Date: {DateTime.Now}";
            transferHistory.Add(transferRecord);
        }

        public static void SaveTransferHistory()
        {
            try
            {
                using (StreamWriter sw = new StreamWriter("transferHistory.txt"))
                {
                    foreach (var record in transferHistory)
                    {
                        sw.WriteLine(record);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving transfer history: {ex.Message}");
            }
        }

        public static void LoadTransferHistory()
        {
            try
            {
                if (File.Exists("transferHistory.txt"))
                {
                    foreach (var line in File.ReadAllLines("transferHistory.txt"))
                    {
                        transferHistory.Add(line);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading transfer history: {ex.Message}");
            }
        }
        public static void SaveTransactions()
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(TransactionFile))
                {
                    for (int i = 0; i < allTransactions.Count; i++)
                    {
                        foreach (var tx in allTransactions[i])
                        {
                            sw.WriteLine($"{accountNumbers[i]}|{tx}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving transactions: {ex.Message}");
            }
        }
        public static void LoadTransactions()
        {
            try
            {
                allTransactions = new List<List<string>>();
                foreach (var _ in accountNumbers)
                {
                    allTransactions.Add(new List<string>());
                }

                if (File.Exists(TransactionFile))
                {
                    foreach (var line in File.ReadAllLines(TransactionFile))
                    {
                        var parts = line.Split('|');
                        string accNum = parts[0];
                        string tx = string.Join("|", parts.Skip(1));

                        int index = accountNumbers.IndexOf(accNum);
                        if (index != -1)
                        {
                            allTransactions[index].Add(tx);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading transactions: {ex.Message}");
            }
        }
        public static void ShowTop3RichestCustomers()
        {
            var top3 = balances
                .Select((balance, i) => new { Name = names[i], Account = accountNumbers[i], Balance = balance })
                .OrderByDescending(x => x.Balance)
                .Take(3);

            Console.WriteLine("--- Top 3 Richest Customers ---");
            foreach (var user in top3)
            {
                Console.WriteLine($"{user.Name} ({user.Account}) - {user.Balance:C}");
            }

            Console.WriteLine("Press any key to return...");
            Console.ReadKey();
            AdminMenu();
        }

    }
}
