using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using WindowsInput;
using WindowsInput.Native;

class LearningCS
{

    private InputSimulator inputSim = new InputSimulator();

    static async Task Main()
    {
        LearningCS lc = new LearningCS();
        Console.WriteLine("Which app would you like to open? (e.g., notepad, calc)");
        string appName = Console.ReadLine() ?? "";
        await lc.openApp(appName);
    }

    private async Task openRun()
    {
        // Simulate Ctrl + R
        inputSim.Keyboard.ModifiedKeyStroke(VirtualKeyCode.LWIN, VirtualKeyCode.VK_R);

        await Task.Delay(500); // Wait for the Run dialog to open

    }

    private async Task openApp(string appName)
    {
        await openRun();

        inputSim.Keyboard.TextEntry(appName);
        inputSim.Keyboard.KeyPress(VirtualKeyCode.RETURN); // Simulate Enter key
        await Task.Delay(200); // Wait for text entry to complete

    }

}