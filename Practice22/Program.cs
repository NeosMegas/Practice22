using Practice22;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using Convert = Practice22.Convert;

namespace Practice22
{
    [StructLayout(LayoutKind.Explicit)]
    public struct ULongToDoube
    {
        [FieldOffset(0)] public double Double;
        [FieldOffset(0)] public ulong ULong;
    }

    internal static class Program
    {
        static void Main(string[] args)
        {
            char delimeter = '.';
            Console.Write("Выберите разделитель дробной части: 1 - точка, 2 - запятая: ");
            int result = Console.Read();
            if (result == '2') delimeter = ',';
            Console.ReadLine();
            Console.WriteLine("разделитель = {0}", delimeter);
            while (true)
            {
                Console.WriteLine("Введите строку для преобразования в число типа double (Ctrl-C для завершения):");
                string? inputString = Console.ReadLine() ?? "";
                try
                {
                    Console.WriteLine($"Результат: {Convert.ToDouble(inputString, delimeter)}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
