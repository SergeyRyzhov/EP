namespace ChipSynthesys.Common.Randoms
{
    /// <summary>
    /// Интерфейс для инкапсуляции генератора случаных чисел.
    /// </summary>
    /// <typeparam name="T">Тип генерируемых величин</typeparam>
    public interface IRandom<out T>
    {
        T Next();

        double? MathematicalExpectation();
    }
}