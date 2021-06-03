using System;
using System.Collections.Generic;
using System.Text;

namespace EncryptionLib
{
    [Serializable]
    public class PlayfearLite: ICryptoBasic  //Playfair Cipher
    {
        private static char[,] keymap =
        {
            { 'Y','A','R','M','O' },
            { 'L','I','K','B','C' },
            { 'D','E','F','G','H' },
            { 'N','P','Q','S','T' },
            { 'U','V','W','X','Z' } };
        private const int ROWS = 5;//keymap.GetLength(0);
        private const int COLUMNS = 5;// keymap.GetLength(1);
        

        public string Key
        {
            get {
                string res = "";
                for (int i = 0; i < keymap.GetLength(0); i++)
                {
                    for (int j = 0; j < keymap.GetLength(1); j++)
                    {
                        res += keymap[i, j] + " ";
                    }
                    res += "\n";
                }
                return res;
            }
            set { }
        }
        string text;
        public string Input { get => text; set => text = value; }
        private string extra = "";
        public string Extra { get { return extra; } }

        public PlayfearLite(string text)
        {
            this.text = text;
        }

        private List<char[]> StringToBigramms(string text)
        {
            List<char[]> buf = new List<char[]>();
            if (text.Length % 2 != 0) text += "X";
            for (int i = 0; i < text.Length; i += 2)
            {
                buf.Add(new char[] { text[i], text[i + 1] });
            }
            return buf;
        }

        private string BigrammsToString(List<char[]> bigramms)
        {
            string result = "";
            if (bigramms[0].Length != 2) throw new Exception("Is not a bigramm list");
            foreach (var item in bigramms)
            {
                result += "" + item[0] + item[1];
            }
            return result;
        }

        private struct pair
        {
            public int i;
            public int j;
            public pair(int i, int j) { this.i = i; this.j = j; }
            public bool isFound() { return i != -1 && j != -1; }
        }

        private pair ReturnIndex(char simbol)
        {
            for (int i = 0; i < ROWS; i++)
            {
                for (int j = 0; j < COLUMNS; j++)
                {
                    if (simbol == keymap[i, j]) return new pair(i, j);
                }
            }
            return new pair(-1, -1);
        }

        /// <summary>
        /// convert text type like = "ddss" into "dxdsxs"
        /// </summary>
        /// <param name="str">input text</param>
        /// <returns>text without consecutive letters</returns>
        private string Rule5(string str) //no consecutive letters
        {

            StringBuilder strb = new StringBuilder(str);
            for (int i = 0; i < strb.Length - 1; i++)
            {
                if (strb[i] == strb[i + 1])
                {
                    if (str[i] == 'X')
                        strb.Insert(i + 1, 'Q');
                    else
                        strb.Insert(i + 1, 'X');
                }
            }
            return strb.ToString();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string Encode()
        {
            text = Rule5(text.Replace(" ","").Trim().ToUpper());
            List<char[]> bigramms = StringToBigramms(text);
            for (int i = 0; i < bigramms.Count; i++)
            {
                pair first = ReturnIndex(bigramms[i][0]);
                pair second = ReturnIndex(bigramms[i][1]);
                if (first.isFound() && second.isFound())
                {
                    if (first.i == second.i)
                    {
                        int row = first.i + 1 >= ROWS ? 0 : first.i + 1; //right
                        bigramms[i][0] = keymap[row, first.j];
                        row = second.i + 1 >= ROWS ? 0 : second.i + 1;
                        bigramms[i][1] = keymap[row, second.j];
                    }
                    else if (first.j == second.j)
                    {
                        int column = first.j + 1 >= COLUMNS ? 0 : first.j + 1; //down
                        bigramms[i][0] = keymap[first.i, column];
                        column = second.j + 1 >= COLUMNS ? 0 : second.j + 1;
                        bigramms[i][1] = keymap[second.i, column];
                    }
                    else
                    {
                        bigramms[i][0] = keymap[second.i, first.j];
                        bigramms[i][1] = keymap[first.i, second.j];
                    }
                }
            }

            return BigrammsToString(bigramms);
        }

        public string Decode()
        {

            text = Rule5(text.Replace(" ", "").Trim().ToUpper());
            List<char[]> bigramms = StringToBigramms(text);
            for (int i = 0; i < bigramms.Count; i++)
            {
                pair first = ReturnIndex(bigramms[i][0]);
                pair second = ReturnIndex(bigramms[i][1]);
                if (first.isFound() && second.isFound())
                {
                    if (first.i == second.i)
                    {
                        int row = first.i - 1 < 0 ? ROWS : first.i - 1; //left
                        bigramms[i][0] = keymap[row, first.j];
                        row = second.i - 1 < 0 ? ROWS : second.i - 1;
                        bigramms[i][1] = keymap[row, second.j];
                    }
                    else if (first.j == second.j)
                    {
                        int column = first.j - 1 < 0 ? COLUMNS : first.j - 1; //up
                        bigramms[i][0] = keymap[first.i, column];
                        column = second.j - 1 < 0 ? COLUMNS : second.j - 1;
                        bigramms[i][1] = keymap[second.i, column];
                    }
                    else
                    {
                        bigramms[i][0] = keymap[second.i, first.j];
                        bigramms[i][1] = keymap[first.i, second.j];
                    }
                }
            }

            return BigrammsToString(bigramms);
        }
    }
}
