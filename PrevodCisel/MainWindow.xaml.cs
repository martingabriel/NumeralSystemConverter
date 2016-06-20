using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PrevodCisel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static Dictionary<char, int> RomanMap = new Dictionary<char, int>
        {
            {'I', 1},
            {'V', 5},
            {'X', 10},
            {'L', 50}, 
            {'C', 100},
            {'D', 500},
            {'M', 1000}
        };

        enum NumType
        {
            Bin = 0,
            Octa,
            Dec,
            Hex,
            Rom
        }

        public MainWindow()
        {
            InitializeComponent();

            fromComboBox.DataContext = Enum.GetValues(typeof(NumType));
        }

        private string Convertor(string input, NumType typeFrom, NumType typeTo)
        {
            int fromBase = 0;
            int toBase = 0;

            switch (typeFrom)
            {
                case NumType.Bin:
                    fromBase = 2;
                    break;
                case NumType.Dec:
                    fromBase = 10;
                    break;
                case NumType.Hex:
                    fromBase = 16;
                    break;
                case NumType.Octa:
                    fromBase = 8;
                    break;
                default:
                    break;
            }

            switch (typeTo)
            {
                case NumType.Bin:
                    toBase = 2;
                    break;
                case NumType.Dec:
                    toBase = 10;
                    break;
                case NumType.Hex:
                    toBase = 16;
                    break;
                case NumType.Octa:
                    toBase = 8;
                    break;
                default:
                    break;
            }

            String result = "";

            if (fromBase != 0)
            {
                // is number only check
                if (!IsNumberOnly(input))
                {
                    MessageBox.Show("Input is not number only. Try again.");
                    return "";
                }
            }
            else
            {
                if (!Regex.IsMatch(input, "^[a-zA-Z]"))
                {
                    MessageBox.Show("Input is not text only.Try again.");
                    return "";
                }
            }

            if (typeFrom == typeTo)
                return input;

            if (typeFrom != NumType.Rom && typeTo != NumType.Rom)
                result = Convert.ToString(Convert.ToInt32(input, fromBase), toBase);

            if (typeTo == NumType.Rom)
            {
                string fromToDec = Convert.ToString(Convert.ToInt32(input, fromBase), 10);
                int fromToInt = Convert.ToInt32(fromToDec);

                result = FromDecToRoman(fromToInt);
            }

            if (typeFrom == NumType.Rom)
                result = Convert.ToString(FromRomanToDec(input.ToUpper()), toBase);
            
            return result;
        }

        private static string FromDecToRoman(int number)
        {
            if ((number < 0) || (number > 3999)) throw new ArgumentOutOfRangeException("insert value betwheen 1 and 3999");
            if (number < 1) return string.Empty;
            if (number >= 1000) return "M" + FromDecToRoman(number - 1000);
            if (number >= 900) return "CM" + FromDecToRoman(number - 900);
            if (number >= 500) return "D" + FromDecToRoman(number - 500);
            if (number >= 400) return "CD" + FromDecToRoman(number - 400);
            if (number >= 100) return "C" + FromDecToRoman(number - 100);
            if (number >= 90) return "XC" + FromDecToRoman(number - 90);
            if (number >= 50) return "L" + FromDecToRoman(number - 50);
            if (number >= 40) return "XL" + FromDecToRoman(number - 40);
            if (number >= 10) return "X" + FromDecToRoman(number - 10);
            if (number >= 9) return "IX" + FromDecToRoman(number - 9);
            if (number >= 5) return "V" + FromDecToRoman(number - 5);
            if (number >= 4) return "IV" + FromDecToRoman(number - 4);
            if (number >= 1) return "I" + FromDecToRoman(number - 1);
            throw new ArgumentOutOfRangeException("something bad happened");
        }

        private static int FromRomanToDec(string input)
        {
            int totalValue = 0, prevValue = 0;

            foreach (var c in input)
            {
                if (!RomanMap.ContainsKey(c))
                    return 0;

                var crtValue = RomanMap[c];
                totalValue += crtValue;

                if (prevValue != 0 && prevValue < crtValue)
                {
                    if (prevValue == 1 &&
                        (crtValue == 5 || crtValue == 10) ||
                        prevValue == 10 &&
                        (crtValue == 50 || crtValue == 100) ||
                        prevValue == 100 &&
                        (crtValue == 500 || crtValue == 1000))
                        totalValue -= 2 * prevValue;
                    else
                        return 0;
                }
                prevValue = crtValue;
            }

            return totalValue;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                NumType fromNum = (NumType)fromComboBox.SelectedIndex;
                NumType toNum = (NumType)toComboBox.SelectedIndex;
                
                OutputTextBlock.Text = Convertor(InputTextBox.Text, fromNum, toNum);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unexpected exception.\nException type: " + ex.GetType());
            }
        }

        private void InputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    NumType fromNum = (NumType)fromComboBox.SelectedIndex;
                    NumType toNum = (NumType)toComboBox.SelectedIndex;

                    OutputTextBlock.Text = Convertor(InputTextBox.Text, fromNum, toNum);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unexpected exception.\nException type: " + ex.GetType());
                }
            }
        }

        private static bool IsNumberOnly(string text)
        {
            Regex regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
            return !regex.IsMatch(text);
        }
    }
}
