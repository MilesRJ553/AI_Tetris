class PieceInstance
{

    public E_PIECE piece;
    public E_ROTATION rotation;
    public E_CELL_STATUS[,] pieceArray;


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

    public void rotate()
    {
        int nbPossibleRotations = 4;
        rotation = (E_ROTATION)((int)(rotation+1) % nbPossibleRotations);
        pieceArray = PieceUtils.rotateClockwise90(pieceArray);
    }

    // PIECE TYPE

    // ORIENTATION
}