using System.Drawing;
using System;
using System.Runtime.InteropServices;

class UI_READING
{

    // Importing the GetSystemMetrics function from user32.dll to get screen dimensions
    [DllImport("user32.dll")]
    static extern int GetSystemMetrics(int nIndex);
    const int SM_CXSCREEN = 0;
    const int SM_CYSCREEN = 1;


    public Color borderColour = Color.FromArgb(255, 0x24, 0x23, 0x23); // Colour of the border to ignore
    private Point sourceTopLeft = new Point(9999, 9999);  // The top left co'ordinate of the game screen
    private Point sourceBottomRight = new Point(0, 0); // The bottom right co'ordinate of the game screen
    private Size gameSize = new Size(0, 0);         // The size of the game screen (Width, Height in pixels)

    public Bitmap getGameScreenshot()
    {
        // Defining screenshot parameters 
        Point destTopLeft = Point.Empty;        // The top left co'ordinate of the screenshot (empty bitmap)

        // Creating a bitmap to hold the screenshot
        Bitmap screenshot = new Bitmap(gameSize.Width, gameSize.Height);

        // Filling the bitmap with the screenshot
        Graphics g = Graphics.FromImage(screenshot);
        g.CopyFromScreen(sourceTopLeft, destTopLeft, gameSize);
        return screenshot;

    }

    public Bitmap getScreenshot(Point sourceTopLeft, Size size)
    {
        // Defining Empty Bitmap
        Point destTopLeft = Point.Empty;        // The top left co'ordinate of the screenshot (empty bitmap)
        Bitmap screenshot = new Bitmap(size.Width, size.Height);

        // Filling the bitmap with the screenshot
        Graphics g = Graphics.FromImage(screenshot);
        g.CopyFromScreen(sourceTopLeft, destTopLeft, size);
        return screenshot;

    }

    public Bitmap getFullScreenshot()
    {
        // Define parameters for full screen capture
        Point sourceTopLeft = new Point(0, 0);  // The top left co'ordinate of the screen

        // Get the screen dimensions from the system
        int screenWidth = GetSystemMetrics(SM_CXSCREEN);
        int screenHeight = GetSystemMetrics(SM_CYSCREEN);
        Size size = new Size(screenWidth, screenHeight); // Full screen size

        // Taking the screenshot
        return getScreenshot(sourceTopLeft, size);

    }

    public bool similarColours(Color colour1, Color colour2, int tolerance)
    {
        if (Math.Abs(colour1.R) - Math.Abs(colour2.R) > tolerance)
        {
            return false;
        }
        if (Math.Abs(colour1.G) - Math.Abs(colour2.G) > tolerance)
        {
            return false;
        }
        if (Math.Abs(colour1.B) - Math.Abs(colour2.B) > tolerance)
        {
            return false;
        }

        return true;
    }

    public void findBottomRight(Bitmap bmp, Color targetColour)
    {
        for (int y = 0; y < bmp.Height; y++)
        {
            for (int x = 0; x < bmp.Width; x++)
            {
                Color pixelColour = bmp.GetPixel(x, y);
                if (pixelColour == targetColour)
                {
                    if (this.sourceBottomRight.X <= x && this.sourceBottomRight.Y < y)
                    {
                        this.sourceBottomRight = new Point(x, y);
                    }
                    if (this.sourceBottomRight.X < x && this.sourceBottomRight.Y <= y)
                    {
                        this.sourceBottomRight = new Point(x, y);
                    }
                }
            }
        }
    }

    public void findRectangleFromBottomRight(Bitmap bmp, Point bottomRight)
    {
        // Defining variables
        Point currentPoint = bottomRight;
        Color targetColour = bmp.GetPixel(currentPoint.X, currentPoint.Y);
        Color currentColour;
        int colourTolerance = 0;
        bool endOfLine = false;
        Direction[] directions = { Direction.Left, Direction.Up, Direction.Right, Direction.Down };

        foreach (Direction currentDirection in directions)
        {
            while (!endOfLine)
            {
                switch (currentDirection)
                {
                    case Direction.Left:
                        currentPoint.X -= 1;
                        break;
                    case Direction.Up:
                        currentPoint.Y -= 1;
                        break;
                    case Direction.Right:
                        currentPoint.X += 1;
                        break;
                    case Direction.Down:
                        currentPoint.Y += 1;
                        break;
                }
                currentColour = bmp.GetPixel(currentPoint.X, currentPoint.Y);
                if (!similarColours(currentColour, targetColour, colourTolerance))
                {
                    endOfLine = true;
                }
            }
        }


    }

    public void getGameDimensions(Bitmap bmp, Color targetColor)
    {
        for (int y = 0; y < bmp.Height; y++)
        {
            for (int x = 0; x < bmp.Width; x++)
            {
                Color pixelColor = bmp.GetPixel(x, y);
                if (pixelColor == targetColor)
                {
                    if (this.sourceTopLeft.X + this.sourceTopLeft.Y > x + y)
                    {
                        this.sourceTopLeft = new Point(x, y);
                    }
                    else if (this.sourceTopLeft.X < x && this.sourceTopLeft.Y < y)
                    {
                        this.sourceBottomRight = new Point(x, y);
                    }
                }
            }
        }
        int width = this.sourceBottomRight.X - this.sourceTopLeft.X;
        int height = this.sourceBottomRight.Y - this.sourceTopLeft.Y;
        this.gameSize = new Size(width, height);
    }
    public static void Main()
    {
        Console.WriteLine("Start of program");
        Thread.Sleep(10000);
        UI_READING ui = new UI_READING();
        Bitmap fullScreenshot = ui.getFullScreenshot();
        fullScreenshot.Save("images/fullScreenshot.png");
        ui.getGameDimensions(fullScreenshot, ui.borderColour);
        ui.getGameScreenshot().Save("images/gameScreenshot.png");
        Console.WriteLine("End of program");
    }
    
    enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }
    
}