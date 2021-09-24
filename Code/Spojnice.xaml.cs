using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SlagalicaPC
{
    /// <summary>
    /// Interaction logic for Spojnice.xaml
    /// </summary>
    public partial class Spojnice : Window
    {
        public static double height;
        public static string[] sideA;
        public static string[] sideB;
        public static int time;
        public static string gameFilesPath;
        public static int set; //0-game off 1-game on
        public static char clicked; //a-left side clicked  b-right side clicked
        public static DispatcherTimer timerGame;
        public static int current; //number of item currently clicked
        public static Button btnCurrent; //item currently clicked
        public static Random rng;
        public static int correct; //number of good answers

        private const int NUM_OF_QUESTIONS = 5;

        private int currentPts;
        private int totalPts;
        bool closingDef = false;


        public Spojnice()
        {
            InitializeComponent();
            SetupTimers();
            gameFilesPath = Directory.GetCurrentDirectory() + "\\Data\\spojnice";
            rng = new Random();
            currentPts = 0;
            totalPts = MainWindow.points;
            score_label.Text = totalPts.ToString();
            StartGame();
        }

        protected void StartGame()
        {
            int fontSize;
            correct = 0;
            int n = rng.Next(1, NUM_OF_QUESTIONS+1);
            string fileT = "f" + n.ToString() + ".spoj";
            string fileA = "f" + n.ToString() + "A.spoj";
            string fileB = "f" + n.ToString() + "B.spoj";
            StreamReader srA = new StreamReader(gameFilesPath + "\\" + fileA, Encoding.Default); //left side
            StreamReader srB = new StreamReader(gameFilesPath + "\\" + fileB, Encoding.Default); //right side
            StreamReader srT = new StreamReader(gameFilesPath + "\\" + fileT, Encoding.Default); //title

            //read title and items and fontSize from file
            sideA = new string[8];
            sideB = new string[8];
            for (int i = 0; i < 8; i++)
            {
                sideA[i] = srA.ReadLine();
                sideB[i] = srB.ReadLine();
            }
            tbkTitle.Text = srT.ReadLine();
            tbkTitle.FontSize = 20;
            try
            {
                fontSize = Int32.Parse(srT.ReadLine());
            }
            catch
            {
                fontSize = 24;
            }

            srA.Close(); srA.Dispose();
            srB.Close(); srB.Dispose();
            srT.Close(); srT.Dispose();

           

            //set items
            string[] mixedSideA = sideA.OrderBy(x => rng.Next()).ToArray();
            string[] mixedSideB = sideB.OrderBy(x => rng.Next()).ToArray();
            for (int i = 0; i < 8; i++)
            {
                Button a = (Button)ItemsPanel.FindName("a" + i.ToString());
                Button b = (Button)ItemsPanel.FindName("b" + i.ToString());
                TextBlock tbA = (TextBlock)a.Content;
                TextBlock tbB = (TextBlock)b.Content;

                //set items text and fontsize
                tbA.FontSize = fontSize;
                tbB.FontSize = fontSize;
                tbA.Text = mixedSideA[i];
                tbB.Text = mixedSideB[i];


                //default buttons
                a.Background = new SolidColorBrush(Color.FromRgb(126, 204, 252));
                b.Background = new SolidColorBrush(Color.FromRgb(126, 204, 252));
                a.IsEnabled = true;
                b.IsEnabled = true;

                clicked = 'b';


            }

            time = 60;
            timerGame.Start();
            set = 1;

        }

        protected void CheckEnd()
        {
            bool end = true;
            foreach (Button b in Utility.GetChildren<Button>(ItemsPanel))
            {
                if (b.IsEnabled)
                {
                    if (b.Name.IndexOf('a') == 0)
                    {
                        end = false;
                        break;
                    }
                }
            }
            if (end)
            {
                EndGame();
            }
        }

        protected void EndGame()
        {

            timerGame.Stop();

            //auto match all pairs
            string[] missedB = new string[8]; //all missed from side b
            string[] missedA = new string[8]; //all missed from side a

            
            for (int i = 0; i < 8; i++)
            {
                Button a = (Button)ItemsPanel.FindName("a" + i.ToString());
                Button b = (Button)ItemsPanel.FindName("b" + i.ToString());
                TextBlock tbA = (TextBlock)a.Content;
                TextBlock tbB = (TextBlock)b.Content;
                if (((SolidColorBrush)a.Background).Color != Color.FromRgb(82, 250, 108))
                {

                    missedA[i] = tbA.Text;
                }
                if (((SolidColorBrush)b.Background).Color != Color.FromRgb(82, 250, 108))
                {
                    missedB[i] = tbB.Text;
                }
                if (b.IsEnabled)
                {
                    b.IsEnabled = false;
                }

            }
            score_label.Text = totalPts.ToString();
            for (int i = 0; i < 8; i++)
            {
                Button a = (Button)ItemsPanel.FindName("a" + i.ToString());
                Button b = (Button)ItemsPanel.FindName("b" + i.ToString());
                TextBlock tbA = (TextBlock)a.Content;
                TextBlock tbB = (TextBlock)b.Content;

                tbA.Text = sideA[i];
                tbB.Text = sideB[i];

                if (IndexOf(missedA, tbA.Text) >= 0)
                {
                    a.Background = new SolidColorBrush(Color.FromRgb(250, 82, 82));

                }
                else
                {
                    a.Background = new SolidColorBrush(Color.FromRgb(82, 250, 108));
                }
                if (IndexOf(missedB, tbB.Text) >= 0)
                {
                    b.Background = new SolidColorBrush(Color.FromRgb(250, 82, 82));
                }
                else
                {
                    b.Background = new SolidColorBrush(Color.FromRgb(82, 250, 108));
                }
            }
            MessageBox.Show("Povezali ste tačno " + correct.ToString() + " parova.\n"+"Osvojili ste "+currentPts.ToString()+" poena.");

            MainWindow.points = totalPts;
            closingDef = true;
            this.Close();
           
        }

        #region events
        private void TimerGame_Tick(object sender, EventArgs e)
        {
            time--;
            time_label.Text = time.ToString();
            if (time == 0)
            {
                MessageBox.Show("Vreme je isteklo!");
                EndGame();
            }

        }

        private void A_Click(object sender, RoutedEventArgs e)
        {
            if (set == 1)
            {
                if (btnCurrent != null)
                {

                    btnCurrent.Background = new SolidColorBrush(Color.FromRgb(126, 204, 252));
                    btnCurrent.BorderThickness = new Thickness(3);

                }
                Button a = (Button)sender;
                TextBlock tbA = (TextBlock)a.Content;
                string item = tbA.Text;
                current = IndexOf(sideA, item);
                a.Background = new SolidColorBrush(Color.FromRgb(250, 250, 82));
                btnCurrent = a;

                clicked = 'a';

            }
        }

        private void B_Click(object sender, RoutedEventArgs e)
        {
            if (set == 1)
            {
                if (clicked == 'a')
                {
                    Button b = (Button)sender;
                    TextBlock tbB = (TextBlock)b.Content;
                    string item = tbB.Text;
                    int index = IndexOf(sideB, item);
                    if (index == current) //hit
                    {
                        currentPts += 4;
                        totalPts += 4;
                        score_label.Text = totalPts.ToString();
                        btnCurrent.Background = new SolidColorBrush(Color.FromRgb(82, 250, 108));
                        b.Background = new SolidColorBrush(Color.FromRgb(82, 250, 108));
                        b.IsEnabled = false;
                        correct++;

                    }
                    else //miss
                    {
                        btnCurrent.Background = new SolidColorBrush(Color.FromRgb(250, 82, 82));
                        b.Background = new SolidColorBrush(Color.FromRgb(126, 204, 252));

                    }

                    btnCurrent.IsEnabled = false;
                    btnCurrent = null;


                    clicked = 'b';
                }
                CheckEnd();
            }
        }

        private void A_MouseEnter(object sender, MouseEventArgs e)
        {
            if (clicked == 'b')
            {
                Button a = (Button)sender;
                if (a.IsEnabled)
                    a.Background = new SolidColorBrush(Color.FromRgb(250, 250, 82));
            }

        }
        private void B_MouseEnter(object sender, MouseEventArgs e)
        {
            if (clicked == 'a')
            {
                Button b = (Button)sender;
                if (b.IsEnabled)
                    b.Background = new SolidColorBrush(Color.FromRgb(250, 250, 82));
            }
        }

        private void A_MouseLeave(object sender, MouseEventArgs e)
        {
            if (clicked == 'b')
            {
                Button a = (Button)sender;
                if (a.IsEnabled)
                    a.Background = new SolidColorBrush(Color.FromRgb(126, 204, 252));
            }
        }

        private void B_MouseLeave(object sender, MouseEventArgs e)
        {
            if (clicked == 'a')
            {
                Button b = (Button)sender;
                if (b.IsEnabled)
                    b.Background = new SolidColorBrush(Color.FromRgb(126, 204, 252));
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!closingDef)
            {
                var confirmation = MessageBox.Show("Da li ste sigurni da želite da želite da izađete? Prekinućete trenutnu partiju.", "Potvrdi", MessageBoxButton.YesNo);
                if (confirmation == MessageBoxResult.Yes)
                {
                    MainWindow.gameON = false;
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        #endregion

        #region utility
        public int IndexOf(string[] arr, string item)
        {
            int i = 0;
            while (i < arr.Length && arr[i] != item)
            {
                i++;
            }
            if (i < arr.Length)
            {
                return i;
            }
            else
            {
                return -1;
            }
        }

        private void SetPoints(int newPts)
        {
            currentPts = newPts;
            score_label.Text = currentPts.ToString();
        }

        public void SetupTimers()
        {
            timerGame = new DispatcherTimer();
            timerGame.Tick += TimerGame_Tick;
            timerGame.Interval = new TimeSpan(0, 0, 1);
        }
        #endregion
    }
}
