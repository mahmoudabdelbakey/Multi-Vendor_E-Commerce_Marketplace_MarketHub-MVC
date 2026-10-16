
using System;
using System.Collections.Generic;
using System.Text;

namespace BankVault.UI
{
    internal class ConsoleHelper
    {
        public static int ReadInt(string message)
        {
            while (true)
            {
                Console.Write(message);
                string? input = Console.ReadLine();

                if (int.TryParse(input, out int result))
                {
                    return result;
                }

                Console.WriteLine("Invalid input. Please enter a valid number.");
            }
        }

        public static decimal ReadAmount(string message)
        {
            while (true)
            {
                Console.Write(message);
                string? input = Console.ReadLine();

                if (decimal.TryParse(input, out decimal result))
                {
                    return result;
                }

                Console.WriteLine("Invalid amount. Please enter a valid amount.");
            }
        }
    }
}
