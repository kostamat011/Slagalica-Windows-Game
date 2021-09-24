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
    /// Interaction logic for Slagalica.xaml
    /// </summary>
    public partial class Slagalica : Window
    {

        public static int set; //0-off 1-choosing letters 2-on
        public static string currentWord;
        public static string startWord;
        public static int time;
        public static string appFolderPath;
        public static string resourceFolderPath;
        public static DispatcherTimer timerLetters;
        public static DispatcherTimer timerGame;
        public static Random rng;
        public static int points;
        public static int totalPoints;

        bool closingDef=false;


        public Slagalica()
        {
            InitializeComponent();
            

            SetupTimers();

            //hide buttons only visible ingame
            button_del.Visibility = Visibility.Hidden;
            button_check.Visibility = Visibility.Hidden;

            rng = new Random();

            //get resources path
            appFolderPath = Directory.GetCurrentDirectory();
            resourceFolderPath = appFolderPath + "\\Data\\Slova";

            StartLetters();
            points = 0;
            totalPoints = MainWindow.points;
            set = 1;
        }

        #region game_methods
        public void StartGame()
        {

            //interface changes
            button_del.Visibility = Visibility.Visible;
            button_check.Visibility = Visibility.Visible;
            button_text.Text = "POTVRDI REC";
            //------------

            score_label.Text = MainWindow.points.ToString();
            timerGame.Start();
            set = 2;
        }

        protected void EndGame()
        {
            timerGame.Stop();
            set = 0;

            int AIlength = GetLengthLatin(startWord); //regulisanje duzine kompjuterove reci za LJ i NJ

            if (!CheckWord(currentWord))
            {
                MessageBox.Show("Vaša reč nije pronađena u bazi.\nOsvojili ste 0 poena." +
                    "\nKompjuter je pronašao reč "+startWord+"("+AIlength.ToString()+" slova)");
            }

            else
            {
                int length = GetLengthLatin(currentWord); //regulisanje duzine korisnikove reci za LJ i NJ

                points = length * 2;

                if (length == AIlength)
                {
                    points += 5;
                }

                score_label.Text = points.ToString();

                MessageBox.Show("Vaša reč je "+currentWord+"("+length+" slova)\n" +"Osvojili ste "+points.ToString()+" poena." +
                  "\nKompjuter je pronašao reč " + startWord + "(" + GetLengthLatin(startWord).ToString() + " slova)");
            }

            MainWindow.points = points;

            closingDef = true;
            this.Close();

        }

        protected bool CheckWord(string word)
        {
            string temp;
            StreamReader sr = new StreamReader(resourceFolderPath + "\\sr-Latn.dic");
            while ((temp = sr.ReadLine()) != null)
            {
                if (word.ToUpper() == temp.ToUpper())
                {
                    sr.Close();
                    sr.Dispose();
                    return true;
                }
            }
            sr.Close();
            sr.Dispose();
            return false;

        }

        protected void StartLetters()
        {
            timerLetters.Start();

            //interface changes
            time = 60;
            time_label.Text = time.ToString();
            currentWord = tbCurrentWord.Text = "";
            button_del.Visibility = Visibility.Hidden;
            button_check.Visibility = Visibility.Hidden;
            foreach (Button b in Utility.GetChildren<Button>(letterPanel))
            {
                b.IsEnabled = true;
            }
            //--------------------

            startWord = GetStartingWord();
            set = 1;
        }

        protected void StopLetters()
        {
            timerLetters.Stop();
            startWord = GetStartingWord();
            string mixedWord = MixWord(startWord); //mix current word and add random letters till 12
            int i = 0;
            foreach (Button b in Utility.GetChildren<Button>(letterPanel))
            {
                if (i < mixedWord.Length)
                {
                    b.Content = mixedWord[i++];
                }
                else
                {
                    b.Content = GetRandomLatinLetter();
                }
            }
        }

        protected string GetStartingWord()
        {
            string word = "a";

            var lines = File.ReadAllLines(resourceFolderPath + "\\sr-Latn.dic");

            while (word.Length < 9 || word.Length > 12)
            {
                int line = rng.Next(2, lines.Length);
                word = lines[line].ToUpper();
            }

            return word;

        }
        #endregion

        #region events
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                TextBox temp = (TextBox)Layout.FindName("focus_shift");
                temp.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ButtonCheck_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckWord(currentWord))
            {

                tbCurrentWord.Background = new SolidColorBrush((Color.FromRgb(242, 52, 48)));
            }
            else
            {
                tbCurrentWord.Background = new SolidColorBrush((Color.FromRgb(64, 227, 88)));

            }
        }

        private void ButtonLetter_Click(object sender, RoutedEventArgs e)
        {
            if (set == 2)
            {
                Button b = (Button)sender;
                string slovo = b.Content.ToString();
                currentWord += slovo;
                tbCurrentWord.Text = currentWord;
                b.IsEnabled = false;
            }
        }

        private void TimerLetters_Tick(object sender, EventArgs e)
        {
            var rnd = new Random();
            foreach (Button b in Utility.GetChildren<Button>(letterPanel))
            {

                b.Content = GetRandomLatinLetter();
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

        private void ButtonControl_Click(object sender,EventArgs e)
        {
   
            if (set == 1) //shuffle letters
            {
                StopLetters();
                StartGame();
            }
            else //set==2, game on
            {
                var confirmation = MessageBox.Show("Da li ste sigurni da želite da potvrdite reč?", "Potvrdi", MessageBoxButton.YesNo);
                if (confirmation == MessageBoxResult.Yes)
                {
                    EndGame();
                }
            }
        }

        private void ButtonDel_Click(object sender, EventArgs e)
        {
            bool removedOne = false;
            if (currentWord.Length >= 1 && set == 2)
            {
                string removedChar = currentWord[currentWord.Length - 1].ToString();
                if (currentWord[currentWord.Length - 1] == 'J')
                {
                    if (currentWord[currentWord.Length - 2] == 'N')
                    {
                        currentWord = currentWord.Remove(currentWord.Length - 2);
                        removedChar = "NJ";
                    }
                    else if (currentWord[currentWord.Length - 2] == 'L')
                    {
                        currentWord = currentWord.Remove(currentWord.Length - 2);
                        removedChar = "LJ";
                    }
                }
                else
                {
                    currentWord = currentWord.Remove(currentWord.Length - 1);
                }
                foreach (Button b in Utility.GetChildren<Button>(letterPanel))
                {

                    if (b.IsEnabled == false && b.Content.ToString() == removedChar)
                    {
                        if (!removedOne)
                        {
                            b.IsEnabled = true;
                            tbCurrentWord.Text = currentWord;
                            removedOne = true;
                        }

                    }
                }
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
        

        public void SetupTimers()
        {
            timerLetters = new DispatcherTimer();
            timerLetters.Tick += TimerLetters_Tick;
            timerLetters.Interval = new TimeSpan(0, 0, 0, 0, 100);

            timerGame = new DispatcherTimer();
            timerGame.Tick += TimerGame_Tick;
            timerGame.Interval = new TimeSpan(0, 0, 1);
        }

        public string GetRandomLatinLetter()
        {

            string val;

            //samoglasnik
            int r = rng.Next(0, 8);

            if (r == 0) val = "A";
            else if (r == 1) val = "E";
            else if (r == 2) val = "I";
            else if (r == 3) val = "O";
            else if (r == 4) val = "U";

            else
            {

                int a = rng.Next(0, 28);
                if (a == 26) val = "Ć";
                else if (a == 27) val = "Č";
                else if (a == 28) val = "Đ";
                else
                {
                    char c = (char)('A' + a);
                    if (c == 'W') val = "NJ";
                    else if (c == 'Q') val = "LJ";
                    else if (c == 'Y') val = "Š";
                    else if (c == 'X') val = "Ž";
                    else val = c.ToString();
                }
            }
            return val;
        }

        public string MixWord(string word)
        {
            char[] newWord = new char[12];
            for (int i = 0; i < 12; i++)
            {
                newWord[i] = '.';
            }
            while (word.Length < 12)
            {
                word = word + GetRandomLatinLetter();
            }
            int pos = rng.Next(0, 12);

            for (int i = 0; i < 12; i++)
            {
                while (newWord[pos] != '.')
                {
                    pos = rng.Next(0, 12);
                }

                newWord[pos] = word[i];
            }
            return new string(newWord);
        }

        public int GetLengthLatin(string word)
        {
            int length = word.Length;
            for (int i = 0; i < word.Length; i++)
            {
                if (word[i] == 'L' || word[i] == 'N')
                {
                    if (i != word.Length - 1 && word[i + 1] == 'J')
                    {
                        length--;
                    }
                }
            }
            return length;
        }

        #endregion
    }


}
