using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace proj_1_gra_w_zycie
{
    public class Options : INotifyPropertyChanged
    {
        private string _rText = "123";
        private string _wText = "123456";
        private string _bText = "23";
        private string _pText = "01";
        private string _zText = "0123";
        private int _m = 1;
        private int _l = 20;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string RText
        {
            get => _rText;
            set
            {
                if (_rText != value)
                {
                    _rText = value;
                    OnPropertyChanged(nameof(RText));
                    OnPropertyChanged(nameof(R));
                }
            }
        }

        public string WText
        {
            get => _wText;
            set
            {
                if (_wText != value)
                {
                    _wText = value;
                    OnPropertyChanged(nameof(WText));
                    OnPropertyChanged(nameof(W));
                }
            }
        }

        public string BText
        {
            get => _bText;
            set
            {
                if (_bText != value)
                {
                    _bText = value;
                    OnPropertyChanged(nameof(BText));
                    OnPropertyChanged(nameof(B));
                }
            }
        }

        public string PText
        {
            get => _pText;
            set
            {
                if (_pText != value)
                {
                    _pText = value;
                    OnPropertyChanged(nameof(PText));
                    OnPropertyChanged(nameof(P));
                }
            }
        }

        public string ZText
        {
            get => _zText;
            set
            {
                if (_zText != value)
                {
                    _zText = value;
                    OnPropertyChanged(nameof(ZText));
                    OnPropertyChanged(nameof(Z));
                }
            }
        }

        public int M
        {
            get => _m;
            set
            {
                if (_m != value)
                {
                    _m = value;
                    OnPropertyChanged(nameof(M));
                }
            }
        }

        public int L
        {
            get => _l;
            set
            {
                if (_l != value)
                {
                    _l = value;
                    OnPropertyChanged(nameof(L));
                }
            }
        }

        public HashSet<int> R => ParseDigits(RText);
        public HashSet<int> W => ParseDigits(WText);
        public HashSet<int> B => ParseDigits(BText);
        public HashSet<int> P => ParseDigits(PText);
        public HashSet<int> Z => ParseDigits(ZText);

        private static HashSet<int> ParseDigits(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return new HashSet<int>();
            }
            return input.Where(char.IsDigit).Select(c => c - '0').ToHashSet();
        }

        public Options(string r = "123", string w = "123456", string b = "23", string p = "01", string z = "0123", int m = 1, int l = 20)
        {
            _rText = r;
            _wText = w;
            _bText = b;
            _pText = p;
            _zText = z;
            _m = m;
            _l = l;
        }
    }
}