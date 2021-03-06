﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wo.Utils
{
    /// <summary>
    /// 一个只读的二维数组
    /// 相当于T[,]
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ReadOnlyArray2<T> : IReadOnlyCollection<T>, IEnumerator<T>
    {
        private T[,] data = null;

        private int enumeratorIndex = -1;

        public int RowCount { get; private set; }
        public int ColumnCount { get; private set; }

        public T this[int i, int j]
        {
            get
            {
                return data[i, j];
            }
        }

        public T[,] ToArray()
        {
            return data.Clone() as T[,];
        }

        /// <summary>
        /// 返回第i行的值
        /// </summary>
        /// <param name="i">第i行（从0开始）</param>
        /// <returns></returns>
        public T[] ToArray(int i)
        {
            T[] arr = new T[ColumnCount];
            for (int col = 0; col < ColumnCount; col++)
                arr[col] = data[i, col];
            return arr;
        }

        public int Count
        {
            get
            {
                return data.Length;
            }
        }

        public T Current
        {
            get
            {
                int row = enumeratorIndex / ColumnCount;
                int col = enumeratorIndex - row * ColumnCount;
                return data[row, col];
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        public ReadOnlyArray2(T[,] array)
        {
            data = array.Clone() as T[,];
            RowCount = data.GetLength(0);
            ColumnCount = data.GetLength(1);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this;

        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this;
        }

        public void Dispose()
        {
            enumeratorIndex = -1;
        }

        public bool MoveNext()
        {
            if (data == null)
                return false;
            return (++enumeratorIndex) < data.Length;
        }

        public void Reset()
        {
            enumeratorIndex = -1;
        }
    }
}
