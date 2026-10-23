using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace proj_1_gra_w_zycie
{
    public class Statistics : INotifyPropertyChanged
    {
        private int _totalPlants;
        private int _plantsWilted;
        private int _plantsGrown;
        private int _totalAnimals;
        private int _AnimalsDied;
        private int _AnimalsBorn;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public int TotalPlants
        {
            get => _totalPlants;
            set
            {
                if (_totalPlants != value)
                {
                    _totalPlants = value;
                    OnPropertyChanged(nameof(TotalPlants));
                }
            }
        }

        public int PlantsWilted
        {
            get => _plantsWilted;
            set
            {
                if (_plantsWilted != value)
                {
                    _plantsWilted = value;
                    OnPropertyChanged(nameof(PlantsWilted));
                }
            }
        }

        public int PlantsGrown
        {
            get => _plantsGrown;
            set
            {
                if (_plantsGrown != value)
                {
                    _plantsGrown = value;
                    OnPropertyChanged(nameof(PlantsGrown));
                }
            }
        }

        public int TotalAnimals
        {
            get => _totalAnimals;
            set
            {
                if (_totalAnimals != value)
                {
                    _totalAnimals = value;
                    OnPropertyChanged(nameof(TotalAnimals));
                }
            }
        }

        public int AnimalsDied
        {
            get => _AnimalsDied;
            set
            {
                if (_AnimalsDied != value)
                {
                    _AnimalsDied = value;
                    OnPropertyChanged(nameof(AnimalsDied));
                }
            }
        }

        public int AnimalsBorn
        {
            get => _AnimalsBorn;
            set
            {
                if (_AnimalsBorn != value)
                {
                    _AnimalsBorn = value;
                    OnPropertyChanged(nameof(AnimalsBorn));
                }
            }
        }

        public Statistics()
        {
            _totalPlants = 0;
            _plantsWilted = 0;
            _plantsGrown = 0;
            _totalAnimals = 0;
            _AnimalsDied = 0;
            _AnimalsBorn = 0;
        }

        public void Reset()
        {
            TotalPlants = 0;
            PlantsWilted = 0;
            PlantsGrown = 0;
            TotalAnimals = 0;
            AnimalsDied = 0;
            AnimalsBorn = 0;
        }

        public override string ToString()
        {
            string stringStats = "";

            stringStats += "P";
            stringStats += TotalPlants.ToString();

            stringStats += "G";
            stringStats += PlantsGrown.ToString();

            stringStats += "W";
            stringStats += PlantsWilted.ToString();

            stringStats += "A";
            stringStats += TotalAnimals.ToString();

            stringStats += "B";
            stringStats += AnimalsBorn.ToString();

            stringStats += "D";
            stringStats += AnimalsDied.ToString();

            return stringStats;
        }
    }
}