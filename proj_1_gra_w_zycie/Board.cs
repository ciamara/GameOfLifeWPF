using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace proj_1_gra_w_zycie
{
    public class Board
    {
        public int Size { get; }

        public Field[,] Fields { get; }

        public Field this[int x, int y] => Fields[x, y];

        public IEnumerable<Field> Cells => Fields.Cast<Field>();

        private readonly Random _random = new Random();

        public Board(int size)
        {
            Size = size;
            Fields = new Field[size, size];

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
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

            var selectedCells = Cells.Where(f => !f.HasPlant).OrderBy(_ => _random.Next()).Take(count);
            foreach (var field in selectedCells)
            {
                field.HasPlant = true;
            }
        }

        public void SetRandomAnimals(Options currOpts)
        {
            int maxAge = currOpts.L;
            int count = (int)Math.Round(Size * Size * 0.03);

            int halfLife = Math.Max(1, maxAge / 2);
            var selectedCells = Cells.Where(f => !f.HasAnimal).OrderBy(_ => _random.Next()).Take(count);
            foreach (var field in selectedCells)
            {
                field.HasAnimal = true;
                field.AnimalAge = _random.Next(0, halfLife);
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

        public int CountAnimalNeighbors(int x, int y)
        {
            int count = 0;
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0) continue;

                    int nx = (x + dx + Size) % Size;
                    int ny = (y + dy + Size) % Size;

                    if (Fields[nx, ny].HasAnimal)
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        public void Tick(Options options, Statistics stats)
        {
            ExecuteAnimalsStep(options, stats);
            ExecutePlantsStep(options, stats);
        }

        private void ExecutePlantsStep(Options options, Statistics stats)
        {
            bool[,] nextPlants = new bool[Size, Size];
            int grown = 0;
            int wilted = 0;

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
                            grown++;
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
                            wilted++;
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

            if (stats != null)
            {
                stats.PlantsGrown = grown;
                stats.PlantsWilted = wilted;
            }
        }

        private void ExecuteAnimalsStep(Options options, Statistics stats)
        {
            bool[,] nextAnimals = new bool[Size, Size];
            int?[,] nextAges = new int?[Size, Size];
            HashSet<Field> eatenPlants = new HashSet<Field>();

            int born = 0;
            int died = 0;

            for (int x = 0; x < Size; x++)
            {
                for (int y = 0; y < Size; y++)
                {
                    if (!Fields[x, y].HasAnimal)
                    {
                        int animalNeighbors = CountAnimalNeighbors(x, y);
                        int plantNeighbors = CountPlantNeighbors(x, y);

                        if (options.B.Contains(animalNeighbors) && options.P.Contains(plantNeighbors))
                        {
                            nextAnimals[x, y] = true;
                            nextAges[x, y] = 0;
                            born++;
                        }
                    }
                }
            }

            for (int x = 0; x < Size; x++)
            {
                for (int y = 0; y < Size; y++)
                {
                    Field currentField = Fields[x, y];

                    if (currentField.HasAnimal)
                    {
                        int animalNeighbors = CountAnimalNeighbors(x, y);
                        int currentAge = currentField.AnimalAge ?? 0;

                        if (options.Z.Contains(animalNeighbors) && currentAge < options.L)
                        {
                            Field targetField;

                            if (currentField.HasPlant && !eatenPlants.Contains(currentField))
                            {
                                targetField = currentField;
                            }
                            else
                            {
                                Field? closePlant = FindPlantInSingleRing(x, y, 1, eatenPlants);

                                if (closePlant != null)
                                {
                                    targetField = closePlant;
                                }
                                else if (options.M > 1)
                                {
                                    Field? outerPlant = FindPlantInSingleRing(x, y, options.M, eatenPlants);
                                    targetField = outerPlant ?? GetRandomNeighbor(x, y);
                                }
                                else
                                {
                                    targetField = GetRandomNeighbor(x, y);
                                }
                            }

                            int tx = targetField.Position.X;
                            int ty = targetField.Position.Y;

                            if (!nextAnimals[tx, ty])
                            {
                                nextAnimals[tx, ty] = true;
                                nextAges[tx, ty] = currentAge + 1;

                                if (targetField.HasPlant)
                                {
                                    eatenPlants.Add(targetField);
                                }
                            }
                            else
                            {
                                if (!nextAnimals[x, y])
                                {
                                    nextAnimals[x, y] = true;
                                    nextAges[x, y] = currentAge + 1;

                                    if (currentField.HasPlant)
                                    {
                                        eatenPlants.Add(currentField);
                                    }
                                }
                                else
                                {
                                    died++;
                                }
                            }
                        }
                        else
                        {
                            died++;
                        }
                    }
                }
            }

            foreach (Field plant in eatenPlants)
            {
                plant.HasPlant = false;
            }

            for (int x = 0; x < Size; x++)
            {
                for (int y = 0; y < Size; y++)
                {
                    Fields[x, y].HasAnimal = nextAnimals[x, y];
                    Fields[x, y].AnimalAge = nextAges[x, y];
                }
            }

            if (stats != null)
            {
                stats.AnimalsBorn = born;
                stats.AnimalsDied = died;
            }
        }

        private Field? FindPlantInSingleRing(int x, int y, int radius, HashSet<Field> alreadyEaten)
        {
            List<Field> plantsInRing = new List<Field>();

            for (int dx = -radius; dx <= radius; dx++)
            {
                for (int dy = -radius; dy <= radius; dy++)
                {
                    if (Math.Abs(dx) == radius || Math.Abs(dy) == radius)
                    {
                        int nx = (x + dx + Size) % Size;
                        int ny = (y + dy + Size) % Size;

                        Field candidate = Fields[nx, ny];
                        if (candidate.HasPlant && !alreadyEaten.Contains(candidate))
                        {
                            plantsInRing.Add(candidate);
                        }
                    }
                }
            }

            if (plantsInRing.Count > 0)
            {
                return plantsInRing[_random.Next(plantsInRing.Count)];
            }

            return null;
        }

        private Field GetRandomNeighbor(int x, int y)
        {
            int rx, ry;
            do
            {
                rx = _random.Next(-1, 2);
                ry = _random.Next(-1, 2);
            } while (rx == 0 && ry == 0);

            return Fields[(x + rx + Size) % Size, (y + ry + Size) % Size];
        }
    }
}