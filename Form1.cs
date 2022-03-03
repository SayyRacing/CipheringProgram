using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;

namespace NetZad1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Wczytywanie plików
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.FileName = "";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //Określanie ścieżki pliku
                label1.Text = openFileDialog1.FileName;

                //Wczytywanie zawartości pliku do okna tekstowego
                var fileStream = openFileDialog1.OpenFile();

                using (StreamReader reader = new StreamReader(fileStream))
                {
                    textBox1.Text = reader.ReadToEnd();
                }
            }
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Konfiguracja zapisu
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;

            //Zapisywanie pliku oraz wyświetlenie ścieżki na etykiecie
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                System.IO.File.WriteAllText(saveFileDialog1.FileName, textBox2.Text);
                label2.Text = saveFileDialog1.FileName;
            }
        }

        public string Encrypt(string source, string key)
        {
            //Szyfrowanie metodą 3DES https://foxlearn.com/windows-forms/encryption-and-decryption-using-triple-des-in-csharp-376.html

            using (TripleDESCryptoServiceProvider tripleDESCryptoService = new TripleDESCryptoServiceProvider())
            {
                using (MD5CryptoServiceProvider hashMD5Provider = new MD5CryptoServiceProvider())
                {
                    byte[] byteHash = hashMD5Provider.ComputeHash(Encoding.UTF8.GetBytes(key));
                    tripleDESCryptoService.Key = byteHash;
                    tripleDESCryptoService.Mode = CipherMode.ECB;//CBC, CFB
                    byte[] data = Encoding.Unicode.GetBytes(source);
                    return Convert.ToBase64String(tripleDESCryptoService.CreateEncryptor().TransformFinalBlock(data, 0, data.Length));
                }
            }
        }

        public static string Decrypt(string encrypt, string key)
        {
            //Szyfrowanie metodą 3DES https://foxlearn.com/windows-forms/encryption-and-decryption-using-triple-des-in-csharp-376.html


            using (TripleDESCryptoServiceProvider tripleDESCryptoService = new TripleDESCryptoServiceProvider())
            {
                using (MD5CryptoServiceProvider hashMD5Provider = new MD5CryptoServiceProvider())
                {
                    byte[] byteHash = hashMD5Provider.ComputeHash(Encoding.UTF8.GetBytes(key));
                    tripleDESCryptoService.Key = byteHash;
                    tripleDESCryptoService.Mode = CipherMode.ECB;//CBC, CFB
                    byte[] byteBuff = Convert.FromBase64String(encrypt);
                    return Encoding.Unicode.GetString(tripleDESCryptoService.CreateDecryptor().TransformFinalBlock(byteBuff, 0, byteBuff.Length));
                }
            }
        }

        public static char cipher(char ch, int key)
        {

            // Szyfrowanie Kodem Cezara https://www.c-sharpcorner.com/article/caesar-cipher-in-c-sharp/


            if (!char.IsLetter(ch))
            {
                return ch;
            }

            char d = char.IsUpper(ch) ? 'A' : 'a';
            return (char)((((ch + key) - d) % 26) + d);


        }


        public static string Encipher(string input, int key)
        {

            // Szyfrowanie Kodem Cezara https://www.c-sharpcorner.com/article/caesar-cipher-in-c-sharp/

            string output = string.Empty;

            foreach (char ch in input)
                output += cipher(ch, key);

            return output;
        }

        public static string Decipher(string input, int key)
        {

            // Szyfrowanie Kodem Cezara https://www.c-sharpcorner.com/article/caesar-cipher-in-c-sharp/

            return Encipher(input, 26 - key);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            //Boolean sprawdzający czy uzytkownik podał klucz
            bool bValidKey = ValidateKey();
            //Boolean sprawdzający czy uzytkownik podał klucz niezawierający liter 
            bool bValidLet = ValidateLetters();

            if (bValidKey == true)
            {
                if (menu1.Text == "3DES")
                {
                    //Jeżeli uzytkownik wybrał z listy 3DES szyfrujemy tym algorytmem używając klucza podanego przez użytkowanika
                    textBox2.Text = textBox1.Text;
                    textBox2.Text = Encrypt(textBox2.Text, textBox3.Text);
                }

                else if (menu1.Text == "Cezar")
                {
                    //Sprawdzamy czy w kluczu nie ma liter bo gdyby były "Cezar" nie zadziała
                    if (bValidLet == true)
                    {
                        //Jeżeli uzytkownik wybrał z listy "Cezara" szyfrujemy tym algorytmem używając klucza podanego przez użytkowanika
                        textBox2.Text = textBox1.Text;
                        textBox2.Text = Encipher(textBox2.Text, Convert.ToInt32(textBox3.Text));
                    }
                    //Jeśli w kluczu były litery użytkownik zostaje o tym powiadomiony wysakaującym okienkiem
                    else MessageBox.Show("Klucz musi być wartością liczbową w przypadku Cezara");

                }
            }
            //Jeżeli użytkownik nie podał klucza zostaje o tym powiadomiony wysakaującym okienkiem
            else MessageBox.Show("Musisz podać klucz");
        }

        private void button8_Click(object sender, EventArgs e)
        {
            //Kod tutaj jest prawie że identyczny tylko zamiast metod szyfrowania używamy odszyfrowywania
            bool bValidKey = ValidateKey();
            bool bValidLet = ValidateLetters();

            if (bValidKey == true)
            {
                if (menu1.Text == "3DES")
                {
                    textBox2.Text = textBox1.Text;
                    textBox2.Text = Decrypt(textBox2.Text, textBox3.Text);
                }

                else if (menu1.Text == "Cezar")
                {
                    if (bValidLet == true)
                    {
                        textBox2.Text = textBox1.Text;
                        textBox2.Text = Decipher(textBox2.Text, Convert.ToInt32(textBox3.Text));
                    }
                    else MessageBox.Show("Klucz musi być wartością liczbową w przypadku Cezara");
                }
            }
            else MessageBox.Show("Musisz podać klucz");
        }

        private void textBox3_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ValidateKey();
        }

        private bool ValidateKey()
        {
            //Jeżeli w oknie tekstowym nie ma tekstu użytkownik zobaczy stosowną wiadomość
            bool keystat = true;
            if (textBox3.Text == "")
            {
                errorProvider1.SetError(textBox3, "Musisz podać klucz");
                keystat = false;
            }
            else
                errorProvider1.SetError(textBox3, "");
            return keystat;
        }

        private bool ValidateLetters()
        {
            bool letstat = true;
            string t = textBox3.Text;
            //Metoda TryParse sprawdza czy można przkonwertować wartość pola tekstowego na integer
            if (int.TryParse(t, out _))
            {
                //warunek prawdziwy jeśli nie ma liter w tekscie
                return letstat;
            }
            else
            {
                letstat = false;
            }
            return letstat;
        }

    }

}
