class PIECE_INSTANCE
{

    public E_PIECE piece {get;}
    E_ROTATION rotation;
    E_CELL_STATUS[,] pieceArray;


    public PIECE_INSTANCE(E_PIECE piece, E_ROTATION rotation)
    {
        this.piece = piece;
        this.rotation = rotation;
        
        pieceArray = TETROMINOES.tetrominoes[piece];
        for (int index = 0; index < (int)rotation; ++index)
        {
            pieceArray = PIECE_UTILS.rotateClockwise90(pieceArray);
        }
    }

    // PIECE TYPE

    // ORIENTATION
}