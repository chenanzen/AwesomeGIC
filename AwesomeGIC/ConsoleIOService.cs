using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwesomeGIC
{
    public interface IIOService
    {
        string GetInput(string message);
        void ShowMessage(string message);
    }

    public class ConsoleIOService : IIOService
    {
        public string GetInput(string message)
        {
            Console.Write(message);
            var input = Console.ReadLine();
            input = input ?? string.Empty;

            return input;
        }

        public void ShowMessage(string message)
        {
            Console.WriteLine(message);
            return;
        }
    }
}
