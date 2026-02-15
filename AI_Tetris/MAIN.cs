using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Threading;
using Gma.System.MouseKeyHook;
using WindowsInput;
using WindowsInput.Native;

class MAIN
{

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
        Thread.Sleep(10000);

        // Instantiate classes used
        UI_READER uiReader = new UI_READER();
        BOARD_HANDLER boardHandler = new BOARD_HANDLER();
        Random rnd = new Random();
        InputSimulator inputSim = new InputSimulator();

        //Instantiate variables used
        bool[,] uiGameBoard;
        bool playing = true;

        // Find the board and define its attributes within uiReader
        uiGameBoard = uiReader.getGameGrid();
        printGameBoard(uiGameBoard);
        int count = 0;

        // Main Loop
        while (playing && count < 1000)
        {
            Thread.Sleep(500);

            // // Stop the program after 30 seconds
            // if (count >= 15)
            // {
            //     Environment.Exit(-1);
            // }

            // Simulating space press for debug
            if (count % 10 == 0)
            {
                inputSim.Keyboard.KeyPress(VirtualKeyCode.SPACE);
                boardHandler.setFallingSettled();
            }

            uiGameBoard = uiReader.getGameGrid();
            boardHandler.boardHandlingMain(uiGameBoard);
            boardHandler.printGameBoard();
            ++count;
        }

        Console.WriteLine("=========\n=========\nEnd of program\n=========\n=========");

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


} 