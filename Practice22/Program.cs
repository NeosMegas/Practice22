using Practice22;
using Convert = Practice22.Convert;

char delimeter = '.';
Console.Write("Выберите разделитель дробной части: 1 - точка, 2 - запятая: ");
int result = Console.Read();
if (result == '2') delimeter = ',';
Console.ReadLine();
Console.WriteLine("result = {0}, delimeter = {1}", result, delimeter);
Console.WriteLine("Для завершения нажмите Ctrl-C");
while (true)
{
    Console.WriteLine("Введите строку для преобразования в чисто типа double:");
    string? inputString = Console.ReadLine() ?? "";
    string preparedString = Convert.PrepareString(inputString, delimeter);
    Console.WriteLine($"M = {Convert.GetMantissa(preparedString)}, E = {Convert.GetExponent(preparedString)}");
}

