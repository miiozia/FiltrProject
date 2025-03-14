using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;




namespace projektFiltr
{
    public partial class Filtr : Form
    {
        [DllImport(@"C:\Users\imart\Documents\projektFiltr\x64\Debug\FiltrCPP.dll")]
        static extern void convertInCpp(int[] red, int[] green, int[] blue, int start, int end);
        [DllImport(@"C:\Users\imart\Documents\projektFiltr\x64\Debug\JAAsm.dll")]
        static extern void GrayOUT(int[] red, int[] green, int[] blue, int start, int end);
        public Filtr()
        {
            InitializeComponent();
            InitializeControls();
            trackBar1.Value=Environment.ProcessorCount;
            label4.Text= Environment.ProcessorCount.ToString();
        }
        private void InitializeControls()
        {
            // Ustawienie właściwości PictureBox1
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;

            // Ustawienie właściwości PictureBox2
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;

            // Dodanie obsługi zdarzenia dla przycisku "Otwórz plik"
            button1.Click += Button1_Click;

            // Dodanie obsługi zdarzenia dla przycisku "Użyj filtra"
            button2.Click += Button2_Click;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            // Kod obsługujący kliknięcie przycisku "Otwórz plik"
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files (*.jpg; *.png; *.bmp)|*.jpg;*.png;*.bmp|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Wczytaj obraz i wyświetl go w PictureBox1
                pictureBox1.Image = new Bitmap(openFileDialog.FileName);
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            // Kod obsługujący kliknięcie przycisku "Użyj filtra"
            if (pictureBox1.Image != null)
            {
                // Przeczytaj piksele z obrazu i przelicz na obraz monochromatyczny
                Bitmap originalBitmap = new Bitmap(pictureBox1.Image);
                Bitmap monochromeBitmap = ConvertToMonochrome(originalBitmap);

                // Wyświetl obraz monochromatyczny w PictureBox2
                pictureBox2.Image = monochromeBitmap;
            }
        }

        //pobiera składowe kolorów i zapisuje je w tablicach
        private void takeColors(Bitmap mapbyte, int[] red, int[] green, int[] blue)
        {
            // Inicjalizacja indeksu do śledzenia pozycji w tablicach kolorów.
            int index = 0;
            // Pętla po szerokości obrazu.
            for (int i = 0; i < mapbyte.Width; i++)
            {
                // Pętla po wysokości obrazu.
                for (int j = 0; j < mapbyte.Height; j++)
                {
                    // Pobierz kolor bieżącego piksela.
                    Color originalColor = mapbyte.GetPixel(i, j);
                    // Zapisz składowe czerwone, zielone i niebieskie w odpowiednich tablicach.
                    red[index] = originalColor.R;
                    green[index] = originalColor.G;
                    blue[index] = originalColor.B;
                    // Inkrementuj indeks dla kolejnej pozycji w tablicach.
                    index++;
                }
            }
        }

        //przetwarza tablice składowych kolorów za pomocą wielu wątków
        private void applyMONO(int[] red, int[] green, int[] blue, int size, int threadCount, bool _asm)
        {
            // Oblicz rozmiar porcji przetwarzanej przez każdy wątek.
            int chunkSize = size / threadCount;
            // Wykonaj równoległą pętlę dla liczby wątków.
            Parallel.For(0, threadCount, i =>
            {

                // Oblicz początkowy i końcowy indeks dla bieżącego wątku.
                int start = i * chunkSize;
                int end = (i == threadCount - 1) ? size : (i + 1) * chunkSize;

                // Wywołaj odpowiednią funkcję przetwarzającą w zależności od wartości flagi _asm.
                if (_asm)
                {
                    // Użyj implementacji w języku C++.
                    convertInCpp(red, green, blue, start, end);

                }
                else
                {
                    // Użyj implementacji w języku asm.
                    GrayOUT(red, green, blue, start, end);
                }

            });

        }

        private Bitmap ConvertToMonochrome(Bitmap original)
        {
            // Utwórz nowy obraz monochromatyczny o takich samych wymiarach co oryginał.
            Bitmap monochromeBitmap = new Bitmap(original.Width, original.Height);

            // Oblicz rozmiar tablic składowych kolorów.
            int size = original.Width * original.Height;
            // Inicjalizuj tablice na składowe kolorów.
            int[] red = new int[size];
            int[] green = new int[size];
            int[] blue = new int[size];
            int index = 0;

            // Zainicjuj indeks do śledzenia pozycji w tablicach.
            takeColors(original, red, green, blue);

            DateTime time = DateTime.Now;

            /*
            for (int i = 0; i < 10; i++)
            {
                // Zapisz długość tablic składowych kolorów do konsoli.
                Console.WriteLine(red[i].ToString() + " " + green[i].ToString() + " " + blue[i].ToString());
            }
            //red[0].ToString() + " " + red[1].ToString() + " " + red[2].ToString()
           
            Console.WriteLine(size);
            */
            // Zastosuj filtr monochromatyczny na tablicach składowych kolorów.
            applyMONO(red, green, blue, size, trackBar1.Value, checkBox1.Checked);

            /*
            Console.WriteLine("After:");
             Wyświetl wartości pierwszych trzech pikseli po zastosowaniu filtra.
            for (int i = 0; i < 10; i++)
            {
                // Zapisz długość tablic składowych kolorów do konsoli.
                Console.WriteLine(red[i].ToString() + " " + green[i].ToString() + " " + blue[i].ToString());
            }
            */
            // Ustaw nowe kolory w obrazie monochromatycznym.

            for (int i = 0; i < original.Width; i++)
            {
                for (int j = 0; j < original.Height; j++)
                {
                    Color newColor = Color.FromArgb((red[index]), red[index], red[index]);
                    monochromeBitmap.SetPixel(i, j, newColor);
                    index++;
                }
            }

            // Zarejestruj czas trwania operacji.
            DateTime endTime = DateTime.Now;
            TimeSpan timer = endTime - time;

            // Wyświetl czas trwania w etykiecie.
            label2.Text = $"Czas: timer {timer.TotalMilliseconds} ms";

            // Zwróć monochromatyczny obraz.
            return monochromeBitmap;

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click_1(object sender, EventArgs e)
        {

        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {
            
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            label4.Text = trackBar1.Value.ToString();
        }
    }
}
