using LotoLibrary.Enums;

namespace LotoLibrary.Extensions
{
    public static class StringExtensions
    {
        public static bool IsModelType(this string str, string modelType)
        {
            return str == modelType;
        }

        public static bool IsAntiFrequency(this string str)
        {
            return str.IsModelType("AntiFrequency");
        }

        public static bool IsAntiFrequencySimple(this string str)
        {
            return str.IsModelType("AntiFrequencySimple");
        }
    }
}
