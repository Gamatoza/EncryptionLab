using System;

namespace EncryptionLib
{
    /// <summary>
    /// Шифрование методом рельсов
    /// Метод рельсов (в алгоритме), записываем текущий символ по позиции сверху вниз слева направо
    /// Если индекс столбца доходит до ключа, то переменная изменения становится -1, иначе если доходит до 0, то становится 1
    /// Таким образом мы как бы скачем от стенки матрицы к стенке, заполняя ее символами из слова, до тех пор, пока колонца не достигнет количества символов в тексте
    /// </summary>
    [Serializable]
    public class Railway:ICryptoBasic
    {
        private int key; //для получения текущего значения K
        private string input; //текст

        public string Key
        {
            get {
                return key.ToString();
            }
            set{
                try
                {
                int.TryParse(value,out key);
                }
                catch (Exception en)
                {
                    Console.WriteLine("value need to be integer\n"+en.Message);
                    throw;
                }

            }
        }

        public string Input
        {
            get {
                return input;
            }
            set {
                input = value;
            }
        }

        private char[,] Buf; //буфер - показ считываемоего слова в виде матрицы
        private bool isChanged = false; //флаг для проверки на наличие хоть чего либо в Buf
        
        public Railway(int key)
        {
            this.key = key;
        }

        public Railway(string text,int key)
        {
            this.key = key;
            this.input = text;
        }

        public char[,] DisplayBuf() //отображение буфера
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
        public string Encode()
        {
            if (!String.IsNullOrEmpty(input))
                return Encode(input);
            else throw new Exception("Default text is empty");
        }
        public string Decode()
        {
            if (!String.IsNullOrEmpty(input))
                return Decode(input);
            else throw new Exception("Default text is empty");
        }

        /// <summary>
        /// зашифровать
        /// </summary>
        /// <param name="text"> текст для шифрования</param>
        /// <returns></returns>
        public string Encode(string text) 
        {
            isChanged = true;

            int length = text.Length;
            char[,] buf = new char[key, length];
            //заполняем буфер звездами
            for (int i = 0; i < key; i++)
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
                buf[row, column] = text[column];
                if (row == key-1) changer = -1; else if(row == 0) changer = 1;
                row += changer;
                column++;
            }
            //считываем по строкам слева направо
            string result = "";
            for (int i = 0; i < key; i++)
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
        /// <param name="text"> текст для расшифрования</param>
        /// <returns></returns>
        public string Decode(string text)
        {
            isChanged = true;

            int length = text.Length;
            char[,] buf = new char[key, length];
            //заполняем буфер звездами
            for (int i = 0; i < key; i++)
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
                else if (row == key - 1) changer = -1;
                row += changer;
                column++;
            }

            //вместо вопросов построчно записываем шифрослово
            int count = 0;
            for (int i = 0; i < key; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    if (buf[i, j] == '?')
                    {
                        buf[i, j] = text[count]; 
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
                else if (row == key - 1) changer = -1;
                row += changer;
                column++;
            }
            Buf = buf;
            return result;//
        }
    }
}
