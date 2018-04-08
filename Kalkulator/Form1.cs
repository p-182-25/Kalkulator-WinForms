using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace Kalkulator
{
    public partial class Form1 : Form
    {
        private Point _lastClick;
        private readonly string _undoArrow = char.ConvertFromUtf32(0x00002190);
        private bool _decimalMode;
        private string _infix;
        private double _wynik;

        public Form1()
        {
            InitializeComponent();
            undo.Text = _undoArrow;
        }

        internal static bool CharIsAnOperator(string userInput)
        {
            return userInput.Equals("+") ||
                   userInput.Equals("-") ||
                   userInput.Equals("*") ||
                   userInput.Equals("/") ||
                   userInput.Equals("^");
        }

        private void button_Click(object sender, EventArgs e)
        {
            if (sender is Button button && button.Text != @"=")
            {
                // a. wystąpienie na początku równania: +, *, /, ^ lub )
                if (result.Text == @"0" && (button.Text == @"+" || button.Text == @"*" || button.Text == @"/" || button.Text == @"^" || button.Text == @")"))
                {
                    MessageBox.Show(@"Proszę wpisać poprawny znak. Pierwszym znakiem równania nie może być: +, *, /, ^ lub ) !", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    label1.Focus();
                    return;
                }

                if (button.Text == _undoArrow)
                {
                    if (result.Text.Length > 0)
                    {
                        result.Text = result.Text.Remove(result.Text.Length - 1, 1);
                        result.SelectionStart = result.Text.Length;
                        label1.Focus();
                    }
                    if (result.Text == "")
                    {
                        result.Text = @"0";
                        label1.Focus();
                    }
                    return;
                }

                if (button.Text == @"C")
                {
                    result.Clear();
                    result.Text = @"0";
                    label1.Focus();
                    return;
                }

                if (CharsCanBeAdjacent(button.Text))
                {
                    if (CharIsAnOperator(button.Text))
                        _decimalMode = false;

                    if (NumberFormatIsCorrect(button.Text))
                    {
                        if (result.Text == @"0" && button.Text != @".")
                            result.Clear();

                        result.AppendText(button.Text);
                        label1.Focus();
                    }

                    if (button.Text == @".")
                        _decimalMode = true;
                }
            }
            else
            {
                // b. wystąpienie na końcu równania: +, -, *, /, ^, . lub (" + "\r\n");
                if (CharIsAnOperator(result.Text[result.Text.Length - 1].ToString()) || (result.Text[result.Text.Length - 1].ToString().Equals(".")) || (result.Text[result.Text.Length - 1].ToString().Equals("(")))
                {
                    MessageBox.Show(@"Proszę wpisać poprawny znak. Ostatnim znakiem równania nie może być operator, separator dziesiętny lub lewy nawias!", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    label1.Focus();
                    return;
                }

                // j. wystąpienie różnej liczby lewych i prawych nawiasów

                if (!ParenthesesAmountIsEqual(result.Text))
                {
                    MessageBox.Show(@"Proszę wpisać poprawny znak. Liczba lewych i prawych nawiasów nie jest taka sama!", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    label1.Focus();
                    return;
                }

                // ONP - zamiana infix na postfix oraz obliczenie wartości postfix
                _infix = result.Text;

                try
                {
                    ONP onp = new ONP(_infix);
                    _wynik = onp.PostfixEvaluation(onp.InfixToPostfix());
                }
                catch
                {
                    MessageBox.Show(@"Wystąpił problem!", @"Błąd!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                result.Text = _wynik.ToString(CultureInfo.InvariantCulture).Replace(',', '.');

                label1.Focus();
            }
        }

        private bool CharsCanBeAdjacent(string userInput)
        {
            string lastChar = result.Text[result.Text.Length - 1].ToString();

            // c. wystąpienie obok siebie sep. dziesiętnych i/lub operatorów
            if ((CharIsAnOperator(lastChar) || lastChar == ".") && (CharIsAnOperator(userInput) || userInput == "."))
            {
                MessageBox.Show(@"Proszę wpisać poprawny znak. Operatory i/lub separatory dziesiętne nie mogą występować obok siebie!", @"Błąd!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                label1.Focus();
                return false;
            }

            // d. wystąpienie sep. dziesiętnego bezpośrednio przed lewym lub prawym nawiasem
            if (lastChar == "." && userInput == ")")
            {
                MessageBox.Show(@"Proszę wpisać poprawny znak. Prawy nawias nie może wystąpić bezpośrednio po separatorze dziesiętnym!", @"Błąd!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                label1.Focus();
                return false;
            }
            if (lastChar == "." && userInput == "(")
            {
                MessageBox.Show(@"Proszę wpisać poprawny znak. Lewy nawias nie może wystąpić bezpośrednio po separatorze dziesiętnym!", @"Błąd!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                label1.Focus();
                return false;
            }

            // e. wystąpienie sep. dziesiętnego bezpośrednio po lewym lub prawym nawiasie
            if (lastChar == "(" && userInput == ".")
            {
                MessageBox.Show(@"Proszę wpisać poprawny znak. Separator dziesiętny nie może wystąpić bezpośrednio po lewym nawiasie!", @"Błąd!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                label1.Focus();
                return false;
            }
            if (lastChar == ")" && userInput == ".")
            {
                MessageBox.Show(@"Proszę wpisać poprawny znak. Separator dziesiętny nie może wystąpić bezpośrednio po prawym nawiasie!", @"Błąd!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                label1.Focus();
                return false;
            }

            // f. wystąpienie cyfry bezpośrednio przed lewym lub po prawym nawiasie
            if (Char.IsNumber(Convert.ToChar(lastChar)) && !result.Text.Equals("0") && userInput == "(")
            {
                MessageBox.Show(@"Proszę wpisać poprawny znak. Lewy nawias nie może wystąpić bezpośrednio po cyfrze!", @"Błąd!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                label1.Focus();
                return false;
            }
            if (lastChar == ")" && Char.IsNumber(Convert.ToChar(userInput)))
            {
                MessageBox.Show(@"Proszę wpisać poprawny znak. Cyfra nie może wystąpić bezpośrednio po prawym nawiasie!", @"Błąd!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                label1.Focus();
                return false;
            }

            // g. wystąpienie operatora innego niż - bezpośrednio po lewym nawiasie
            if (lastChar == "(" && (userInput == "+" || userInput == "*" || userInput == "/" || userInput == "^"))
            {
                MessageBox.Show(@"Proszę wpisać poprawny znak. Operator inny niż - nie może wystąpić bezpośrednio po lewym nawiasie!", @"Błąd!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                label1.Focus();
                return false;
            }

            // h. wystąpienie operatora bezpośrednio przed prawym nawiasem
            if (CharIsAnOperator(lastChar) && userInput == ")")
            {
                MessageBox.Show(@"Proszę wpisać poprawny znak. Prawy nawias nie może wystąpić bezpośrednio po operatorze!", @"Błąd!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                label1.Focus();
                return false;
            }

            // i. wystąpienie lewego nawiasu bezpośrednio po prawym nawiasie
            if (lastChar == ")" && userInput == "(")
            {
                MessageBox.Show(@"Proszę wpisać poprawny znak. Lewy nawias nie może wystąpić bezpośrednio po prawym nawiasie!", @"Błąd!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                label1.Focus();
                return false;
            }

            // j. wystąpienie różnej liczby lewych i prawych nawiasów - wystąpienie prawego nawiasu bez korespondującego lewego nawiasu
            if (userInput == ")")
            {
                int leftBracketsAmount = 0;
                int rightBracketsAmount = 0;
                foreach (var item in result.Text)
                {
                    if (item.Equals('('))
                        leftBracketsAmount++;
                    if (item.Equals(')'))
                        rightBracketsAmount++;
                }
                if (rightBracketsAmount >= leftBracketsAmount)
                {
                    MessageBox.Show(@"Proszę wpisać poprawny znak. Brak korespondującego lewego nawiasu!", @"Błąd!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    label1.Focus();
                    return false;
                }
            }

            return true;
        }

        private bool NumberFormatIsCorrect(string userInput)
        {
            string lastChar = result.Text[result.Text.Length - 1].ToString();
            string oneBeforeLastChar = "";

            if (result.Text.Length > 1)
                oneBeforeLastChar = result.Text[result.Text.Length - 2].ToString();

            // k. wystąpienia liczby, która rozpoczyna się 0 w trybie niedziesiętnym
            if (result.Text.Length > 1 && !Char.IsNumber(Convert.ToChar(oneBeforeLastChar)) && lastChar == "0" && !_decimalMode && Char.IsNumber(Convert.ToChar(userInput)))
            {
                MessageBox.Show(@"Proszę wpisać poprawny znak. Nieprawidłowy format liczby!", @"Błąd!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                label1.Focus();
                return false;
            }

            // l. wystąpienia liczby, która w częsci dziesiętnej ma separatory dziesiętne
            if (result.Text.Length > 1 && _decimalMode && userInput == ".")
            {
                MessageBox.Show(@"Proszę wpisać poprawny znak. Nieprawidłowy format liczby!", @"Błąd!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                label1.Focus();
                return false;
            }
            return true;
        }

        private bool ParenthesesAmountIsEqual(string userInput)
        {
            int leftParenthesesAmount = 0;
            int rightParenthesesAmount = 0;

            foreach (var item in userInput)
            {
                if (item.Equals('('))
                    leftParenthesesAmount++;
                else if (item.Equals(')'))
                    rightParenthesesAmount++;
            }

            return leftParenthesesAmount == rightParenthesesAmount;
        }        

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            _lastClick = e.Location;
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Left += e.X - _lastClick.X;
                Top += e.Y - _lastClick.Y;
            }
        }

        private void Zamknij_Click(object sender, EventArgs e)
        {
            label1.Focus();
            for ( ; Opacity > 0; Opacity -= 0.01)
            {
                System.Threading.Thread.Sleep(5);
            }
            Close();
        }

        private void Minimalizuj_Click(object sender, EventArgs e)
        {
            label1.Focus();
            WindowState = FormWindowState.Minimized;
        }

        private void Instrukcja_Click(object sender, EventArgs e)
        {
            label1.Focus();
            Form2 form2 = new Form2();
            form2.ShowDialog();
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar.ToString())
            {
                case "0":
                    zero.PerformClick();
                    break;
                case "1":
                    one.PerformClick();
                    break;
                case "2":
                    two.PerformClick();
                    break;
                case "3":
                    three.PerformClick();
                    break;
                case "4":
                    four.PerformClick();
                    break;
                case "5":
                    five.PerformClick();
                    break;
                case "6":
                    six.PerformClick();
                    break;
                case "7":
                    seven.PerformClick();
                    break;
                case "8":
                    eight.PerformClick();
                    break;
                case "9":
                    nine.PerformClick();
                    break;
                case "+":
                    plus.PerformClick();
                    break;
                case "-":
                    minus.PerformClick();
                    break;
                case "*":
                    multiply.PerformClick();
                    break;
                case "/":
                    divide.PerformClick();
                    break;
                case "^":
                    pow.PerformClick();
                    break;
                case "(":
                    leftBracket.PerformClick();
                    break;
                case ")":
                    rightBracket.PerformClick();
                    break;
                case ".":
                    point.PerformClick();
                    break;
                case ",":
                    point.PerformClick();
                    break;
                case "=":
                    equal.PerformClick();
                    break;
            }

            if (e.KeyChar.Equals((char)Keys.Enter))
                equal.PerformClick();

            if (e.KeyChar.Equals((char)Keys.Back))
                undo.PerformClick();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                clear.PerformClick();
        }
    }
}
