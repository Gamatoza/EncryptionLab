using System;
using System.Collections.Generic;
using System.Text;

namespace EncryptionLib
{
    //TODO: добавить вариацию выбора языка через конструктор или через свойство (лучше свойство)
    public class Vijener
    {
        private static char[] fullalpha_en = {
            'A','B','C','D','E','F',
            'G','H','I','J','K','L',
            'M','N','O','P','Q','R',//' ',
            'S','T', 'U','V','W','X',
            'Y','Z'};//"ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        private static char[] fullalpha_ru = new char[] {
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

        private static char[] alphabet_array = (char[])fullalpha_en.Clone(); //Main array for ecripting //Default on EN


        //Ключ для улучшенной версии Виженера
        private static char[,] progressivekey;
        public static char[,] PrograssiveKey {
            get{
                if (progressivekey != null)
                    return progressivekey;
                else throw new Exception("Progressive key was not filled yet");
            } 
        }

        //заполнение ключа согласно установленному алфавиту
        private static void FillProgressiveKey()
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
        public static int N
        {
            get {
                return alphabet_array.Length;
            }
        }


        public enum Language
        {
            EN = 0,
            RU = 1,
            /*ASKII = 2,
            UNICODE = 3*/
        }//TODO: связку с XOR что бы можно было добавлять еще и спец символы, как это делали в плюсах через |, ну ты понял

        private static Language language = Language.EN;
        public static Language LanguageChooser { get { return language; }
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

        static Vijener() {
            FillProgressiveKey(); 
        }


        //Classic Formula encript: ci = (pi + ki) mod N;
        //Where: ci - code simbol, pi - input simbol, ki - key simbol, N - power of alphabet (count of simpols in aplhabet)

        #region Default Vijener Encoding
        /// <summary>
        /// Encoding vijener
        /// </summary>
        /// <param name="input"> input text</param>
        /// <param name="key"> keyword </param>
        /// <returns>cipertext</returns>
        public static string Encode(string input, string key_word)
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
        public static string Decode(string input, string key_word)
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
        public static string EncodePrograssiveKey(string input)
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
                    result += progressivekey[level, index];
            }

            return result;
        }
        public static string DecodePrograssiveKey(string input)
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
                    result += progressivekey[index, level];
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
        public static string EncodePrograssiveKey(string input, string key)
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
        public static string DecodePrograssiveKey(string input, string key)
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
                {
                    result += progressivekey[index,N - 1 - level];
                }
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
        public static string Generate_XORCode(int length, int startSeed)
        {
            Random rand = new Random(startSeed);

            string result = "";

            for (int i = 0; i < length; i++)
                result += alphabet_array[rand.Next(0, alphabet_array.Length)];

            return result;
        }
    }

}
