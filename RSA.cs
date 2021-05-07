using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;

namespace EncryptionLib
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
    class RSA:ICryptoBasic
    {
        char[] ru_characters = new char[] { '#', 'А', 'Б', 'В', 'Г', 'Д', 'Е', 'Ё', 'Ж', 'З', 'И',
                                            'Й', 'К', 'Л', 'М', 'Н', 'О', 'П', 'Р', 'С',
                                            'Т', 'У', 'Ф', 'Х', 'Ц', 'Ч', 'Ш', 'Щ', 'Ь', 'Ы', 'Ъ',
                                            'Э', 'Ю', 'Я', ' ', '1', '2', '3', '4', '5', '6', '7',
                                            '8', '9', '0' };
        char[] en_characters = new char[] { '#', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K',
                                            'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T',
                                            'U', 'V', 'W', 'X', 'Y', 'Z', ' ', '1', '2', '3', '4', '5', '6', '7',
                                            '8', '9', '0' };


        public string Key { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Input { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public string Extra => throw new NotImplementedException();

        public string Encode()
        {
            long p = 0, q = 0; //реализовать p и q

            if (isSimple(p) && isSimple(q))
            {
                string s = "";

                StreamReader sr = new StreamReader("in.txt");

                while (!sr.EndOfStream)
                {
                    s += sr.ReadLine();
                }

                sr.Close();

                s = s.ToUpper();

                long n = p * q;
                long m = (p - 1) * (q - 1);
                long d = Calculate_d(m);
                long e_ = Calculate_e(d, m);

                List<string> result = RSA_Endoce(s, e_, n);
                string res = "";

                using (StreamWriter sw = new StreamWriter("out1.txt"))
                {
                    foreach (string item in result)
                    {
                        res += item + '\n';
                        sw.WriteLine(item);
                    }
                    sw.Close();
                }

                //передача d и n

                Process.Start("out1.txt");
                return res;
            }
            else
                throw new Exception("the p or q is not simple");
        }

        public string Decode()
        {
            long d = 0, n = 0; //получение

            List<string> input = new List<string>();

            StreamReader sr = new StreamReader("out1.txt");

            while (!sr.EndOfStream)
            {
                input.Add(sr.ReadLine());
            }

            sr.Close();

            string result = RSA_Dedoce(input, d, n);

            StreamWriter sw = new StreamWriter("out2.txt");
            sw.WriteLine(result);
            sw.Close();

            Process.Start("out2.txt");

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
        int Euqlid()
        {
            return 0;
        }

        private long Calculate_d(long m)
        {
            long d = m - 1;

            for (long i = 2; i <= m; i++)
                if ((m % i == 0) && (d % i == 0)) //если имеют общие делители
                {
                    d--;
                    i = 1;
                }

            return d;
        }

        private long Calculate_e(long d, long m)
        {
            long e = 10;

            while (true)
            {
                if ((e * d) % m == 1)
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

                bi = new BigInteger(index);
                bi = BigInteger.Pow(bi, (int)e);

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
                bi = new BigInteger(Convert.ToDouble(item));
                bi = BigInteger.Pow(bi, (int)d);

                BigInteger n_ = new BigInteger((int)n);

                bi = bi % n_;

                int index = Convert.ToInt32(bi.ToString());

                result += en_characters[index].ToString();
            }

            return result;
        }

    }
}
