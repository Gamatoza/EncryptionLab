using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;

namespace EncryptionLib.SyncCrypt
{
    public class ElGamal
    {
        /*
        Первый этап алгоритма Эль-Гамаля заключается в генерации ключей. Этот этап включает следующую последовательность действий:
        
        Генерируется случайное простое число p длины n бит.
        Выбирается произвольное целое число a, являющееся первообразным (примитивным) корнем по модулю p.
        Выбирается случайное число x из интервала (1,p), взаимно простое с p-1.
        Вычисляется y = a x(mod p).
        Открытым ключом является тройка (a, p ,y) , закрытым ключом — число x.
        */

        Random rand = new Random();

        //Public key

        private int p;

        public int P
        {
            get { return p; }
            set { p = value; }
        }

        private int a;

        public int A
        {
            get { return a; }
            set { a = value; }
        }

        private int y;

        public int Y
        {
            get { return y; }
            set { y = value; }
        }


        //Private key
        private int x;

        public int X
        {
            get { return x; }
            set { x = value; }
        }


        public void SetArgs(int p, int a, int y, int x)
        {
            P = p; A = a; Y = y; X = x;
        }

        public string Encode(string input_path = "in.txt", string output_path = "out.txt")
        {
            //текст
            string text = File.ReadAllText(input_path).ToUpper();

            //переменные

            //я предположил что n бит это колво бит в тексте
            do
            {
                P = rand.Next(1, Encoding.Default.GetBytes(text).Length); //Генерируется случайное простое число p длины n бит.
                A = GFG.findPrimitive(P); //Выбирается произвольное целое число a, являющееся первообразным (примитивным) корнем по модулю p.
            }
            while (A != -1);
            X = 2; //Выбирается случайное число x из интервала(1,p), взаимно простое с p-1.
            for (int i = 0; i < P; i++)
            {
                if (euclidNOD(X, P) == 1)
                {
                    break;
                }
                else x++;
            }

            BigInteger bi = BigInteger.Pow(A, X);
            bi = bi % P;
            Y = Convert.ToInt32(bi.ToString());

            // Шифрование
            
            int k = 2; //Выбирается случайное секретное число k, взаимно простое с p − 1.
            for (int i = 0; i < P; i++)
            {
                if (euclidNOD(k, P-1) == 1)
                {
                    break;
                }
                else k++;
            }

            BigInteger first;
            BigInteger second;
            string res = "";
            //Вычисляется γ = a^k(mod p),
            first = BigInteger.Pow(A, k);
            first = first % P;
            foreach (char item in text)
            {
                //Вычисляется δ = M * y^k(mod p),
                second = BigInteger.Pow(Y, k);
                second *= item; //заменить на число из встроенного алфавита как в рса, если все будет ломаться из-за больших чисел
                second = second % P; //поменять местами с модом если что то идет не так, я не уверен в очередности

                //Пара чисел (γ,δ) является шифртекстом
                res += Convert.ToInt32(first.ToString()) + "," + Convert.ToInt32(second.ToString()) + "\n";
            }
            
            File.WriteAllText(output_path, res);

            return res;
        }

        struct Pair<T>
        {
            public T first { get; set; }
            public T second { get; set; }

            public Pair(T first, T second)
            {
                this.first = first;
                this.second = second;
            }

            public Pair(T[] arr)
            {
                first = arr[0];
                second = arr[1];
            }
            
        }
        public string Decode(string input_path = "in.txt", string output_path = "out.txt")
        {
            List<Pair<string>> input = new List<Pair<string>>();

            StreamReader sr = new StreamReader(input_path);

            while (!sr.EndOfStream)
            {
                input.Add(new Pair<string>(sr.ReadLine().Split(',')));
            }

            sr.Close();

            string res = "";
            foreach (var item in input)
            {
                BigInteger first = BigInteger.Parse(item.first);
                BigInteger second = BigInteger.Parse(item.second);

                BigInteger M = BigInteger.Pow(first, P-1-X);
                M = M * second;
                M = M % P;
                res += ((char)M).ToString();
                //M = (δ * γ^p-1-x)(mod p)
                //M = γ^-x * δ (mod p) XXXXXXXXXXXX
            }

            File.WriteAllText(output_path,res);

            return res;
        }

        int euclidNOD(int val1, int val2)
        {
            /*if (b == 0)
                return a;
            else
                return gcd(b, a % b);*/
            while ((val1 != 0) && (val2 != 0))
            {
                if (val1 > val2)
                    val1 %= val2;
                else
                    val2 %= val1;
            }
            return Math.Max(val1, val2);
        }
    }

    public class GFG //:Rajput-Ji
    {
        // Возвращает true, если n простое
        static bool isPrime(int n)
        {
            // Угловые шкафы
            if (n <= 1)
            {
                return false;
            }
            if (n <= 3)
            {
                return true;
            }
            // Это проверено, чтобы мы могли пропустить
            // средние пять чисел в нижнем цикле
            if (n % 2 == 0 || n % 3 == 0)
            {
                return false;
            }
            for (int i = 5; i * i <= n; i = i + 6)
            {
                if (n % i == 0 || n % (i + 2) == 0)
                {
                    return false;
                }
            }
            return true;
        }

        /* Итеративная функция для вычисления(x ^ n)% p в
        O(логи) */
        static int power(int x, int y, int p)
        {
            int res = 1;     // Инициализировать 
            x = x % p; // Обновление x, если оно больше или
            // равно p
            while (y > 0)
            {
                // Если y нечетно, умножаем x на результат
                if (y % 2 == 1)
                {
                    res = (res * x) % p;
                }
                // у должен быть даже сейчас
                y = y >> 1; // y = y / 2
                x = (x * x) % p;
            }
            return res;
        }

        // Полезная функция для хранения простых множителей числа
        public static void findPrimefactors(HashSet<int> s, int n)
        {
            // Вывести число 2, которые делят n
            while (n % 2 == 0)
            {
                s.Add(2);
                n = n / 2;
            }
            // n должно быть нечетным в этой точке. Так что мы можем пропустить
            // один элемент (примечание i = i +2)
            for (int i = 3; i <= Math.Sqrt(n); i = i + 2)
            {
                // Пока я делю n, выводим i и делим n
                while (n % i == 0)
                {
                    s.Add(i);
                    n = n / i;
                }
            }
            // Это условие для обработки случая, когда
            // n простое число больше 2
            if (n > 2)
            {
                s.Add(n);
            }
        }
        // Функция для поиска наименьшего примитивного корня из n
        public static int findPrimitive(int n)
        {
            HashSet<int> s = new HashSet<int>();
            // Проверяем, является ли n простым или нет
            if (isPrime(n) == false)
            {
                return -1;
            }
            // Найти значение функции Эйлера Totient n
            // Поскольку n - простое число, значение Эйлера
            // Субъект функции n-1, так как существует n-1
            // относительно простых чисел.
            int phi = n - 1;
            // Находим простые факторы фи и храним в наборе
            findPrimefactors(s, phi);
            // Проверяем каждое число от 2 до фи
            for (int r = 2; r <= phi; r++)
            {
                // Перебираем все простые факторы фи.
                // и проверяем, нашли ли мы силу со значением 1
                bool flag = false;
                foreach (int a in s)
                {
                    // Проверка, если r ^ ((phi) / primefactors) mod n
                    // равен 1 или нет
                    if (power(r, phi / (a), n) == 1)
                    {
                        flag = true;
                        break;
                    }
                }
                // Если не было мощности со значением 1.
                if (flag == false)
                {
                    return r;
                }
            }
            // Если первичный корень не найден
            return -1;
        }
    }
}