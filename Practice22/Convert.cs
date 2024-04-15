using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practice22
{
    internal static class Convert
    {
        const double minValue = -1.7976931348623157E+308;
        const double maxValue = 1.7976931348623157E+308;
        const double epsilon = 4.9406564584124654E-324;
        static char defaultDelimeter = '.';

        public static double ToDouble(string value, char delimeter = '.')
        {
            if (value  == null)
                return 0;
            value = PrepareString(value, delimeter);
            /// ToDo
            return 0;
        }

        /// <summary>
        /// Подготавливает строку к обработке: удалеяет все символы, кроме цифр,
        /// знака "минус", разделителя дробной части и симпола e(E), обозначающего
        /// </summary>
        /// <param name="value">Подготавливаемая строка</param>
        /// <param name="delimeter">Разделитель дробной части</param>
        /// <returns>Подготовленная строка</returns>
        public static string PrepareString(string value, char delimeter)
        {
            if (value == null) return "";
            delimeter = CheckDelimeter(delimeter);
            string resultString = "";
            bool delimeterFound = false;
            bool minus = false;
            bool e = false;
            bool eMinus = false;
            int eMinusPosition = 0;
            for (int i = 0; i < value.Length; i++)
            {
                switch (value[i])
                {
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        resultString += value[i];
                        break;
                    case '.':
                        if (!delimeterFound && delimeter == '.')
                        {
                            resultString += value[i];
                            delimeterFound = true;
                        }
                        break;
                    case ',':
                        if (!delimeterFound && delimeter == ',')
                        {
                            resultString += value[i];
                            delimeterFound = true;
                        }
                        break;
                    case '-':
                        if (!e)
                        {
                            if (!minus && resultString.Length == 0)
                            { 
                                resultString += value[i];
                                minus = true;
                            }
                        }
                        else
                        {
                            if (!eMinus && eMinusPosition == resultString.Length)
                            {
                                resultString += value[i];
                                eMinus = true;
                            }
                        }
                        break;
                    case 'e':
                    case 'E':
                        if(!e)
                        {
                            resultString += value[i];
                            e = true;
                            eMinusPosition = resultString.Length;
                        }
                        break;
                }
            }
            return resultString;
        }

        /// <summary>
        /// Проверяет, является ли разделитель дробной части точкой или запятой.
        /// Если не является, заменяет на разделитель по умолчанию - точку.
        /// </summary>
        /// <param name="delimeter">Разделитель дробной части</param>
        /// <returns>Корректный разделитель дробной части</returns>
        static char CheckDelimeter(char delimeter)
        {
            if (!(delimeter == '.' || delimeter == ','))
                return defaultDelimeter;
            return delimeter;
        }

        /// <summary>
        /// Получает строку, содержащую мантиссу числа
        /// </summary>
        /// <param name="value">Подготовленная строка</param>
        /// <returns>Мантисса числа</returns>
        public static string GetMantissa(string value)
        {
            if (value == null) return "0";
            string resultString = "";
            for (int i = 0; i < value.Length; i++)
            {
                if (!(value[i] == 'e' || value[i] == 'E'))
                    resultString += value[i];
                else
                    break;
            }
            return resultString;
        }

        /// <summary>
        /// Получает строку, содержащую экспоненту числа
        /// </summary>
        /// <param name="value">Подготовленная строка</param>
        /// <returns>Экспонента числа</returns>
        public static string GetExponent(string value)
        {
            if (value == null) return "0";
            string resultString = "";
            int ePosition = -1;
            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] == 'e' || value[i] == 'E')
                {
                    ePosition = i;
                    break;
                }
            }
            if (ePosition == -1) { return "1"; }
            for (int i = ePosition + 1; i < value.Length; i++)
            {
                resultString += value[i];
            }
            return resultString;
        }

    }
}
