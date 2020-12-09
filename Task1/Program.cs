using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Task1
{
    class Program
    {
        //для гнерации строк для записи

        static Random rnd = new Random();
        //инициализируем латинский алфавит
        const string alphabet = "QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm";
        //инициализируем сегодняшнюю дату
        static DateTime today = DateTime.Today;
        //инициализируем количетво дней с сегодняшней даты за последние 5 лет
        static int range = (today - today.AddYears(-5)).Days;

        static void Main(string[] args)
        {
            // меню
            bool menuIsExist = true; 
            while (menuIsExist)
            {
                Console.WriteLine("");
                Console.WriteLine("Напишите номер нужной команды:");
                Console.WriteLine("");
                Console.WriteLine("1- создать n файлов c m рандомных строк");
                Console.WriteLine("2- объединение всех файлов в один");
                Console.WriteLine("3- объединение конкретных файлов в один");
                Console.WriteLine("4- импорта файлов");
                Console.WriteLine("0- Выход");

                string item = Console.ReadLine();
                switch (item)
                {
                    case "0":
                        menuIsExist = false;
                        break;
                    case "1":
                        //метод создания n файлов с m-ым количеством рандомных строк
                        Console.Write("Введте количество файлов: ");
                        int files = int.Parse(Console.ReadLine());
                        Console.Write("Введте количество строк: ");
                        int rows = int.Parse(Console.ReadLine());
                        CreateFiles(files, rows);
                        break;
                    case "2":
                        //метод для объединения всех файлов
                        ConcatenatingAllFilesWithDeletingLines();
                        break;
                    case "3":
                        //метод для объединения конкретных файлов, указанных пользователем
                        ConcatenatingSomeFilesWithDeletingLines();
                        break;
                    case "4":
                        //метод для импорта указанного файла в БД
                        SaveFileToDatabaseAsync();
                        break;
                    default:
                        //в случаи вызова неизвестной операции
                        Console.WriteLine("Вы выбрали неизвестную операцию");
                        Console.WriteLine("");
                        break;
                }
            }
            Console.ReadLine();
        }

        //метод создания директория и файлов (передаем количество файлов и количество строк в каждом файле)
        public static void CreateFiles(int countFiles, int rows)
        {
            //создаем директорий для файла

            string path = @"D:\КУРС\EY\Task1\Fails"; //путь где будут находиться файлы
            DirectoryInfo dirInfo = new DirectoryInfo(path); 
            //создаем директорй, если еще нет
            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }
            
            //записываем строки в файл
            for (int i = 1; i <= countFiles; i++)
            {
                //прописываем путь где будут находится файлы и формируем название файлов
                //файлы будут называться "text[порядковый номер документа]" (например, text1)
                string writePath = String.Concat(@"D:\КУРС\EY\Task1\Fails\", "text", i.ToString(), ".txt");
                //вызываем асинхронный метод для записи строк в файл, передаем путь и количесво строк
                WritingToFailsAsync(writePath, rows);
            }
        }

        //вызываем асинхронный метод для записи строк в файл, передаем путь и количесво строк
        public static async void WritingToFailsAsync(string writePath, int rows)
        {
            //вызов асинхронной операции
            await Task.Run(() => WritingToFail(writePath, rows)); 

            Console.WriteLine("Запись в методе WritingToFailsAsync окончена");
        }

        //метод для записи строк в файл, передаем путь и количесво строк
        public static void WritingToFail(string writePath, int rows)
        {
            for (int j = 1; j < rows; j++)
            {
                //строка для записи
                //вызываем метод, который генерирует нам рандомную строку
                //передаваемые аргументы инициализированы в классе, 
                //так как их достаточно определить только один раз, а не пересоздавать каждый раз
                string str = RandomString(rnd, alphabet, today, range);
                try
                {
                    //создаем объект StreamWriter для записи в файл 
                    //(writePath - путь, true -  разрешаем новые данные добавлять в конец файла, 
                    //System.Text.Encoding.Default -  кодировка, которая будет применяться при записи)
                    using (StreamWriter sw = new StreamWriter(writePath, true, System.Text.Encoding.Default))
                    {
                        sw.WriteLine(str); //запись в файл
                    }
                }
                //исключения
                catch (Exception e)
                {
                    //выводим текст этого исключения
                    Console.WriteLine(e.Message);
                }
            }
        }

        //метод, который генерирует нам рандомную строку
        //rnd - объект рандомайзера, alphabet - строка латинского алфавита, 
        //today - сегодняшняя дата, range -  количетво дней с сегодняшней даты за последние 5 лет
        public static string RandomString(Random rnd, string alphabet, DateTime today, int range)
        {

            //метод, генерирующий случайное положительное четное целочисленное число в диапазоне от 1 до 100 000 000 
            string intOddNumber = RandomIntOddNumber(rnd, 1, 100000000);
            //метод, генерирующий случайное положительное число с 8 знаками после запятой в диапазоне от 1 до 20
            string doubleNumber = RandomDoubleNumber(rnd, 1, 20);
            //метод, генерирующий случайный набор 10 латинских символов
            string latinString = RandomLatinString(rnd, alphabet, 10);
            //метод, генерирующий случайный набор 10 русских символов 
            string russionString = RandomRussionString(rnd, 10);
            //метод, генерирующий случайную датy за последние 5 лет
            string date = RandomData(rnd, today, range);

            //получаем строку для записи
            string str = String.Concat(date, "||", latinString, "||", russionString, "||", intOddNumber, "||", doubleNumber);

            return str;
        }

        //метод, генерирующий случайное положительное четное целочисленное число в диапазоне от min до max
        public static string RandomIntOddNumber(Random rnd, int min, int max)
        {
            //генерация рандомного числа в диапазоне от min до max
            int rand = rnd.Next(min, max);
            //рандомим число, пока остаток от деление не будет равен не 1, то есть будет равен 0, след. число четное
            while (rand % 2 == 1) // odd
            {
                rand = rnd.Next(min, max);
            }
            //возвращаем строковое предсталение этого числа
            return rand.ToString();
        }

        //метод, генерирующий случайное положительное число с 8 знаками после запятой в диапазоне от min до max
        public static string RandomDoubleNumber(Random rnd, int min, int max)
        {
            //рандомим число с плавающей запятой (оно бует от 0 до 1) умножаем его на (максимальное значение - 1),
            //т.к. мы не можем выйти за пределы max, и нужно начинать с min (поэтому + min)
            double rand = rnd.NextDouble() * (max - 1) + min;
            //приводим полученное число к нужному формату, 8 знаков после запятой
            return string.Format("{0:0.00000000}", rand);
        }

        //метод, генерирующий случайный набор латинских символов, длины length
        public static string RandomLatinString(Random rnd, string alphabet, int length)
        {
            //объект StringBuilder с заранее заданным размером буфера под результирующую строку
            StringBuilder sb = new StringBuilder(length - 1);
            //переменная для хранения случайной позиции символа из строки alphabet
            int position = 0;
            //в цикле генерируем случайную строку
            for(int i = 0; i < length; i++)
            {
                //получаем случайное число от 0 до последнего символа в строке Alphabet
                position = rnd.Next(0, alphabet.Length - 1);
                //добавляем выбранный символ в объект StringBuilder
                sb.Append(alphabet[position]);
            }
            //возвращаем полученную строку, преобразовав ее в string
            return sb.ToString();
        }

        //метод, генерирующий случайный набор русских символов, длины length 
        public static string RandomRussionString(Random rnd, int length)
        {
            //инициализируем пустую строку
            string str = "";
            //в цикле добаляем рандомный символ русского алфавита, преобразованный по Юникоду (10-чный HTML) 
            for (int i = 0; i < length; i++)
            {
                str += (char)rnd.Next(1040, 1104);
            }
            return str;
        }

        //метод, генерирующий случайную датy за последние 5 лет (range - количество дней) с сегодняшнего дня (today)
        public static string RandomData(Random rnd, DateTime today, int range)
        {
            //от сегодняшнего дня отнимаем рандомное количество дней в промежутке от 0 до 5 лет(в днях) 
            //и преобразовываю в нужный формат string
            return today.AddDays(-(rnd.Next(range))).ToString("dd.MM.yyyy");
        }

        //метод для объединения всех файлов в директории \Fails в файл combinedAllFile.txt 
        //с удалением строк с указанной подстрокой
        public static void ConcatenatingAllFilesWithDeletingLines()
        {
            Console.Write("Введите словосочетание символов, строки с которым нужно удалить: ");
            string chars = Console.ReadLine();
            //счетчик количества удаленных строк
            int count = 0;
            //запись в combinedAllFile.txt файлов формата @"*.txt" (заканчивающихся на .txt) из директория Fails
            File.WriteAllLines(@"D:\КУРС\EY\Task1\Fails\combinedAllFile.txt", Directory.GetFiles(@"D:\КУРС\EY\Task1\Fails", @"*.txt")
                //с помощью LINQ выбираем строки в которых не содержится подстрока в строке и увеличиваем счетчик, если содержится
               .SelectMany(file => File.ReadLines(file, Encoding.GetEncoding(1251))).Where(line =>
               {
                   if (line.Contains(chars))
                   {
                       count++;
                       return false;
                   }
                   else
                       return true;
               }).Select(line => line));

            Console.WriteLine($"Количество удленных строк: {count}");
            Console.WriteLine("");
        }

        //метод для объединения конкретных файлов, указанных пользователем
        public static void ConcatenatingSomeFilesWithDeletingLines()
        {
            Console.WriteLine("Введите имена файлов (без .txt) через запятую без пробелов (например, text1,text2,text3): ");
            //записали строку
            StringBuilder a = new StringBuilder(Console.ReadLine());
            //получили массив имен файлов
            string[] names = a.ToString().Split(',');
            //создали массив для путей к этим файлам
            string[] fileNames = new string[names.Length];
            //заполнили его
            for (int i = 0; i < names.Length; i++)
            {
                fileNames[i] = String.Concat(@"D:\КУРС\EY\Task1\Fails\", names[i], ".txt");
            }
            Console.Write("Введите словосочетание символов, строки с которым нужно удалить: ");
            string chars = Console.ReadLine();
            //счетчик количества удаленных строк
            int count = 0;
            //запись в combinedFile.txt файлов из fileNames
            File.WriteAllLines(@"D:\КУРС\EY\Task1\Fails\combinedFile.txt", fileNames
               //с помощью LINQ выбираем строки в которых не содержится подстрока в строке и увеличиваем счетчик, если содержится
               .SelectMany(file => File.ReadLines(file, Encoding.GetEncoding(1251)))
               .Where(line =>
               {
                   if (line.Contains(chars))
                   {
                       count++;
                       return false;
                   }
                   else
                       return true;
               })
               .Select(line => line));

            Console.Write($"Количество удленных строк: {count}");
            Console.WriteLine("");
        }

        //метод для импорта указанного файла в БД
        private static async void SaveFileToDatabaseAsync()
        {
            //строка подключения
            string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=task1db;Integrated Security=True";
           
            //создаем оъек подключения
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                //отрываем подключение
                connection.Open();
                //обект класса, который инкапсулирует sql-выражение, которое должно быть выполнено
                SqlCommand command = new SqlCommand();
                //подключаемся
                command.Connection = connection;
                //задаем действие, которое должно выполниться в бд
                command.CommandText = @"INSERT INTO my_row VALUES (@date_str, @latin_str, @rus_str, @int_str, @double_str)";

                //передается название параметра и тип
                command.Parameters.Add("@date_str", SqlDbType.NVarChar, 30);
                command.Parameters.Add("@latin_str", SqlDbType.NVarChar, 30);
                command.Parameters.Add("@rus_str", SqlDbType.NVarChar, 30);
                command.Parameters.Add("@int_str", SqlDbType.Int);
                command.Parameters.Add("@double_str", SqlDbType.Float);

                //путь к файлу для загрузки

                Console.WriteLine("Введите имя файла, который хотите загрузить (например, text1): ");
                string nameFile = Console.ReadLine();
                //формируем путь к файлу
                string filePath = String.Concat(@"D:\КУРС\EY\Task1\Fails\", nameFile, ".txt");

                //чтение из файла
                using (StreamReader sr = new StreamReader(filePath, Encoding.GetEncoding(1251)))
                {
                    string line;
                    //инициализируем массив разделителей для строки
                    string[] separators = { "|", "|" };
                    //счетчик загруженных строк
                    int count = 0;
                    
                    //читаем все строки
                    while ((line = await sr.ReadLineAsync()) != null)
                    {
                        //разделяем строку по || на массив строк 
                        string[] myline = line.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                        //передаем данные в команду через параметры
                        command.Parameters["@date_str"].Value = myline[0];
                        command.Parameters["@latin_str"].Value = myline[1];
                        command.Parameters["@rus_str"].Value = myline[2];
                        command.Parameters["@int_str"].Value = int.Parse(myline[3]); //приводим к типу int
                        command.Parameters["@double_str"].Value = Math.Round(double.Parse(myline[4]),8); //приводим к double и округляем до 8 знаков после запятой

                        //возвращаем количество задействованных строк
                        command.ExecuteNonQuery();
                        
                        //увеличиваем счетчик для количества загруженных строк
                        count++;
                        Console.Write("Количество загруженных строк: ");
                        Console.Write($"{count} ");
                    }
                    Console.WriteLine("");
                }
            }
        }
        

       
    }
}





