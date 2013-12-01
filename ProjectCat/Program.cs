using System;
using ClientCore;

namespace ProjectCat
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new Client("127.0.0.1", 9001);

            client.Connect();

            client.TextReceived += ShowText;

            Console.WriteLine("lalala");

            while (true)
            {
                var text = Console.ReadLine();
                if (!string.IsNullOrEmpty(text))
                {
                    client.Write(text);
                }
            }
        }

        private static void ShowText(string text)
        {
            Console.WriteLine(text);
        }
    }
}
