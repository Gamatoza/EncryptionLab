using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EncryptionLib.Help;
namespace EncryptionLib.LFSR
{
    public class LFSR
    {
        public int bufkey { get; set; } = -1;           // Копия ключа, используется как не стартовое значение ключа

        public string Text
        {
            get {
                string buf = "";
                foreach (var item in text)
                {
                    if (item != 0)
                        buf += (char)item;
                    else break;
                }
                return buf;
            }

            set {
                text = TextIntoBites(value);
            }
        }

        public byte[] text { get; private set; }    // Преобразовочный/Преобразованный текст
        public byte[] exKey { get; private set; }   // Расширенный ключ до размеров текста
        byte[] args;                                // Аргументы функции

        //Хелперы
        //Преобразуют файл или текст в массив битов
        Byte[] FileIntoBites(string path)
            => System.IO.File.ReadAllBytes(@"" + path);
        Byte[] TextIntoBites(string text)
            => System.Text.Encoding.Default.GetBytes(text);

        //Функция преобразования формулы в аргументы // формат входной строки: x^10 + x^5 + x^1
        byte[] ParseFormula(string formula) =>// преобразует строковую формулу в номера
              formula.ToLower() //все в нижний
            .Split('+') //сплитим по плюсам
            .Select(i => Convert.ToByte(i.Split('^').Last())) //сплитим по степеням и переводим в число
            .OrderByDescending(i => i).ToArray(); //ордерим по большему для быстрого нахождения максимального




        //конструкторы
        /// <summary>
        /// Пустой конструктор
        /// Постепенное заполнение
        /// </summary>
        public LFSR() { }

        /// <summary>
        /// Стандартный конструктор
        /// </summary>
        /// <param name="text">текст в байтах</param>
        /// <param name="key">начальное значение регистра / ключ </param>
        /// <param name="args">аргументы функции</param>
        public LFSR(byte[] text, byte key, params byte[] args)
        {
            this.text = text;
            bufkey = key;
            this.args = args;
            exKey = new byte[text.Length];
        }

        /// <summary>
        /// Дополнительный конструктор
        /// </summary>
        /// <param name="path">путь к файлу</param>
        /// <param name="key">начальное значение регистра / ключ </param>
        /// <param name="args">аргументы функции</param>
        public LFSR(string path, byte key, params byte[] args)
        {
            text = FileIntoBites(path);
            bufkey = key;
            this.args = args;
            exKey = new byte[text.Length];
        }

        /// <summary>
        /// Стандартный конструктор с формулой
        /// </summary>
        /// <param name="path">путь к файлу</param>
        /// <param name="key">начальное значение регистра / ключ </param>
        /// <param name="args">аргументы функции</param>
        public LFSR(string path, byte key, string formula):this(path,key)
        {
            this.args = ParseFormula(formula);
        }

        /// <summary>
        /// Дополнительный конструктор с формулой
        /// </summary>
        /// <param name="path">путь к файлу</param>
        /// <param name="key">начальное значение регистра / ключ </param>
        /// <param name="args">аргументы функции</param>
        public LFSR(byte[] text, byte key, string formula) : this(text, key)
        {
            this.args = ParseFormula(formula);
        }

        bool isAllCheck() => text != null && bufkey != -1 && exKey != null && args != null;

        public void SetText(byte[] text) { this.text = text; exKey = new byte[text.Length]; }
        public void SetText(string path) { this.text = FileIntoBites(path); }
        public void SetKey(int key) { bufkey = key; }
        public void SetArgs(string formula) { args = ParseFormula(formula); }
        public void SetArgs(params byte[] args) { this.args = args; }


        /// <summary>
        /// Основная функция преобразования
        /// </summary>
        /// <returns>Преобразует переменную text</returns>
        public int Transmute()
        {
            if (!isAllCheck()) throw new Exception("no");
            int key = bufkey;
            byte mask = (byte)getMask();                     //можно без переменной сразу передавать в поле маски TODO
            getHash(key, mask);
            for (int i = 0; i < text.Length; i++)
            {
                text[i] ^= exKey[i];
            }
             
            return 0;
        }

        /// <summary>
        /// Асинхронный вызов Transmute
        /// </summary>
        public async void TransmuteAsync()
        {
            await Task.Run(() => Transmute());
        }


        //StepByStep mod
        
        public Dictionary<byte, byte> AllSteps;

        

        /// <summary>
        /// Получение хеша функции
        /// По сути просто расширяет ключ до размеров текста используя xns
        /// </summary>
        /// <returns></returns>
        int getHash(int key, byte mask)
        {
            exKey[0] = (byte)key;
            for (int i = 1; i < text.Length-1; i++)
            {
                exKey[i] = (byte)xns(ref key, mask);
            }

            return 0;
        }

        /// <summary>
        /// костылек со строками 
        /// преобразует аргументы в маску
        /// </summary>
        /// <returns>Возвращает маску, преобразованную согласно аргументам функции</returns>
        //TODO: Проделать такой же трюк только с битатми и getBit();
        int getMask()
        {
            char[] _byte = new String('0', 64).ToCharArray();
            for (int i = 0; i < args.Length; i++)
            {
                _byte[args[i]] = '1';
            }

            return Convert.ToInt32((new string(_byte)).Reverse(), 2);
        }

        /// <summary>
        /// Xor and shift
        /// Смещает obj на одну позицию, и ксорит его согласно маске
        /// </summary>
        /// <param name="obj">Регистр (возвращается благодаря параметру ref)</param>
        /// <param name="mask">Маска преобразования</param>
        /// <returns>так же возвращает obj, но для краткости использовать через ref</returns>
        int xns(ref int obj, byte mask)
        {
            byte high = (byte)highbit(obj);
            obj ^= high;            //избавляемся от старшего бита
            obj = obj << 1;         //смещаем на одну позицию
            obj += high != 0 ?1:0;  //добавляем бит на начало
            obj ^= mask;            //используем маску для того, что бы получить хэш
            return obj;
        }

        /// <summary>
        /// Функция, возвращающая бит на конкретном месте числа
        /// Если был возвращен 0, то бита нет или там стоит 0 (логично)
        /// </summary>
        /// <param name="w">word</param>
        /// <param name="n">position</param>
        /// <returns>exists bit in bite</returns>
        int getBit(byte w, byte n)
        {
            return w & (1 << n);
        }

        /// <summary>
        /// Возвращает старший бит байта
        /// Старший бит - самый левый не нулевой бит
        /// </summary>
        /// <param name="x">bite</param>
        /// <returns></returns>
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
