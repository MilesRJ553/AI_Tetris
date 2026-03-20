class MoveRater : IScorer
{

    private double nbRowsClearedScoreWeight;
    private double avgHeightScoreWeight;
    private double nbGapsScoreWeight;
    private double elevationChangeScoreWeight;
    
    public MoveRater(double nbRowsClearedScoreWeight, double avgHeightScoreWeight, double nbGapsScoreWeight, double elevationChangeScoreWeight)
    {
        double tolerance = 0.00005;
        double weightsSum = nbRowsClearedScoreWeight + avgHeightScoreWeight + nbGapsScoreWeight + elevationChangeScoreWeight;
        if (Math.Abs(weightsSum - 1) > tolerance)
        {
            throw new Exception("Invalid weights, they should sum to 1, got: " + weightsSum.ToString());
        }

        this.nbRowsClearedScoreWeight = nbRowsClearedScoreWeight;
        this.avgHeightScoreWeight = avgHeightScoreWeight;
        this.nbGapsScoreWeight = nbGapsScoreWeight;
        this.elevationChangeScoreWeight = elevationChangeScoreWeight;
    }


    /* =============== Evaluators =============== */

    /// <summary>
    /// returns a double representing the quality of the move
    /// </summary>
    /// <returns></returns>
    public double rateMove(MoveOption move)
    {
        E_CELL_STATUS [,] gameBoard = move.getResultingGameBoard();

        // Define all scores
        double nbRowsClearedScore = IScorer.normaliseScore(getNbRowsCleared(gameBoard), 0, gameBoard.GetLength(0));
        double avgHeightScore = 1 - IScorer.normaliseScore(getAvgHeight(gameBoard), 0, gameBoard.GetLength(0));
        double nbGapsScore = 1 - IScorer.normaliseScore(getNbGaps(gameBoard), 0, gameBoard.GetLength(0)*gameBoard.GetLength(1));
        double elevationChangeScore = 1 - IScorer.normaliseScore(getElevationChange(gameBoard), 0, gameBoard.GetLength(0)*gameBoard.GetLength(1));

        // Define weights in the list
        List<(double, double)> scoreWeightingList = new List<(double, double)>
        {
            (nbRowsClearedScore, nbRowsClearedScoreWeight),
            (avgHeightScore, avgHeightScoreWeight),
            (nbGapsScore, nbGapsScoreWeight),
            (elevationChangeScore, elevationChangeScoreWeight)
        };

        // Calculate and return the rating
        double rating = IScorer.combineScores(scoreWeightingList);
        return rating;
    }

    /// <summary>
    /// returns the number of rows cleared by the move
    /// </summary>
    /// <returns></returns>
    private int getNbRowsCleared(E_CELL_STATUS[,] gameBoard)
    {
        int nbRowsCleared = 0;
        bool emptySpace;
        for (int row = 0; row < gameBoard.GetLength(0); ++row)
        {
            emptySpace = false;
            for (int col = 0; col < gameBoard.GetLength(1); ++col)
            {
                if (gameBoard[row,col] == E_CELL_STATUS.EMPTY) // sets a flag if an empty space is found in the row
                {
                    emptySpace = true;
                }
            }
            if (!emptySpace)
            {
                ++nbRowsCleared; // Increment the number of rows cleared if the row is full
            }
        }
        return nbRowsCleared;
    }

    /// <summary>
    /// Returns the average height of all falling cells in gameBoard
    /// </summary>
    /// <param name="gameBoard"></param>
    /// <returns></returns>
    private double getAvgHeight(E_CELL_STATUS[,] gameBoard)
    {
        // Get a list of the rownum of all falling cells
        List<int> pieceHeights = new List<int>();
        for (int row = 0; row < gameBoard.GetLength(0); ++row)
        {
            for (int col = 0; col < gameBoard.GetLength(1); ++col)
            {
                if (gameBoard[row,col] == E_CELL_STATUS.FALLING)
                {
                    pieceHeights.Add(row);
                }
            }
        }

        // Return the average height of the pieces
        double avgHeight = pieceHeights.Sum();
        avgHeight = avgHeight/pieceHeights.Count();
        avgHeight = 20-avgHeight;
        return avgHeight;

    }

    /// <summary>
    /// Returns the number of empty cells with a settled or falling cell above it
    /// </summary>
    /// <param name="gameBoard"></param>
    /// <returns></returns>
    private int getNbGaps(E_CELL_STATUS[,] gameBoard)
    {
        // Initialize local variables
        int nbGaps = 0;
        List<int> coveredColumns = new List<int>();

        for (int row = 0; row < gameBoard.GetLength(0); ++row)
        {
            for (int col = 0; col < gameBoard.GetLength(1); ++col)
            {
                // Add the column number to the coveredColumns list if the cell is settled or falling
                if (!coveredColumns.Contains(col) && (gameBoard[row,col] == E_CELL_STATUS.SETTLED || gameBoard[row,col] == E_CELL_STATUS.FALLING))
                {
                    coveredColumns.Add(col);
                }
                // If the column is covered and the cell is empty, increment nbGaps
                else if (coveredColumns.Contains(col) && gameBoard[row,col] == E_CELL_STATUS.EMPTY)
                {
                    ++nbGaps;
                }
            }
            
        }

        return nbGaps;
    }


    private int getElevationChange(E_CELL_STATUS[,] gameBoard)
    {
        // Initialize local variables using -1 to indicate not yet set
        int elevationChange = 0;
        int height;
        int prev_height = -1;

        // Iterate through every cell going down each column L-R
        for (int col = 0; col < gameBoard.GetLength(1); ++col)
        {
            for (int row = 0; row < gameBoard.GetLength(0); ++row)
            {
                // When an occupied cell is found, add that elevation change to the running count
                if (gameBoard[row,col] != E_CELL_STATUS.EMPTY || row == gameBoard.GetLength(0)-1)
                {
                    // Get the height of the occupied cell (or 0 if we've reached the bottom)
                    if (gameBoard[row,col] == E_CELL_STATUS.EMPTY)
                    {
                        height = 0;
                    }
                    else {
                        height = gameBoard.GetLength(0)-row;
                    }

                    // update the elevation change and prev_height
                    if (prev_height != -1)
                    {
                        elevationChange = elevationChange + Math.Abs(height-prev_height);
                    }
                    prev_height = height;
                    break;
                }
            }
            
        }
        return elevationChange;
    }

}