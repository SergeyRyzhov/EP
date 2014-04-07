using PlaceModel;

namespace ChipSynthesys.UnitTests
{
    public class TestsBase
    {
        protected virtual string TestFilePath(string fileName)
        {
            return string.Format("..\\..\\{0}", fileName);
        }

        protected virtual Design TestDesign()
        {
            var p = new Component.Pool();
            p.Add(3, 3);
            p.Add(3, 3);
            p.Add(5, 3);
            p.Add(2, 3);
            p.Add(3, 2);
            p.Add(2, 2);
            var n = new Net.Pool();
            var design = new Design(new Field(0, 0, 20, 20), p, n);
            return design;
        }
    }
}
