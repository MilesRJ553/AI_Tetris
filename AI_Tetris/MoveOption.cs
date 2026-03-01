using System.Data;
using WindowsInput;
using WindowsInput.Native;

class MoveOption : IScoreable
{
    
    /* =============== Class Attributes =============== */
    private Queue<VirtualKeyCode> inputSequence;
    E_CELL_STATUS[,] resultingGameBoard;
    private double moveRating;

    /* =============== Constructors =============== */
    public MoveOption(Queue<VirtualKeyCode> inputSequence, E_CELL_STATUS[,] resultingGameBoard)
    {
        this.inputSequence = inputSequence;
        this.resultingGameBoard = resultingGameBoard;
        this.moveRating = rateMove();
    }


    /* =============== Evaluators =============== */

    /// <summary>
    /// returns a double representing the quality of the move
    /// </summary>
    /// <returns></returns>
    private double rateMove()
    {
        // Define all scores
        double nbRowsClearedScore = IScoreable.normaliseScore(getNbRowsCleared(), 0, 20);
        double avgHeightScore = 1 - IScoreable.normaliseScore(getAvgHeight(), 0, 20);

        // Define weights in the list
        List<(double, double)> scoreWeightingList = new List<(double, double)>
        {
            (nbRowsClearedScore, 0.5),
            (avgHeightScore, 0.5)
        };

        // Calculate and return the rating
        double rating = IScoreable.combineScores(scoreWeightingList);
        return rating;
        
    }

    /// <summary>
    /// returns the number of rows cleared by the move
    /// </summary>
    /// <returns></returns>
    private int getNbRowsCleared()
    {
        int nbRowsCleared = 0;
        bool emptySpace;
        for (int row = 0; row < resultingGameBoard.GetLength(0); ++row)
        {
            emptySpace = false;
            for (int col = 0; col < resultingGameBoard.GetLength(1); ++col)
            {
                if (resultingGameBoard[row,col] == E_CELL_STATUS.EMPTY) // sets a flag if an empty space is found in the row
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

    private double getAvgHeight()
    {
        // Get a list of the rownum of all settled cells
        List<int> pieceHeights = new List<int>();
        for (int row = 0; row < resultingGameBoard.GetLength(0); ++row)
        {
            for (int col = 0; col < resultingGameBoard.GetLength(1); ++col)
            {
                if (resultingGameBoard[row,col] == E_CELL_STATUS.FALLING)
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


    /* =============== Getters =============== */

    public Queue<VirtualKeyCode> getInputSequence()
    {
        return inputSequence;
    }

    public E_CELL_STATUS[,] getResultingGameBoard()
    {
        return resultingGameBoard;
    }

    public double getMoveRating()
    {
        return moveRating;
    }

}