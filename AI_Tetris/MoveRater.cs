class MoveRater : IScorer
{

    private double nbRowsClearedWeight;
    private double avgHeightScoreWeight;
    
    public MoveRater(double nbRowsClearedWeight, double avgHeightScoreWeight)
    {
        this.nbRowsClearedWeight = nbRowsClearedWeight;
        this.avgHeightScoreWeight = avgHeightScoreWeight;
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
        double nbRowsClearedScore = IScorer.normaliseScore(getNbRowsCleared(gameBoard), 0, 20);
        double avgHeightScore = 1 - IScorer.normaliseScore(getAvgHeight(gameBoard), 0, 20);

        // Define weights in the list
        List<(double, double)> scoreWeightingList = new List<(double, double)>
        {
            (nbRowsClearedScore, nbRowsClearedWeight),
            (avgHeightScore, avgHeightScoreWeight)
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

    private double getAvgHeight(E_CELL_STATUS[,] gameBoard)
    {
        // Get a list of the rownum of all settled cells
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

}