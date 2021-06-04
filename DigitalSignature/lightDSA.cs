using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace EncryptionLib.DigitalSignature
{
    public class lightDSA
    {
        static Random rand = new Random();

        private static int q;
        private static int p;
        private static double g;
        private static string message;
        private static int hash;


        //secret key
        private static int x;

        //public key
        private static int y;
        public lightDSA(string Message)
        {
            //INIT
            message = Message;
            hash = Hash.getLightHash(Message);
            int sum = 0;

            for (int i = 0; i < message.Length; i++)
            {
                sum += message[i];
            }
            q = sum; //Выбор простого числа 
            p = q + 1;  //Выбор простого числа p, такого, что (p-1) делится на q.
            while (true)
            {
                bool f = false;
                for (int i = 2; i < p - 1; i++)
                {
                    if (p % i == 0)
                    {
                        f = true;
                        break;
                    }
                }
                if (!f && (p - 1) % q == 0)
                {
                    break;
                }
                else
                {
                    p++;
                }
            }
            g = 0; //Выбор числа g такого, что его мультипликативный порядок по модулю p равен q. 
            while (g < 1) //по сути функция долна быть != 1, но там так выходит что одна херня, а так хотя бы минус не будет
            {
                g = Math.Pow(rand.Next(1, p - 1), (p - 1) / q); //maybe use double???
            } //В большинстве случаев значение rand = 2 удовлетворяет этому требованию


            //


            //get private
            x = rand.Next(q);

            //get public
            //BigInteger bi = FastPow(g, x);
            //bi = bi % p;
            y = fast(Convert.ToInt32(g), x, p); //int.Parse(bi.ToString());
        }

        public string GetSignature()
        {
            int s = 0;
            int r = 0;

            while (true)
            {
                int k = rand.Next(0, q);
                r = fast(Convert.ToInt32(g), k, p) % q;
                int k1 = 0;
                while ((k1 * k) % q != 1)
                {
                    k1++;
                }
                s = Convert.ToInt32(k1 * (hash + x * r)) % q;
                if (r != 0 || s != 0)
                {
                    return r.ToString() + "," + s.ToString();
                }
            }
        }
        private bool isPrime(int num)
        {
            for (int i = 2; i <= 100; i++) //Сто попыток, меняем основание.
            {
                var n = BigInteger.ModPow(i, num - 1, num);//fast(i, num - 1, num);
                if (n != 1) //Если нечетное, то точно непростое.
                {
                    return false;
                }

            }
            return true;   //Вероятно простое.            
        }

        public bool CheckSignature(string message, string pair)
        {
            int hash = Hash.getLightHash(message);
            string[] mas = pair.Split(',');
            int r = int.Parse(mas[0]);
            int s = int.Parse(mas[1]);
            int s1 = 0;
            while ((s1 * s) % q != 1) //малая теорема ферма
            {
                s1++;
            }
            int w = s1 % q;
            int u1 = (hash * w) % q;
            int u2 = (r * w) % q;
            double mp1 = fast(Convert.ToInt32(g), u1, p);
            double mp2 = fast(y, u2, p);
            double res = mp1 * mp2;
            res %= p;
            res %= q;
            int v = Convert.ToInt32(res);
            return v == r;
        }


        //fast pow with mod included
        public static int fast(int a, int r, int n)
        {
            int a1 = a;
            int z1 = r;
            int x = 1;
            while (z1 != 0)
            {
                while (z1 % 2 == 0)
                {
                    z1 /= 2;
                    a1 = (a1 * a1) % n;
                }
                z1 -= 1;
                x = (x * a1) % n;
            }
            return x;
        }
    }
}
