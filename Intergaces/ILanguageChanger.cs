using System;
using System.Collections.Generic;
using System.Text;

namespace EncryptionLib
{
    [Serializable]
    public enum Language
    {
        EN = 0,
        RU = 1,
        /*ASKII = 2,
        UNICODE = 3*/
    }//TODO: связку с XOR что бы можно было добавлять еще и спец символы, как это делали в плюсах через |, ну ты понял
        
    interface ILanguageChanger
    {
        Language LanguageChooser { get; set; }
    }
}
