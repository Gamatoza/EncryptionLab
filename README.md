# EncryptionLab
Encription lib for lab
.NET 7.1 used
Example to use:
```c#
ICryptoBasic crypto = new [PastEncConstuctor](specific,variables);
string enc = crypto.Encode();
crypto.Input = enc;
string dec = crypto.Decode();
File.WriteAllText("output",String.Format("[The_Method_Name: {0} => {1} => {2}", text, enc, dec));
```
