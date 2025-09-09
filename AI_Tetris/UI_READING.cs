using System.Drawing;
using System;
using System.Runtime.InteropServices;
using WindowsInput;
using WindowsInput.Native;

class UI_READING
{

    // Importing the GetSystemMetrics function from user32.dll to get screen dimensions
    [DllImport("user32.dll")]
    static extern int GetSystemMetrics(int nIndex);
    const int SM_CXSCREEN = 0;
    const int SM_CYSCREEN = 1;


    //  public Color borderColour = Color.FromArgb(255, 0x24, 0x23, 0x23); // Colour of the border to ignore
    public Color borderColour = Color.FromArgb(255, 0, 0, 0); // Colour of the border to ignore
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
        Console.WriteLine(String.Format("Width: {0}, Height: {1}", screenWidth, screenHeight));
        Size size = new Size(screenWidth,screenHeight); // Full screen size

        // Taking the screenshot
        return getScreenshot(sourceTopLeft, size);

    }

    public bool similarColours(Color colour1, Color colour2)
    {
        int tolerance = 5;
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

    public void getGameDimensions(Bitmap bmp, Color targetColor)
    {
        IList<Point> points = new List<Point>();

        for (int y = 0; y < bmp.Height; y++)
        {
            for (int x = 0; x < bmp.Width; x++)
            {
                Color pixelColor = bmp.GetPixel(x, y);
                if (pixelColor == targetColor)
                {
                    points.Add(new Point(x, y));
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

        this.paintPoints(points);

    }

    // For debugging only
    private async void paintPoints(IList<Point> points)
    {
        InputSimulator inputSim = new InputSimulator();

        await Task.Delay(1000); // Wait for text entry to complete
/*
        inputSim.Keyboard.TextEntry("mspaint");

        Thread.Sleep(500);

        inputSim.Keyboard.KeyPress(VirtualKeyCode.RETURN);

        Thread.Sleep(5000);
*/
        foreach (Point point in points)
        {
            Thread.Sleep(10);
            inputSim.Mouse.MoveMouseTo(point.X * (65535.0 / GetSystemMetrics(SM_CXSCREEN)), point.Y * (65535.0 / GetSystemMetrics(SM_CYSCREEN)));
            inputSim.Mouse.LeftButtonClick();
        }


    }
    public static void Main()
    {
        Console.WriteLine("Start of program");
        Thread.Sleep(10000);
        UI_READING ui = new UI_READING();
        Bitmap fullScreenshot = ui.getFullScreenshot();
        fullScreenshot.Save("images/fullScreenshot.png");
        ui.getGameDimensions(fullScreenshot, ui.borderColour);
        // Check a game has been detected and save an image of it
        if (ui.gameSize.Width <= 0 || ui.gameSize.Width <= 0)
        {
            throw new EntryPointNotFoundException("No game detected on screen");
        }
        else
        {
            ui.getGameScreenshot().Save("images/gameScreenshot.png");
        }
        Console.WriteLine("End of program");
    }
    
}