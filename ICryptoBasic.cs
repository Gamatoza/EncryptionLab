using System;
using System.Collections.Generic;
using System.Text;

namespace EncryptionLib
{
    //TODO: дописать интерфейс для возможности использования любого класса через переменную интерфейса или его реализации
    interface ICryptoBasic
    {
        string key { get;}
        string input_text { get;}
        string Encode();
        string Decode();
    }
}
