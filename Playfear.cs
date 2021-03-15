using System;
using System.Collections.Generic;
using System.Text;

namespace EncryptionLib
{
    //Шифр плейфера
    public class Playfear //Playfair Cipher
    {
        private static char[,] keymap =
        {
            { 'Y','A','R','M','O' },
            { 'L','I','K','B','C' },
            { 'D','E','F','G','H' },
            { 'N','P','Q','S','T' },
            { 'U','V','W','X','Z' } };
        private static int rows = keymap.GetLength(0);
        private static int columns = keymap.GetLength(1);
        public char[,] Key { get { return keymap; } }

        protected struct Point:ICloneable //one simbol
        {

            private char simbol;
            private int i, j;
            public Point(int i,int j, char simbol)
            {
                this.i = i;
                this.j = j;
                this.simbol = simbol;
            }
            public char Simbol
            {
                get { return simbol; }
                set {
                    simbol = value;
                    //TakeASimbolFromKeyMap(simbol);
                }
            }

            public void UpdateSimbol()
            {
                simbol = keymap[i, j];
            }
            
            public int I
            {
                get { return i; }
                set {
                    i = value;
                    /*if (i < 0) while (i < 0) i -= rows;
                    else if (i > rows) while (i >= rows) i += rows;*/
                    //simbol = keymap[i, j];
                }
            }
            public int J {
                get { return j; }
                set {
                    j = value;
                    /*if (j < 0) while (j < 0) j -= columns;
                    else if (j > columns) while (j >= columns) j += columns;*/
                    //simbol = keymap[i, j];
                }
            }

            bool isNull()
            {
                return i != -1 && j != -1;
            }

            public override string ToString()
            {
                return simbol.ToString();
            }

            //specific for alghoritm

            public void ChangeTo_Left()
            {
                //this.J-=1;
                j--;
                if (j < 0) j = columns - 1;
                simbol = keymap[i, j];
            }
            public void ChangeTo_Right()
            {
                //this.J+=1;
                j++;
                if (j >= columns) j = 0;
                simbol = keymap[i, j];
            }
            public void ChangeTo_Lower()
            {
                //this.I-=1;
                i--;
                if (i < 0) i = rows - 1;
                simbol = keymap[i, j];
            }
            public void ChangeTo_Upper()
            {
                //this.I+=1;
                i++;
                if (i >= rows) i = 0;
                simbol = keymap[i, j];
            }

            public object Clone()
            {
                return new Point(i,j,simbol);
            }
        }

        //ok

        private Bigramm BigramTo_Right(Bigramm from)
        {
            Point first = from[0];
            Point second = from[1];
            first.J += 1;
            second.J += 1;
            first.UpdateSimbol();
            second.UpdateSimbol();
            return new Bigramm(first, second); ;
        }
        private Bigramm BigramTo_Left(Bigramm from)
        {
            Point first = from[0];
            Point second = from[1];
            first.J -= 1;
            second.J -= 1;
            first.UpdateSimbol();
            second.UpdateSimbol();
            return new Bigramm(first, second); ;
        }
        private Bigramm BigramTo_Down(Bigramm from)
        {
            Point first = from[0];
            Point second = from[1];
            first.I -= 1;
            second.I -= 1;
            first.UpdateSimbol();
            second.UpdateSimbol();
            return new Bigramm(first, second); ;
        }
        private Bigramm BigramTo_Up(Bigramm from)
        {
            Point first = from[0];
            Point second = from[1];
            first.I += 1;
            second.I += 1;
            first.UpdateSimbol();
            second.UpdateSimbol();
            return new Bigramm(first, second); ;
        }

        private Bigramm BigramTo_RectangleRule(Bigramm from)
        {
            Point first = from[0];
            Point second = from[1];

            first.I = second.I;
            first.UpdateSimbol();
            second.UpdateSimbol();
            
            return new Bigramm(first, second); ;
        }

        protected class Bigramm //two simbols in the shell
        {

            Point[] data;
            //string data;
            public Point this[int index]
            {
                get { return data[index]; }
                set { data[index] = value; }
            }
            public Point[] Data { get { return data; } }

            public string SData { get { return "" + data[0] + data[1]; } }

            private Point TakeaPointFromKeymap(char simbol)
            {
                int row = -1, column = -1;
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < columns; j++)
                    {
                        if (simbol == keymap[i, j])
                        {
                            row = i;
                            column = j;
                            break;
                        }
                    }
                }
                if (row == -1 || column == -1) throw new Exception("The simbol is not on the keymap");
                return new Point(row, column, simbol);
            }

            public Bigramm(char first,char second)
            {
                data = new Point[2];
                data[0] = TakeaPointFromKeymap(first);
                data[1] = TakeaPointFromKeymap(second);
                //data = ""+first+second;
            }

            public Bigramm(Point first, Point second)
            {
                data = new Point[2];
                data[0] = (Point)first.Clone();
                data[1] = (Point)second.Clone();
            }

            public void ChangeData(char first, char second)
            {
                data[0].Simbol = first;
                data[1].Simbol = second;
            }

            public string Out()
            {
                return "[" + data[0] + ";" + data[1] + "]";
            }
            public override string ToString()
            {
                return data[0] + data[1].ToString();
            }
        }

        private List<Bigramm> biarray;

        private string ExcludeRepetitionsLatters(string str)
        {
            StringBuilder strb = new StringBuilder(str);
            for (int i = 0; i < strb.Length-1; i++)
            {
                if (strb[i] == strb[i + 1])
                {
                    strb.Insert(i + 1, 'X');
                }
            }
            return strb.ToString();
        }

        private List<Bigramm> Convert__StrToBigramm(string input)
        {
            List<Bigramm> buf = new List<Bigramm>();
            input = ExcludeRepetitionsLatters(input);
            if (input.Length % 2 != 0) input += "X"; //X the space simb
            for (int i = 0; i < input.Length; i+=2)
            {
                buf.Add(new Bigramm(input[i], input[i+1]));
            }
            return buf;
        }

        /// <BUGREPORTED>
        /// Не хочет возвращать по ссылке и даже по значению измененное значение, изменяет, вроде как, правильно
        /// </BUGREPORTED>

        public string Encode(string input)
        {
            input = input.Replace(" ", string.Empty); //delete the spaces
            biarray = Convert__StrToBigramm(input); //convert to bigrams
            /*foreach (var item in biarray) //just test
            {
                Console.Write(item + " ");
            }*/
            ///////////////////////////
            string result = "";
            Bigramm item;
            for (int i = 0; i < biarray.Count; i++)
            {
                item = biarray[i];
                if (item[0].I == item[1].I) //если они в одной строке, брать правые
                {/*
                    item[0].ChangeTo_Right();
                    item[1].ChangeTo_Right();*/
                    biarray[i] = BigramTo_Right(item);
                }
                else if(item[0].J == item[1].J) //если они в одном столбце, брать нижние
                {
                    /* item[0].ChangeTo_Lower();
                     item[1].ChangeTo_Lower();*/
                    biarray[i] = BigramTo_Down(item);
                }
                else if(item[0].Simbol != item[1].Simbol) //если они в разных местах, брать по противоположным углам
                {
                    biarray[i] = BigramTo_RectangleRule(item);
                    /*
                    Point first, second;
                    //выбираем верхний и нижний
                    if( item[0].I < item[1].I)
                    {
                        first = item[0];
                        second = item[1];
                    }
                    else
                    {
                        first = item[1];
                        second = item[0];
                    }
                    int diff = first.I - second.I; //diff не будет отрицательным, потому что по условию first - всегда выше (индекс строки меньше)
                    //верхний смещаем на низ ровно столько раз, сколько разница между столбцами
                    first.I -= diff;
                    //с нижним наоборот
                    first.I += diff;*/
                }
                else //если они одинаковые 
                {
                    throw new Exception("Text has repeated simbols, check ExcludeRepetitionsLatters() for work");
                }
                result += "" + item[0].Simbol + item[1].Simbol;
            }

            return result;
        }
        public static string Decode()
        {
            return "";
        }


    }
}
