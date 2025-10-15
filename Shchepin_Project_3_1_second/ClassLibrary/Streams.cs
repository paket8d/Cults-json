using ClassLibrary;
using System.IO;
using System.Security;
using System.Text;
namespace ClassLibrary
{
    /// <summary>
    /// Класс, объект которого управляет потоками ввода/вывода
    /// </summary>
    public class Streams
    {
        public StreamReader sr;
        public StreamWriter sw;
        public bool StreamReadStart(string path)
        {
            try
            {
                sr = new StreamReader(path);
                Console.SetIn(sr);
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                Menu.ShowError("Нет доступа");
            }
            catch (ArgumentNullException)
            {
                Menu.ShowError("Строка не введена");
            }
            catch (ArgumentException)
            {
                Menu.ShowError("Строка не введена");
            }
            catch (FileNotFoundException)
            {
                Menu.ShowError("Файл не найден");
            }
            catch (DirectoryNotFoundException)
            {
                Menu.ShowError("Недопустимый путь");
            }
            catch (IOException)
            {
                Menu.ShowError("Недопустимый формат пути");
            }
            return false;
        }
        public void StreamReadEnd()
        {
            sr.Dispose();
            StreamReader standart = new StreamReader(Console.OpenStandardInput(), Console.InputEncoding);
            Console.SetIn(standart);
        }
        public bool StreamWriteStart(string path)
        {
            try
            {
                sw = new StreamWriter(path, false);
                Console.SetOut(sw);
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                Menu.ShowError("Нет доступа");
            }
            catch (ArgumentNullException)
            {
                Menu.ShowError("Строка не введена");
            }
            catch (ArgumentException)
            {
                Menu.ShowError("Строка не введена");
            }
            catch (FileNotFoundException)
            {
                Menu.ShowError("Файл не найден");
            }
            catch (DirectoryNotFoundException)
            {
                Menu.ShowError("Недопустимый путь");
            }
            catch (IOException)
            {
                Menu.ShowError("Недопустимый формат пути");
            }
            return false;
        }
        public void StreamWriteEnd()
        {
            sw.Dispose();
            StreamWriter standart = new StreamWriter(Console.OpenStandardOutput(), Console.OutputEncoding);
            standart.AutoFlush = true;
            Console.SetOut(standart);
        }
    }
}