# EncryptionLib
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
File.WriteAllText("output",String.Format("[The_Method_Name: {0} => {1} => {2}", text, enc, dec));
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

