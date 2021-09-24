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
    /// Interaction logic for KoZnaZna.xaml
    /// </summary>
    public partial class KoZnaZna : Window
    {
        public static string answA, answB, answC, answD;
        public static string answCorrect;
        public static int set; //0-game off 1-game on 2-game on, question off
        public static int time;
        public static DispatcherTimer timerGame;
        public static Random rng;
        public static int goodAnswers; //number of good answers
        public static string gameFilesPath;
        public static int questionNum;

        public static int currentPts;
        public static int totalPts;
        public static bool closingDef = false;
        public static bool answerOK = false;

        private const int NUM_OF_QUESTIONS = 15;

        ISet<int> doneQuestions = new SortedSet<int>();
        public KoZnaZna()
        {
            InitializeComponent();
            SetupTimers();
            gameFilesPath = Directory.GetCurrentDirectory() + "\\Data\\pitanja";
            rng = new Random();
            totalPts = MainWindow.points;
            currentPts = 0;
            set = 0;
            StartGame();
        }

        #region game_methods

        public void StartGame()
        {
            currentPts = 0;
            questionNum = 0;
            score_label.Text = totalPts.ToString();

            NextQuestion();
        }

        public void MarkCorrect()
        {
            for (int i = 0; i < 4; i++)
            {
                var parent = (Button)ItemsPanel.FindName("ans" + i.ToString());
                TextBlock tb = (TextBlock)parent.Content;

                if (tb.Text.Substring(3) == answCorrect)
                {
                    if(!answerOK) //nije igrac pogodio vec mu PC obelezava odgovor zuto
                        parent.Background = new SolidColorBrush(Color.FromRgb(250, 250, 82));
                    else
                        parent.Background = new SolidColorBrush(Color.FromRgb(82, 250, 108));

                    answerOK = false;
                }
            }
        }

        public void EndQuestion()
        {
            set = 2;
            MarkCorrect();

            ans0.IsEnabled = false;
            ans1.IsEnabled = false;
            ans2.IsEnabled = false;
            ans3.IsEnabled = false;

            timerGame.Stop();
            var delay = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
            delay.Start();
            delay.Tick += (sender, args) =>
            {
                delay.Stop();
                NextQuestion();
            };
        }

        public void NextQuestion()
        {
            set = 1;
            questionNum++;
            if (questionNum > 10)
            {
                EndGame();
                return;
            }
            tbkTitle.FontSize = 28;

            int qnumber;
            //choose next question randomly
            while (true)
            {
                qnumber = rng.Next(1, NUM_OF_QUESTIONS+1);
                if (!doneQuestions.Contains(qnumber))
                {
                    doneQuestions.Add(qnumber);
                    break;
                }
            }

            //read question file

            var lines = File.ReadAllLines(gameFilesPath + "\\" + "k"+qnumber.ToString()+".kzz", Encoding.Default);
            tbkTitle.Text = questionNum.ToString() + ". " + lines[0];
            if (tbkTitle.Text.Length > 33)
            {
                tbkTitle.FontSize -= 4;
            }
            answCorrect = lines[1];
            //get answers and mix 
            string[] answers = lines.Skip(1).ToArray();
            string[] mixedAnswers = answers.OrderBy(x => rng.Next()).ToArray();
            //set answers to item texts
            for (int i = 0; i < 4; i++)
            {
                var parent = (Button)ItemsPanel.FindName("ans" + i.ToString());
                TextBlock tb = (TextBlock)parent.Content;
                tb.Text = Char.ConvertFromUtf32(65 + i) + ") " + mixedAnswers[i];
            }

            //enable buttons
            ans0.IsEnabled = true;
            ans1.IsEnabled = true;
            ans2.IsEnabled = true;
            ans3.IsEnabled = true;

            ans0.Background = new SolidColorBrush(Color.FromRgb(126, 204, 252));
            ans1.Background = new SolidColorBrush(Color.FromRgb(126, 204, 252));
            ans2.Background = new SolidColorBrush(Color.FromRgb(126, 204, 252));
            ans3.Background = new SolidColorBrush(Color.FromRgb(126, 204, 252));


            time = 6;
            timerGame.Start();
        }

        public void EndGame()
        {
            timerGame.Stop();
            score_label.Text = (MainWindow.points + currentPts).ToString();

            MainWindow.points = totalPts;
            MessageBox.Show("Osvojili ste " + currentPts.ToString() + " poena.");
            closingDef = true;
            this.Close();
        }

        #endregion


        #region events

        private void TimerGame_Tick(object sender, EventArgs e)
        {
            time--;
            time_label.Text = time.ToString();
            if (time == 0)
            {
                EndQuestion();
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

        private void Ans_Click(object sender, RoutedEventArgs e)
        {
            if (set == 1)
            {
                Button b = (Button)sender;
                TextBlock tb = (TextBlock)b.Content;
                if (tb.Text.Substring(3) != answCorrect)
                {
                    b.Background = new SolidColorBrush(Color.FromRgb(250, 82, 82));
                    currentPts -= 3;
                    totalPts -= 3;
                    answerOK = false;
                }
                else
                {
                    currentPts += 6;
                    totalPts += 6;
                    answerOK = true;
                }
                score_label.Text = totalPts.ToString();
                EndQuestion();
            }
        }

        private void ButtonN_Click(object sender, RoutedEventArgs e)
        {
            if (set == 1)
               EndQuestion();
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

