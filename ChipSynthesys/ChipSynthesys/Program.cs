using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlaceModel;

namespace ChipSynthesys
{
    class Program
    {
        static void Main(string[] args)
        {
            Test.tests.run();
            
            Design.Save(Generator.Random(10, 10, 50), "test.xml");
        }
    }
}
