using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using EncryptionLib.SyncCrypt;

namespace EncryptionLib.DigitalSignature
{
    public class lighDS_RSA //LightDigitalSignature_RSA
    {

        char[] characters = new char[] { '#', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '-' };

        private long p;

        public long P
        {
            get { return p; }
            set { p = value; }
        }

        private long q;

        public long Q
        {
            get { return q; }
            set { q = value; }
        }

        private long d;

        public long D
        {
            get { return d; }
            set { d = value; }
        }

        private long e;

        public long E
        {
            get { return e; }
            set { e = value; }
        }


        public lighDS_RSA(long p, long q)
        {
            this.p = p;
            this.q = q;
        }

        public string GenerateKey(string input_path,string output_path)
        {

            //if (isPrime(p) && isPrime(q))
            if (isPrime_Ferm(p) && isPrime_Ferm(q))
            {
                string hash = File.ReadAllText(input_path).GetHashCode().ToString();

                long n = p * q;
                long m = (p - 1) * (q - 1);
                long d = D = Calculate_d(m);
                long e = E = Calculate_e(d, m);

                List<string> result = RSA_Endoce(hash, e, n);

                string res = "";
                StreamWriter sw = new StreamWriter(output_path);
                foreach (string item in result) {
                    sw.WriteLine(item);
                    res += item;
                }
                sw.Close();


                return res;
            }
            else
                throw new Exception("p or q is not simple!");
        }

        public bool CheckFile(string file_path, long d, long n)
        {
            List<string> input = new List<string>();

            StreamReader sr = new StreamReader(file_path);

            while (!sr.EndOfStream)
            {
                input.Add(sr.ReadLine());
            }

            sr.Close();

            string result = RSA_Dedoce(input, d, n);

            string hash = File.ReadAllText(file_path).GetHashCode().ToString();

            if (result.Equals(hash))
                return true;
            else
                return false;
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

        private bool isPrime_RabinMiller(int n, int k)
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

        //для охренительно больших чисел
        private bool isProbablePrime(BigInteger source, int certainty)
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

            // There is no built-in method for generating random BigInteger values.
            // Instead, random BigIntegers are constructed from randomly generated
            // byte arrays of the same length as the source.
            RandomNumberGenerator rng = RandomNumberGenerator.Create();
            byte[] bytes = new byte[source.ToByteArray().LongLength];
            BigInteger a;

            for (int i = 0; i < certainty; i++)
            {
                do
                {
                    // This may raise an exception in Mono 2.10.8 and earlier.
                    // http://bugzilla.xamarin.com/show_bug.cgi?id=2761
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

        //быстрое возведение в степень
        public static BigInteger fast(BigInteger a, BigInteger r)
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

        //тест ферма
        private Boolean isPrime_Ferm(BigInteger num)
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

        //зашифровать
        private List<string> RSA_Endoce(string s, long e, long n)
        {
            List<string> result = new List<string>();

            BigInteger bi;

            for (int i = 0; i < s.Length; i++)
            {
                int index = Array.IndexOf(characters, s[i]);

                bi = new BigInteger(index);
                bi = fast(bi, e);//BitInteger.Pow

                BigInteger n_ = new BigInteger((int)n);

                bi = bi % n_;

                result.Add(bi.ToString());
            }

            return result;
        }

        //расшифровать
        private string RSA_Dedoce(List<string> input, long d, long n)
        {
            string result = "";

            BigInteger bi;

            foreach (string item in input)
            {
                bi = new BigInteger(Convert.ToDouble(item));
                bi = fast(bi, d);//BitInteger.Pow

                BigInteger n_ = new BigInteger((int)n);

                bi = bi % n_;

                int index = Convert.ToInt32(bi.ToString());

                result += characters[index].ToString();
            }

            return result;
        }

        //вычисление параметра d. d должно быть взаимно простым с m
        private long Calculate_d(long m)
        {
            long d = 2;

            for (long i = 2; i <= m; i++)
                if ((m % i == 0) && (d % i == 0)) //если имеют общие делители
                {
                    d++;
                    i = 1;
                }

            return d;
        }

        //вычисление параметра e
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
    }
}
