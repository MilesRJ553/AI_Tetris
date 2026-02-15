using System.Drawing;
using System.Drawing.Imaging;

class DEBUG
{
    public static void main()
    {
        Thread.Sleep(30);

        UI_READER uiReader = new UI_READER();
        Bitmap bmp = new Bitmap("gameScreenshot.png");
        bool[,] gameBoard = uiReader.getGameGrid(bmp);
        Console.WriteLine(gameBoard.ToString());
        
    }
}