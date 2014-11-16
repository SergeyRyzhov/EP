namespace ChipSynthesys.Statistic.Models
{
    public class Result<T>
    {
        public Result(T before) : this(before, default(T))
        {
        }

        public Result()
        {
        }

        public Result(T before, T after)
        {
            Before = before;
            After = after;
        }

        public T Before { get; set; }
        public T After { get; set; }
    }
}