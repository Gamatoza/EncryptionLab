using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
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
        char[] ru_characters = new char[] { '#', 'А', 'Б', 'В', 'Г', 'Д', 'Е', 'Ё', 'Ж', 'З', 'И',
                                            'Й', 'К', 'Л', 'М', 'Н', 'О', 'П', 'Р', 'С',
                                            'Т', 'У', 'Ф', 'Х', 'Ц', 'Ч', 'Ш', 'Щ', 'Ь', 'Ы', 'Ъ',
                                            'Э', 'Ю', 'Я', ' ', 
                                            '1', '2', '3', '4', '5', '6', '7','8', '9', '0', 
                                            ',', '.', '!', '?', ':', ';', '\'','"', '/','\\' };
        char[] en_characters = new char[] { '#', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K',
                                            'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T',
                                            'U', 'V', 'W', 'X', 'Y', 'Z', ' ', 
                                            '1', '2', '3', '4', '5', '6', '7', '8', '9', '0',
                                            ',', '.', '!', '?', ':', ';', '\'','"', '/','\\' };

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
        public string Encode(string input_path = "in.txt", string output_path = "out.txt")
        {

            if (isSimple(P) && isSimple(Q))
            {
                string s = "";

                StreamReader sr = new StreamReader(input_path);

                while (!sr.EndOfStream)
                {
                    s += sr.ReadLine();
                }

                sr.Close();

                s = s.ToUpper();

                List<string> result = RSA_Endoce(s, E, N);
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

        public string Decode(string input_path = "in.txt", string output_path = "out.txt")
        {

            List<string> input = new List<string>();

            StreamReader sr = new StreamReader(input_path);

            while (!sr.EndOfStream)
            {
                input.Add(sr.ReadLine());
            }

            sr.Close();

            string result = RSA_Dedoce(input, D, N);

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

        /*
         Алгоритм поиска простых чисел
            N - нечетное число. Найти s и t, удовлетворяющие уравнению: N - 1 = 2s * t
            Случайным образом выбрать число a, 1 < a < N
            Если N делится на a, перейти к пункту 6
            Если условие at = 1 (mod N) выполняется, перейти к пункту 2
            Если найдется такое k, 0 <= k < s, что a2k * t = -1 (mod N), перейти к пункту 2
            Число N - составное: выбрать другое нечетное число N, перейти к пункту 1 
         */
        //Простая проверка на простоту числа
        bool isSimple(long n) //пока простая реализация
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

        private List<string> RSA_Endoce(string s, long e, long n)
        {
            List<string> result = new List<string>();

            BigInteger bi;

            for (int i = 0; i < s.Length; i++)
            {
                int index = Array.IndexOf(en_characters, s[i]);
                char buf = s[i];
                bi = new BigInteger(index);
                if (index == -1) throw new Exception("There is no letter");
                bi = BigInteger.Pow(bi, (int)e); //FastPow(bi, (int)e); //

                BigInteger n_ = new BigInteger((int)n);

                bi = bi % n_;

                result.Add(bi.ToString());
            }

            return result;
        }

        private string RSA_Dedoce(List<string> input, long d, long n)
        {
            string result = "";

            BigInteger bi;

            foreach (string item in input)
            {
                bi = new BigInteger(Convert.ToInt32(item));
                bi = BigInteger.Pow(bi, (int)d); //FastPow(bi, (int)d);   //

                BigInteger n_ = new BigInteger((int)n);

                bi = bi % n_;

                int index = Convert.ToInt32(bi.ToString());

                result += en_characters[index].ToString();
            }

            return result;
        }

    }
}
