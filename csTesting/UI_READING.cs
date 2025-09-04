using System.Drawing;
using System.Windows.Forms;

class UI_READING
{

    public void getScreenshot()
    {
        // Defining screenshot parameters
        Point sourceTopLeft = new Point(0, 0);  // The top left co'ordinate of the game screen
        Point destTopLeft = Point.Empty;        // The top left co'ordinate of the screenshot (empty bitmap)
        Size size = new Size(800, 600);         // The size of the game screen (Width, Height in pixels)

        // Creating a bitmap to hold the screenshot
        Bitmap screenshot = new Bitmap(size.Width, size.Height);

        // Filling the bitmap with the screenshot
        Graphics g = Graphics.FromImage(screenshot);
        g.CopyFromScreen(sourceTopLeft, destTopLeft, size);
        screenshot.Save("images/testScreenshot.png");

    }

    public void getScreenshot(Point sourceTopLeft, Size size)
    {
        // Defining Empty Bitmap
        Point destTopLeft = Point.Empty;        // The top left co'ordinate of the screenshot (empty bitmap)
        Bitmap screenshot = new Bitmap(size.Width, size.Height);

        // Filling the bitmap with the screenshot
        Graphics g = Graphics.FromImage(screenshot);
        g.CopyFromScreen(sourceTopLeft, destTopLeft, size);
        screenshot.Save("images/testScreenshot.png");

    }

    public void getFullScreenshot()
    {
        Point sourceTopLeft = new Point(0, 0);  // The top left co'ordinate of the screen
        Size size = new Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height); // Full screen size


    }

    [STAThread]
    public static void Main()
    {
        UI_READING ui = new UI_READING();
        ui.getScreenshot();
    }
    
}