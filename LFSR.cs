using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EncryptionLib.LFSR
{
    public class LFSR
    {
        int key;                                 // какой то рандомный ключ
        int keylen;
        string path;
        byte[] input;                               // текст
        int inLength;

        string formula;                             // формула по которой происходит ксор //формат формулы: x^27 + x^8 + x^7 + x + 1
        int[] points;                               // номера битов для ксора
        int maxptr = -1;                            // маскимальное число из номеров, определяет разрядность

        int[] output;

        int Depth  //определяет разрядность с которой мы будем работать //использовал раньше, оставил на случай
        {
            get {
                if (maxptr >= 64) return  64;           
                else if (maxptr >= 32) return  32;
                else if (maxptr >= 16) return 16;
                else return 8;
            }
        }

        int[] intkey;                                // ключ, преобразованый по формуле в длинный ключ размером с текст

        Byte[] FileIntoBites(string path) 
            => System.IO.File.ReadAllBytes(@"" + path);
        Byte[] TextIntoBites(string text)
            => System.Text.Encoding.Default.GetBytes(text);

        void ParseFormula() { }                     // преобразует строковую формулу в номера


        public int[] TransmuteText(string text)
        {
            input = TextIntoBites(text);
            inLength = input.Length;

            Longinium();

            output = new int[inLength];
            for (int i = 0; i < inLength; i++)
            {
                output[i] = (int)(input[i] ^ intkey[i]);
            }

            return output;
        }

        public int[] TransmuteFile(string path)
        {
            input = FileIntoBites(path);
            inLength = input.Length;

            Longinium();

            output = new int[inLength];
            for (int i = 0; i < inLength; i++)
            {
                output[i] = (int)(input[i] ^ intkey[i]);
            }

            return output;
        }

        public LFSR(string key, params int[] points)
        {
            this.key = Convert.ToInt32(key,2);
            keylen = key.Length;

            this.points = points;
            maxptr = points.Max();
        }

        void Longinium() //lol //создает длинный ключ для xora вместе с битами
        {
            intkey = new int[inLength];
            intkey[0] = key;
            int next = XORnShift(key);
            for (int i = 1; i < inLength; i++)
            {
                intkey[i] = next;
                next = XORnShift(next);
            }
        }
        
        int XORnShift(int word)
        {
            int high = (int)(word >> keylen); //получает старший бит
            
            //жуткие костыли
            //работа через строку
            //исправить как только возможно исправить

            string str = Convert.ToString(word,2); //преобразует число в строку байт // 9 = 1001
            int[] bites = new int[points.Length]; //создает массив точек
            for (int i = 0; i < points.Length; i++)
            {
                //берет по номеру функции бит в последовательности битов из word
                bites[i] = int.Parse(str[points[i]].ToString()); //еще большие костыли
            }
            
            //ксорит с полученными битами
            for (int i = 0; i < points.Length; i++)
            {
                high ^= bites[i];
            }

            //еще чутка костылей
            //убирает первый бит и добавляет тот, что мы заксорили
            str = str.Substring(1);
            str += high.ToString();
            
            return Convert.ToInt32(str,2); //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!убирает нули в начале, так что надо всегда работать со строкой, а не с числом, бляд
        }


       //переделать
    }
}
