using System;

namespace Test
{
    public partial class Tests
    {
        private static void Main(string[] args)
        {
            run();
        }

        static public void run()
        {
            run_PlaceModel();
        }

        private static void log(string text)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(text);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        private static void test(string name, bool result)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("\n{0}\t", name);
            Console.ForegroundColor = (result) ? ConsoleColor.Green : ConsoleColor.Red;
            Console.Write("{0}", (result) ? "OK" : "ERROR");
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}