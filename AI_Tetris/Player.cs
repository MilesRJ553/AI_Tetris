using WindowsInput;
using WindowsInput.Native;

class Player
{

    private InputSimulator inputSim = new InputSimulator();
    private BoardHandler boardHandler;
    
    /* =============== Constructors =============== */
    /// <summary>
    /// Constructor for Player
    /// </summary>
    public Player(BoardHandler boardHandler)
    {
        this.boardHandler = boardHandler;
    }


    /* =============== Methods =============== */

    public void chooseAndMakeMove()
    {
        MoveOption? chosenMove = chooseMove();
        if (chosenMove != null)
        {
            makeMove(chosenMove);            
        }
    }

    private List<MoveOption> getMoveOptions()
    {
        // Getting the current game board
        E_CELL_STATUS[,] gameBoard = boardHandler.getGameBoard();
        int boardWidth = 10;
        int boardHeight = 20;
        if (gameBoard.GetLength(0) != boardHeight || gameBoard.GetLength(1) != boardWidth)
        {
            throw new Exception("Game board must be 20x10");
        }

        // Defining localvariables
        int nbPossibleRotations = Enum.GetValues(typeof(E_ROTATION)).GetLength(0);
        List<MoveOption> moveOptions = new List<MoveOption>();
        PieceInstance? fallingPiece = boardHandler.findFallingPiece();

        if (fallingPiece != null)
        {

            for (int rotationIndex = 0; rotationIndex < nbPossibleRotations; ++rotationIndex) // iterate through each rotation
            {
                
                int distL = boardHandler.findFirstFallingCell().Item2;  // Distance from left wall
                int distR = boardWidth - distL - fallingPiece.pieceArray.GetLength(1);  // Distance from right wall
                
                for (int leftMoves = 1; leftMoves <= distL; ++leftMoves) // Iterate through each possible number of left moves before hitting the edge
                {
                    // Add the move sequence for each possibility to the list
                    Queue<VirtualKeyCode> movesQueue = new Queue<VirtualKeyCode>(Enumerable.Repeat(VirtualKeyCode.UP, rotationIndex));
                    for (int index = 0; index < leftMoves;  ++index) { movesQueue.Enqueue(VirtualKeyCode.LEFT); }
                    MoveOption moveOption = new MoveOption(movesQueue, getGameBoardAfterMove(new Queue<VirtualKeyCode>(movesQueue)));
                    moveOptions.Add(moveOption);
                }
                
                for (int rightMoves = 1; rightMoves <= distR; ++rightMoves) // Iterate through each possible number of left moves before hitting the edge
                {
                    // Add the move sequence for each possibility to the list
                    Queue<VirtualKeyCode> movesQueue = new Queue<VirtualKeyCode>(Enumerable.Repeat(VirtualKeyCode.UP, rotationIndex));
                    for (int index = 0; index < rightMoves ;  ++index) { movesQueue.Enqueue(VirtualKeyCode.RIGHT); }
                    MoveOption moveOption = new MoveOption(movesQueue, getGameBoardAfterMove(new Queue<VirtualKeyCode>(movesQueue)));
                    moveOptions.Add(moveOption);
                } 
 

                // Rotate the piece for the next iteration
                fallingPiece.rotate();
            }

        }

        return moveOptions;

    }


    private MoveOption? chooseMove()
    {
        Random rnd = new Random();
        List<MoveOption> moveOptions = getMoveOptions(); // Get all move options
        
        // If there are any possible moves, choose one randomly
        int nbMoveOptions = moveOptions.Count();
        if (nbMoveOptions > 0)
        {
            int choice = rnd.Next(0, nbMoveOptions);
            return moveOptions[choice];
        }

        return null;
    }

    private void makeMove(Queue<VirtualKeyCode> movesQueue)
    {
        while (movesQueue.Count() > 0)
        {
            VirtualKeyCode nextKey = movesQueue.Dequeue();
            inputSim.Keyboard.KeyPress(nextKey);
            Console.Write(nextKey.ToString() + " ");
            Thread.Sleep(20);
        }
        Console.WriteLine(VirtualKeyCode.SPACE.ToString());
        inputSim.Keyboard.KeyPress(VirtualKeyCode.SPACE);
        boardHandler.setFallingSettled();
    }

    private void makeMove(MoveOption moveOption)
    {
        makeMove(moveOption.getInputSequence());
    }

    /// <summary>
    /// Returns a E_CELL_STATUS[,] of what the game board would be if a given move is made
    /// </summary>
    /// <returns></returns>
    private E_CELL_STATUS[,] getGameBoardAfterMove(Queue<VirtualKeyCode> inputSequence)
    {
        
        // finding the current state
        E_CELL_STATUS[,] newGameBoard = boardHandler.getGameBoard();
        PieceInstance? fallingPiece = boardHandler.findFallingPiece();

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
                }
            }
            newGameBoard = visualiseDropPiece(newGameBoard);
        }

        return newGameBoard; // returns the game board after all moves have been carried out

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
                    int row_num = firstFallingCell.Item1 + row;
                    int col_num = firstFallingCell.Item2 + col;
                    if (gameBoard[row_num, col_num] == E_CELL_STATUS.EMPTY)
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

        return newGameBoard; 
    }

    /// <summary>
    /// Returns the game board that would occur if the piece drops all the way
    /// </summary>
    /// <param name="gameBoard"></param>
    /// <returns></returns>
    private E_CELL_STATUS[,] visualiseDropPiece(E_CELL_STATUS[,] gameBoard)
    {
        E_CELL_STATUS[,] newGameBoard = gameBoard;
        E_CELL_STATUS[,] newGameBoardTmp = gameBoard;

        while (true)
        {
            // move each falling cell down
            for (int row = gameBoard.GetLength(0)-1; row >= 0; --row)
            {
                for (int col = 0; col < gameBoard.GetLength(1); ++col)
                {
                    if (newGameBoard[row, col] == E_CELL_STATUS.FALLING)
                    {
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
            newGameBoard = newGameBoardTmp; // update the new game board to match the tmp board once all cells have been checked
        }
    }
}