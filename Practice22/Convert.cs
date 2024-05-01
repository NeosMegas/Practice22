using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Practice22
{
    internal static class Convert
    {
        //const double minValue = -1.7976931348623157E+308;
        //const double maxValue = 1.7976931348623157E+308;
        //const double epsilon = 4.9406564584124654E-324;
        const char defaultDelimeter = '.';
        //const ulong maxMantissaIntegerPart = 1;
        //const ulong maxMantissaFractionalPart = 7976931348623157;
        //const int maxExponentPower = 308;
        //const ulong minMantissaIntegerPart = 4;
        //const ulong minMantissaFractionalPart = 9406564584124654;
        //const int minExponentPower = -324;


        public static double ToDouble(string value, char delimeter = defaultDelimeter)
        {
            if (value == null || value == string.Empty || value == "0")
                return 0;
            //string tempValue = value;
            value = PrepareString(value, delimeter);
            Console.WriteLine("Prepared string = {0}", value);
            ulong tempBytes = value[0] == '-' ? 9223372036854775808 : 0;
            if (value[0] == '-') value = value.Substring(1);
            bool lessThanOne = value[0] == '0';
            int dotIndex = value.IndexOf(delimeter);
            int eIndex = value.IndexOfAny(['e', 'E']);
            int power = eIndex == -1 ? 0 : ToInt32(value.Substring(eIndex + 1));
            if (power > 308 || power < -324)
                throw new ArgumentOutOfRangeException(value, "Число невозможно поместить в тип double.");
            ulong integerPart = 0;
            int n = dotIndex == -1 ? (eIndex == -1 ? value.Length - 1 : eIndex - 1) : dotIndex - 1;
            ulong multiplier = 1;
            ulong tempFractionalPart = 0;
            ulong tempFractionalmultiplier = 1;
            for (int i = n; i >=0; i--)
            {
                integerPart += (ulong)(value[i] - '0') * multiplier;
                if (integerPart >= 4503599627370496)
                {
                    tempFractionalPart += (byte)(integerPart - (integerPart / 10) * 10) * tempFractionalmultiplier;
                    tempFractionalmultiplier *= 10;
                    integerPart /= 10;
                    power++;
                }
                else
                    multiplier *= 10;
            }
            //Console.WriteLine("integerPart = {0}", integerPart);
            //Console.WriteLine("tempFractionalPart = {0}", tempFractionalPart);
            //Console.WriteLine("power = {0}", power);
            int integerPartNumberOfBits = 0;
            ulong quotient = integerPart;
            do
            {
                quotient >>= 1;
                integerPartNumberOfBits++;
            } while (quotient != 0);
            //Console.WriteLine("integerPartNumberOfBits = {0}", integerPartNumberOfBits);
            tempBytes += (ulong)(integerPartNumberOfBits - 1 + 1023) << 52;
            //Console.WriteLine("tempBytes = {0:b}", tempBytes);
            
            ulong integerMask = 0;
            for (int i = 0; i < integerPartNumberOfBits-1; i++)
                integerMask = (integerMask << 1) + 1;
            //Console.WriteLine("integerMask = {0:b}", integerMask);
            tempBytes += (ulong)((integerPart & integerMask) << (52 - integerPartNumberOfBits+1));
            //Console.WriteLine("tempBytes = {0:b}", tempBytes);

            ulong maxNumberForFractionalPart = 2;
            for (int i = 0; i < (52 - integerPartNumberOfBits + 1); i++)
                maxNumberForFractionalPart <<= 1;

            if (dotIndex > -1)
            {
                n = eIndex == -1 ? value.Length : eIndex;
                for (int i = dotIndex+1; i < n; i++)
                    if(tempFractionalPart < maxNumberForFractionalPart)
                        tempFractionalPart = tempFractionalPart * 10 + value[i] - '0';
            }
            //Console.WriteLine("tempFractionalPart = {0}", tempFractionalPart);

            ulong divider = 1;
            quotient = tempFractionalPart;
            do
            {
                quotient /= 10;
                divider *= 10;
            } while (quotient != 0);

            //Console.WriteLine("divider = {0}", divider);

            ulong fractionalPart = 0;
            ulong integerPartOfFractionalPart;
            for (int i = 0; i < (52 - integerPartNumberOfBits); i++)
            {
                if (divider > 0)
                {
                    tempFractionalPart <<= 1;
                    integerPartOfFractionalPart = tempFractionalPart / divider;
                    fractionalPart += integerPartOfFractionalPart;
                    tempFractionalPart -= integerPartOfFractionalPart * divider;
                }
                fractionalPart <<= 1;
            }

            //Console.WriteLine("fractionalPart = {0:b}", fractionalPart);

            tempBytes += fractionalPart;
            //Console.WriteLine("tempBytes = {0:b}", tempBytes);

            ULongToDoube uLongToDoube = new ULongToDoube() { ULong = tempBytes };
            if (lessThanOne)
                uLongToDoube.Double = uLongToDoube.Double > 0 ? uLongToDoube.Double - 1 : uLongToDoube.Double + 1;
            //Console.WriteLine("result = {0}", uLongToDoube.Double);

            for (int i = 0; i < Math.Abs(power); i++)
            {
                uLongToDoube.Double = power > 0 ? uLongToDoube.Double * 10 : uLongToDoube.Double / 10;
            }

            //Console.WriteLine("uLongToDoube.ULong = {0:b}", uLongToDoube.ULong);
            //Console.WriteLine("result = {0}", uLongToDoube.Double);
            //Console.WriteLine("Исходный double = {0:b}", new ULongToDoube() { Double = System.Convert.ToDouble(tempValue) }.ULong);


            return uLongToDoube.Double;
        }

        /// <summary>
        /// Подготавливает строку к обработке: удалеяет все символы, кроме цифр,
        /// знака "минус", разделителя дробной части и симпола e(E), обозначающего
        /// </summary>
        /// <param name="value">Подготавливаемая строка</param>
        /// <param name="delimeter">Разделитель дробной части</param>
        /// <returns>Подготовленная строка</returns>
        static string PrepareString(string value, char delimeter)
        {
            if (value == null) return string.Empty;
            delimeter = CheckDelimeter(delimeter);
            string resultString = string.Empty;
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
                        if (resultString.Length == 0 || (resultString[0] == '-' && resultString.Length == 1))
                                continue;
                        resultString += value[i];
                        break;
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
                        if (!e)
                        {
                            resultString += value[i];
                            e = true;
                            eMinusPosition = resultString.Length;
                        }
                        break;
                }
            }

            if (resultString.Length > 0)
            {
                if (resultString[0] == delimeter)
                    resultString = resultString.Insert(0, "0");
                else if (resultString.Length == 1)
                {
                    if (resultString[0] == '-')
                        resultString = "0";
                    else if (resultString[0] == 'e' || resultString[0] == 'E')
                        resultString = "1e1";
                }
                else if (resultString.Length > 1 && (resultString[0] == '-' && resultString[1] == delimeter))
                {
                    resultString = resultString.Insert(1, "0");
                }
            }
            else if (resultString.Length == 0)
                resultString = "0";

            if (resultString.Length > 1)
            {
                if (e)
                {
                    int positionOfFirstFractionalNumber = resultString.IndexOf(delimeter) + 1;
                    int eIndex = resultString.IndexOfAny(['E', 'e']);
                    int power = ToInt32(resultString.Substring(eIndex+1));
                    int newPower = power;
                    int zerosToRemove = 0;
                    for (int i = positionOfFirstFractionalNumber; i < eIndex; i++)
                    {
                        if (resultString[i] == '0')
                        {
                            zerosToRemove++;
                            newPower--;
                        }
                    }
                    if (newPower != power)
                    {
                        resultString = resultString.Remove(positionOfFirstFractionalNumber, zerosToRemove);
                        eIndex = resultString.IndexOfAny(['E', 'e']);
                        if (newPower != 0)
                            resultString = resultString.Remove(eIndex + 1) + newPower;
                        else
                            resultString = resultString.Remove(eIndex);
                    }
                }
                else
                {
                    int positionOfFirstFractionalNumber = resultString.IndexOf(delimeter) + 1;
                    int zerosToRemove = 0;
                    for (int i = positionOfFirstFractionalNumber; i < resultString.Length; i++)
                    {
                        if (resultString[i] == '0')
                        {
                            zerosToRemove++;
                        }
                    }
                    if (zerosToRemove > 0)
                    {
                        resultString = resultString.Remove(positionOfFirstFractionalNumber, zerosToRemove);
                        resultString = resultString + "e-" + zerosToRemove;
                    }
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
        /// Преобзраует строку в целое число
        /// </summary>
        /// <param name="value">Строка</param>
        /// <returns>Число</returns>
        static int ToInt32(string value)
        {
            int result = 0;
            int sign = value[0] == '-' ? -1 : 1;
            for (int i = value[0] == '-' ? 1 : 0; i < value.Length; i++)
            {
                switch (value[i])
                {
                    case '0':
                        result *= 10;
                        break;
                    case '1':
                        result = result * 10 + 1;
                        break;
                    case '2':
                        result = result * 10 + 2;
                        break;
                    case '3':
                        result = result * 10 + 3;
                        break;
                    case '4':
                        result = result * 10 + 4;
                        break;
                    case '5':
                        result = result * 10 + 5;
                        break;
                    case '6':
                        result = result * 10 + 6;
                        break;
                    case '7':
                        result = result * 10 + 7;
                        break;
                    case '8':
                        result = result * 10 + 8;
                        break;
                    case '9':
                        result = result * 10 + 9;
                        break;
                    default:
                        i = value.Length;
                        break;
                }
            }
            return result*sign;
        }

    }
}
