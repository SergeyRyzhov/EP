namespace ChipSynthesys.Statistic.Models
{
    public class Interserction
    {
        public Interserction(int firstComponentIndex, int secondComponentIndex)
        {
            FirstComponentIndex = firstComponentIndex;
            SecondComponentIndex = secondComponentIndex;
        }

        public int FirstComponentIndex { get; private set; }

        public int SecondComponentIndex { get; private set; }

        public double Area { get; set; }
    }
}