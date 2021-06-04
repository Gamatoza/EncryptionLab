using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace EncryptionLib.DigitalSignature
{
    static class Hash
    {
        static Random rand = new Random();
        public static int GetStringHashCode20(string value)
        {
            int num = 352654597;
            int num2 = num;

            for (int i = 0; i < value.Length; i += 4)
            {
                int ptr0 = value[i] << 16;
                if (i + 1 < value.Length)
                    ptr0 |= value[i + 1];

                num = (num << 5) + num + (num >> 27) ^ ptr0;

                if (i + 2 < value.Length)
                {
                    int ptr1 = value[i + 2] << 16;
                    if (i + 3 < value.Length)
                        ptr1 |= value[i + 3];
                    num2 = (num2 << 5) + num2 + (num2 >> 27) ^ ptr1;
                }
            }

            return num + num2 * 1566083941;
        }

        public static int GetStringHashCode40(string value)
        {
            int num = 5381;
            int num2 = num;
            for (int i = 0; i < value.Length; i += 2)
            {
                num = (((num << 5) + num) ^ value[i]);

                if (i + 1 < value.Length)
                    num2 = (((num2 << 5) + num2) ^ value[i + 1]);
            }
            return num + num2 * 1566083941;
        }


        /*
        RS Hash Function
        A simple hash function from Robert Sedgwicks Algorithms in C book. I've added some simple optimizations to the algorithm in order to speed up its hashing process.
        */
        public static int GetRSHash(string str)
        {
            long b = 378551;
            long a = 63689;
            long hash = 0;
            int i = 0;
            char c = str[0];
            for (i = 0; i < str.Length; i++)
            {
                hash = hash * a + c;
                c = str[i];
                a = a * b;
            }
            return (int)hash;
        }

        public static string GetMD5Hash(string input)
        {
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));

            return Convert.ToBase64String(hash);
        }

        public static int GetSummaryAllHash(string s, string[] array)
        {
            int total = 0;
            char[] c;
            c = s.ToCharArray();
            // Суммируем все значения ASCII
            // каждого алфавита в строке
            for (int k = 0; k <= c.GetUpperBound(0); k++)

                total += (int)c[k];
            return total % array.GetUpperBound(0);

        }

        public static int GetHornerHash(string s, string[] array)
        {
            long total = 0;
            char[] c;
            c = s.ToCharArray();
            // Правило Хорнера для генерации полинома
            // из 11 используя значения символов ASCII
            for (int k = 0; k <= c.GetUpperBound(0); k++)
                total += 11 * total + (int)c[k];
            total = total % array.GetUpperBound(0);
            if (total < 0)

                total += array.GetUpperBound(0);
            return (int)total;

        }

        public static char[] ru_characters { get; } = new char[] { 'А', 'Б', 'В', 'Г', 'Д', 'Е', 'Ё', 'Ж', 'З', 'И',
                                            'Й', 'К', 'Л', 'М', 'Н', 'О', 'П', 'Р', 'С',
                                            'Т', 'У', 'Ф', 'Х', 'Ц', 'Ч', 'Ш', 'Щ', 'Ь', 'Ы', 'Ъ',
                                            'Э', 'Ю', 'Я'}; 
        public static char[] en_characters { get; } = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K',
                                            'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T',
                                            'U', 'V', 'W', 'X', 'Y', 'Z', ' ',
                                            '1', '2', '3', '4', '5', '6', '7', '8', '9', '0'};
        public static char[] special_characters { get; } = new char[] { '#', ',', '.', '!', '?', ':', ';', '\'', '"', '/', '\\', ' ' };
        public static char[] numbers { get; } = new char[] { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' };

        public static char[] arrayOut { get; } = ru_characters.Concat(en_characters).Concat(special_characters).Concat(numbers).ToArray(); //or Union


        public static int getLightHash(string s, int n = 1) //n for mod
        {
            //n = 391;
            int f = 0;
            s = s.ToUpper();
            int k = rand.Next(100,666); //произвольное начальное значение
            for (int i = 1; i < s.Length; i++)
            {
                for (int j = 0; j < arrayOut.Length; j++)
                {
                    if (arrayOut[j] == s[i])
                    {
                        f = j + 1;
                    }
                }
                k = Convert.ToInt32(Math.Pow(k + f, 2) % n); // (Hi−1 + Mi)^2 mod n,
            }
            return k;
        }
    }
}
