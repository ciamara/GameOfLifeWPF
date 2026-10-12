using System;
using System.Collections.Generic;
using System.Text;

namespace proj_1_gra_w_zycie
{
    public class Board
    {
        public int Size;

        public Field[,] Fields;

        public Field this[int x, int y] => Fields[x, y];

        public IEnumerable<Field> Cells => Fields.Cast<Field>();        // flattened for ItemsControl

        public Board(int size)
        {
            this.Size = size;

            Fields = new Field[size, size];

            for (int i=0; i<size; i++)
            {
                for (int j=0; j<size; j++)
                {
                    Fields[i, j] = new Field(i, j);
                }
            }
        }

    }
}
