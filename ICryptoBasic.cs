using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace EncryptionLib
{
    public interface ICryptoBasic
    {
        string Key { get; set; }
        string Input { get; set; }
        string Extra { get; }
        string Encode();
        string Decode();
    }

    public static class CryptoExtended
    {

        public static void Serialize(this ICryptoBasic obj, string name)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fs = new FileStream(name + ".dat", FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, obj);

                //Console.WriteLine("Объект сериализован");
            }
        }

        public static void Deserialize(this ICryptoBasic obj, string name)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fs = new FileStream(name + ".dat", FileMode.OpenOrCreate))
            {
                obj = (ICryptoBasic)formatter.Deserialize(fs);

                //Console.WriteLine("Объект десериализован");
            }
        }
        
        // запись в файл для сохранения данных
        public static void WriteFileData(this ICryptoBasic obj, string path)
        {
            using (FileStream fstream = new FileStream($"{path}", FileMode.OpenOrCreate))
            {
                string enc = obj.Encode();
                obj.Input = enc;
                string dec = obj.Decode();
                Console.Read();
                // преобразуем строку в байты
                byte[] array = System.Text.Encoding.Default.GetBytes(obj.Input + ";" + obj.Key + ";" + enc + ";" + dec);
                // запись массива байтов в файл
                fstream.Write(array, 0, array.Length);
            }
        }

        // запись в файл для красоты :D
        public static void WriteFileText(this ICryptoBasic obj, string path)
        {
            using (FileStream fstream = new FileStream($"{path}", FileMode.OpenOrCreate))
            {
                string enc = obj.Encode();
                obj.Input = enc;
                string dec = obj.Decode();
                Console.Read();
                // преобразуем строку в байты
                byte[] array = System.Text.Encoding.Default.GetBytes(obj.Input + ";" + obj.Key + ";" + enc + ";" + dec);
                // запись массива байтов в файл
                fstream.Write(array, 0, array.Length);
            }
        }

        // чтение из файла данных
        public static void ReadFileData(this ICryptoBasic obj, string path)
        {
            using (FileStream fstream = File.OpenRead($"{path}"))
            {
                // преобразуем строку в байты
                byte[] array = new byte[fstream.Length];
                // считываем данные
                fstream.Read(array, 0, array.Length);
                // декодируем байты в строку
                string[] textFromFile = System.Text.Encoding.Default.GetString(array).Split(';');
                obj.Input = textFromFile[0];
                obj.Key = textFromFile[1];
            }
        }
    }
}
