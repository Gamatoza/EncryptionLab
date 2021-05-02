using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EncryptionLib.Help;
namespace EncryptionLib.LFSR
{
    public class LFSR
    {
        int key;
        int backkey;
        public byte[] text { get; private set; }
        public string Text { 
            get {

                return "";
            }
        }
        public byte[] exKey { get; private set; }
        byte[] args;
        byte mask;


        Byte[] FileIntoBites(string path)
            => System.IO.File.ReadAllBytes(@"" + path);
        Byte[] TextIntoBites(string text)
            => System.Text.Encoding.Default.GetBytes(text);

        void ParseFormula() { }         // преобразует строковую формулу в номера

        
        public LFSR(string path, byte key, params byte[] args)
        {
            text = FileIntoBites(path);
            this.backkey = key;
            this.args = args;
        }

        public LFSR(byte[] text, byte key, params byte[] args)
        {
            this.text = text;
            this.backkey = key;
            this.args = args;
        }

        public int Transmute()
        {
            key = backkey;
            mask = (byte)getMask();                     //можно без переменной сразу передавать в поле маски TODO
            getHash();
            for (int i = 0; i < text.Length; i++)
            {
                text[i] ^= exKey[i];
            }

            return 0;
        }

        int getHash()
        {
            exKey = new byte[text.Length];
            exKey[0] = (byte)key;
            for (int i = 1; i < text.Length-1; i++)
            {
                exKey[i] = (byte)xns(ref key);
            }

            return 0;
        }

        int getMask() //костылек со строками //преобразует аргументы в маску //TODO: Проделать такой же трюк только с битатми и getBit();
        {
            char[] _byte = new String('0', 64).ToCharArray();
            for (int i = 0; i < args.Length; i++)
            {
                _byte[args[i]] = '1';
            }

            return Convert.ToInt32((new string(_byte)).Reverse(), 2);
        }

        int xns(ref int obj) // xor and shift
        {
            byte high = (byte)highbit(obj);
            obj ^= high;            //избавляемся от старшего бита
            obj = obj << 1;         //смещаем на одну позицию
            obj += high != 0 ?1:0;  //добавляем бит на начало
            obj ^= mask;            //используем маску для того, что бы получить хэш
            return obj;
        }

        int getBit(byte w, byte n)
        {
            return w & (1 << n);
        }

        int highbit(int x)
        {
            x |= x >> 1;
            x |= x >> 2;
            x |= x >> 4;
            x |= x >> 8;
            x |= x >> 16;
            return  x - (x >> 1);
        }

    }
}
