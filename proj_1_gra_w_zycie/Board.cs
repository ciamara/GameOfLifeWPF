using System;
using System.Collections.Generic;
using System.Text;

namespace proj_1_gra_w_zycie
{
    public class Board
    {
        public int Size { get; }

        public Field[,] Fields { get; }

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

        public bool Empty()
        {
            return Cells.All(f => !f.HasPlant && !f.HasAnimal);
        }

        public void Clear()
        {
            foreach (var field in Cells)
            {
                field.HasPlant = false;
                field.HasAnimal = false;
                field.AnimalAge = null;
            }
        }

        public void SetRandomPlants()
        {
            int count = (int)Math.Round(Size * Size * 0.2);

            Random random = new Random();
            var selectedCells = Cells.Where(f => !f.HasPlant).OrderBy(_ => random.Next()).Take(count);
            foreach (var field in selectedCells)
            {
                field.HasPlant = true;
            }
        }

        public void SetRandomAnimals(Options currOpts)
        {
            int maxAge = currOpts.L;
            int count = (int)Math.Round(Size * Size * 0.03);

            Random random = new Random();
            int halfLife = Math.Max(1, maxAge / 2);
            var selectedCells = Cells.Where(f => !f.HasAnimal).OrderBy(_ => random.Next()).Take(count);
            foreach (var field in selectedCells)
            {
                field.HasAnimal = true;
                field.AnimalAge = random.Next(0, halfLife);
            }
        }

        public int CountPlantNeighbors(int x, int y)
        {
            int count = 0;

            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0) continue;

                    int nx = (x + dx + Size) % Size;
                    int ny = (y + dy + Size) % Size;

                    if (Fields[nx, ny].HasPlant)
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        /// <summary>
        /// Simulation step
        /// </summary>
        public void Tick(Options options, Statistics stats)
        {
            ExecutePlantsStep(options, stats);

            // TODO: ExecuteAnimalsStep(options, stats);
        }

        /// <summary>
        /// Plants simulation step, honoring paramenets R (grow) and W (vegetate), with state buffer
        /// </summary>
        private void ExecutePlantsStep(Options options, Statistics stats)
        {
            bool[,] nextPlants = new bool[Size, Size];

            for (int x = 0; x < Size; x++)
            {
                for (int y = 0; y < Size; y++)
                {
                    int neighbors = CountPlantNeighbors(x, y);
                    bool hasPlant = Fields[x, y].HasPlant;

                    if (!hasPlant)
                    {
                        if (options.R.Contains(neighbors))
                        {
                            nextPlants[x, y] = true;
                            stats.PlantsGrown++;
                        }
                    }
                    else
                    {
                        if (options.W.Contains(neighbors))
                        {
                            nextPlants[x, y] = true;
                        }
                        else
                        {
                            nextPlants[x, y] = false;
                            stats.PlantsWilted++;
                        }
                    }
                }
            }

            for (int x = 0; x < Size; x++)
            {
                for (int y = 0; y < Size; y++)
                {
                    Fields[x, y].HasPlant = nextPlants[x, y];
                }
            }
        }

    }
}
