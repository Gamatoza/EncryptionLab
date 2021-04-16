using System;
using System.Collections.Generic;
using System.Text;
using EncryptionLib;

namespace EncryptionLib.LFSR
{
    public class LFSR:ICryptoBasic
    {

        public string Key { get; set; } = "x^27 + x^8 + x^7 + x + 1"; //formula //def: x^27 + x^8 + x^7 + x + 1
        
        

        public string Input { get; set; } //text //byte

        public string Extra => "";

        public string Decode()
        {
            return "";
        }

        public string Encode()
        {

            return "";
        }

        Byte[] AllIntoBytes(string path) => System.IO.File.ReadAllBytes(@"" + path);

    }
}
