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
        public int bufkey { get; set; }             // Копия ключа, используется как не стартовое значение ключа

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

        //Функция преобразования формулы в аргументы (в разработке)
        void ParseFormula() { }         // преобразует строковую формулу в номера




        //конструкторы

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
        /// Основная функция преобразования
        /// </summary>
        /// <returns>Преобразует переменную text</returns>
        public int Transmute()
        {
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
        /// Вызов Transmute в режиме Step by Step, что бы можно было отслеживать изменение всего
        /// </summary>
        public void StartStapByStapTransmute()
        {
            isStepByStep = true;
            iKey = bufkey;
            iMask = (byte)getMask();
            iMax = text.Length;
            iIndex = 0;
            exKey[0] = (byte)iKey;
            AllSteps.Add(text[0], exKey[0]);
        }


        //intermidate


        public bool isStepByStep { get; private set; }                          // Проверка на работу в режиме step-by-step функции Transmute

        int iIndex;
        byte iMask;
        int iKey;
        int iMax;

        public byte iExKey { get; private set; }
        public byte iText { get; private set; }

        public void StepNext()
        {
            if (!isStepByStep) throw new Exception("Step by step mod isn't enable");
            iText = text[iIndex] ^= exKey[iIndex];
            if (++iIndex == iMax) { isStepByStep = false; return; }
            iExKey = exKey[iIndex] = (byte)xns(ref iKey, iMask);
        }

        public void StepPrev() //не знаю зачем
        {

        }

        public void Finish()
        {
            while (isStepByStep && ++iIndex < iMax)
            {
                iText = text[iIndex] ^= exKey[iIndex];
                iExKey = exKey[iIndex] = (byte)xns(ref iKey, iMask);
            }
            isStepByStep = false;
        }

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
