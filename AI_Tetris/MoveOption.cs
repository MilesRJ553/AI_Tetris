using System.Data;
using WindowsInput;
using WindowsInput.Native;

class MoveOption
{
    
    /* =============== Class Attributes =============== */
    private Queue<VirtualKeyCode> inputSequence;
    E_CELL_STATUS[,] resultingGameBoard;
    private bool hold;

    /* =============== Constructors =============== */
    public MoveOption(Queue<VirtualKeyCode> inputSequence, E_CELL_STATUS[,] resultingGameBoard, bool hold)
    {
        this.hold = hold;
        this.resultingGameBoard = resultingGameBoard;
        if (hold)
        {
            this.inputSequence = new Queue<VirtualKeyCode>(Enumerable.Repeat(VirtualKeyCode.VK_C, 1));
        }
        else
        {
            this.inputSequence = inputSequence;            
        }
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

    public bool isHold()
    {
        return this.hold;
    }

}