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
using MojBroj;
using SlagalicaPC.MojBrojSolve;

namespace SlagalicaPC
{
    /// <summary>
    /// Interaction logic for MojBroj.xaml
    /// </summary>
    public partial class MojBroj : Window
    {
        public static int set; //1-choosing numbers 2-on
        public static int target;
        public static int[] numbers;
        public static string[] exprStack;
        public static int top;
        public static int time;
        public static DispatcherTimer timerNumbers;
        public static DispatcherTimer timerGame;
        public static Random rng;
        public static char lastClicked; //n - number o-operation

        private int currentPts;
        private int totalPts;
        bool closingDef = false;

        public MojBroj()
        {
            InitializeComponent();


            SetupTimers();

            rng = new Random();

            currentPts = 0;
            totalPts = MainWindow.points;
            lblScore.Text = totalPts.ToString();
            StartNumbers();
            set = 1;
        }

        protected void StartGame()
        {
            time = 60;
            set = 2;
            timerGame.Start();

            exprStack = new string[40];
            top = 0;
            lastClicked = 'o';

            button_text.Text = "POTVRDI";
        }

        protected void EndGame()
        {
            timerGame.Stop();

            string aiSolution;
            int aiNumber=target;
            int x = -1;
            try
            {
                x = (int)(Utility.Evaluate(tbEquation.Text));
                if (x <= 9999)
                {
                    tbResult.Text = x.ToString();
                    tbResult.Background = new SolidColorBrush(Color.FromRgb(0, 255, 0));
                }
            }
            catch
            {
                tbResult.Background = new SolidColorBrush(Color.FromRgb(255, 0, 0));

            }

            int result = x;
            int diff = Math.Abs(result - target);

             var aiRes = Model.Solve(numbers, target);
             int solutionCnt = aiRes.Solutions.Count;

             if(solutionCnt==0)
             {
                 aiSolution = aiRes.ClosestMatch;
                 //if solution isn't found calculate what is the closest match
                 string cleanAi = aiSolution.Replace(" ", "");
                 cleanAi = cleanAi.Replace("×", "*");
                 cleanAi = cleanAi.Replace("÷", "/");
                 aiNumber = (int)(Utility.Evaluate(cleanAi));


             }
             else
             {
                 aiSolution = aiRes.Solutions[0].ToString();
             }


            if (result == -1)
                currentPts = 0;

            else
            {
                if (diff==0) currentPts = 30;
                else if (diff <= 3) currentPts = 20;
                else if (diff <= 5) currentPts = 10;
                else if (diff <= 10) currentPts = 5;
            }
            totalPts += currentPts;
            lblScore.Text = totalPts.ToString();
            if (diff == 0)
            {
                MessageBox.Show("Tačan broj.\nOsvojili ste " + currentPts.ToString() + " poena."
                    +"\nResenje kompjutera:\n"+aiSolution + " = " + aiNumber.ToString());

            }
            else
            {
                MessageBox.Show("Promašili ste za "+diff.ToString()+".\n"+"Osvojili ste " + currentPts.ToString() + " poena."
                    + "\nResenje kompjutera:\n" + aiSolution + " = " + aiNumber.ToString());
            }
            MainWindow.points = totalPts; 
            closingDef = true;
            this.Close();
        }
        protected void StartNumbers()
        {
            timerNumbers.Start();
            time = 60;
            time_label.Text = time.ToString();
        }

        protected void StopNumbers()
        {
            timerNumbers.Stop();
            target = Int32.Parse(btnNumTarget.Content.ToString());
            numbers = new int[6];
            numbers[0] = Int32.Parse(btnNum1.Content.ToString());
            numbers[1] = Int32.Parse(btnNum2.Content.ToString());
            numbers[2] = Int32.Parse(btnNum3.Content.ToString());
            numbers[3] = Int32.Parse(btnNum4.Content.ToString());
            numbers[4] = Int32.Parse(btnNum5.Content.ToString());
            numbers[5] = Int32.Parse(btnNum6.Content.ToString());

            StartGame();
        }


        #region events
        private void Number_Click(object sender, EventArgs e)
        {
            if (top < 39)
            {
                Button b = (Button)sender;
                if (b.Name.IndexOf("Num") >= 0)
                {
                    if (lastClicked == 'o')
                    {
                        lastClicked = 'n';
                        tbEquation.Text += b.Content.ToString();
                        b.IsEnabled = false;
                        exprStack[top++] = b.Content.ToString();
                        return;
                    }
                    return;
                }
                lastClicked = 'o';
                tbEquation.Text += b.Content.ToString();
                exprStack[top++] = b.Content.ToString();
            }
        }
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

        private void TimerNumbers_Tick(object sender, EventArgs e)
        {
            var rnd = new Random();
            btnNum1.Content = rng.Next(1, 10).ToString();
            btnNum2.Content = rng.Next(1, 10).ToString();
            btnNum3.Content = rng.Next(1, 10).ToString();
            btnNum4.Content = rng.Next(1, 10).ToString();
            int ctrl = rng.Next(1, 4);
            switch(ctrl)
            {
                case 1: btnNum5.Content = 15;
                    break;
                case 2:
                    btnNum5.Content = 10;
                    break;
                case 3:
                    btnNum5.Content = 20;
                    break;
            }
            ctrl = rng.Next(1, 4);
            switch (ctrl)
            {
                case 1:
                    btnNum6.Content = 25;
                    break;
                case 2:
                    btnNum6.Content = 75;
                    break;
                case 3:
                    btnNum6.Content = 100;
                    break;
            }
            btnNumTarget.Content = rng.Next(1, 1000);
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

        public void SetupTimers()
        {
            timerNumbers = new DispatcherTimer();
            timerNumbers.Tick += TimerNumbers_Tick;
            timerNumbers.Interval = new TimeSpan(0, 0, 0, 0, 100);

            timerGame = new DispatcherTimer();
            timerGame.Tick += TimerGame_Tick;
            timerGame.Interval = new TimeSpan(0, 0, 1);
        }

        private void ButtonDel_Click(object sender, EventArgs e)
        {
            try
            {
                string x = exprStack[--top];

                foreach (Button b in Utility.GetChildren<Button>(numbersPanel))
                {
                    if (b.Content.ToString() == x && b.IsEnabled == false)
                    {
                        b.IsEnabled = true;
                        lastClicked = 'o';
                        break;
                    }

                }
                if(top>0)
                {
                    string y = exprStack[top-1];
                    if(IsOperation(y))
                    {
                        lastClicked = 'o';
                    }
                    else
                    {
                        lastClicked = 'n';
                    }
                }
                else
                {
                    lastClicked = 'o';
                }
                tbEquation.Text = tbEquation.Text.Substring(0, tbEquation.Text.Length - x.Length);
            }
            catch
            {
                top++;
                //empty
            }
            
        }

        public bool IsOperation(string str)
        {
            if (str == "/" || str == "+" || str == "*" || str == "-" || str == "(" || str == ")")
                return true;
            return false;
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

        private void ButtonControl_Click(object sender, RoutedEventArgs e)
        {
            if(set==1) //shuffling numbers
            {
                StopNumbers();
            }
            else //ingame
            {
                var confirmation = MessageBox.Show("Da li ste sigurni da želite da potvrdite broj?", "Potvrdi", MessageBoxButton.YesNo);
                if (confirmation == MessageBoxResult.Yes)
                {
                    EndGame();
                }
            }
        }

        private void TbEquation_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                int x = (int)Utility.Evaluate(tbEquation.Text);
                if (x <= 9999)
                {
                    tbResult.Text = x.ToString();
                    tbResult.Background = new SolidColorBrush(Color.FromRgb(0, 255, 0));
                }
            }
            catch
            {
                tbResult.Background = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            }
        }
    }
}
