using System.Drawing;
using System.Runtime.InteropServices;
using WindowsInput;
using System;
using System.Drawing.Imaging;

class UIReader
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
    public UIReader()
    {
        try 
        {
            // Find the game board and define its attributes in this instance of UI_READER
            getGameDimensions();
        }
        catch
        {
            Console.WriteLine("Unable to find game board, please go to https://play.tetris.com/");
            Environment.Exit(1);
        }

    }

    /// <summary>
    /// Constructor for UI_READER
    /// Defines the game dimensions based on the border Colour provided as an argument
    /// Changes the background Colour to the one provided as an argument
    /// </summary>
    public UIReader(Color borderColour, Color backgroundColour)
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
    public UIReader(Color borderColour)
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
    private Bitmap getGameScreenshot()
    {
        return getScreenshot(this.sourceTopLeft, this.gameSize);

    }

    /// <summary>
    /// Gets a screenshot of a portion of the screen based on arguments provided
    /// </summary>
    /// <param name="sourceTopLeft"></param>
    /// <param name="size"></param>
    /// <returns>Screenshot</returns>
    private Bitmap getScreenshot(Point sourceTopLeft, Size size)
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
    private Bitmap getFullScreenshot()
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
    private void getGameDimensions(Bitmap bmp, Color borderColour)
    {
        int borderLineWidth = 15;
        // Finding the bottom right most instance of the borderColour
        this.sourceBottomRight = this.findBottomRight(bmp, borderColour);
        // Finding the top left corner of a rectange of the borderColour
        this.sourceTopLeft = this.findOppositeCorner(bmp, this.sourceBottomRight, borderLineWidth);

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
    private void getGameDimensions(Color borderColour)
    {
        // Defining local variables based on class attributes
        Bitmap bmp = getFullScreenshot();
        getGameDimensions(bmp, borderColour);
    }

    /// <summary>
    /// Finds the dimensions of the game board based on the border colour defined as an attribute of this instance of UI_READER
    /// Automatically takes a screenshot of the entire screen to do this
    /// </summary>
    /// <param name="borderColour"></param>
    private void getGameDimensions()
    {
        // Defining local variables based on class attributes
        getGameDimensions(this.borderColour);
    }


    private Point findBottomRight(Bitmap bmp, Color targetColour)
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

    private Point findOppositeCorner(Bitmap bmp, Point startCorner, int lineWidth)
    {
        
        int boardWidth = Math.Max(findDistToNextCorner(bmp, startCorner, E_DIRECTION.Left, lineWidth), findDistToNextCorner(bmp, startCorner, E_DIRECTION.Right, lineWidth));
        int boardHeight = boardWidth * 2;
        Point oppositeCorner = new Point(startCorner.X-boardWidth, startCorner.Y-boardHeight);

        Console.WriteLine(String.Format("Top Left Point Found: X: {0}, Y: {1}", oppositeCorner.X, oppositeCorner.Y));

        return oppositeCorner; // Returns the opposite corner found

    }

    private int findDistToNextCorner(Bitmap bmp, Point startPoint, E_DIRECTION direction, int lineWidth)
    {
        // Defining local variables
        Point currentPoint = startPoint;
        bool endOfLine = false;
        bool corner = false;
        Color currentColour;
        Color[] beyondLineColour = new Color[2];
        Color targetColour = bmp.GetPixel(currentPoint.X, currentPoint.Y);
        int colourTolerance = 0;
        int edgeOfLineTolerance = 5;

        while (!(endOfLine || corner))
        {
            // Moves the current point in the specified direction
            switch (direction)
            {
                case E_DIRECTION.Left:
                    currentPoint.X -= 1;
                    beyondLineColour[0] = bmp.GetPixel(currentPoint.X, currentPoint.Y+lineWidth+edgeOfLineTolerance);
                    beyondLineColour[1] = bmp.GetPixel(currentPoint.X, currentPoint.Y+lineWidth-edgeOfLineTolerance);
                    break;
                case E_DIRECTION.Up:
                    currentPoint.Y -= 1;
                    beyondLineColour[0] = bmp.GetPixel(currentPoint.X+lineWidth+edgeOfLineTolerance, currentPoint.Y);
                    beyondLineColour[1] = bmp.GetPixel(currentPoint.X+lineWidth-edgeOfLineTolerance, currentPoint.Y);
                    break;
                case E_DIRECTION.Right:
                    currentPoint.X += 1;
                    beyondLineColour[0] = bmp.GetPixel(currentPoint.X, currentPoint.Y+lineWidth+edgeOfLineTolerance);
                    beyondLineColour[1] = bmp.GetPixel(currentPoint.X, currentPoint.Y+lineWidth-edgeOfLineTolerance);
                    break;
                case E_DIRECTION.Down:
                    currentPoint.Y += 1;
                    beyondLineColour[0] = bmp.GetPixel(currentPoint.X+lineWidth+edgeOfLineTolerance, currentPoint.Y);
                    beyondLineColour[1] = bmp.GetPixel(currentPoint.X+lineWidth-edgeOfLineTolerance, currentPoint.Y);
                    break;
            }
            currentColour = bmp.GetPixel(currentPoint.X, currentPoint.Y);
            if (!similarColours(currentColour, targetColour, colourTolerance)) // If the colour encountered is different, end the while loop as it must be the end of the line
            { 
                endOfLine = true;
            }
            else if (similarColours(currentColour, beyondLineColour[0], colourTolerance) || similarColours(currentColour, beyondLineColour[1], colourTolerance)) // If the colour outside the line is the same, end the while loop as it must be a corner
            {
                corner = true;
            }
        }

        // Return the length difference in X or Y depending on the line direction
        if (direction == E_DIRECTION.Left || direction == E_DIRECTION.Right)
        {
            return Math.Abs(currentPoint.X - startPoint.X);
        }
        if (direction == E_DIRECTION.Up || direction == E_DIRECTION.Down)
        {
            return Math.Abs(currentPoint.Y - startPoint.Y);
        }
        else
        {
            throw new Exception("Invalid Direction");      
        }  
    }

    private int findLineLength(Bitmap bmp, Point startPoint, E_DIRECTION direction)
    {
        // Defining local variables
        Point currentPoint = startPoint;
        bool endOfLine = false;
        Color currentColour;
        Color targetColour = bmp.GetPixel(currentPoint.X, currentPoint.Y);
        int colourTolerance = 0;

        while (!endOfLine)
        {
            // Moves the current point in the specified direction
            switch (direction)
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

        // Return the length difference in X or Y depending on the line direction
        if (direction == E_DIRECTION.Left || direction == E_DIRECTION.Right)
        {
            return Math.Abs(currentPoint.X - startPoint.X);
        }
        if (direction == E_DIRECTION.Up || direction == E_DIRECTION.Down)
        {
            return Math.Abs(currentPoint.Y - startPoint.Y);
        }
        else
        {
            throw new Exception("Invalid Direction");
        }
    }

    private bool similarColours(Color colour1, Color colour2, int tolerance)
    {
        if (Math.Abs(Math.Abs(colour1.R) - Math.Abs(colour2.R)) > tolerance)
        {
            return false;
        }
        if (Math.Abs(Math.Abs(colour1.G) - Math.Abs(colour2.G)) > tolerance)
        {
            return false;
        }
        if (Math.Abs(Math.Abs(colour1.B) - Math.Abs(colour2.B)) > tolerance)
        {
            return false;
        }

        return true;
    }

    private Color getAvgColor(Bitmap bmp, Point topLeft, Point bottomRight)
    {
        List<int> aVals = new List<int>();
        List<int> rVals = new List<int>();
        List<int> gVals = new List<int>();
        List<int> bVals = new List<int>();

        // Create a list of all colours in the region given
        for (int y = topLeft.Y; y <= bottomRight.Y; ++y)
        {
            for (int x = topLeft.X; x <= bottomRight.X; ++x)
            {
                Color clr = bmp.GetPixel(x, y);
                aVals.Add(clr.A);
                rVals.Add(clr.R);
                gVals.Add(clr.G);
                bVals.Add(clr.B);


            }

        }

        // Calculat the avg colour
        Color avgClr = Color.FromArgb(
            aVals.Sum()/aVals.Count,
            rVals.Sum()/rVals.Count,
            gVals.Sum()/gVals.Count,
            bVals.Sum()/bVals.Count
        );


        /* DEBUG SETP -> save each region as an image*/
        Rectangle cellRegion = new Rectangle(topLeft.X, topLeft.Y, bottomRight.X-topLeft.X, bottomRight.Y-topLeft.Y);
        Bitmap cellImg = bmp.Clone(cellRegion, bmp.PixelFormat);
        String fileName = String.Format("cellImages/topLeft{0}_bottomRight-{1}.png", topLeft, bottomRight);
        cellImg.Save(fileName);
        /* END OF DEBUG STEP */

        return avgClr;
    } 

    /// <summary>
    /// Reutns a 2D boolean array showing which cells are filled based on gameScreenshot and the attributes defined in the class
    /// Assumes a 10X20 grid (standard tetris)
    /// </summary>
    /// <param name="gameScreenshot"></param>
    /// <returns>2D boolean array</returns>
    public bool[,] getGameGrid(Bitmap gameScreenshot)
    {
        // Define the grid size and create 2D array
        int gridWidth = 10;
        int gridHeight = 20;
        bool[,] gameGrid = new bool[gridHeight, gridWidth];

        // calculate the size of each cell in the grid
        int cellWidth = gameScreenshot.Width / gridWidth;
        int cellHeight = gameScreenshot.Height / gridHeight;

        // Define other variables to be used
        Point cellTopLeft = new Point(0, 0);
        Point cellBottomRight = new Point(0, 0);
        Color cellColour;
        int colourSimilarityTolerance = 50;


        // Iterate through each cell
        for (int row = 0; row < gridHeight; ++row)
        {
            for (int col = 0; col < gridWidth; ++col)
            {
                // Find the average colour of each cell
                int marginX = (int)(cellWidth * 0.125);  // 12.5% margin on each side
                int marginY = (int)(cellHeight * 0.125);

                cellTopLeft.X = (col * cellWidth) + marginX;
                cellTopLeft.Y = (row * cellHeight) + marginY;

                cellBottomRight.X = ((col+1) * cellWidth) - marginX;
                cellBottomRight.Y = ((row+1) * cellHeight) - marginY;

                cellColour = getAvgColor(gameScreenshot, cellTopLeft, cellBottomRight);                

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
    /// Assumes a 10X20 grid (standard tetris)
    /// </summary>
    /// <returns>2D boolean array</returns>
    public bool[,] getGameGrid()
    {
        Bitmap gameScreenshot = getGameScreenshot();
        gameScreenshot.Save("gameScreenshot.png");
        return getGameGrid(gameScreenshot);
    }



 







    // For debugging only
    public void debug()
    {
        Console.WriteLine("=========\n=========\nStart of Program\n=========\n=========");

        UIReader ui = new UIReader();
        Bitmap fullScreenshot = ui.getFullScreenshot();
        Console.WriteLine("Saving full screenshot");
        fullScreenshot.Save("fullScreenshot.png");
        ui.getGameDimensions(fullScreenshot, ui.borderColour);

        // Check a game has been detected and save an image of it
        if (ui.gameSize.Width <= 0 || ui.gameSize.Width <= 0)
        {
            throw new EntryPointNotFoundException("No game detected on screen");
        }
        else
        {
            ui.getGameScreenshot().Save("gameScreenshot.png");
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
