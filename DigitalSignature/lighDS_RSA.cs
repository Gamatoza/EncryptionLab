using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
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

            if (IsTheNumberSimple(p) && IsTheNumberSimple(q))
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
        private bool IsTheNumberSimple(long n)
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

        //зашифровать
        private List<string> RSA_Endoce(string s, long e, long n)
        {
            List<string> result = new List<string>();

            BigInteger bi;

            for (int i = 0; i < s.Length; i++)
            {
                int index = Array.IndexOf(characters, s[i]);

                bi = new BigInteger(index);
                bi = BigInteger.Pow(bi, (int)e);

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
                bi = BigInteger.Pow(bi, (int)d);

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
