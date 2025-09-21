using System;
using System.Drawing;
using System.Threading;
using Gma.System.MouseKeyHook;
using WindowsInput;
using WindowsInput.Native;

class MAIN
{



    public static void Main()
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

        
        while (playing && count < 1000)
        {
            Thread.Sleep(100);

            // Simulating space press for debug
            inputSim.Keyboard.KeyPress(VirtualKeyCode.SPACE);
            boardHandler.setFallingSettled();
            
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