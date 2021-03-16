using System;
using System.Collections.Generic;
using System.Text;

namespace EncryptionLib
{
    /// <summary>
    /// Класс создан с помощью листа на быструю руку, что бы можно было доставать сам лист текущих обьектов, 
    /// а так же конвертить их в строку по вомзожности
    /// </summary>
    /// <typeparam name="T">IComparable included</typeparam>
    [Serializable]
    public class QueueExtended<T> where T:IComparable
    {
        public static List<T> arr { get; set; }
        int count;
        int Count { get {
                return count;
            } 
        }


        public QueueExtended() { arr = new List<T>(); }

        public QueueExtended(T[] fuu):this()
        {
            arr.AddRange(fuu);
            count = fuu.Length;
        }

        public T Encueue(T elem)
        {
            arr.Add(elem);
            count++;
            return elem;
        }

        public T Decueue()
        {
            if (count - 1 <= 0) {
                if (arr[0] != null) throw new Exception("Something go wrong in Decueue() method");
                Console.WriteLine("There is nothing");
                return default(T); 
            }
            T ret = arr[0];
            arr.RemoveAt(0);
            count--;
            return ret;
        }

        public T Peek()
        {
            T buf = Decueue();
            Encueue(buf);
            return buf;
        }

        public T First()
        {
            return arr[0];
        }

        public T Last()
        {
            return arr[count-1];
        }

        public bool isEmpty()
        {
            return count == 0;
        }

        public bool Contains(T elem)
        {
            foreach (T item in arr)
            {
                if(elem.CompareTo(elem) == 0) return true;
            }
            return false;
        }

        

        //intotheFstring

        string ConvertToLine()
        {
            if(arr is List<char>) //7.0 c#
            {
                string res = "";
                foreach (var item in arr)
                {
                    res += item;
                }
                return res;
            }
            return ToString();
        }

        public override string ToString()
        {
            return ConvertToLine();
        }
    }
}
