using System;

namespace EncryptionLib
{
    /// <summary>
    /// Шифрование методом рельсов
    /// Метод рельсов (в алгоритме), записываем текущий символ по позиции сверху вниз слева направо
    /// Если индекс столбца доходит до ключа, то переменная изменения становится -1, иначе если доходит до 0, то становится 1
    /// Таким образом мы как бы скачем от стенки матрицы к стенке, заполняя ее символами из слова, до тех пор, пока колонца не достигнет количества символов в тексте
    /// </summary>
    public class Railway
    {
        public static int K { get; set; } //для получения текущего значения K
        private static char[,] Buf; //буфер - показ считываемоего слова в виде матрицы
        private static bool isChanged = false; //флаг для проверки на наличие хоть чего либо в Buf
        public static char[,] DisplayBuf() //отображение буфера
        {
            if(isChanged)
            for (int i = 0; i < Buf.GetLength(0); i++)
            {
                for (int j = 0; j < Buf.GetLength(1); j++)
                {
                    Console.Write(Buf[i,j]);
                }
                Console.WriteLine();
            }
            return Buf;
        }
        /// <summary>
        /// зашифровать
        /// </summary>
        /// <param name="k"> ключ </param>
        /// <param name="input"> текст для шифрования</param>
        /// <returns></returns>
        public static string Encode(int k, string input) 
        {
            isChanged = true;
            K = k;

            int length = input.Length;
            char[,] buf = new char[k, length];
            //заполняем буфер звездами
            for (int i = 0; i < k; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    buf[i, j] = '*';
                }
            }

            int column = 0,         //индекс столбца
                row = 0,            //индекс строки
                changer = 1;        //переменная изменения
            //выстраиваем слово по принципу рельс
            while (column < length)
            {
                buf[row, column] = input[column];
                if (row == k-1) changer = -1; else if(row == 0) changer = 1;
                row += changer;
                column++;
            }
            //считываем по строкам слева направо
            string result = "";
            for (int i = 0; i < k; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    if (buf[i, j] != '*') result += buf[i, j];
                }
            }
            Buf = buf;
            return result;//
        }
        /// <summary>
        /// расшифровать
        /// </summary>
        /// <param name="k"> ключ </param>
        /// <param name="input"> текст для расшифрования</param>
        /// <returns></returns>
        public static string Decode(int k, string input)
        {
            isChanged = true;
            K = k;

            int length = input.Length;
            char[,] buf = new char[k, length];
            //заполняем буфер звездами
            for (int i = 0; i < k; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    buf[i, j] = '*';
                }
            }
            int column = 0,         //индекс столбца
                row = 0,            //индекс строки
                changer = 1;        //переменная изменения

            //заполняем матрицу вопросами в тех местах, где изначально стояло слово/текст
            while (column < length)
            {
                buf[row, column] = '?';
                if (row == 0) changer = 1;
                else if (row == k - 1) changer = -1;
                row += changer;
                column++;
            }

            //вместо вопросов построчно записываем шифрослово
            int count = 0;
            for (int i = 0; i < k; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    if (buf[i, j] == '?')
                    {
                        buf[i, j] = input[count]; 
                        count++;
                    }
                }
            }

            column = 0; row = 0;
            changer = 1;
            string result = "";

            //считываем по принципу рельсов
            while (column < length)
            {
                result += buf[row, column];
                if (row == 0) changer = 1;
                else if (row == k - 1) changer = -1;
                row += changer;
                column++;
            }
            Buf = buf;
            return result;//
        }
    }
}
