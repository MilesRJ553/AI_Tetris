using System.Drawing;

class MAIN
{



    public static void Main()
    {

        // Wait to allow the user to open the game
        Thread.Sleep(10000);

        // Instantiate classes used
        UI_READER uiReader = new UI_READER();

        // Instantiate variables used
        bool[,] gameBoard;


        Console.WriteLine("=========\n=========\nStart of Program\n=========\n=========");

        // Find the board and define its attributes within uiReader
        gameBoard = uiReader.getGameGrid();
        printGameBoard(gameBoard);

        Console.WriteLine("=========\n=========\nEnd of program\n=========\n=========");
    }



    /* =============== Debug Methods =============== */
    public static void printGameBoard(bool[,] gameBoard)
    {
        for (int row = 0; row < gameBoard.GetLength(0); ++row)
        {
            for (int col = 0; col < gameBoard.GetLength(1); ++col)
            {
                Console.Write(String.Format("{0}, ", gameBoard[row, col]));
            }
            Console.Write("\n");
        }
    }

} 