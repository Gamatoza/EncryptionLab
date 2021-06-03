using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EncryptionLib.Help;
namespace EncryptionLib.LFSR
{
    /// <summary>
    /// Регистр сдвига с линейной обратной связью 
    /// (РСЛОС, англ. linear feedback shift register, LFSR) — сдвиговый регистр битовых слов, у которого значение входного (вдвигаемого) бита равно линейной булевой функции от значений остальных битов регистра до сдвига.
    /// </summary>
    public class LFSR
    {
        #region Main Getters Setters
        public int bufkey { get; set; } = -1;           // Копия ключа, используется как не стартовое значение ключа

        public string TextinBytes
        {
            get {
                string buf = "";
                foreach (var item in text)
                {
                    if (item != 0)
                        buf += Convert.ToString(item, 2);
                    else break;
                }
                return buf;
            }
        }
        public string Text
        {
            get {
                string buf = "";
                foreach (var item in text)
                {
                    if (item != 0)
                        buf += (char)item;//Convert.ToString(item,2);
                    else break;
                }
                return buf;
            }

            set {
                text = TextIntoBites(value);
            }
        }

        public string FullKey => String.Join("", exKey.Select(i => Convert.ToString(i, 2)));

        public string LastKey => Convert.ToString(exKey.Last(), 2);

        public int[] text { get; private set; }    // Преобразовочный/Преобразованный текст
        public byte[] btext { 
            get {
                return text.Select(i => (byte)i).ToArray();
            } }
        public int[] exKey { get; private set; }   // Расширенный ключ до размеров текста
        int[] args;                                // Аргументы функции
        int maxarg;
        #endregion
        #region Helpers
        //Преобразуют файл или текст в массив битов
        int[] FileIntoBites(string path)
            => System.IO.File.ReadAllBytes(@"" + path).Select(i=>(int)i).ToArray();
        int[] TextIntoBites(string text)
            => System.Text.Encoding.Default.GetBytes(text).Select(i => (int)i).ToArray();

        //Функция преобразования формулы в аргументы // формат входной строки: x^10 + x^5 + x^1
        int[] ParseFormula(string formula) =>// преобразует строковую формулу в номера
              formula.ToLower() //все в нижний
            .Split('+') //сплитим по плюсам
            .Select(i => Convert.ToInt32(i.Split('^').Last())) //сплитим по степеням и переводим в число
            .OrderByDescending(i => i).ToArray(); //ордерим по большему для быстрого нахождения максимального
        #endregion
        #region Constructors
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
        public LFSR(int[] text, int key, params int[] args)
        {
            this.text = text;
            bufkey = key;
            this.args = args;
            maxarg = args.Max();
            exKey = new int[text.Length];
        }

        /// <summary>
        /// Дополнительный конструктор
        /// </summary>
        /// <param name="path">путь к файлу</param>
        /// <param name="key">начальное значение регистра / ключ </param>
        /// <param name="args">аргументы функции</param>
        public LFSR(string path, int key, params int[] args)
        {
            text = FileIntoBites(path);
            bufkey = key;
            this.args = args;
            exKey = new int[text.Length];
        }

        /// <summary>
        /// Стандартный конструктор с формулой
        /// </summary>
        /// <param name="path">путь к файлу</param>
        /// <param name="key">начальное значение регистра / ключ </param>
        /// <param name="args">аргументы функции</param>
        public LFSR(string path, int key, string formula) : this(path, key)
        {
            this.args = ParseFormula(formula);
        }

        /// <summary>
        /// Дополнительный конструктор с формулой
        /// </summary>
        /// <param name="path">путь к файлу</param>
        /// <param name="key">начальное значение регистра / ключ </param>
        /// <param name="args">аргументы функции</param>
        public LFSR(int[] text, int key, string formula) : this(text, key)
        {
            this.args = ParseFormula(formula);
        }

        #endregion
        #region Getters Setters for Null Constructor
        bool isAllCheck() => text != null && bufkey != -1 && exKey != null && args != null;

        public void SetText(byte[] text) { this.text = text.Select(i => (int)i).ToArray(); exKey = new int[text.Length]; }
        public void SetText(int[] text) { this.text = text; exKey = new int[text.Length]; }
        public void SetText(string path) { this.text = FileIntoBites(path); exKey = new int[text.Length]; }
        public void SetKey(int key) { bufkey = key; }
        public void SetArgs(string formula) { args = ParseFormula(formula); maxarg = args[0]; }
        public void SetArgs(params int[] args) { this.args = args; maxarg = args.Max(); }
        #endregion
        #region Main functions
        /// <summary>
        /// Основная функция преобразования
        /// </summary>
        /// <returns>Преобразует переменную text</returns>
        public int Transmute()
        {
            if (!isAllCheck()) throw new Exception("no");
            int key = bufkey;
            int mask = (int)getMask();                     //можно без переменной сразу передавать в поле маски TODO
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
            isAsyncWorkDone = false;
            await Task.Run(() => Transmute());
            Task.WaitAll();
            isAsyncWorkDone = true;
        }

        #endregion
        #region StepByStep mod


        List<BWA<int>> AllSteps;

        BWA<int> pointer;
        int index = 0;
        public bool StepByStep { get; private set; }
        public bool isAsyncWorkDone {get; private set;}

        public async void TransmuteStepByStepAsync()
        {
            if (!isAllCheck()) throw new Exception("no");
            isAsyncWorkDone = false;
            StepByStep = true;
            AllSteps = new List<BWA<int>>();
            int key = bufkey;
            int mask = (int)getMask();                     //можно без переменной сразу передавать в поле маски TODO
            getHash(key, mask);
            Step(0);
            pointer = AllSteps[0];
            for (int i = 1; i < text.Length; i++)
            {
                await Task.Run(() => Step(i));
                Task.WaitAll();
            }
            isAsyncWorkDone = true;
        }
        
        
        void Step(int index)
        {
            BWA<int> next = new BWA<int>();
            next.Before = text[index];
            next.With = exKey[index];
            text[index] ^= exKey[index];
            next.After = text[index];
            AllSteps.Add(next);
        }


        class BWA<T> //Before With After
        {
            public BWA() { }

            public BWA(T before, T With, T After)
            {
                this.Before = before;
                this.With = With;
                this.After = After;
            }

            public T Before { get; set; }
            public T With { get; set; }

            public T After { get; set; }

            public override string ToString()
            {
                return String.Join(";", Before, With, After);
            }

        }
        public int MoveNext()
        {
            if (!StepByStep) throw new Exception("Step by step not started");
            if (++index > StepsCount) { --index; throw new Exception("Overflow"); }
            if (StepsCount <= index) Task.WaitAll();
            if (StepsCount <= index) { StepByStep = false; return 1; } //it's over
            pointer = AllSteps[index];
            return 0; //next
        }
        public int MovePrev()
        {
            if (!StepByStep) return 1;//throw new Exception("Step by step not started");
            if (--index < 0) { ++index; return 2; } //throw new Exception("Went Abroad");
            if (StepsCount < index) Task.WaitAll();
            if (StepsCount < index) { StepByStep = false; return 0; } //It's all over
            pointer = AllSteps[index];
            return 0;
        }
        
        //? add nullable ex if u nervious
        public int CurrentIndex => index;
        public int GetBefore => pointer.Before;
        public int GetWith => pointer.With;
        public int GetAfter => pointer.After;

        public string GetStep => pointer.ToString();
        public int StepsCount => AllSteps.Count;

        #endregion
        #region Additional

        /// <summary>
        /// Получение хеша функции
        /// По сути просто расширяет ключ до размеров текста используя xns
        /// </summary>
        /// <returns></returns>
        int getHash(int key, int mask)
        {
            exKey[0] = (int)key;
            for (int i = 1; i < text.Length-1; i++)
            {
                exKey[i] = (int)xns(ref key, mask);
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
            char[] _int = new String('0', 64).ToCharArray();
            for (int i = 0; i < args.Length; i++)
            {
                _int[args[i]] = '1';
            }

            return Convert.ToInt32((new string(_int)).Reverse(), 2);
        }

        /// <summary>
        /// Xor and shift
        /// Смещает obj на одну позицию, и ксорит его согласно маске
        /// </summary>
        /// <param name="obj">Регистр (возвращается благодаря параметру ref)</param>
        /// <param name="mask">Маска преобразования</param>
        /// <returns>так же возвращает obj, но для краткости использовать через ref</returns>
        int xns(ref int obj, int mask)
        {
            int high = (int)getBit((int)obj,maxarg);
            obj ^= high;            //избавляемся от бита за пределами
            obj = obj << 1;         //смещаем на одну позицию 
            obj += high != 0 ? 1:0; //добавляем бит на начало
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
        int getBit(int w, int n)
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

        #endregion

    }
}
