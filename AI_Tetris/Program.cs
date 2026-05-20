using System;
using System.Drawing;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Forms;
using Gma.System.MouseKeyHook;
using WindowsInput;
using WindowsInput.Native;

class Program
{

    static bool dbg = true;

    /// <summary>
    /// A dictionary containing all error codes used as the key and a description of them as the value
    /// </summary>
    Dictionary<int, string> errorCodes = new Dictionary<int, string>
    {
        {-1, "Stopped for debug"},
        { 1, "Unable to find board"}
    };

    public static void Main()
    {
        // DEBUG.main();
        mainMethod();
    }

    public static void mainMethod()
    {
        Console.WriteLine("=========\n=========\nStart of Program\n=========\n=========");

        // Wait 10 seconds to allow the user to open the game
        // Thread.Sleep(10_000);

        // Define classes used
        UIReader uiReader = new UIReader();
        BoardHandler boardHandler;
        MoveRaterFactory moveRaterFactory = new MoveRaterFactory();
        Player player;

        //Instantiate variables used
        bool[,] uiGameBoard;
        bool playing = true;
        bool gameOver;
        bool isFirstGame = true;

        // Find the board and define its attributes within uiReader
        uiGameBoard = uiReader.getGameGrid();
        printGameBoard(uiGameBoard);
        int count = 0;

        // Main Loop
        while(playing) {
            // Create new player
            boardHandler = new BoardHandler();
            MoveRater moveRater = moveRaterFactory.createCandidateMoveRater();
            player = new Player(boardHandler, moveRater);

            Console.WriteLine($"=========\n=========\nNew Game\nNew Move Rater: {moveRater.ToString()}\n=========\n=========");

            // Start a new game
            if (!isFirstGame)
            {
                player.startNewGame(uiReader);
                Thread.Sleep(100);
            }
            
            // Play the game
            gameOver = false;
            while (!gameOver)
             {
                Thread.Sleep(50);
                uiGameBoard = uiReader.getGameGrid();
                boardHandler.boardHandlingMain(uiGameBoard, dbg);
                if (count % 1 == 0)
                {
                    // Make a move
                    player.chooseAndMakeMove(uiReader, dbg); 
                    
                    // Check for game over
                    if (player.checkGameOver())
                    {
                        gameOver = true;
                    }
                }
                if (dbg)
                {
                    boardHandler.printGameBoard();
                }
                ++count;
            }
            System.TimeSpan gameTime = player.getTotalGameTime();
            Console.WriteLine($"=========\n=========\nGame Over\nTotal Game Time: {gameTime.TotalMinutes:F2} minutes\n=========\n=========");
            // moveRaterFactory.saveResults(gameTime.TotalSeconds);
            isFirstGame = false;
        }
    }


/* =============== Debug Methods =============== */
    public static void printGameBoard(bool[,] gameBoard)
    {
        for (int row = 0; row < gameBoard.GetLength(0); ++row)
        {
            for (int col = 0; col < gameBoard.GetLength(1); ++col)
            {
                Console.Write(String.Format("|{0}| ", gameBoard[row, col]));
            }
            Console.Write("\n");
        }
    }

    public static void printListOfQueues(List<Queue<VirtualKeyCode>> lst)
    { 
        foreach (Queue<VirtualKeyCode> q in lst) 
        {
            Console.WriteLine("");
            while (q.Count > 0)
            {
                Console.Write(q.Dequeue().ToString());
                Console.Write(" ");
            }
        }
    }


} 