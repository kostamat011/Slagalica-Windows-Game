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
    /// Interaction logic for Asocijacije.xaml
    /// </summary>

   
    public partial class Asocijacije : Window
    {


        public static string[] acol, bcol, ccol, dcol;
        public static string final;
        public static int time;
        public static string dataPath;
        public static DispatcherTimer timerLetters;
        public static DispatcherTimer timerGame;
        public static Random rng;

        private const int NUM_OF_QUESTIONS = 5;
        private int currentPts;
        private int totalPts;
        bool closingDef = false;

        public Asocijacije()
        {
            InitializeComponent();
            SetupTimers();
            dataPath = Directory.GetCurrentDirectory() + "\\Data\\asocijacije";

            totalPts = MainWindow.points;
            currentPts = 0;
            score_label.Text = totalPts.ToString();
            rng = new Random();
            StartGame();
        }

        #region game_methods
        protected void StartGame()
        {
            currentPts = 0;
            time = 100;
            string line;

            int taskNum  = rng.Next(1, NUM_OF_QUESTIONS+1);
            StreamReader sr = new StreamReader(dataPath + "\\"+taskNum.ToString()+".asoc", Encoding.Default);

            acol = new string[5];
            bcol = new string[5];
            ccol = new string[5];
            dcol = new string[5];

            line = sr.ReadLine();
            acol = line.Split('|');
            line = sr.ReadLine();
            bcol = line.Split('|');
            line = sr.ReadLine();
            ccol = line.Split('|');
            line = sr.ReadLine();
            dcol = line.Split('|');

            final = sr.ReadLine();

            sr.Dispose();


            timerGame.Start();
        }

        private void EndGame()
        {
            timerGame.Stop();
            RevealAll();
            score_label.Text = totalPts.ToString();

            MainWindow.points = totalPts;
            MessageBox.Show("Osvojili ste " + currentPts.ToString() + " poena.");

            closingDef = true;
            this.Close();
        }

        private void Field_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            char col = b.Name[0];
            int num = b.Name[1] - '0';

            TextBlock label = (TextBlock)b.Content;
            switch(col)
            {
                case 'a':
                    label.Text = acol[num-1];
                    break;
                case 'b':
                    label.Text = bcol[num-1];
                    break;
                case 'c':
                    label.Text = ccol[num-1];
                    break;
                case 'd':
                    label.Text = dcol[num-1];
                    break;
            }
            b.IsEnabled = false;
            currentPts -= 1;
            totalPts -= 1;
            score_label.Text = totalPts.ToString();

        }

        private void Header_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            TextBox tb = (TextBox)b.Content;
            tb.Focus();
        }

        private void RevealAll()
        {

            for (char col = 'a'; col < 'e'; col++)
            {
                WrapPanel colPnl = (WrapPanel)fullPanel.FindName(col + "Panel");

                for (int i = 1; i < 5; i++)
                {
                    Button btn = (Button)colPnl.FindName(col + i.ToString());
                    btn.Background = new SolidColorBrush(Color.FromRgb(0, 255, 0));
                    TextBlock tblk = (TextBlock)btn.Content;
                    switch (col)
                    {
                        case 'a':
                            tblk.Text = acol[i - 1];
                            break;
                        case 'b':
                            tblk.Text = bcol[i - 1];
                            break;
                        case 'c':
                            tblk.Text = dcol[i - 1];
                            break;
                        case 'd':
                            tblk.Text = ccol[i - 1];
                            break;
                    }
                }
                Button colFinal = (Button)(colPnl.FindName(col.ToString()));
                TextBox inpt = (TextBox)colFinal.Content;
                inpt.Background = new SolidColorBrush(Color.FromRgb(0, 255, 0));
                switch (col)
                {
                    case 'a':
                        inpt.Text = acol[4];
                        break;
                    case 'b':
                        inpt.Text = bcol[4];
                        break;
                    case 'c':
                        inpt.Text = ccol[4];
                        break;
                    case 'd':
                        inpt.Text = dcol[4];
                        break;
                }
                colFinal.Background = new SolidColorBrush(Color.FromRgb(0, 255, 0));
                inpt.IsEnabled = false;
                inpt.Text = inpt.Text.ToUpper();
            }
        }

        private void Header_TextChanged(object sender, TextChangedEventArgs e)
        {
            string correct="";
            TextBox inpt = (TextBox)sender;
            if (inpt.Name == "Final")
            {
                correct = final;
               
                if (inpt.Text.ToUpper() == correct)
                {
                    currentPts += 12;
                    totalPts += 12;
                    fldFinal.Background = new SolidColorBrush(Color.FromRgb(0, 255, 0));
                    EndGame();
                }
            }
            else
            {
                char col = inpt.Name[1];
                switch (col)
                {
                    case 'a':
                        correct = acol[4];
                        break;
                    case 'b':
                        correct = bcol[4];
                        break;
                    case 'c':
                        correct = ccol[4];
                        break;
                    case 'd':
                        correct = dcol[4];
                        break;
                }
                WrapPanel colPnl = (WrapPanel)fullPanel.FindName(col + "Panel");
                if (inpt.Text.ToUpper() == correct)
                {
                    for (int i = 1; i < 5; i++)
                    {
                        Button btn = (Button)colPnl.FindName(col + i.ToString());
                        btn.Background = new SolidColorBrush(Color.FromRgb(0, 255, 0));
                        TextBlock tblk = (TextBlock)btn.Content;
                        switch (col)
                        {
                            case 'a':
                                tblk.Text = acol[i - 1];
                                break;
                            case 'b':
                                tblk.Text = bcol[i - 1];
                                break;
                            case 'c':
                                tblk.Text = dcol[i - 1];
                                break;
                            case 'd':
                                tblk.Text = ccol[i - 1];
                                break;
                        }
                    }
                    Button colFinal = (Button)(colPnl.FindName(col.ToString()));
                    inpt.Background = new SolidColorBrush(Color.FromRgb(0, 255, 0));
                    colFinal.Background = new SolidColorBrush(Color.FromRgb(0, 255, 0));
                    inpt.IsEnabled = false;
                    currentPts += 6;
                    totalPts += 6;
                    score_label.Text = totalPts.ToString();
                    inpt.Text = inpt.Text.ToUpper();
                } //pogodak
                else
                {
                   //promasaj
                }
            }
        }
        #endregion

        #region events
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

        
        #endregion

        #region utility
        public void SetupTimers()
        {
            timerGame = new DispatcherTimer();
            timerGame.Tick += TimerGame_Tick;
            timerGame.Interval = new TimeSpan(0, 0, 1);
        }
        #endregion
    }
}
