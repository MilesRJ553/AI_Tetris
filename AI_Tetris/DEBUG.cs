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

        (int, int)[] settledCoords = {(19,1), (18,2), (18,3), (19,3), (15,7),(18,9)};
        foreach ((int,int) coord in settledCoords) {
            debugGameBoard[coord.Item1, coord.Item2] = E_CELL_STATUS.SETTLED;
        }

        (int,int)[] fallingCoords = {};

        foreach ((int,int) coord in fallingCoords) {
            debugGameBoard[coord.Item1, coord.Item2] = E_CELL_STATUS.FALLING;
        }

        MoveRater moveRater = new MoveRater(0, 0, 0, 1);
        // int elevChange = moveRater.getElevationChange(debugGameBoard);
        // Console.WriteLine(elevChange.ToString());
    }



}