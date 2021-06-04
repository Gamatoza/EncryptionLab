# Default EnDecs
Encription lib for lab
.NET 7.1 used

Example to use standart encrypt:
```c#
using EncryptionLib;
...
ICryptoBasic crypto = new [PastEncConstuctor](specific,variables);
string enc = crypto.Encode();
crypto.Input = enc;
string dec = crypto.Decode();
File.WriteAllText("output",String.Format("[The_Method_Name]: {0} => {1} => {2}", text, enc, dec));
```
# LFSR
```c#
using EncryptionLib.LFSR;
...
LFSR lfsr = new LFSR("text.txt", 143, 8, 6, 5, 2);
Console.WriteLine("\nText before:\n");
foreach (var item in lfsr.text)
{
  Console.Write(item);
}
if (lfsr.Transmute() == 0) Console.WriteLine("\nAll done\n");
Console.WriteLine("\nText after:\n");
foreach (var item in lfsr.text)
{
  Console.Write(item);
}
lfsr.Transmute();
Console.WriteLine("\nAnd again, for test:\n");
foreach (var item in lfsr.text)
{
  Console.Write(item);
}
```
# SyncCrypt
```c#
using EncryptionLib.SyncCrypt
...
//RSA
RSA rsa = new RSA(89, 317);
rsa.Encode(); //[input.txt], [outout.txt] // 3th paramether is a permanent false to useAskii, but u can change it
rsa.Decode("out.txt", "out2.txt"); //[input.txt], [outout.txt]
//ElGamal
ElGamal el = new ElGamal();
//if u want to take args, user el.P|A|Y|X, X - is private key, other public
el.Encode(); //[input.txt], [outout.txt]
//if u want to use only decode, add el.SetArgs
el.Decode("out.txt", "out2.txt");
```
# DigitalSignature
```c#
using EncryptionLib.DigitalSignature
...
lightDSA dsa = new lightDSA("lorem");
            string some = dsa.GetSignature();
            bool gotit = dsa.CheckSignature("lorem", some);
            Console.WriteLine(some);
            Console.WriteLine(gotit?"yeah":"nonono");
```
