using System.Runtime.InteropServices;
using WindowsInput;
using WindowsInput.Native;
using System.Windows.Forms;
using System.Drawing;
using System.Linq.Expressions;

class Player
{

    private InputSimulator inputSim = new InputSimulator();
    private BoardHandler boardHandler;
    private MoveRater moveRater = new MoveRater(0.2, 0.2, 0.2, 0.2, 0.2);
    private bool canHold = true;
    PieceInstance? heldPiece = null;
    private int nbFailedCycles = 0;
    DateTime lastMoveTime = DateTime.UtcNow;
    DateTime startTime = DateTime.UtcNow;
    
    /* =============== Constructors =============== */
    /// <summary>
    /// Constructor for Player with the default moveRater
    /// </summary>
    public Player(BoardHandler boardHandler)
    {
        this.boardHandler = boardHandler;
        lastMoveTime = DateTime.UtcNow;
        startTime = DateTime.UtcNow;
    }

    /// <summary>
    /// Constructor for the Player which takes a MoveRater as an argument
    /// </summary>
    /// <param name="uiReader"></param>
    public Player(BoardHandler boardHandler, MoveRater moveRater)
    {
        this.boardHandler = boardHandler;
        this.moveRater = moveRater;
        lastMoveTime = DateTime.UtcNow;
        startTime = DateTime.UtcNow;
    }


    /* =============== Methods =============== */

    public void chooseAndMakeMove(UIReader uiReader, bool verbose)
    {
        MoveOption? chosenMove = chooseMove();
        if (chosenMove != null)
        {
            makeMove(chosenMove, uiReader, verbose); 
            lastMoveTime = DateTime.UtcNow;   
        }
    }

    private List<MoveOption> getMoveOptions(E_CELL_STATUS[,] gameBoard, bool isHeldPiece=false)
    {
        // Getting the current game board
        int boardWidth = 10;
        int boardHeight = 20;
        if (gameBoard.GetLength(0) != boardHeight || gameBoard.GetLength(1) != boardWidth)
        {
            throw new Exception("Game board must be 20x10");
        }

        // Defining localvariables
        int nbPossibleRotations = Enum.GetValues(typeof(E_ROTATION)).GetLength(0);
        List<MoveOption> moveOptions = new List<MoveOption>();
        PieceInstance? fallingPiece = boardHandler.findFallingPiece(gameBoard);

        if (fallingPiece != null)
        {

            for (int rotationIndex = 0; rotationIndex < nbPossibleRotations; ++rotationIndex) // iterate through each rotation
            {
                
                int distL = boardHandler.findLeftMostFallingCell(gameBoard).Item2;  // Distance from left wall
                int distR = boardWidth - (distL + fallingPiece.pieceArray.GetLength(1));  // Distance from right wall

                {   // Add the option for no lateral movement
                    Queue<VirtualKeyCode> movesQueue = new Queue<VirtualKeyCode>(Enumerable.Repeat(VirtualKeyCode.UP, rotationIndex));
                    try { 
                        MoveOption moveOption = new MoveOption(movesQueue, getGameBoardAfterMove(new Queue<VirtualKeyCode>(movesQueue), gameBoard), isHeldPiece);
                        moveOptions.Add(moveOption);                    
                    }
                    catch
                    {
                        continue;
                    }
                }
                
                for (int leftMoves = 1; leftMoves <= distL; ++leftMoves) // Iterate through each possible number of left moves before hitting the edge
                {
                    // Add the move sequence for each possibility to the list
                    Queue<VirtualKeyCode> movesQueue = new Queue<VirtualKeyCode>(Enumerable.Repeat(VirtualKeyCode.UP, rotationIndex));
                    for (int index = 0; index < leftMoves;  ++index) { movesQueue.Enqueue(VirtualKeyCode.LEFT); }
                    try {
                        MoveOption moveOption = new MoveOption(movesQueue, getGameBoardAfterMove(new Queue<VirtualKeyCode>(movesQueue), gameBoard), isHeldPiece);
                        moveOptions.Add(moveOption);
                    }
                    catch
                    {
                        continue;
                    }
                }
                
                for (int rightMoves = 1; rightMoves <= distR; ++rightMoves) // Iterate through each possible number of left moves before hitting the edge
                {
                    // Add the move sequence for each possibility to the list
                    Queue<VirtualKeyCode> movesQueue = new Queue<VirtualKeyCode>(Enumerable.Repeat(VirtualKeyCode.UP, rotationIndex));
                    for (int index = 0; index < rightMoves ;  ++index) { movesQueue.Enqueue(VirtualKeyCode.RIGHT); }
                    try {
                        MoveOption moveOption = new MoveOption(movesQueue, getGameBoardAfterMove(new Queue<VirtualKeyCode>(movesQueue), gameBoard), isHeldPiece);
                        moveOptions.Add(moveOption);
                    }
                    catch
                    {
                        continue;
                    }
                } 
 

                // Rotate the piece for the next iteration
                fallingPiece.rotate();
            }

        }
        else if(fallingPiece == null && isHeldPiece == true) // Add an option to hold if there is no held piece
        {
            Queue<VirtualKeyCode> movesQueue = new Queue<VirtualKeyCode>();
            MoveOption moveOption = new MoveOption(movesQueue, gameBoard, isHeldPiece);
            moveOptions.Add(moveOption);
        }

        return moveOptions;

    }

    /// <summary>
    /// If there are any possible moves, returns the one with the highest rating
    /// Otherwise returns null
    /// </summary>
    /// <returns>MoveOption?</returns>
    private MoveOption? chooseMove()
    {
        Random rnd = new Random();
        MoveOption? chosenMove = null;

        // Get all move options with the current piece
        E_CELL_STATUS[,] gameBoard = boardHandler.getGameBoard();
        List<MoveOption> moveOptions = getMoveOptions(gameBoard);
        
        // Get all move options with the held piece
        if (canHold && moveOptions.Count > 0)
        {
            gameBoard = getGameBoardWithHeldPiece(gameBoard);
            moveOptions.AddRange(getMoveOptions(gameBoard, true));            
        }
         
        if (moveOptions.Count()  > 0)
        {
            // Create a list of move options with the joint highest rating
            List<MoveOption> highestRatedOption = new List<MoveOption>();
            double highestRating = 0.0;
            foreach (MoveOption option in moveOptions)
            {
                double rating = moveRater.rateMove(option);
                if (highestRatedOption.Count() == 0)
                {
                    highestRatedOption.Add(moveOptions[0]);
                    highestRating = rating;
                }
                else if (rating >= highestRating)
                {
                    if (rating > highestRating)
                    {
                        highestRatedOption = new List<MoveOption>(); // Clear the list if a higher rating is found   
                        highestRating = rating;      
                    }
                    highestRatedOption.Add(option); // Add to the list if its rating is the same or higher
                }
            }

            int rndIndex = rnd.Next(highestRatedOption.Count());
            chosenMove = highestRatedOption[rndIndex];
            if (chosenMove.isHold())
            {
                this.heldPiece = boardHandler.findFallingPiece();
            }
            this.canHold = !chosenMove.isHold();
        }

        return chosenMove;
    }

    private void makeMove(MoveOption moveOption, UIReader uiReader, bool verbose)
    {
        bool moveOk = true;
        Queue<VirtualKeyCode> movesQueue = moveOption.getInputSequence();

        while (movesQueue.Count() > 0)
        {
            VirtualKeyCode nextKey = movesQueue.Dequeue();
            inputSim.Keyboard.KeyPress(nextKey);
            Thread.Sleep(100);
        }

        updateBoardHandler(uiReader);

        if (canHold) // If can hold == false, then we have just held a piece so we don't need to check it has worked
        {
            moveOk = checkFallingPiecePos(moveOption.getResultingGameBoard());
            if (!moveOk)
            {
                correctLaterally(moveOption.getResultingGameBoard(), verbose); 
            }
            inputSim.Keyboard.KeyPress(VirtualKeyCode.SPACE);
            nbFailedCycles = 0;
        }
    }

    private bool checkFallingPiecePos(E_CELL_STATUS[,] expectedGameBoard)
    {

        // Check the LHS
        int leftEdgeCol = boardHandler.findLeftMostFallingCell(boardHandler.getGameBoard()).Item2;
        int expectedLeftEdgeCol = boardHandler.findLeftMostFallingCell(expectedGameBoard).Item2;

        // Check the RHS
        int rightEdgeCol = boardHandler.findRightMostFallingCell(boardHandler.getGameBoard()).Item2;
        int expectedRightEdgeCol = boardHandler.findRightMostFallingCell(expectedGameBoard).Item2;
        
        // Return a boolean indicating whether the piece is in the correct position
        return (leftEdgeCol == expectedLeftEdgeCol) && (rightEdgeCol == expectedRightEdgeCol);

    }

    private void correctLaterally(E_CELL_STATUS[,] expectedGameBoard, bool verbose, int delayBetweenMoves = 50)
    {
        // Calculate offset
        int leftEdgeCol = boardHandler.findLeftMostFallingCell(boardHandler.getGameBoard()).Item2;
        int expectedLeftEdgeCol = boardHandler.findLeftMostFallingCell(expectedGameBoard).Item2;
        int offset = leftEdgeCol-expectedLeftEdgeCol;
        
        // Create Moves Queue
        VirtualKeyCode direction = offset < 0 ? VirtualKeyCode.RIGHT : VirtualKeyCode.LEFT;
        Queue<VirtualKeyCode> movesQueue =  new Queue<VirtualKeyCode>(Enumerable.Repeat(direction, Math.Abs(offset)));

        // Apply moves
        while (movesQueue.Count() > 0)
        {
            VirtualKeyCode nextKey = movesQueue.Dequeue();
            if (verbose)
            {
                Console.WriteLine("Correcting... " + nextKey.ToString());
            }
            inputSim.Keyboard.KeyPress(nextKey);
            Thread.Sleep(delayBetweenMoves);
        }

    }

    /// <summary>
    /// Returns a E_CELL_STATUS[,] of what the game board would be if a given move is made
    /// </summary>
    /// <returns></returns>
    private E_CELL_STATUS[,] getGameBoardAfterMove(Queue<VirtualKeyCode> inputSequence, E_CELL_STATUS[,] gameBoard)
    {
        
        // finding the current state
        E_CELL_STATUS[,] newGameBoard = (E_CELL_STATUS[,]) gameBoard.Clone();
        PieceInstance? fallingPiece = boardHandler.findFallingPiece(gameBoard);

        if (fallingPiece != null) // only carries out operations if fallingPiece isn't null
        {
            while (inputSequence.Count > 0)
            {
                VirtualKeyCode nextKey = inputSequence.Dequeue();
                switch (nextKey)
                {
                    case VirtualKeyCode.LEFT:
                        newGameBoard = visualiseMoveFallingPieceLeft(newGameBoard);
                        break;
                    case VirtualKeyCode.RIGHT:
                        newGameBoard = visualiseMoveFallingPieceRight(newGameBoard);
                        break;
                    case VirtualKeyCode.UP:
                        newGameBoard = visualiseRotateFallingPiece(newGameBoard);
                        break;
                    case VirtualKeyCode.VK_C:
                        newGameBoard = getGameBoardNoneFalling(boardHandler.getGameBoard());
                        break;
                }
            }
            newGameBoard = visualiseDropPiece(newGameBoard);
        }

        return newGameBoard; // returns the game board after all moves have been carried out

    }

    private E_CELL_STATUS[,] getGameBoardWithHeldPiece(E_CELL_STATUS[,] gameBoard)
    {

        // Remove all falling pieces
        gameBoard = getGameBoardNoneFalling(gameBoard);


        if (heldPiece != null)
        {
            // Introduce the held piece
            for (int row = 0; row < this.heldPiece.pieceArray.GetLength(0); row++)
            {
                for (int col = 0; col < this.heldPiece.pieceArray.GetLength(1); col++)
                {
                    gameBoard[row, col] = this.heldPiece.pieceArray[row,col];
                }
            }
        }

        return gameBoard;
    }

    private E_CELL_STATUS[,] getGameBoardNoneFalling(E_CELL_STATUS[,] gameBoard)
    {
        // Declare local variables
        E_CELL_STATUS cellStatus;
        E_CELL_STATUS[,] newGameBoard = (E_CELL_STATUS[,])gameBoard.Clone();

        // Iterate through each cell of the gameBoard
        for (int row = 0; row < newGameBoard.GetLength(0); ++row)
        {
            for (int col = 0; col <newGameBoard.GetLength(1); ++col)
            {
                cellStatus = newGameBoard[row, col];

                // Updates the cell if its status is falling
                if (cellStatus == E_CELL_STATUS.FALLING)
                {
                    newGameBoard[row, col] = E_CELL_STATUS.EMPTY;
                }
            }
        }
        return newGameBoard;
    }

    /// <summary>
    /// Returns a transformed version of gameBoard with any falling cells moved left (if direction = -1 or right (if direction = 1))
    /// </summary>
    /// <param name="gameBoard"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private E_CELL_STATUS[,] visualiseMoveFallingPiece(E_CELL_STATUS[,] gameBoard, int direction)
    {
        if (Math.Abs(direction) != 1) // check that direction is 1 or -1
        {
            throw new Exception("direction must be 1 or -1");
        }
        E_CELL_STATUS[,] newGameBoard = (E_CELL_STATUS[,])gameBoard.Clone();
        int height = gameBoard.GetLength(0);
        int width = gameBoard.GetLength(1);

        for (int row = 0; row < height; ++row)
        {
            for (int col = 0; col < width; ++col)
            {
                if (gameBoard[row, col] == E_CELL_STATUS.FALLING) // Identify a falling cell
                {
                    if (col+direction >= 0 && col+direction < width) 
                    {
                        if (col-direction >= 0 && col-direction < width)
                        {
                            newGameBoard[row, col] = gameBoard[row, col-direction] == E_CELL_STATUS.FALLING ? E_CELL_STATUS.FALLING : E_CELL_STATUS.EMPTY;
                        }
                        else
                        {
                            newGameBoard[row, col] = E_CELL_STATUS.EMPTY;
                        }

                        newGameBoard[row, col+direction] = E_CELL_STATUS.FALLING; // Move each falling cell in the specified direction
                    }
                    else
                    {
                        return gameBoard;
                    }
                }
            }
        }
        return newGameBoard;
    }
    
    /// <summary>
    /// Returns a transformed version of gameBoard with any falling cells moved left
    /// </summary>
    /// <param name="gameBoard"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private E_CELL_STATUS[,] visualiseMoveFallingPieceLeft(E_CELL_STATUS[,] gameBoard)
    {
        return visualiseMoveFallingPiece(gameBoard, -1);
    }
    
    /// <summary>
    /// Returns a transformed version of gameBoard with any falling cells moved right
    /// </summary>
    /// <param name="gameBoard"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>

    private E_CELL_STATUS[,] visualiseMoveFallingPieceRight(E_CELL_STATUS[,] gameBoard)
    {
        return visualiseMoveFallingPiece(gameBoard, 1);
    }

    /// <summary>
    /// Returns a transformed version of gameBoard after rotating any falling pieces
    /// </summary>
    /// <param name="gameBoard"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private E_CELL_STATUS[,] visualiseRotateFallingPiece(E_CELL_STATUS[,] gameBoard)
    {
        E_CELL_STATUS[,] newGameBoard = (E_CELL_STATUS[,])gameBoard.Clone();

        // Find the piece to be rotated
        PieceInstance? piece = boardHandler.findFallingPiece(gameBoard);

        if (piece != null) // rotate piece if one is found
        {
            (int, int) firstFallingCell = boardHandler.findFirstFallingCell(gameBoard);

            // remove all falling cells temporarily
            for (int row = 0; row < gameBoard.GetLength(0); ++row)
            {
                for (int col = 0; col < gameBoard.GetLength(1); ++col)
                {
                    if (gameBoard[row, col] == E_CELL_STATUS.FALLING)
                    {
                        newGameBoard[row, col] = E_CELL_STATUS.EMPTY;
                    }
                }
            }

            // Rotate the piece into its new orientation
            piece.rotate();

            // Add the new piece back in its correct orientation
            for (int row = 0; row < piece.pieceArray.GetLength(0); ++row)
            {
                for (int col = 0; col < piece.pieceArray.GetLength(1); ++col)
                {
                    if (piece.pieceArray[row, col] == E_CELL_STATUS.FALLING) { // places all falling cells from the piece array
                        int row_num = firstFallingCell.Item1 + row;
                        int col_num = firstFallingCell.Item2 + col;
                        if (newGameBoard[row_num, col_num] == E_CELL_STATUS.EMPTY) // checks there's enough space
                        {
                            newGameBoard[row_num, col_num] = E_CELL_STATUS.FALLING;
                        }
                        else
                        {
                            throw new Exception("Unable to rotate piece: not enough space");
                        }
                    }
                }
            }
        }

        return newGameBoard; 
    }

    /// <summary>
    /// Returns the game board that would occur if the piece drops all the way
    /// </summary>
    /// <param name="gameBoard"></param>
    /// <returns></returns>
    public static E_CELL_STATUS[,] visualiseDropPiece(E_CELL_STATUS[,] gameBoard)
    {
        E_CELL_STATUS[,] newGameBoard = (E_CELL_STATUS[,])gameBoard.Clone();
        E_CELL_STATUS[,] newGameBoardTmp = (E_CELL_STATUS[,])newGameBoard.Clone();
        int nbFallingCells;

        while (true)
        {
            nbFallingCells = 0;
            // move each falling cell down
            for (int row = gameBoard.GetLength(0)-1; row >= 0; --row)
            {
                for (int col = 0; col < gameBoard.GetLength(1); ++col)
                {
                    if (newGameBoard[row, col] == E_CELL_STATUS.FALLING)
                    {
                        nbFallingCells++;
                        if (row == gameBoard.GetLength(0)-1) // return the game board if the falling piece is in the bottom row
                        {
                            return newGameBoard;
                        }
                        else if (newGameBoard[row+1, col] == E_CELL_STATUS.SETTLED) // return the game board if the cell below the falling cell is settled
                        {
                            return newGameBoard;
                        } 
                        else // otherwise, move the falling cell down in tmp
                        {
                            newGameBoardTmp[row, col] = E_CELL_STATUS.EMPTY;
                            newGameBoardTmp[row+1, col] = E_CELL_STATUS.FALLING;
                        }
                    }
                }
            }
            if (nbFallingCells == 0)
            {
                return newGameBoard;
            }
            newGameBoard = (E_CELL_STATUS[,])newGameBoardTmp.Clone(); // update the new game board to match the tmp board once all cells have been checked
        }
    }

    /// <summary>
    /// Updates the Player's internal boardHandler
    /// </summary>
    /// <param name="uiReader"></param>
    private void updateBoardHandler(UIReader uiReader)
    {
        bool[,] uiGameBoard = uiReader.getGameGrid();
        boardHandler.boardHandlingMain(uiGameBoard, false);
        
    }

    /// <summary>
    /// Return true if the game has ended, otherwise, returns false.
    /// </summary>
    /// <returns></returns>
    public bool checkGameOver()
    {
        bool gameOver = false;
        int maxTimeBtwMoves = 10;
        TimeSpan timeSinceLastMove = DateTime.UtcNow - lastMoveTime;
        
        if (timeSinceLastMove.TotalSeconds > maxTimeBtwMoves)
        {
            gameOver = true;
        }
        return gameOver;
    }

    public TimeSpan getTotalGameTime()
    {
        return DateTime.UtcNow - startTime;
    }

    /// <summary>
    /// Clicks position indicated where relative X and Y are between 1 and 0 and represent how far 
    /// up/across the board the pixel is starting from the top left
    /// </summary>
    /// <param name="relativeX"></param>
    /// <param name="relativeY"></param>
    /// <returns></returns>
    private void clickPos(UIReader uiReader, double relativeX, double relativeY)
    {

            Point pointToClick = uiReader.findAbsCoords(relativeX, relativeY);
            
            var bounds = Screen.PrimaryScreen.Bounds;

            double absoluteX = (pointToClick.X - bounds.Left) * 65535.0 / bounds.Width;
            double absoluteY = (pointToClick.Y - bounds.Top) * 65535.0 / bounds.Height;

            inputSim.Mouse.MoveMouseTo(absoluteX, absoluteY);
            this.inputSim.Mouse.LeftButtonClick();  
        
    }

    public void startNewGame(UIReader uiReader)
    {
        // Define local variables
        double relativeX;
        double relativeY;

        // Press OK
        relativeY = 0.54;
        relativeX = 0.5;

        this.clickPos(uiReader, relativeX, relativeY); 


        // Press replay button
        relativeX = 0.44;
        relativeY = 0.70;

        this.clickPos(uiReader, relativeX, relativeY);
    }
}