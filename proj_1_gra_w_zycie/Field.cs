using System;
using System.Collections.Generic;
using System.ComponentModel;
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

    public class Field : INotifyPropertyChanged
    {
        private bool _hasPlant;
        private bool _hasAnimal;
        private int? _animalAge;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Position Position { get; set; }

        public bool HasPlant
        {
            get => _hasPlant;
            set
            {
                if (_hasPlant != value)
                {
                    _hasPlant = value;
                    OnPropertyChanged(nameof(HasPlant));
                }
            }
        }

        public bool HasAnimal
        {
            get => _hasAnimal;
            set
            {
                if (_hasAnimal != value)
                {
                    _hasAnimal = value;
                    OnPropertyChanged(nameof(HasAnimal));
                }
            }
        }

        public int? AnimalAge
        {
            get => _animalAge;
            set
            {
                if (_animalAge != value)
                {
                    _animalAge = value;
                    OnPropertyChanged(nameof(AnimalAge));
                }
            }
        }

        public Field(int x, int y)
        {
            this.Position = new Position(x, y);
        }

        public Field(int x, int y, bool hasPlant, bool hasAnimal, int? animalAge)
        {
            this.Position = new Position(x, y);
            this.HasPlant = hasPlant;
            this.HasAnimal = hasAnimal;
            this.AnimalAge = animalAge;
        }

        public override string ToString()
        {

            string stringField = "";

            stringField += HasPlant.ToString();
            stringField += ",";
            stringField += HasAnimal.ToString();
            stringField += ",";
            stringField += AnimalAge.ToString();

            return stringField;
        }
    }
}