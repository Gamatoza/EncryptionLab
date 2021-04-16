using System;
using System.Collections.Generic;
using System.Linq;
namespace EncryptionLib.LFSR
{
    public class LFSR
    {
        string formula;         // формула по которой происходит ксор //формат формулы: x^27 + x^8 + x^7 + x + 1
            byte[] points;      // номера битов для ксора
        string key;             // какой то рандомный ключ
        string input;           // текст
        
        Byte[] bytekey;         // ключ, преобразованый по формуле в длинный ключ размером с текст
            byte block_len;     // длина блока

        Byte[] AllIntoBytes(string path) => System.IO.File.ReadAllBytes(@"" + path);

        void ParseFormula() { } // преобразует строковую формулу в номера
        
        LFSR(string key,string text, params byte[] points)
        {
            this.key = key;
            block_len = (byte)key.Length; //lowgrade
            this.input = text;
            this.points = points;

            //longinium
           
        }

        byte[] XORnShift(byte[] word)
        {
            if (word.Length != block_len) throw new Exception("word != block_len");
            byte[] buf = word;

            for (int i = 0; i < block_len; i++)
            {

            }
        }
        

    }
}
