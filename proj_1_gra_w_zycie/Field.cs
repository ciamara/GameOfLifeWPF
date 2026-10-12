using System;
using System.Collections.Generic;
using System.Text;

namespace proj_1_gra_w_zycie
{

    public struct Position
    {
        public int X;
        public int Y;

        public Position(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }

    public class Field
    {
        public Position Position { get; set; }
        public bool HasPlant { get; set; }
        public bool HasAnimal { get; set; }
        public int? AnimalAge { get; set; }


        public Field(int x, int y)
        {
            this.Position = new Position(x, y);
        }

        public Field(int x, int y, bool hasPlant, bool hasAnimal, int? animalAge)
        {
            this.Position = new Position(x, y);
            this.HasAnimal = hasAnimal;
            this.HasPlant = hasPlant;
            this.AnimalAge = animalAge;
        }

    }
}
