using AwesomeGIC;
using System.Runtime.CompilerServices;


var menu1 =
    "Welcome to AwesomeGIC Bank! What would you like to do?" +
    "[T] Input transactions\n" +
    "[I] Define interest rules\n" +
    "[P] Print statement\n" +
    "[Q] Quit\n" +
    "> ";

var menu2 =
    "Please enter transaction details in <Date> <Account> <Type> <Amount> format" +
    "(or enter blank to go back to main menu):\n>" +
    "> ";

var keepRunning = true;
while (keepRunning)
{
    Console.Write(menu1);
    option = Console.ReadLine();
    option = option.ToLower();

    switch (option)
    {
        case "t":
            break;
        case "i":
            break;
        case "p":
            break;
        case "q":
            keepRunning = false;
            break;
        default:
            break;
    }
}


public class GICData
{
    private readonly int _currentSelection;
    private readonly string _transaction;

    public void EnterSelection(int selection)
    {

    }



}

//public class GICService
//{
//    public bool KeepRunning { get; }

//    public GICService()
//    {
//        KeepRunning = true;
//    }

//    public void PrintMenu()
//    {
//        Console.WriteLine();
//    }

//    public IGICProcess GetUserSelectedProcess()
//    {
//        return new QuitProcess(ref KeepRunning);
//    }
//}


