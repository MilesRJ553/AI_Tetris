using System.Drawing;
using System.Drawing.Imaging;

class DEBUG
{
    public static void main()
    {

        // UIReader uiReader = new UIReader();
        // Bitmap bmp = new Bitmap("gameScreenshot.png");
        // bool[,] gameBoard = uiReader.getGameGrid(bmp);
        // Console.WriteLine(gameBoard.ToString());

        E_CELL_STATUS[,] debugGameBoard = new E_CELL_STATUS[20,10];

        // Iterate through each cell of the gameBoard
        for (int row = 0; row < debugGameBoard.GetLength(0); ++row)
        {
            for (int col = 0; col < debugGameBoard.GetLength(1); ++col)
            {
                // Set each cell as empty
                debugGameBoard[row, col] = E_CELL_STATUS.EMPTY;
            }
        }

        (int, int)[] settledCoords = {(19,1), (19,2), (19,4), (19,8), (19,9), (18,2), (18,3), (18,4), (18,5), (18,8), (18,9), (17,4)};
        foreach ((int,int) coord in settledCoords) {
            debugGameBoard[coord.Item1, coord.Item2] = E_CELL_STATUS.SETTLED;
        }

        (int,int)[] fallingCoords = {(0, 5), (1,5), (1,4), (1,3)};

        foreach ((int,int) coord in fallingCoords) {
            debugGameBoard[coord.Item1, coord.Item2] = E_CELL_STATUS.FALLING;
        }

        debugGameBoard = Player.visualiseDropPiece(debugGameBoard);
    }



}