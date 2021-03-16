using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace EncryptionLib
{
    //TODO: дописать интерфейс для возможности использования любого класса через переменную интерфейса или его реализации
    public interface ICryptoBasic
    {
        string Key { get; set; }
        string Input { get; set; }
        string Encode();
        string Decode();
    }

    public static class Serializable
    {

        public static void Serialize(this ICryptoBasic obj, string name)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fs = new FileStream(name + ".dat", FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, obj);

                Console.WriteLine("Объект сериализован");
            }
        }

        public static void Deserialize(this ICryptoBasic obj, string name)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fs = new FileStream(name + ".dat", FileMode.OpenOrCreate))
            {
                obj = (ICryptoBasic)formatter.Deserialize(fs);

                Console.WriteLine("Объект десериализован");
            }
        }
    }
}
