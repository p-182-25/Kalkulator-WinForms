using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kalkulator
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            textBox1.AppendText("1. Liczby ujemne wpisuj w nawiasach, np. 5*(-4) lub 4+6/(-2)" + "\r\n");
            textBox1.AppendText("2. Aby obliczyć pierwiastek podnieś liczbę do odwrotności potęgi, np. 4^(1/2) lub 27^(1/3)" + "\r\n");
            textBox1.AppendText("3. Uwaga! Wyrażenie typu -x^(1/potęga) = NaN" + "\r\n");
            textBox1.AppendText("4. Wprowadzone do obliczenia równanie sprawdzane jest pod kątem: " + "\r\n");
            textBox1.AppendText("     " + "a. wystąpienia na początku równania: +, *, /, ^ lub )" + "\r\n");
            textBox1.AppendText("     " + "b. wystąpienia na końcu równania: +, -, *, /, ^, . lub (" + "\r\n");
            textBox1.AppendText("     " + "c. wystąpienia obok siebie sep. dziesiętnych i/lub operatorów, np. .+- lub ^/.." + "\r\n");
            textBox1.AppendText("     " + "d. wystąpienia sep. dziesiętnego bezpośrednio przed lewym lub prawym nawiasem: .( lub .)" + "\r\n");
            textBox1.AppendText("     " + "e. wystąpienia sep. dziesiętnego bezpośrednio po lewym lub prawym nawiasie: (. lub )." + "\r\n");
            textBox1.AppendText("     " + "f. wystąpienia cyfry bezpośrednio przed lewym lub po prawym nawiasie, np. )5 lub 222(" + "\r\n");
            textBox1.AppendText("     " + "g. wystąpienia operatora innego niż minus bezpośrednio po lewym nawiasie, np. (+ lub (/" + "\r\n");
            textBox1.AppendText("     " + "h. wystąpienia operatora bezpośrednio przed prawym nawiasem, np. -) lub ^)" + "\r\n");
            textBox1.AppendText("     " + "i. wystąpienia lewego nawiasu bezpośrednio po prawym nawiasie: )(" + "\r\n");
            textBox1.AppendText("     " + "j. wystąpienia różnej liczby lewych i prawych nawiasów" + "\r\n");
            textBox1.AppendText("     " + "k. wystąpienia liczby, która rozpoczyna się od 0 w trybie niedziesiętnym, np. 00001 lub 01234" + "\r\n");
            textBox1.AppendText("     " + "l. wystąpienia liczby, która w części dziesiętnej posiada separatory dziesiętne, np. 192.168.0.0" + "\r\n");
            textBox1.AppendText("Jeżeli któryś z powyższych warunków (a-l) zostanie spełniony, dalsze wprowadzanie równania jest blokowane, " +
                                "gdyż obliczenie takiego równania nie byłoby możliwe. Użytkownik proszony jest o wpisanie poprawnego znaku." + "\r\n");
            textBox1.AppendText("5. Kalkulator można obsługiwać z poziomu klawiatury. Klawisz Enter odpowiada znakowi =, Backspace znakowi <-, "+
                                " natomiast klawisz Delete jest odpowiednikiem znaku całkowitego czyszczenia okna wyniku, czyli C." + "\r\n");
            textBox1.AppendText("6. Separator dziesiętny można uzyskać zarówno po naciśnięciu przecinka jak i kropki.");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            for ( ; Opacity > 0; Opacity -= 0.01)
            {
                System.Threading.Thread.Sleep(5);
            }
            Close();
        }
    }
}
