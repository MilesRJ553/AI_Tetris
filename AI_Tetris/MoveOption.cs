using System.Data;
using WindowsInput;
using WindowsInput.Native;

class MoveOption
{
    
    /* =============== Class Attributes =============== */
    private Queue<VirtualKeyCode> inputSequence;
    E_CELL_STATUS[,] resultingGameBoard;

    /* =============== Constructors =============== */
    public MoveOption(Queue<VirtualKeyCode> inputSequence, E_CELL_STATUS[,] resultingGameBoard)
    {
        this.inputSequence = inputSequence;
        this.resultingGameBoard = resultingGameBoard;
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

}