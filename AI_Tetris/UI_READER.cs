using System.Drawing;
using System.Runtime.InteropServices;
using WindowsInput;

class UI_READER
{

    // Importing the GetSystemMetrics function from user32.dll to get screen dimensions
    [DllImport("user32.dll")]
    static extern int GetSystemMetrics(int nIndex);
    const int SM_CXSCREEN = 0;
    const int SM_CYSCREEN = 1;

    // Colours to be used
    private Color borderColour = Color.FromArgb(255, 0x24, 0x23, 0x23); // Colour of the border to ignore
    private Color backgroundColour = Color.FromArgb(255, 0, 0, 0); // Colour of the border to ignore
    private Point sourceTopLeft = new Point(9999, 9999);  // The top left co'ordinate of the game screen
    private Point sourceBottomRight = new Point(0, 0); // The bottom right co'ordinate of the game screen
    private Size gameSize = new Size(0, 0);         // The size of the game screen (Width, Height in pixels)



    /* =============== Constructors =============== */

    /// <summary>
    /// Constructor for UI_READER
    /// Defines the game dimensions based on the default border Colour
    /// </summary>
    public UI_READER()
    {
        // Find the game board and define its attributes in this instance of UI_READER
        getGameDimensions();

    }

    /// <summary>
    /// Constructor for UI_READER
    /// Defines the game dimensions based on the border Colour provided as an argument
    /// Changes the background Colour to the one provided as an argument
    /// </summary>
    public UI_READER(Color borderColour, Color backgroundColour)
    {
        // Defines the borderColour and backgroundcolour based on the arguments provided
        this.borderColour = borderColour;
        this.backgroundColour = backgroundColour;


        // Find the game board and define its attributes in this instance of UI_READER
        getGameDimensions();
    }

    /// <summary>
    /// Constructor for UI_READER
    /// Defines the game dimensions based on the border Colour provided as an argument
    /// </summary>
    public UI_READER(Color borderColour)
    {
        // Defines the borderColour based on the argument provided
        this.borderColour = borderColour;

        // Find the game board and define its attributes in this instance of UI_READER
        getGameDimensions();
    }




    /* =============== Methods =============== */
    /// <summary>
    /// Gets a Screenshot of the game board based on the attributes defined of this class
    /// </summary>
    /// <returns>Screenshot</returns>
    public Bitmap getGameScreenshot()
    {
        return getScreenshot(this.sourceTopLeft, this.gameSize);

    }

    /// <summary>
    /// Gets a screenshot of a portion of the screen based on arguments provided
    /// </summary>
    /// <param name="sourceTopLeft"></param>
    /// <param name="size"></param>
    /// <returns>Screenshot</returns>
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

    /// <summary>
    /// </summary>
    /// <returns>Screenshot of entire display</returns>
    public Bitmap getFullScreenshot()
    {
        // Define parameters for full screen capture
        Point sourceTopLeft = new Point(0, 0);  // The top left co'ordinate of the screen

        // Get the screen dimensions from the system
        int screenWidth = GetSystemMetrics(SM_CXSCREEN);
        int screenHeight = GetSystemMetrics(SM_CYSCREEN);
        Console.WriteLine(String.Format("Width: {0}, Height: {1}", screenWidth, screenHeight));
        Size size = new Size(screenWidth, screenHeight); // Full screen size

        // For debugging purposes


        // Taking the screenshot
        Bitmap fullScreenshot = getScreenshot(sourceTopLeft, size);
        fullScreenshot.Save("fullScreenshot.png");

        return fullScreenshot;

    }

    /// <summary>
    /// Finds the dimensions of the game board based on the border colour and a bitmap of the screen to check
    /// </summary>
    /// <param name="borderColour"></param>
    public void getGameDimensions(Bitmap bmp, Color borderColour)
    {
        // Finding the bottom right most instance of the borderColour
        this.sourceBottomRight = this.findBottomRight(bmp, borderColour);
        // Finding the top left corner of a rectange of the borderColour
        this.sourceTopLeft = this.findOppositeCorner(bmp, this.sourceBottomRight);

        // Defining gameSize based on the border found
        int width = this.sourceBottomRight.X - this.sourceTopLeft.X;
        int height = this.sourceBottomRight.Y - this.sourceTopLeft.Y;
        this.gameSize = new Size(width, height);
    }

    /// <summary>
    /// Finds the dimensions of the game board based on the border colour
    /// Automatically takes a screenshot of the entire screen to do this
    /// </summary>
    /// <param name="borderColour"></param>
    public void getGameDimensions(Color borderColour)
    {
        // Defining local variables based on class attributes
        Bitmap bmp = getFullScreenshot();

        // Finding the bottom right most instance of the borderColour
        this.sourceBottomRight = this.findBottomRight(bmp, borderColour);
        // Finding the top left corner of a rectange of the borderColour
        this.sourceTopLeft = this.findOppositeCorner(bmp, this.sourceBottomRight);

        // Defining gameSize based on the border found
        int width = this.sourceBottomRight.X - this.sourceTopLeft.X;
        int height = this.sourceBottomRight.Y - this.sourceTopLeft.Y;
        this.gameSize = new Size(width, height);
    }

    /// <summary>
    /// Finds the dimensions of the game board based on the border colour defined as an attribute of this instance of UI_READER
    /// Automatically takes a screenshot of the entire screen to do this
    /// </summary>
    /// <param name="borderColour"></param>
    public void getGameDimensions()
    {
        // Defining local variables based on class attributes
        Bitmap bmp = getFullScreenshot();
        Color borderColour = this.borderColour;

        // Finding the bottom right most instance of the borderColour
        this.sourceBottomRight = this.findBottomRight(bmp, borderColour);
        // Finding the top left corner of a rectange of the borderColour
        this.sourceTopLeft = this.findOppositeCorner(bmp, this.sourceBottomRight);

        // Defining gameSize based on the border found
        int width = this.sourceBottomRight.X - this.sourceTopLeft.X;
        int height = this.sourceBottomRight.Y - this.sourceTopLeft.Y;
        this.gameSize = new Size(width, height);
    }


    public Point findBottomRight(Bitmap bmp, Color targetColour)
    {

        // Define Variables
        Point currentPoint = new Point(0, 0);

        for (int y = 0; y < bmp.Height; y++)
        {
            for (int x = 0; x < bmp.Width; x++)
            {
                Color pixelColour = bmp.GetPixel(x, y);
                if (pixelColour == targetColour)
                {
                    if (currentPoint.X < x || currentPoint.Y < y)
                    {
                        currentPoint = new Point(x, y);
                    }
                }
            }
        }

        Console.WriteLine(String.Format("Bottom Right Point Found: X: {0}, Y: {1}", currentPoint.X, currentPoint.Y));

        return currentPoint;
    }

    public Point findOppositeCorner(Bitmap bmp, Point bottomRight)
    {
        // Defining variables
        Point currentPoint = bottomRight;
        Color targetColour = bmp.GetPixel(currentPoint.X, currentPoint.Y);
        Color currentColour;
        int colourTolerance = 0;
        bool endOfLine = false;
        E_DIRECTION[] directionsToCheck = { E_DIRECTION.Left, E_DIRECTION.Up };


        // Iterates through each direction to find a complete rectangle
        foreach (E_DIRECTION currentDirection in directionsToCheck)
        {
            endOfLine = false; // Resets the endOfLine variable for the next direction
            while (!endOfLine)
            {
                // Moves the current point in the specified direction
                switch (currentDirection)
                {
                    case E_DIRECTION.Left:
                        currentPoint.X -= 1;
                        break;
                    case E_DIRECTION.Up:
                        currentPoint.Y -= 1;
                        break;
                    case E_DIRECTION.Right:
                        currentPoint.X += 1;
                        break;
                    case E_DIRECTION.Down:
                        currentPoint.Y += 1;
                        break;
                }
                currentColour = bmp.GetPixel(currentPoint.X, currentPoint.Y);
                if (!similarColours(currentColour, targetColour, colourTolerance)) //If the colour encountered is different, end the while loop
                {
                    endOfLine = true;
                }
            }


        }

        Console.WriteLine(String.Format("Top Left Point Found: X: {0}, Y: {1}", currentPoint.X, currentPoint.Y));

        return currentPoint; // Returns the top left point found

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

    /// <summary>
    /// Reutns a 2D boolean array showing which cells are filled based on gameScreenshot and the attributes defined in the class
    /// Assumes a 10X15 grid (standard tetris)
    /// </summary>
    /// <param name="gameScreenshot"></param>
    /// <returns>2D boolean array</returns>
    public bool[,] getGameGrid(Bitmap gameScreenshot)
    {
        // Define the grid size and create 2D array
        int gridWidth = 10;
        int gridHeight = 15;
        bool[,] gameGrid = new bool[gridHeight, gridWidth];

        // calculate the size of each cell in the grid
        int cellWidth = gameScreenshot.Width / gridWidth;
        int cellHeight = gameScreenshot.Height / gridHeight;

        // Define other variables to be used
        Point cellCentrePixel = new Point(0, 0);
        Color cellColour;
        int colourSimilarityTolerance = 10;


        // Iterate through each cell
        for (int row = 0; row < gridHeight; ++row)
        {
            for (int col = 0; col < gridWidth; ++col)
            {
                Console.WriteLine(String.Format($"row: {row}, col: {col}"));
                // Find the centre of each cell
                cellCentrePixel.X = (col * cellWidth) + (cellWidth / 2); // Half the width of a cell + the number of cells to its left * their width
                cellCentrePixel.Y = (row * cellHeight) + (cellHeight / 2); // Half the height of a cell + the number of cells above it * their height

                // Find the colour of the centre of each cell
                cellColour = gameScreenshot.GetPixel(cellCentrePixel.X, cellCentrePixel.Y);

                // Set the corresaponding value in the gameGrid to true if the cell is not black
                if (similarColours(cellColour, backgroundColour, colourSimilarityTolerance))
                {
                    gameGrid[row, col] = false;
                }
                else
                {
                    gameGrid[row, col] = true;
                }

            }
        }

        // Returns the gameGrid (2D array)
        return gameGrid;

    }

    /// <summary>
    /// Reutns a 2D boolean array showing which cells are filled based on a game board found using attributes set in this instance of UI_READER
    /// Assumes a 10X15 grid (standard tetris)
    /// </summary>
    /// <param name="gameScreenshot"></param>
    /// <returns>2D boolean array</returns>
    public bool[,] getGameGrid()
    {
        Bitmap gameScreenshot = getGameScreenshot();
        gameScreenshot.Save("images/gameScreenshot.png");
        return getGameGrid(gameScreenshot);
    }











    // For debugging only
    public void debug()
    {
        Console.WriteLine("=========\n=========\nStart of Program\n=========\n=========");

        UI_READER ui = new UI_READER();
        Bitmap fullScreenshot = ui.getFullScreenshot();
        Console.WriteLine("Saving full screenshot");
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

    public void debug_dump_everything()
    {
        Console.WriteLine("borderColour: {0}", borderColour);
        Console.WriteLine("backgroundColour: {0}", backgroundColour);
        Console.WriteLine("sourceTopLeft: {0}", sourceTopLeft);
        Console.WriteLine("sourceBottomRight: {0}", sourceBottomRight);
        Console.WriteLine("gameSize: {0}", gameSize);
    }

}
