class TETROMINOES
{
    public static readonly Dictionary<E_PIECE, E_CELL_STATUS[,]> tetrominoes = new Dictionary<E_PIECE, E_CELL_STATUS[,]>()
    {
        // Line
        { E_PIECE.I, new E_CELL_STATUS[,]
            {
                {E_CELL_STATUS.FALLING},
                {E_CELL_STATUS.FALLING},
                {E_CELL_STATUS.FALLING},
                {E_CELL_STATUS.FALLING}
            }
        },

        // Square
        { E_PIECE.O, new E_CELL_STATUS[,]
            {
                {E_CELL_STATUS.FALLING, E_CELL_STATUS.FALLING},
                {E_CELL_STATUS.FALLING, E_CELL_STATUS.FALLING}
            }
        },

        // S
        { E_PIECE.S, new E_CELL_STATUS[,]
            {
                {E_CELL_STATUS.EMPTY, E_CELL_STATUS.FALLING, E_CELL_STATUS.FALLING},
                {E_CELL_STATUS.FALLING, E_CELL_STATUS.FALLING, E_CELL_STATUS.EMPTY}
            }
        },

        // Z
        { E_PIECE.Z, new E_CELL_STATUS[,]
            {
                {E_CELL_STATUS.FALLING, E_CELL_STATUS.FALLING, E_CELL_STATUS.EMPTY},
                {E_CELL_STATUS.EMPTY, E_CELL_STATUS.FALLING, E_CELL_STATUS.FALLING}
            }
        },

        // L
        { E_PIECE.L, new E_CELL_STATUS[,]
            {
                {E_CELL_STATUS.FALLING, E_CELL_STATUS.EMPTY},
                {E_CELL_STATUS.FALLING, E_CELL_STATUS.EMPTY},
                {E_CELL_STATUS.FALLING, E_CELL_STATUS.FALLING}
            }
        },

        // J
        { E_PIECE.J, new E_CELL_STATUS[,]
            {
                {E_CELL_STATUS.EMPTY, E_CELL_STATUS.FALLING},
                {E_CELL_STATUS.EMPTY, E_CELL_STATUS.FALLING},
                {E_CELL_STATUS.FALLING, E_CELL_STATUS.FALLING}
            }
        },

        // T
        { E_PIECE.T, new E_CELL_STATUS[,]
            {
                {E_CELL_STATUS.FALLING, E_CELL_STATUS.FALLING, E_CELL_STATUS.FALLING},
                {E_CELL_STATUS.EMPTY, E_CELL_STATUS.FALLING, E_CELL_STATUS.EMPTY}
            }
        },

    };
}