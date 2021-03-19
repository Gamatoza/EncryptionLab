﻿using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace EncryptionLib
{
    //TODO: Преобразование в DataGridWiew 
    //  TODO: Интерфейс для преобразования каждого в пкаждом (в разных можно будет не удалять а изменять на * к примеру)

    //TODO: Добавить метод ля проверки на наличие чисел и символов того или иного языка
    public class Helper
    {

        public static string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        public static string PrepareKey__Fill(string key, int length)
        {
            if (key.Length != 0)
            {
                int count = length / key.Length + 1;
                if (count == 1) return key;
                if (count > 1)
                    key = string.Concat(Enumerable.Repeat(key, count));
                //int len = key.Length;
                return key.Substring(0, length);
            }
            else throw new DivideByZeroException("key is null");
        }

        public static string GetTextFromFile(string path)
        {
            try
            {

                return File.ReadAllText(path);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public static void SaveTextIntoFile(string text, string path)
        {
            try
            {
                File.WriteAllText(path, text);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

    }
    
}
