using System;
using System.Collections.Generic;
using System.Text;

namespace EncryptionLib
{
    
    // Шифр виженера
    [Serializable]
    public class Vjneer:ICryptoBasic // Vigenere Cipher
    {
        private char[] fullalpha_en = {
            'A','B','C','D','E','F',
            'G','H','I','J','K','L',
            'M','N','O','P','Q','R',//' ',
            'S','T', 'U','V','W','X',
            'Y','Z'};//"ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        private char[] fullalpha_ru = new char[] {
            'А' , 'Б' , 'В' , 'Г' , 'Д' ,
            'Е' , 'Ё' , 'Ж' , 'З' , 'И' ,
            'Й' , 'К' , 'Л' , 'М' , 'Н' ,
            'О' , 'П' , 'Р' , 'С' , 'Т' ,
            'У' , 'Ф' , 'Х' , 'Ц' , 'Ч' ,
            'Ш' , 'Щ' , 'Ъ' , 'Ы' , 'Ь' ,
            'Э' , 'Ю' , 'Я' };

        /*
        //not included
        char[] char_special = new char[]{
            '!','?',',','/','\\','\'','"'
        };*/

        private char[] alphabet_array; //Main array for ecripting //Default on EN

        //Ключ для улучшенной версии Виженера
        private char[,] progressivekey;
        public char[,] PrograssiveKey {
            get{
                if (progressivekey != null)
                    return progressivekey;
                else throw new Exception("Progressive key was not filled yet");
            } 
        }

        //заполнение ключа согласно установленному алфавиту
        private void FillProgressiveKey()
        {
            progressivekey = new char[N, N];

            QueueExtended<char> que = new QueueExtended<char>(alphabet_array);

            for (int i = 0; i < N; i++)
            {

                for (int j = 0; j < N; j++)
                {
                    progressivekey[i, j] = que.ToString()[j];
                }
                que.Peek();
            }
        }

        //Мощность алфовита (не включает спец символы (test 0.1))
        public int N
        {
            get {
                return alphabet_array.Length;
            }
        }


        /*public enum Language
        {
            EN = 0,
            RU = 1,
            //ASKII = 2,
            //UNICODE = 3
        }*/
        //TODO: связку с XOR что бы можно было добавлять еще и спец символы, как это делали в плюсах через |, ну ты понял

        private Language language = Language.EN;
        public Language LanguageChooser { get { return language; }
            set {
                switch (value)
                {
                    case Language.EN:
                        alphabet_array = (char[])fullalpha_en.Clone();
                        break;
                    case Language.RU:
                        alphabet_array = (char[])fullalpha_ru.Clone();
                        break;
                    /*case Language.ASKII:
                        break;
                    case Language.UNICODE:
                        break;*/
                    default:
                        alphabet_array = (char[])fullalpha_en.Clone(); //def
                        break;
                }
                FillProgressiveKey();
                language = value;

            }
        }

        private string key;
        private string input;

        public string Key {
            get{
                string res = "\t";

                for (int i = 0,j = 0 ; i < progressivekey.GetLength(0); i++,j++)
                {
                    res += j + " ";
                    if (j >= 9) j = 0;
                }
                res += '\n';
                for (int i = 0; i < progressivekey.GetLength(0); i++)
                {
                    res += i + ":\t";
                    for (int j = 0; j < progressivekey.GetLength(1); j++)
                    {
                        res += progressivekey[i, j] + " ";
                    }
                    res += "\n";
                }
                return res;
            }
            set { key = value; }
        }

        public string Input {
            get => input;
            set => input = value; }

        private string extra = "";
        public string Extra { get { return extra; } }

        public Vjneer(string text,Language lang)
        {
            LanguageChooser = lang;
            input = text;
        }

        public Vjneer(string text, string key, Language lang) : this(text, lang)
        {
            this.key = key;
        }

        //Classic Formula encript: ci = (pi + ki) mod N;
        //Where: ci - code simbol, pi - input simbol, ki - key simbol, N - power of alphabet (count of simpols in aplhabet)

        #region ICryptoBasic EDcoding
        public string Encode()
        {
            if (!String.IsNullOrEmpty(input))
            {
                if (String.IsNullOrEmpty(key))
                    return EncodeProgressiveKey(input);
                else return EncodeProgressiveKey(input, key);
            }
            else throw new Exception("Default text is empty");
        }
        public string Decode()
        {

            if (!String.IsNullOrEmpty(input))
            {
                if (String.IsNullOrEmpty(key))
                    return DecodeProgressiveKey(input);
                else return DecodeProgressiveKey(input, key);
            }
            else throw new Exception("Default text is empty");
        }
        #endregion

        #region Default Vijener Encoding
        /// <summary>
        /// Encoding vijener
        /// </summary>
        /// <param name="input"> input text</param>
        /// <param name="key"> keyword </param>
        /// <returns>cipertext</returns>
        public string Encode(string input, string key_word)
        {
            input = input.ToUpper();
            key_word = key_word.ToUpper();

            string result = "";

            int key_index = 0;

            foreach (char symbol in input)
            {
                //ignoring not alphabet simbols
                int pi = Array.IndexOf(alphabet_array, symbol);
                int ki = Array.IndexOf(alphabet_array, key_word[key_index]);
                if (pi == -1 || ki == -1) result += symbol;
                else
                {
                    int c = (pi + ki) % N; //not a legit MOD //TODO: create method to legit MOD
                    result += alphabet_array[c];
                }

                key_index++;

                if ((key_index + 1) == key_word.Length) 
                    key_index = 0;

            }

            return result;
        }

        /// <summary>
        /// Decoding vijener
        /// Formula for dectipt: pi = (ci + N - ki) mod N
        /// Where: ci - code simbol, pi - input simbol, ki - key simbol, N - power of alphabet (count of simpols in aplhabet)
        /// </summary>
        /// <param name="input"> input text</param>
        /// <param name="key_word"> keyword </param>
        /// <returns>source text</returns>
        public string Decode(string input, string key_word)
        {
            input = input.ToUpper();
            key_word = key_word.ToUpper();

            string result = "";

            int key_index = 0;

            foreach (char symbol in input)
            {

                //ignoring not alphabet simbols
                int ci = Array.IndexOf(alphabet_array, symbol);
                int ki = Array.IndexOf(alphabet_array, key_word[key_index]);
                if (ci == -1 || ki == -1) result += symbol;
                else
                {
                    int pi = (ci + N - ki) % N; //not a legit MOD //TODO: create method to legit MOD
                    result += alphabet_array[pi];
                }
                
                key_index++;
                if ((key_index + 1) == key_word.Length) key_index = 0;
            }

            return result;
        }
        #endregion
        #region Vijener Encoding with Generated key (Tritemius progressive key)
        /// <summary>
        /// Encode by using a Progressive generated key (Tritemius progressive key)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string EncodeProgressiveKey(string input)
        {
            input = input.ToUpper();

            string result = "";
            int length = input.Length;
            int index,level = 0;
            for (int i = 0; i < length; i++)
            {
                level++;
                if (level > 25) level = 1;
                index = Array.IndexOf(alphabet_array, input[i]);
                if (index == -1) result += input[i]; //ignorring if is not our alphabet
                else
                    result += progressivekey[index, level + 1];
            }

            return result;
        }
        public string DecodeProgressiveKey(string input)
        {
            input = input.ToUpper();

            string result = "";
            int length = input.Length;
            int index,level = 0;
            for (int i = 0; i < length; i++)
            {
                level--;
                if (level < 0) level = 25;
                index = Array.IndexOf(alphabet_array, input[i]);
                if (index == -1) result += input[i]; //ignorring if is not our alphabet
                else
                    result += progressivekey[index, N - 1 - level];
            }

            return result;
        }

        #endregion
        #region Vijener Encoding with User key (Tritemius progressive key)
        /// <summary>
        /// Encode by using a Progressive generated key (Tritemius progressive key)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string EncodeProgressiveKey(string input, string key)
        {
            input = input.ToUpper();
            key = Helper.PrepareKey__Fill(key, input.Length).ToUpper();

            string result = "";
            int length = input.Length;
            if (length != key.Length) throw new Exception("Key length don't equals to input text length");
            int index, level;
            for (int i = 0; i < length; i++)
            {
                level = Array.IndexOf(alphabet_array, key[i]);
                index = Array.IndexOf(alphabet_array, input[i]);
                if (index == -1) result += input[i]; //ignorring if is not our alphabet
                else
                    result += progressivekey[index, level + 1];
            }

            return result;
        }
        public string DecodeProgressiveKey(string input, string key)
        {
            input = input.ToUpper();
            key = Helper.PrepareKey__Fill(key, input.Length).ToUpper();

            string result = "";
            int length = input.Length;
            int index, level;
            for (int i = 0; i < length; i++)
            {
                level = Array.IndexOf(alphabet_array, key[i]);
                index = Array.IndexOf(alphabet_array, input[i]);
                if (index == -1) result += input[i]; //ignorring if is not our alphabet
                else 
                    result += progressivekey[index,N - 1 - level];
            }

            return result;
        }




        #endregion

        /// <summary>
        /// Gammir encript
        /// </summary>
        /// <param name="length">key length</param>
        /// <param name="startSeed"> start random num point, (generated key, specially designed for identical beaks)</param>
        /// <returns> Gammir encript</returns>
        public string Generate_XORCode(int length, int startSeed)
        {
            Random rand = new Random(startSeed);

            string result = "";

            for (int i = 0; i < length; i++)
                result += alphabet_array[rand.Next(0, alphabet_array.Length)];

            return result;
        }
    }

}
