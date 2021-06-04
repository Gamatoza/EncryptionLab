using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace EncryptionLib.SyncCrypt
{

    //TODO: Реализовать проверку на простоту числа тестами Ферма и Миллера-Рабина
    //TODO: 

    /*
         Алгоритм RSA состоит из следующих пунктов:
            Выбрать простые числа p и q
            Вычислить n = p * q
            Вычислить m = (p - 1) * (q - 1)
            Выбрать число d взаимно простое с m
            Выбрать число e так, чтобы e * d = 1 (mod m)
         
         */
    public class RSA
    {
        static char[] ru_characters = new char[] { 'А', 'Б', 'В', 'Г', 'Д', 'Е', 'Ё', 'Ж', 'З', 'И',
                                            'Й', 'К', 'Л', 'М', 'Н', 'О', 'П', 'Р', 'С',
                                            'Т', 'У', 'Ф', 'Х', 'Ц', 'Ч', 'Ш', 'Щ', 'Ь', 'Ы', 'Ъ',
                                            'Э', 'Ю', 'Я'};
        static char[] en_characters = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K',
                                            'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T',
                                            'U', 'V', 'W', 'X', 'Y', 'Z', ' ', 
                                            '1', '2', '3', '4', '5', '6', '7', '8', '9', '0'};
        static char[] special_characters = new char[] { '#', ',', '.', '!', '?', ':', ';', '\'', '"', '/', '\\', ' ' };
        static char[] numbers = new char[] { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' };

        static char[] arrayOut = ru_characters.Concat(en_characters).Concat(special_characters).Concat(numbers).ToArray(); //or Union


        public RSA(long p, long q)
        {
            this.p = p;
            this.q = q;
            UpdateArgs();
        }

        void UpdateArgs()
        {
            N = P * Q;
            M = (P - 1) * (Q - 1);
            D = Calculate_d(M);//Calculate_dNOD(M); только с малыми простыми числами, ибо это рекурсивно
            E = Calculate_e(D, M);
        }

        long p;
        public long P
        {
            get { return q; }
            set {
                p = value;
                UpdateArgs();
            }
        }
        long q;
        public long Q 
        {
            get { return q; }
            set {
                q = value;
                UpdateArgs();
            }
        }
        public long N { get; private set; }
        public long M { get; private set; }
        public long D { get; private set; }
        public long E { get; private set; }

        /// <summary>
        /// Внешняя оболочка для EncodeRSA
        /// </summary>
        /// <param name="input_path"></param>
        /// <param name="output_path"></param>
        /// <returns></returns>
        public string Encode(string input_path = "in.txt", string output_path = "out.txt",bool useAskii = false)
        {

            if (isPrime(P) && isPrime(Q))
            {
                string s = "";

                StreamReader sr = new StreamReader(input_path);

                while (!sr.EndOfStream)
                {
                    s += sr.ReadLine();
                }

                sr.Close();

                s = s.ToUpper();

                List<string> result = RSA_Endoce(s, E, N, useAskii);
                string res = "";

                using (StreamWriter sw = new StreamWriter(output_path))
                {
                    foreach (string item in result)
                    {
                        res += item + '\n';
                        sw.WriteLine(item);
                    }
                    sw.Close();
                }

                //передача d и n

                return res;
            }
            else
                throw new Exception("the p or q is not simple");
        }

        public string Decode(string input_path = "in.txt", string output_path = "out.txt", bool useAskii = false)
        {

            List<string> input = new List<string>();

            StreamReader sr = new StreamReader(input_path);

            while (!sr.EndOfStream)
            {
                input.Add(sr.ReadLine());
            }

            sr.Close();

            string result = RSA_Dedoce(input, D, N, useAskii);

            StreamWriter sw = new StreamWriter(output_path);
            sw.WriteLine(result);
            sw.Close();

            return result;
        }

        //Быстрое возведение в степень – это алгоритм, который позволяет возвести любое число в натуральную степень 
        //за сокращенное количество умножений.
        /*
         
         Для любого числа x и четной степени n выполняется тождество:
         x^n = (x^(n/2))^2 = x^(n/2) ⋅ x^(n/2)
        Это и является основой алгоритма быстрого возведения в степень. 
        Поскольку такое разбиение позволяет, за одну операцию умножения, вдвое уменьшить вычисляемую степень.
        Для случая нечетной степени, достаточно её понизить на единицу:

        x^n = x^n - 1 ⋅ x, при этом (n - 1) четное число.
         */
        BigInteger FastPow(BigInteger x, int n)
        {
            BigInteger result = 1L;
            while (n > 0)
            {
                if ((n & 1) == 0)
                {
                    x *= x;
                    n >>= 1;
                }
                else
                {
                    result *= x;
                    --n;
                }
            }
            return result;
        }

        //быстрое возведение в степень
        public static BigInteger FastPow2(BigInteger a, BigInteger r)
        {
            BigInteger a1 = a;
            BigInteger z1 = r;
            BigInteger x = 1;
            while (z1 != 0)
            {
                while (z1 % 2 == 0)
                {
                    z1 /= 2;
                    a1 = (a1 * a1);
                }
                z1 -= 1;
                x = (x * a1);
            }
            return x;
        }

        //проверка: простое ли число?
        private bool isPrime(long n)
        {
            if (n < 2)
                return false;

            if (n == 2)
                return true;

            for (long i = 2; i < n; i++)
                if (n % i == 0)
                    return false;

            return true;
        }

        public static bool isPrime_RabinMiller(int n, int k)
        {
            if ((n < 2) || (n % 2 == 0)) return (n == 2);

            int s = n - 1;
            while (s % 2 == 0) s >>= 1;

            Random r = new Random();
            for (int i = 0; i < k; i++)
            {
                int a = r.Next(n - 1) + 1;
                int temp = s;
                long mod = 1;
                for (int j = 0; j < temp; ++j) mod = (mod * a) % n;
                while (temp != n - 1 && mod != 1 && mod != n - 1)
                {
                    mod = (mod * mod) % n;
                    temp *= 2;
                }

                if (mod != n - 1 && temp % 2 == 0) return false;
            }
            return true;
        }

        //Миллера Робина для еще больших проверочных проверок
        public static bool isProbablePrime(BigInteger source, int certainty) //число и количество проверок
        {
            if (source == 2 || source == 3)
                return true;
            if (source < 2 || source % 2 == 0)
                return false;

            BigInteger d = source - 1;
            int s = 0;

            while (d % 2 == 0)
            {
                d /= 2;
                s += 1;
            }

            RandomNumberGenerator rng = RandomNumberGenerator.Create();
            byte[] bytes = new byte[source.ToByteArray().LongLength];
            BigInteger a;

            for (int i = 0; i < certainty; i++)
            {
                do
                {
                    rng.GetBytes(bytes);
                    a = new BigInteger(bytes);
                }
                while (a < 2 || a >= source - 2);

                BigInteger x = BigInteger.ModPow(a, d, source);
                if (x == 1 || x == source - 1)
                    continue;

                for (int r = 1; r < s; r++)
                {
                    x = BigInteger.ModPow(x, 2, source);
                    if (x == 1)
                        return false;
                    if (x == source - 1)
                        break;
                }

                if (x != source - 1)
                    return false;
            }

            return true;
        }

        //тест ферма
        public static bool isPrime_Ferm(BigInteger num)
        {
            for (int i = 2; i <= 100; i++) //Сто попыток, меняем основание.
            {
                var n = BigInteger.ModPow(i, num - 1, num);
                if (n != 1) //Если нечетное, то точно непростое.
                {
                    return false;
                }

            }
            return true;   //Вероятно простое.            
        }

        /*
         Алгоритм Евклида
            Исходные числа a и b
            Вычислить r - остаток от деления a на b: a = b * q + r
            Если r = 0, то b - искомое число (наибольший общий делитель), конец
            Заменить пару чисел <a, b> парой <b, r>, перейти к пункту 2
         */
        int gcd(int val1, int val2)
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

        private long Calculate_d_gcdmod(long m)
        {
            long d = m - 1;
            for (long i = 2; i <= m; i++)
                if (gcd((int)d, (int)m) !=0) //Выбрать число d взаимно простое с m
                {
                    d--;
                    i = 1;
                }
            return d;
        }

        private long Calculate_d(long m)
        {
            long d = 0;
            for (long i = 2; i <= m; i++)
                if ((m % i == 0) && (d % i == 0)) //Выбрать число d взаимно простое с m   
                {
                    d++;
                    i = 1;
                }
            return d;
        }

        //not so recomeded, coz recursy
        private long Calculate_dNOD(long m)
        {
            long d = m - 1;
            for (int i = 0; i <= m; i++)
            {
                if (NOD(m, d) == 1)
                    return d;
                else d--;
            }
            return -1;
        }

        private long NOD(long a, long b)
        {
            if (a == b)
                return a;
            else
                if (a > b)
                return NOD(a - b, b);
            else
                return NOD(b - a, a);
        }

        private long Calculate_e(long d, long m)
        {
            long e = 0;

            while (true)
            {
                if (e * d % m == 1) //Выбрать число e так, чтобы e* d = 1(mod m)
                    break;
                else
                    e++;
            }

            return e;
        }

        private List<string> RSA_Endoce(string s, long e, long n, bool useAskii = false)
        {
            List<string> result = new List<string>();

            BigInteger bi;

            for (int i = 0; i < s.Length; i++)
            {

                int index;
                if (useAskii) index = s[i];
                else index = Array.IndexOf(arrayOut, s[i]);
                if (index == -1) throw new Exception("There is no letter");
                char buf = s[i];
                bi = new BigInteger(index);
                bi = FastPow2(bi, e); //BigInteger.Pow(bi, (int)e); //

                BigInteger n_ = new BigInteger((int)n);

                bi = bi % n_;

                result.Add(bi.ToString());
            }

            return result;
        }

        private string RSA_Dedoce(List<string> input, long d, long n, bool useAskii = false)
        {
            string result = "";

            BigInteger bi;

            foreach (string item in input)
            {
                bi = new BigInteger(Convert.ToInt32(item));
                bi = FastPow2(bi, (int)d); //BigInteger.Pow(bi, (int)d);   //

                BigInteger n_ = new BigInteger((int)n);

                bi = bi % n_;

                int index = Convert.ToInt32(bi.ToString());
                if (useAskii) result += ((char)index).ToString();
                else result += arrayOut[index].ToString();

            }

            return result;
        }

    }
}
