using WindowsInput;
using WindowsInput.Native;
using System;
using System.Data;
using System.Collections;
using System.Linq.Expressions;

class BoardHandler
{

    /* =============== Class Attributes =============== */
    private E_CELL_STATUS[,] gameBoard = new E_CELL_STATUS[20, 10];


    /* =============== Constructors =============== */

    public BoardHandler()
    {
        // Iterate through each cell of the gameBoard
        for (int row = 0; row < this.gameBoard.GetLength(0); ++row)
        {
            for (int col = 0; col < this.gameBoard.GetLength(1); ++col)
            {
                // Set each cell as empty
                this.gameBoard[row, col] = E_CELL_STATUS.EMPTY;
            }
        }
    }


    public void boardHandlingMain(bool[,] uiGameBoard, bool verbose)
    {
        this.gameBoard = calculateGameBoard(uiGameBoard);
        PieceInstance? fallingPiece = findFallingPiece();
        if (verbose)
        {
            if (fallingPiece != null)
            {
                Console.WriteLine(fallingPiece.piece);
            }
            else
            {
                Console.WriteLine("None");
            }  
        }
    }

    public E_CELL_STATUS[,] calculateGameBoard(bool[,] uiGameBoard)
    {
        
        E_CELL_STATUS[,] gameBoard =  new E_CELL_STATUS[20, 10];

        // Set all occupied cells on the bottom row and their adjacent cells to be settled
        for (int col = 0; col < uiGameBoard.GetLength(1); ++col)
        {
            int row = uiGameBoard.GetLength(0)-1;
            if (uiGameBoard[row,col])
            {
                gameBoard[row,col] = E_CELL_STATUS.SETTLED;
                setNeighbourCellsSettled(row, col, uiGameBoard, gameBoard);
            }
        }

        // Set all occupied and not already settled cells in clusters of >5 to be settled
        int minClusterSize = 5;
        setClustersSettled(uiGameBoard, minClusterSize, gameBoard);

        // Set all occupied cells which aren't settled to be falling
        for (int row = 0; row < uiGameBoard.GetLength(0); ++row)
        {
            for (int col = 0; col < uiGameBoard.GetLength(1); ++col)
            {
                if (uiGameBoard[row,col] && gameBoard[row,col] != E_CELL_STATUS.SETTLED)
                {
                    gameBoard[row,col] = E_CELL_STATUS.FALLING;
                }
            }
        }


        return gameBoard;
    }

    private void setClustersSettled(bool[,] uiGameBoard, int minClusterSize, E_CELL_STATUS[,] gameBoard)
    {

        int height = gameBoard.GetLength(0);
        int width = gameBoard.GetLength(1);
        // Iterate through each cell in the gameBoard
        for (int row = 0; row < height; ++row)
        {
            for (int col = 0; col < width; ++col)
            {
                if (uiGameBoard[row,col] && gameBoard[row,col] != E_CELL_STATUS.SETTLED)
                {
                    // Get the cluster around the cell
                    (int,int) coords = (row,col);
                    HashSet<(int,int)> cellsInCluster = findFallingCluster(uiGameBoard, coords);
                    
                    // If it's as large as the threshold, set all cells in the cluster to SETTLED
                    if (cellsInCluster.Count >= minClusterSize)
                    {
                        gameBoard[row,col] = E_CELL_STATUS.SETTLED;
                    }
                }
            }
        }
        
    }

    /// <summary>
    /// Returns a HasSet of all cells in a continuous orthongally connected cluser with the initialCell
    /// </summary>
    /// <param name="uiGameBoard"></param>
    /// <param name="initialCell"></param>
    /// <returns>cellsInCluster</returns>
    private HashSet<(int,int)> findFallingCluster(bool[,] uiGameBoard, (int, int) initialCell)
    {
        int row = initialCell.Item1;
        int col = initialCell.Item2;

        // Create the queue
        Queue<(int,int)> cellsQueue = new Queue<(int,int)>();

        // Create the structure to be returned
        HashSet<(int,int)> cellsInCluster = new HashSet<(int, int)>();

        // Add the cell to the queue if it's occupied and not settled
        if (uiGameBoard[row,col] && gameBoard[row,col] != E_CELL_STATUS.SETTLED)
        {
            cellsQueue.Enqueue(initialCell);
        }

        while (cellsQueue.Count > 0)
        {
            
            // Get the next cell to be checked
            (int,int) cell = cellsQueue.Dequeue();
            row = cell.Item1;
            col = cell.Item2;

            // Get all adjacent cells
            (int, int)[] cellsToCheck =
            {
                (row-1, col), // above
                (row+1, col), // below
                (row, col-1), // left
                (row, col+1), // right
            };

            foreach ((int,int)neighbour in cellsToCheck)
            {

                int neighRow = neighbour.Item1;
                int neighCol = neighbour.Item2;

                if (neighRow > 0 && neighRow < uiGameBoard.GetLength(0) && neighCol > 0 && neighCol < uiGameBoard.GetLength(1)) // Check the neighbour is in the grid
                {
                    if (uiGameBoard[neighRow,neighCol] == true && gameBoard[neighRow,neighCol] != E_CELL_STATUS.SETTLED) // Check it's occupied and not already settled
                    {
                        if (cellsInCluster.Add(neighbour))
                        {
                            cellsQueue.Enqueue(neighbour);
                        }
                    }
                }
            }
            
        }

        // Return a hashset with all cells in the cluster
        return cellsInCluster;

    }

    private void setNeighbourCellsSettled(int row, int col, bool[,] uiGameBoard, E_CELL_STATUS[,] gameBoard)
    {

        // Get all adjacent and diagonal cells
        (int, int)[] cellsToCheck =
        {
            (row-1, col), // above
            (row+1, col), // below
            (row, col-1), // left
            (row, col+1), // right
            (row-1, col-1), // above left
            (row-1, col+1), // above right
            (row+1, col-1), // below left
            (row+1, col+1) // below right
        };
        foreach ((int,int) cell in cellsToCheck)
        {
            // Check they're occupied
            if (
                cell.Item1 >= 0 && cell.Item1 < uiGameBoard.GetLength(0) &&
                cell.Item2 >= 0 && cell.Item2 < uiGameBoard.GetLength(1) &&
                uiGameBoard[cell.Item1,cell.Item2]
            )
            {
                // If not already settled, set to settled and recurse
                if (gameBoard[cell.Item1,cell.Item2] != E_CELL_STATUS.SETTLED)
                {
                    gameBoard[cell.Item1,cell.Item2] = E_CELL_STATUS.SETTLED;
                    setNeighbourCellsSettled(cell.Item1, cell.Item2, uiGameBoard, gameBoard);
                }
            }
        }
    }

    private int getNbFullRows()
    {
        // Declare local variables
        bool rowComplete;
        int col;
        int nbFullRows = 0;

        // Iterate through each row
        for (int row = 0; row < this.gameBoard.GetLength(0); ++row)
        {
            rowComplete = true;
            col = 0;
            // Iterates through a row of cells until it finds an empty cell
            while (col < this.gameBoard.GetLength(1) && rowComplete)
            {
                if (this.gameBoard[row, col] != E_CELL_STATUS.SETTLED)
                {
                    rowComplete = false; // Sets the rowComplete flag to false if there are any unsettled cells
                }
                ++col;
            }
            if (rowComplete)
            {
                ++nbFullRows;
            }
        }
        return nbFullRows;
    }


    public PieceInstance? findFallingPiece(E_CELL_STATUS[,]? localGameBoard  = null)
    {
        if (localGameBoard == null)
        {
            localGameBoard = (E_CELL_STATUS[,])gameBoard.Clone();
        }
        int height = localGameBoard.GetLength(0);
        int width = localGameBoard.GetLength(1);
        // Iterate through each cell in the gameBoard
        for (int row = 0; row < height; ++row)
        {
            for (int col = 0; col < width; ++col)
            {
                if (localGameBoard[row, col] == E_CELL_STATUS.FALLING) // Identify a falling cell
                {
                    // Add the first falling cell to the list
                    (int, int) startIndex = (row, col);
                    List<(int, int)> lst = new List<(int, int)>();
                    lst.Add(startIndex);

                    int lstIndex = 0;
                    while (lstIndex < lst.Count()) // Iterate through each cell in the list
                    {
                        (int, int) currentCell = lst[lstIndex];
                        (int, int)[] indexesToCheck =
                        {
                            (currentCell.Item1-1, currentCell.Item2), // above
                            (currentCell.Item1+1, currentCell.Item2), // below
                            (currentCell.Item1, currentCell.Item2-1), // left
                            (currentCell.Item1, currentCell.Item2+1) // right
                        };

                        foreach ((int, int) cell in indexesToCheck)
                        {
                            if (!lst.Contains(cell)) // Check that the cell isn't already in the List
                            {
                                if (cell.Item1 >= 0 && cell.Item1 < height && cell.Item2 >= 0 && cell.Item2 < width) // Check the cell is in the bounds of gameBoard
                                {
                                    if (localGameBoard[cell.Item1, cell.Item2] == E_CELL_STATUS.FALLING)
                                    {
                                        lst.Add((cell.Item1, cell.Item2)); // Add the coordingates to the list of the cell is falling
                                    }
                                }
                            }
                        }
                        ++lstIndex;
                    }

                    // Create array of piece found
                    int minRow = lst[0].Item1;
                    int maxRow = lst[0].Item1;
                    int minCol = lst[0].Item2;
                    int maxCol = lst[0].Item2;

                    foreach ((int, int) coord in lst) // Define the "four corners" of the piece
                    {
                        minRow = Math.Min(minRow, coord.Item1);
                        maxRow = Math.Max(maxRow, coord.Item1);
                        minCol = Math.Min(minCol, coord.Item2);
                        maxCol = Math.Max(maxCol, coord.Item2);
                    }

                    // Define the piece array
                    E_CELL_STATUS[,] pieceArray = new E_CELL_STATUS[maxRow-minRow+1, maxCol-minCol+1];
                    for (int rowNum = minRow; rowNum <= maxRow; ++rowNum)
                    {
                        for (int colNum = minCol; colNum <= maxCol; ++colNum)
                        {
                            pieceArray[rowNum-minRow, colNum-minCol] = localGameBoard[rowNum, colNum];
                        }
                    }
                    return PieceUtils.pieceIdentifier(pieceArray);
                }
            }
        }
        return null;
    }

    public (int, int) findLeftMostFallingCell(E_CELL_STATUS[,]? localGameBoard  = null)
    {
        if (localGameBoard == null)
        {
            localGameBoard = this.gameBoard;
        }
        int height = localGameBoard.GetLength(0);
        int width = localGameBoard.GetLength(1);

        for (int col = 0; col < width; ++col)
        {
            for (int row = 0; row < height; ++row)
            {
                if (localGameBoard[row, col] == E_CELL_STATUS.FALLING) // Identify a falling cell
                {
                    return (row, col);
                }
            }
        }
        
        return (-1,-1);
        
    }

    public (int, int) findRightMostFallingCell(E_CELL_STATUS[,]? localGameBoard  = null)
    {
        if (localGameBoard == null)
        {
            localGameBoard = this.gameBoard;
        }
        int height = localGameBoard.GetLength(0);
        int width = localGameBoard.GetLength(1);

        for (int col = width-1; col >= 0; --col)
        {
            for (int row = 0; row < height; ++row)
            {
                if (localGameBoard[row, col] == E_CELL_STATUS.FALLING) // Identify a falling cell
                {
                    return (row, col);
                }
            }
        }
        
        return (-1,-1);
        
    }


    /// <summary>
    /// Starts at the top row and returns the row, col of the firt falling cell
    /// </summary>
    /// <param name="startRight"></param>
    /// <returns></returns>
    public (int, int) findFirstFallingCell(E_CELL_STATUS[,]? localGameBoard  = null)
    {
        if (localGameBoard == null)
        {
            localGameBoard = this.gameBoard;
        }
        int height = localGameBoard.GetLength(0);
        int width = localGameBoard.GetLength(1);

        for (int row = 0; row < height; ++row)
        {
            for (int col = 0; col < width; ++col)
            {
                if (localGameBoard[row, col] == E_CELL_STATUS.FALLING) // Identify a falling cell
                {
                    return (row, col);
                }
            }
        }
        
        throw new Exception("No falling cell found");
    }


    public E_CELL_STATUS[,] getGameBoard()
    {
        return (E_CELL_STATUS[,])this.gameBoard.Clone();
    }

    public void setGameBoard(E_CELL_STATUS[,] newGameBoard)
    {
        if (
            newGameBoard.GetLength(0) != gameBoard.GetLength(0) ||
            newGameBoard.GetLength(1) != gameBoard.GetLength(1)
        )
        {
            throw new Exception("Invalid new game board");
        }
        this.gameBoard = (E_CELL_STATUS[,])newGameBoard.Clone();
    }
    
    /* =============== Debug Methods =============== */

    public void printGameBoard()
    {
        E_CELL_STATUS cellStatus;
        String boardOutput = "";

        for (int row = 0; row < this.gameBoard.GetLength(0); ++row)
        {
            for (int col = 0; col < this.gameBoard.GetLength(1); ++col)
            {
                cellStatus = this.gameBoard[row, col];
                boardOutput = boardOutput + "|";
                switch (cellStatus)
                {
                    case E_CELL_STATUS.EMPTY:
                        boardOutput = boardOutput + " |";
                        break;
                    case E_CELL_STATUS.FALLING:
                        boardOutput = boardOutput + "F|";
                        break;
                    case E_CELL_STATUS.SETTLED:
                        boardOutput = boardOutput + "S|";
                        break;
                }
            }
            boardOutput = boardOutput + Environment.NewLine;
        }

        boardOutput = boardOutput + Environment.NewLine;
        boardOutput = boardOutput + Environment.NewLine;
        boardOutput = boardOutput + ("=  =  =  =  =  =  =  =  =  =");
        boardOutput = boardOutput + Environment.NewLine;
        boardOutput = boardOutput + Environment.NewLine;
        
        Console.WriteLine(boardOutput);
    }

}