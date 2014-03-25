namespace ChipSynthesys.Common.Randoms
{
    /// <summary>
    /// Интерфейс для инкапсуляции генератора случаных чисел.
    /// </summary>
    /// <typeparam name="T">Тип генерируемых величин</typeparam>
    public interface IRandom<out T>
    {
        /// <summary>
        /// Следующий элемент случаной последовательности
        /// </summary>
        /// <returns></returns>
        T Next();

        /// <summary>
        /// Математическое ожидание случаной величины
        /// </summary>
        /// <returns>Мат. ожидание, если его расчёт возможен. Иначе null</returns>
        double? MathematicalExpectation();
    }
}