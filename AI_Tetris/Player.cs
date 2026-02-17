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
        Queue<VirtualKeyCode> chosenMove = chooseMove();
        if (chosenMove != null)
        {
            makeMove(chosenMove);            
        }
    }

    private List<Queue<VirtualKeyCode>> getMoveOptions()
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
        List<Queue<VirtualKeyCode>> moveOptions = new List<Queue<VirtualKeyCode>>();
        PieceInstance? fallingPiece = boardHandler.findFallingPiece();

        if (fallingPiece != null)
        {

            for (int rotationIndex = 0; rotationIndex < nbPossibleRotations; ++rotationIndex) // iterate through each rotation
            {
                
                int distL = boardHandler.findFirstFallingCell().Item2-1;  // Distance from left wall
                int distR = boardWidth - distL - fallingPiece.pieceArray.GetLength(0) - 1;  // Distance from right wall
                
                for (int leftMoves = 1; leftMoves <= distL; ++leftMoves) // Iterate through each possible number of left moves before hitting the edge
                {
                    // Add the move sequence for each possibility to the list
                    Queue<VirtualKeyCode> movesQueue = new Queue<VirtualKeyCode>(Enumerable.Repeat(VirtualKeyCode.UP, rotationIndex));
                    for (int index = 0; index < leftMoves;  ++index) { movesQueue.Enqueue(VirtualKeyCode.LEFT); }
                    moveOptions.Add(movesQueue);
                }
                
                for (int rightMoves = 1; rightMoves <= distR; ++rightMoves) // Iterate through each possible number of left moves before hitting the edge
                {
                    // Add the move sequence for each possibility to the list
                    Queue<VirtualKeyCode> movesQueue = new Queue<VirtualKeyCode>(Enumerable.Repeat(VirtualKeyCode.UP, rotationIndex));
                    for (int index = 0; index < rightMoves ;  ++index) { movesQueue.Enqueue(VirtualKeyCode.RIGHT); }
                    moveOptions.Add(movesQueue);
                } 
 

                // Rotate the piece for the next iteration
                fallingPiece.rotate();
            }

        }

        return moveOptions;

    }


    private Queue<VirtualKeyCode>? chooseMove()
    {
        Random rnd = new Random();
        List<Queue<VirtualKeyCode>> moveOptions = getMoveOptions(); // Get all move options
        
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

}