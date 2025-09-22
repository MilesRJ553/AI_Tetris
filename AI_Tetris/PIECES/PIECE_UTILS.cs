class PIECE_UTILS
{
    /// <summary>
    /// Rotates a 2D array 90 degrees Clockwise and returns the result
    /// </summary>
    public E_CELL_STATUS[,] rotateClockwise90(E_CELL_STATUS[,] original) 
    {
        // Define the length of the original 2D array
        int oldRows = original.GetLength(0);
        int oldCols = original.GetLength(1);

        // Declare the new, rotated, array with the opposite rows and columns
        E_CELL_STATUS[,] rotated = new E_CELL_STATUS[oldCols,oldRows];

        for (int r = 0; r < oldRows; ++r) 
        {
            for (int c = 0; c < oldCols; ++c)
            {
                // Place each value into its rotated position
                int newColNum = oldRows-1-r;  // The new column number is the number of rows from the top (r) away from the end (oldRowNum-1)
                int newRowNum = c;  // The new row number is the old column number

                rotated[newRowNum, newColNum] = original[r, c];
            }
        }

        return rotated;
    }

    public bool compareArrays(E_CELL_STATUS[,] pieceArray1, E_CELL_STATUS[,] pieceArray2)
    {
        // Checks the array dimensions are the same and returns false if not
        if (pieceArray1.GetLength(0) != pieceArray2.GetLength(0) || pieceArray1.GetLength(1) != pieceArray2.GetLength(1))
        {
            return false;
        }

        // Iterates through each position in the arrays
        for(int row = 0; row < pieceArray1.GetLength(0); ++row)
        {
            for(int col = 0; col < pieceArray1.GetLength(1); ++col)
            {
                // Returns false if the cells don't match
                if (pieceArray1[row, col] != pieceArray2[row, col])
                {
                    return false;
                }
            }
        }

        // Returns true if the arrays are identical
        return true;
    }

    /// <summary>
    /// Checks whether a 2D E_CELL_STATUS corresponds with a tetromino
    /// If it does, returns a PIECE_INSTANCE containing the piece type (E_PIECE) and its Orientation
    /// </summary>
    /// <param name="piece1"></param>
    /// <param name="piece2"></param>
    /// <returns></returns>
    public E_PIECE pieceIdentifier(E_CELL_STATUS[,] targetPieceArray)
    {
        // Declaring local variables
        int numRotations = Enums.GetValues(typeof(E_ROTATION)).Length;
        int numPieces = Enums.GetValues(typeof(E_PIECE)).Length;
        E_PIECE currentPiece;
        E_CELL_STATUS[,] currentPieceArray;


        for (int rotationIndex = 0; rotationIndex < numRotations; ++rotationIndex)
        {
            for (int pieceIndex = 0; pieceIndex < numPieces; ++pieceIndex)
            {
                currentPiece = (E_PIECE) pieceIndex;
                currentPieceArray = TETROMINOES.tetrominoes.GetValue(currentPiece);
            }
        }

        return E_PIECE.UNKNOWN;
    }

    // recogniser
}