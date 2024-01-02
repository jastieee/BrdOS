using Cosmos.Core;
using Cosmos.System.FileSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Sys = Cosmos.System;

namespace brdOS
{
    public class Kernel : Sys.Kernel
    {
        private readonly string[] userCredentials = { "admin:admin", "brd:brd" };
        private string currentUser;
        private List<string> commandHistory = new List<string>();
        private List<string> systemMenuCommandHistory = new List<string>();
        private readonly string kernelVersion = "brdOS v2.0.0";
        private bool isInitialized = false;

        public class BootManager
        {
            // Static property to store the boot time
            public static DateTime BootTime { get; private set; }

            // Initialize the boot time
            public static void InitializeBootTime()
            {
                BootTime = DateTime.Now;
            }

            // Calculate and return the uptime
            public static TimeSpan Uptime
            {
                get { return DateTime.Now - BootTime; }
            }
        }


        // WELCOME PAGE
        protected override void BeforeRun()
        {



            // Initialize boot time
            BootManager.InitializeBootTime();


            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("                __                       __   _______   ______               ");
            Console.WriteLine("               |  |                     |  | |  ___  | |  ____|              ");
            Console.WriteLine("               |  |____   __  ___   ____|  | | |   | | |  |___               ");
            Console.WriteLine("               |  ___  | |  |/ __| |  ___  | | |   | | |___   |              ");
            Console.WriteLine("               | |___| | |    /    | |___| | | |___| |  ___|  |              ");
            Console.WriteLine("               |_______| |___/     |_______| |_______| |______|              ");
            Console.WriteLine(" ");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("   DISCLAIMER: Unauthorized hacking or any malicious activities against brdOS");
            Console.WriteLine("      are strictly prohibited and will result in legal consequences.         ");




            Console.WriteLine(" ");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("===============================================================================");
            Console.WriteLine(" ");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("            Welcome to brdOS! Please put your credentials to log in.         ");
            Console.WriteLine(" ");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("===============================================================================");
            Console.WriteLine(" ");

            if (!isInitialized)
            {
                Initialize();
                isInitialized = true;
            }

            LogIn();


        }

        private void Initialize()
        {
            // Perform any necessary initialization tasks
            InitializeVFS();
        }
        private void LogOut()
        {
            currentUser = null;  // Reset the current user
            Console.Clear();
            BeforeRun();  // Return to the welcome page and re-execute the BeforeRun logic
        }

        // LOG IN INFORMATION
        private void LogIn()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("                  Username: ");
            string username = Console.ReadLine();

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("                  Password: ");
            string password = MaskPasswordInput();

            if (IsValidCredentials(username, password))
            {
                currentUser = username;
                Console.Clear();
                DisplayMenu();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid username or password. Please try again.");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(" ");
                Console.Clear();
                BeforeRun();
            }
        }

        // CONCEAL PASSWORD TO "*"
        private string MaskPasswordInput()
        {
            string password = "";
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Backspace)
                {
                    if (password.Length > 0)
                    {
                        Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                        Console.Write(' ');
                        Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);

                        password = password.Substring(0, password.Length - 1);
                    }
                    continue;
                }

                if (char.IsControl(key.KeyChar))
                {
                    continue;
                }

                Console.Write('*');
                password += key.KeyChar;

            } while (key.Key != ConsoleKey.Enter);

            Console.WriteLine();
            return password;
        }

        // VALIDATING CREDENTIALS
        private bool IsValidCredentials(string username, string password)
        {
            return Array.Exists(userCredentials, cred => cred == $"{username}:{password}");
        }

        // MAIN MENU INTERFACE
        private void DisplayMenu()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("===============================================================================");

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Logged in as '{currentUser}'");

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Welcome to brdOS v2.0.0! It's a simple OS made by Group 7!.");
            Console.WriteLine(" ");
            Console.WriteLine("Please enter your desired command below:");

            Console.WriteLine(" ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("1)  [restart]     To Restart the system.");
            Console.WriteLine("2)  [shutdown]    To Shutdown the system.");
            Console.WriteLine("3)  [logout]      To Logout.");
            Console.WriteLine("4)  [clear]       To clear old commands.");
            Console.WriteLine(" ");
            Console.WriteLine("5)  [datetime]    To View the current system date and time.");
            Console.WriteLine("6)  [disuser]     To display the username.");
            Console.WriteLine("8)  [history]     To show command history.");
            Console.WriteLine(" ");
            Console.WriteLine("9)  [calc]        To use the Calculator.");
            Console.WriteLine("10) [rps]         To play Rock-Paper-Scissors.");
            Console.WriteLine("11) [tictactoe]   To play Tic-Tac-Toe.");
            Console.WriteLine(" ");
            Console.WriteLine("12) [system]      To see system options");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("===============================================================================");
        }


        private void DisplaySystemMenu()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("===============================================================================");

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Logged in as '{currentUser}'");

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Welcome to brdOS v2.0.0! It's a simple OS made by Group 7!.");
            Console.WriteLine(" ");
            Console.WriteLine("Please enter your desired command below:");

            Console.WriteLine(" ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("1) [storage]    It provides how much storage is left with the system.");
            Console.WriteLine("2) [info]       To see the system info");
            Console.WriteLine("3) [version]    To View Kernel Version.");
            Console.WriteLine("4) [listdirs]   Listing directories");
            Console.WriteLine(" ");
            Console.WriteLine("6) [create]     To create new file");
            Console.WriteLine("5) [read]       To read files in the directory");    
            Console.WriteLine("7) [update]     To update and edit a file");
            Console.WriteLine("8) [delete]     To delete a file");
            Console.WriteLine(" ");
            Console.WriteLine("9) [sysclear]   To clear system option commands.");

            Console.WriteLine(" ");
            Console.WriteLine("Type 'end' to go back to main menu");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("===============================================================================");
        }

    

        // CALCULATOR CODE
        private double Calculate(string expression)
        {
            string[] elements = expression.Split(' ');

            if (elements.Length != 3)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                throw new ArgumentException("Invalid expression. Please use the format: operand1 operator operand2");
            }

            double operand1 = double.Parse(elements[0]);
            double operand2 = double.Parse(elements[2]);
            string op = elements[1];

            switch (op)
            {
                case "+":
                    return operand1 + operand2;
                case "-":
                    return operand1 - operand2;
                case "*":
                    return operand1 * operand2;
                case "/":
                    if (operand2 == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        throw new ArithmeticException("Cannot divide by zero.");
                    }
                    return operand1 / operand2;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    throw new ArgumentException("Invalid operator. Supported operators are +, -, *, /");
            }
        }

        private void RunCalculator()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("===============================================================================");

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Welcome to the Calculator!");
            Console.WriteLine("Available operations: +, -, *, /");
            Console.WriteLine("Enter your expression (e.g., 5 + 3) or type 'end' to exit:");

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("===============================================================================");
            Console.WriteLine(" ");

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("EXPRESSION > ");

            while (true)
            {
                string expression = Console.ReadLine();

                if (expression.ToLower() == "end")
                {
                    break;
                }

                try
                {
                    double result = Calculate(expression);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Result: {result}");
                    Console.WriteLine(" ");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(" ");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Error: {ex.Message}");
                }

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Enter another expression or type 'end' to exit:");
                Console.WriteLine(" ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("EXPRESSION > ");
            }

            Console.Clear();
            DisplayMenu();
        }

        // ROCK PAPER SCISSORS GAME
        private void RunRockPaperScissors()
        {
            Console.Clear();
            Console.WriteLine(" ");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("===============================================================================");

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Welcome to Rock-Paper-Scissors!");
            Console.WriteLine("Enter your choice (rock, paper, or scissors) or type 'end' to exit:");

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("===============================================================================");
            Console.WriteLine(" ");

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("YOUR CHOICE > ");

            while (true)
            {
                string playerChoice = Console.ReadLine().ToLower();

                if (playerChoice == "end")
                {
                    break;
                }

                if (playerChoice != "rock" && playerChoice != "paper" && playerChoice != "scissors")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid choice. Please enter 'rock', 'paper', or 'scissors'.");
                    Console.WriteLine(" ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("YOUR CHOICE > ");
                    continue;
                }

                // COMPUTER'S CHOICE
                string[] choices = { "rock", "paper", "scissors" };
                Random random = new Random();
                int randomIndex = random.Next(choices.Length);
                string computerChoice = choices[randomIndex];

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"Computer's Choice: {computerChoice}");

                // DETERMINING THE WINNER
                if (playerChoice == computerChoice)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("It's a tie!");
                }
                else if ((playerChoice == "rock" && computerChoice == "scissors") ||
                         (playerChoice == "paper" && computerChoice == "rock") ||
                         (playerChoice == "scissors" && computerChoice == "paper"))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("You win!");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Computer wins!");
                }

                Console.WriteLine(" ");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Enter another choice or type 'end' to exit:");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(" ");
                Console.Write("YOUR CHOICE > ");
            }

            Console.Clear();
            DisplayMenu();
        }

        // TIC-TAC-TOE GAME
        private void RunTicTacToe()
        {
            Console.Clear();
            Console.WriteLine(" ");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("===============================================================================");

            Console.WriteLine(" ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Welcome to Tic-Tac-Toe!");
            Console.WriteLine("Player 1 (X) - Computer (O)");
            Console.WriteLine(" ");
            char[] board = { '1', '2', '3', '4', '5', '6', '7', '8', '9' };

            int currentPlayer = 1;
            bool isGameOver = false;

            do
            {
                Console.ForegroundColor = ConsoleColor.White;
                DrawBoard(board);

                if (currentPlayer == 1)
                {
                    // Player's turn
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"Player {currentPlayer}, enter your choice (1-9): ");
                    bool validInput = int.TryParse(Console.ReadLine(), out int choice);
                    Console.WriteLine(" ");

                    if (validInput && choice >= 1 && choice <= 9)
                    {
                        char marker = 'X';

                        if (IsChoiceValid(board, choice))
                        {
                            UpdateBoard(board, choice, marker);

                            if (CheckForWin(board, marker))
                            {
                                Console.Clear();
                                DrawBoard(board);
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine($"Player {currentPlayer} wins!");
                                isGameOver = true;
                            }
                            else if (IsBoardFull(board))
                            {
                                Console.Clear();
                                DrawBoard(board);
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine("It's a tie!");
                                isGameOver = true;
                            }

                            currentPlayer = 2; // Switch to computer's turn
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Invalid choice. Please select an empty cell.");
                            Console.WriteLine(" ");
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Invalid input. Please enter a number between 1 and 9.");
                        Console.WriteLine(" ");
                    }
                }
                else
                {
                    // Computer's turn
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("Computer is making a move...");
                    System.Threading.Thread.Sleep(1000); // Add a delay to simulate the computer's "thinking"

                    int computerChoice;
                    do
                    {
                        computerChoice = GetRandomMove();
                    } while (!IsChoiceValid(board, computerChoice));

                    char marker = 'O';
                    UpdateBoard(board, computerChoice, marker);

                    if (CheckForWin(board, marker))
                    {
                        Console.Clear();
                        DrawBoard(board);
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Computer wins!");
                        isGameOver = true;
                    }
                    else if (IsBoardFull(board))
                    {
                        Console.Clear();
                        DrawBoard(board);
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("It's a tie!");
                        isGameOver = true;
                    }

                    currentPlayer = 1; // Switch to player's turn
                }

            } while (!isGameOver);

            Console.WriteLine(" ");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("Press Enter to return to the main menu...");
            Console.ReadLine();
            Console.Clear();
            DisplayMenu();
        }

        private int GetRandomMove()
        {
            Random random = new Random();
            return random.Next(1, 10);
        }

        private bool IsBoardFull(char[] board)
        {
            return !board.Any(cell => cell != 'X' && cell != 'O');
        }

        private bool CheckForWin(char[] board, char marker)
        {
            // Check rows
            for (int i = 0; i < 3; i++)
            {
                if (board[i * 3] == marker && board[i * 3 + 1] == marker && board[i * 3 + 2] == marker)
                {
                    return true;
                }
            }

            // Check columns
            for (int i = 0; i < 3; i++)
            {
                if (board[i] == marker && board[i + 3] == marker && board[i + 6] == marker)
                {
                    return true;
                }
            }

            // Check diagonals
            if (board[0] == marker && board[4] == marker && board[8] == marker)
            {
                return true;
            }
            if (board[2] == marker && board[4] == marker && board[6] == marker)
            {
                return true;
            }

            return false;
        }


        // Helper method to draw the Tic-Tac-Toe board
        private void DrawBoard(char[] board)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("===============================================================================");
            Console.ForegroundColor = ConsoleColor.White;

            for (int i = 0; i < 9; i += 3)
            {
                Console.WriteLine($"  {board[i]}  |  {board[i + 1]}  |  {board[i + 2]}  ");

                if (i < 6)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("-----|-----|-----");
                }
            }

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("===============================================================================");
            Console.WriteLine(" ");
        }

        // Helper method to check if a cell is already occupied
        private bool IsChoiceValid(char[] board, int choice)
        {
            return board[choice - 1] != 'X' && board[choice - 1] != 'O';
        }

        // Helper method to update the board with the player's marker
        private void UpdateBoard(char[] board, int choice, char marker)
        {
            board[choice - 1] = marker;
        }

        // Helper method to update the board with the player's marker
        private void UpdateBoard(char[,] board, int choice, char marker)
        {
            int row = (choice - 1) / 3;
            int col = (choice - 1) % 3;

            board[row, col] = marker;
        }

        // Helper method to check if a player has won
        private bool CheckForWin(char[,] board, char marker)
        {
            for (int i = 0; i < 3; i++)
            {
                if ((board[i, 0] == marker && board[i, 1] == marker && board[i, 2] == marker) ||
                    (board[0, i] == marker && board[1, i] == marker && board[2, i] == marker))
                {
                    return true;
                }
            }

            if ((board[0, 0] == marker && board[1, 1] == marker && board[2, 2] == marker) ||
                (board[0, 2] == marker && board[1, 1] == marker && board[2, 0] == marker))
            {
                return true;
            }

            return false;
        }

        // Helper method to check if the board is full
        private bool IsBoardFull(char[,] board)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (board[i, j] != 'X' && board[i, j] != 'O')
                    {
                        return false;
                    }
                }
            }

            return true;
        }


        // HISTORY
        private void ShowCommandHistory()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("===============================================================================");

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("                             COMMAND HISTORY");

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("===============================================================================");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" ");

            foreach (string command in commandHistory)
            {
                string timestampedCommand = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} - {command}";
                Console.WriteLine(timestampedCommand);
            }

            Console.WriteLine(" ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Type 'end' to go back to the main menu.");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" ");
            Console.Write("COMMAND > ");

            string input = Console.ReadLine();

            if (input.ToLower() == "end")
            {
                Console.Clear();
                DisplayMenu();
            }
        }

        // Display System Information
        private void DisplaySystemInformation()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine();
            Console.WriteLine($"Kernel Version: {kernelVersion}");

            Console.WriteLine($"CPU: {CPU.GetCPUBrandString()}");

            uint maxMem = CPU.GetAmountOfRAM();
            ulong availableMem = GCImplementation.GetAvailableRAM();
            ulong usedMem = maxMem - availableMem;
            Console.WriteLine($"RAM: {usedMem}/{maxMem}MB");

            Console.WriteLine($"Time at boot: {BootManager.BootTime.ToString("yyyy-MM-dd HH:mm:ss")}");
            TimeSpan uptime = BootManager.Uptime;
            Console.WriteLine($"Uptime: {uptime.Hours} hours {uptime.Minutes} minutes and {uptime.Seconds} seconds");
        }

        private void InitializeVFS()
        {
            CosmosVFS fs = new Sys.FileSystem.CosmosVFS();
            Sys.FileSystem.VFS.VFSManager.RegisterVFS(fs);

        }

        private void ListRootDirectories()
        {
            Console.WriteLine(" ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Directories in the root directory (0:\\):");

            Console.WriteLine(" ");
            var directoryList = Sys.FileSystem.VFS.VFSManager.GetDirectoryListing("0:\\");
            foreach (var directoryEntry in directoryList)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(directoryEntry.mName);
            }
        }

        private void ReadFilesInRootDirectory()
        {
            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("===============================================================================");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Reading files in the root directory (0:\\):");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("===============================================================================");
                var directoryList = Sys.FileSystem.VFS.VFSManager.GetDirectoryListing("0:\\");

                try
                {
                    // Display available files
                    Console.WriteLine(" ");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("AVAILABLE FILES:");
                    Console.ForegroundColor = ConsoleColor.White;
                    foreach (var directoryEntry in directoryList)
                    {
                        if (directoryEntry.mEntryType == Sys.FileSystem.Listing.DirectoryEntryTypeEnum.File)
                        {
                            Console.WriteLine(directoryEntry.mName);
                        }
                    }

                    Console.WriteLine(" ");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("Enter the name of the file you want to read (or type 'back' to go back): ");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    string inputCommand = Console.ReadLine();

                    if (inputCommand.ToLower() == "back")
                    {
                        // Go back to the main menu
                        DisplaySystemMenu();
                        return;
                    }

                    string fileName = inputCommand;
                    string filePath = $"0:\\{fileName}.txt";

                    // Check if the file exists
                    if (!Sys.FileSystem.VFS.VFSManager.FileExists(filePath))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"File '{fileName}.txt' does not exist. Please enter a valid file name.");
                        Console.WriteLine("Press Enter to continue...");
                        Console.ReadLine();
                        continue;
                    }

                    var file = Sys.FileSystem.VFS.VFSManager.GetFile(filePath);
                    var fileStream = file.GetFileStream();

                    byte[] content = new byte[fileStream.Length];
                    fileStream.Read(content, 0, (int)fileStream.Length);

                    Console.WriteLine(" ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("FILE NAME: " + fileName);
                    Console.WriteLine("FILE SIZE: " + file.mSize);

                    Console.WriteLine(" ");
                    Console.WriteLine("===============================================================================");
                    Console.WriteLine(" ");

                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("CONTENT: ");

                    Console.ForegroundColor = ConsoleColor.White;
                    foreach (char ch in content)
                    {
                        Console.Write(ch.ToString());
                    }

                    // Ask the user if they want to go back or read another file
                    Console.WriteLine();
                    Console.WriteLine(" ");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("Type 'back' to go back to the main menu, or press Enter to read another file:");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    string userInput = Console.ReadLine();

                    if (userInput.ToLower() == "back")
                    {
                        // Go back to the main menu
                        Console.Clear();
                        DisplaySystemMenu();
                        return;
                    }
                    // Continue the loop to read another file if the user presses Enter
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error reading file: " + e.ToString());
                    Console.WriteLine("Press Enter to continue...");
                    Console.ReadLine();
                    break;
                }
            }

            systemMenuCommandHistory.Clear();
        }



        private void WriteToFile()
        {
            try
            {
                while (true)
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine("===============================================================================");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("Editing files in the root directory (0:\\):");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine("===============================================================================");

                    var directoryList = Sys.FileSystem.VFS.VFSManager.GetDirectoryListing("0:\\");

                    // Display available text files
                    Console.WriteLine(" ");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("AVAILABLE TEXT FILES:");
                    Console.ForegroundColor = ConsoleColor.White;
                    foreach (var directoryEntry in directoryList)
                    {
                        if (directoryEntry.mEntryType == Sys.FileSystem.Listing.DirectoryEntryTypeEnum.File && directoryEntry.mName.EndsWith(".txt"))
                        {
                            Console.WriteLine(directoryEntry.mName);
                        }
                    }

                    Console.WriteLine(" ");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("Enter the title (filename) of the file you want to edit (or type 'back' to go back): ");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    string title = Console.ReadLine();

                    if (title.ToLower() == "back")
                    {
                        // Go back to the main menu
                        DisplaySystemMenu();
                        return;
                    }

                    string filePath = $"0:\\{title}.txt";

                    // Check if the file exists
                    if (!Sys.FileSystem.VFS.VFSManager.FileExists(filePath))
                    {
                        Console.WriteLine(" ");
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"File '{title}.txt' does not exist. Please enter a valid file name.");
                        Console.WriteLine("Press Enter to continue...");
                        Console.ReadLine();
                        continue;
                    }

                    var helloFile = Sys.FileSystem.VFS.VFSManager.GetFile(filePath);
                    var helloFileStream = helloFile.GetFileStream();

                    if (helloFileStream.CanWrite)
                    {
                        // Set the file length to 0 to clear existing content
                        helloFileStream.SetLength(0);

                        Console.WriteLine(" ");
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine($"Enter the new content for '{title}.txt': ");
                        Console.ForegroundColor = ConsoleColor.White;
                        string userInput = Console.ReadLine();

                        byte[] textToWrite = Encoding.ASCII.GetBytes(userInput);
                        helloFileStream.Write(textToWrite, 0, textToWrite.Length);

                        Console.WriteLine(" ");
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"File '{title}.txt' written successfully!");
                    }
                    else
                    {
                        Console.WriteLine(" ");
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"File '{title}.txt' is not writable.");
                    }

                    // Ask the user if they want to edit another file or go back
                    Console.WriteLine(" ");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("Press Enter to edit another file, or type 'back' to go back to the main menu:");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    string userInput2 = Console.ReadLine();

                    if (userInput2.ToLower() == "back")
                    {
                        systemMenuCommandHistory.Clear();
                        Console.Clear();
                        DisplaySystemMenu();
                        return;
                    }
                    // Continue with the loop to edit another file based on the user input
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(" ");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error writing to file: {e.ToString()}");
            }
        }
        private void WriteContentToFile(string filePath, string content, bool truncate = false)
        {
            try
            {
                var file = Sys.FileSystem.VFS.VFSManager.GetFile(filePath);
                using (var fileStream = file.GetFileStream())
                {
                    if (fileStream.CanWrite)
                    {
                        if (truncate)
                        {
                            // Set the file length to 0, effectively truncating the file
                            fileStream.SetLength(0);
                        }

                        byte[] textToWrite = Encoding.UTF8.GetBytes(content);
                        fileStream.Write(textToWrite, 0, textToWrite.Length);
                    }
                }

                Console.WriteLine(" ");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"File '{filePath}' written successfully!");
            }
            catch (Exception e)
            {
                Console.WriteLine(" ");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error writing content to file: {e.ToString()}");
            }
        }

        private void CreateFile()
        {
            try
            {
                while (true)
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine("===============================================================================");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("Creating files in the root directory (0:\\):");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine("===============================================================================");

                    var directoryList = Sys.FileSystem.VFS.VFSManager.GetDirectoryListing("0:\\");

                    // Display available text files
                    Console.WriteLine(" ");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("AVAILABLE TEXT FILES:");
                    Console.ForegroundColor = ConsoleColor.White;
                    foreach (var directoryEntry in directoryList)
                    {
                        if (directoryEntry.mEntryType == Sys.FileSystem.Listing.DirectoryEntryTypeEnum.File && directoryEntry.mName.EndsWith(".txt"))
                        {
                            Console.WriteLine(directoryEntry.mName);
                        }
                    }

                    Console.WriteLine(" ");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("Enter the title (filename) for the new file (or type 'back' to go back): ");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    string title = Console.ReadLine();

                    if (title.ToLower() == "back")
                    {
                        // Go back to the main menu
                        DisplaySystemMenu();
                        return;
                    }

                    string filePath = $"0:\\{title}.txt";

                    // Check if the file already exists
                    if (Sys.FileSystem.VFS.VFSManager.FileExists(filePath))
                    {
                        Console.WriteLine(" ");
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"File '{title}.txt' already exists. Please enter a different file name.");
                        Console.WriteLine("Press Enter to continue...");
                        Console.ReadLine();
                        continue;
                    }

                    Console.WriteLine(" ");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"Enter the content for '{title}': ");
                    Console.ForegroundColor = ConsoleColor.White;
                    string content = Console.ReadLine();

                    Sys.FileSystem.VFS.VFSManager.CreateFile(filePath);
                    WriteContentToFile(filePath, content, false); // Pass false to indicate no truncation

                    Console.WriteLine(" ");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"File '{title}.txt' created and content written successfully!");

                    // Ask the user if they want to create another file or go back
                    Console.WriteLine(" ");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("Press Enter to create another file, or type 'back' to go back to the main menu:");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    string userInput = Console.ReadLine();

                    if (userInput.ToLower() == "back")
                    {
                        systemMenuCommandHistory.Clear();
                        Console.Clear();
                        DisplaySystemMenu();
                        return;
                    }
                    // Continue with the loop to create another file based on the user input
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(" ");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error creating file: {e.ToString()}");
            }
        }


        private void DeleteFile()
        {
            try
            {
                while (true)
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine("===============================================================================");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("Deleting files in the root directory (0:\\):");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine("===============================================================================");

                    var directoryList = Sys.FileSystem.VFS.VFSManager.GetDirectoryListing("0:\\");

                    // Display available text files
                    Console.WriteLine(" ");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("AVAILABLE TEXT FILES:");
                    Console.ForegroundColor = ConsoleColor.White;
                    foreach (var directoryEntry in directoryList)
                    {
                        if (directoryEntry.mEntryType == Sys.FileSystem.Listing.DirectoryEntryTypeEnum.File && directoryEntry.mName.EndsWith(".txt"))
                        {
                            Console.WriteLine(directoryEntry.mName);
                        }
                    }

                    Console.WriteLine(" ");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("Enter the title (filename) of the file you want to delete (or type 'back' to go back): ");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    string title = Console.ReadLine();

                    if (title.ToLower() == "back")
                    {
                        // Go back to the main menu
                        DisplaySystemMenu();
                        return;
                    }

                    string filePath = $"0:\\{title}.txt";

                    // Check if the file exists before attempting to delete
                    if (Sys.FileSystem.VFS.VFSManager.FileExists(filePath))
                    {
                        Sys.FileSystem.VFS.VFSManager.DeleteFile(filePath);

                        Console.WriteLine(" ");
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"File '{title}.txt' deleted successfully!");
                    }
                    else
                    {
                        Console.WriteLine(" ");
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"File '{title}.txt' does not exist or could not be deleted.");
                    }

                    // Ask the user if they want to delete another file or go back
                    Console.WriteLine(" ");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("Press Enter to delete another file, or type 'back' to go back to the main menu:");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    string userInput = Console.ReadLine();

                    if (userInput.ToLower() == "back")
                    {
                        systemMenuCommandHistory.Clear();
                        Console.Clear();
                        DisplaySystemMenu();
                        return;
                    }
                    // Continue with the loop to delete another file based on the user input
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(" ");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error deleting file: {e.ToString()}");
            }
        }


        // CONDITION LOGIC TO RUN THE COMMAND
        protected override void Run()
        {
            Console.WriteLine(" ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("COMMAND > ");
            var input = Console.ReadLine();

            if (input.ToLower() == "system")
            {
                Console.Clear();
                DisplaySystemMenu();
            }
            else if (input.ToLower() == "restart")
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Restarting brdOS...");
                Cosmos.System.Power.Reboot();
            }
            else if (input.ToLower() == "shutdown")
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Shutting down brdOS...");
                Cosmos.System.Power.Shutdown();
            }
            else if (input.ToLower() == "datetime")
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"Current System Date and Time: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
            }
            else if (input.ToLower() == "disuser")
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"Current User: {currentUser}");
            }
            else if (input.ToLower() == "clear")
            {
                Console.Clear();
                DisplayMenu();
            }
            else if (input.ToLower() == "logout")
            {
                LogOut();
            }
            else if (input.ToLower() == "calc")
            {
                RunCalculator();
            }
            else if (input.ToLower() == "rps")
            {
                RunRockPaperScissors();
            }
            else if (input.ToLower() == "tictactoe")
            {
                RunTicTacToe();
            }
            else if (input.ToLower() == "history")
            {
                ShowCommandHistory();
            }
      
            else if (input.ToLower() == "info")
            {
                Kernel kernel = new Kernel();
                kernel.DisplaySystemInformation();
            }

            else if (input.ToLower() == "storage")
            {
                Console.WriteLine(" ");
                Console.ForegroundColor = ConsoleColor.Cyan;
                long availableSpace = Sys.FileSystem.VFS.VFSManager.GetAvailableFreeSpace("0:\\");
                Console.WriteLine("Available Free Space: " + availableSpace + " bytes");


                string fsType = Sys.FileSystem.VFS.VFSManager.GetFileSystemType("0:\\");
                Console.WriteLine("File System Type: " + fsType);
            }

            else if (input.ToLower() == "version")
            {
                Console.WriteLine(" ");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"Kernel Version: {kernelVersion}");
            }

            else if (input.ToLower() == "listdirs")
            {
                ListRootDirectories();
            }

            else if (input.ToLower() == "read")
            {
                ReadFilesInRootDirectory();
            }

            else if (input.ToLower() == "update")
            {
                WriteToFile();
            }

            else if (input.ToLower() == "create")
            {
                CreateFile();
            }

            else if (input.ToLower() == "delete")
            {
                DeleteFile();
            }

            else if (input.ToLower() == "back")
            {  
                Console.Clear();
                DisplaySystemMenu();
            }

            else if (input.ToLower() == "sysclear")
            {
                systemMenuCommandHistory.Clear();
                Console.Clear();
                DisplaySystemMenu();
            }

            else if (input.ToLower() == "end")
            {
                Console.Clear();
                DisplayMenu();

                //commandHistory.AddRange(systemMenuCommandHistory);
                systemMenuCommandHistory.Clear();
                return;
            }

            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Unknown command. Please type a valid command.");
            }

            commandHistory.Add(input);
            Console.WriteLine();
        }

    }
}
