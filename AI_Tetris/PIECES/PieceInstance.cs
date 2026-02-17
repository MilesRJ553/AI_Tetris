class PieceInstance
{

    public E_PIECE piece {get;}
    E_ROTATION rotation;
    E_CELL_STATUS[,] pieceArray;


    public PieceInstance(E_PIECE piece, E_ROTATION rotation)
    {
        this.piece = piece;
        this.rotation = rotation;
        
        pieceArray = Tetrominoes.tetrominoes[piece];
        for (int index = 0; index < (int)rotation; ++index)
        {
            pieceArray = PieceUtils.rotateClockwise90(pieceArray);
        }
    }

    // PIECE TYPE

    // ORIENTATION
}