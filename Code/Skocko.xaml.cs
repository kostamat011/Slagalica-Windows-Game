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
    /// Interaction logic for Skocko.xaml
    /// </summary>


    public partial class Skocko : Window
    {

        public static int[] target;
        public static int[] comb;
        public static int time;
        public static DispatcherTimer timerGame;
        public static Random rng;
        public static int active;
        public static int prevActive;
        public static int correctPos;
        public static int correctSign;
        public static string imgDir;

        private int currentPts;
        private int totalPts;
        bool closingDef = false;


        public Skocko()
        {
            InitializeComponent();
            SetupTimers();
            imgDir = Directory.GetCurrentDirectory() + "\\Images";
            for (int i = 0; i < 6; i++)
            {
                Button b = (Button)ButtonsPanel.FindName("btn" + i.ToString());
                b.Background = new ImageBrush(new BitmapImage(new Uri(imgDir + "\\" + i.ToString() + ".png")));
            }
            btnCheck1.Background = new ImageBrush(new BitmapImage(new Uri(imgDir + "\\check1.png")));
            btnCheck1.Visibility = Visibility.Hidden;
            btnCheck2.Background = new ImageBrush(new BitmapImage(new Uri(imgDir + "\\check1.png")));
            btnCheck2.Visibility = Visibility.Hidden;
            btnCheck3.Background = new ImageBrush(new BitmapImage(new Uri(imgDir + "\\check1.png")));
            btnCheck3.Visibility = Visibility.Hidden;
            btnCheck4.Background = new ImageBrush(new BitmapImage(new Uri(imgDir + "\\check1.png")));
            btnCheck4.Visibility = Visibility.Hidden;
            btnCheck5.Background = new ImageBrush(new BitmapImage(new Uri(imgDir + "\\check1.png")));
            btnCheck5.Visibility = Visibility.Hidden;
            btnCheck6.Background = new ImageBrush(new BitmapImage(new Uri(imgDir + "\\check1.png")));
            btnCheck6.Visibility = Visibility.Hidden;
            totalPts = MainWindow.points;
            score_label.Text = totalPts.ToString();
            currentPts = 0;
            StartGame();
            

        }

        private void StartGame()
        {
            target = new int[4];
             rng = new Random();
             target[0] = rng.Next(0, 6);
             target[1] = rng.Next(0, 6);
             target[2] = rng.Next(0, 6);
             target[3] = rng.Next(0, 6);

            active = prevActive = 0;
            comb = new int[4];
            time = 90;
            timerGame.Start();
        }

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

        private void EndGame()
        {
            timerGame.Stop();

            Button act = ActiveIntToItem(prevActive-1);
            int comb_no = Int32.Parse(act.Name[1].ToString());

            for(int i=0; i<4; i++) //display correct combination
            {
                Button b = (Button)CorrectCombPanel.FindName("c" + i.ToString());
                b.Background = new ImageBrush(new BitmapImage(new Uri(imgDir + "\\" + target[i].ToString() + ".png")));
            }

            if (correctPos == 4) //win
            {
                currentPts = 20;
                currentPts += (5 - comb_no); //bonus points depending on number of tries
                totalPts += currentPts;
                score_label.Text = totalPts.ToString();
                MessageBox.Show("Pogodili ste tačnu kombinaciju.\nOsvojili ste " + currentPts.ToString() + " poena.");

            }
            else //lose
            {
                currentPts = correctPos;
                totalPts += currentPts;
                score_label.Text = totalPts.ToString();
                closingDef = false;
                MessageBox.Show("Niste pogodili tačnu kombinaciju.\nOsvojili ste " + currentPts.ToString() + " poena.");
            }



            MainWindow.points = totalPts;
            
            closingDef = true;
            this.Close();
        }

        private void Item_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            int sign = Int32.Parse(b.Name[3].ToString()); // btn0
            ImageBrush img = (ImageBrush)b.Background;

            Button act = ActiveIntToItem(active);
            if (act == null)
            {
                return;
            }
            act.Background = img;

            comb[Int32.Parse(act.Name[2].ToString())] = sign; //b02 0-number of combination 2-order in combination

            int prevprevActive = active;
            active++;
            prevActive = active;

            if (active % 4 == 0)
            {
                active = -1;
                Button prevAct = ActiveIntToItem(prevprevActive);
                int rowNum = Int32.Parse(prevAct.Name[1].ToString());
                switch (rowNum)
                {
                    case 0: btnCheck1.Visibility = Visibility.Visible; break;
                    case 1: btnCheck2.Visibility = Visibility.Visible; break;
                    case 2: btnCheck3.Visibility = Visibility.Visible; break;
                    case 3: btnCheck4.Visibility = Visibility.Visible; break;
                    case 4: btnCheck5.Visibility = Visibility.Visible; break;
                    case 5: btnCheck6.Visibility = Visibility.Visible; break;
                }

            }
            else
            {
                btnCheck1.Visibility = Visibility.Hidden;
                btnCheck2.Visibility = Visibility.Hidden;
                btnCheck3.Visibility = Visibility.Hidden;
                btnCheck4.Visibility = Visibility.Hidden;
                btnCheck5.Visibility = Visibility.Hidden;
                btnCheck6.Visibility = Visibility.Hidden;
            }
           
          
        }

        private void Check_Click(object sender, RoutedEventArgs e)
        {
            if(active!=-1)
            {
                return;
            }

            correctPos = NumberOfSamePositions(comb, target);
            correctSign =NumberOfSameElements(comb,target);

            correctSign -= correctPos;

          

            if(correctPos==4)
            {
                EndGame();
            }

            Button b = ActiveIntToItem(prevActive-1); //get current row
            int row = Int32.Parse(b.Name[1].ToString());
            int counter;
            for(counter=0; counter<correctPos; counter++) //color red for every correct position
            {
                Button res = (Button)ResultPanel.FindName("r" + row.ToString() + counter.ToString());
                res.Background = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            }
            for(;counter<correctPos+correctSign;counter++) //color remaining for every correct sign
            {
                Button res = (Button)ResultPanel.FindName("r" + row.ToString() + counter.ToString());
                res.Background = new SolidColorBrush(Color.FromRgb(255, 255, 0));
            }

            active = prevActive;
            btnCheck1.Visibility = Visibility.Hidden;
            btnCheck2.Visibility = Visibility.Hidden;
            btnCheck3.Visibility = Visibility.Hidden;
            btnCheck4.Visibility = Visibility.Hidden;
            btnCheck5.Visibility = Visibility.Hidden;
            btnCheck6.Visibility = Visibility.Hidden;
            if (row==5 && correctPos!=4)
            {
                EndGame();
            }
            
        }

        private int NumberOfSamePositions(int[] comb, int[] target)
        {
            int cnt = 0;
            for(int i=0; i<comb.Length; i++)
            {
                if(comb[i]==target[i])
                {
                    cnt++;
                }
            }
            return cnt;
        }

        private int NumberOfSameElements(int[] comb, int[] target)
        {
            int[] t = new int[4], c=new int[4];
            comb.CopyTo(c, 0);
            target.CopyTo(t, 0);

            int cnt = 0;
            for(int i=0; i<comb.Length; i++)
            {
                for(int j=0; j<comb.Length; j++)
                {
                    if(t[i]==c[j])
                    {
                        cnt++;
                        c[j] = -1;
                        t[i] = -1;
                        break;
                    }

                }
            }
            return cnt;
        }

        public void Del_click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            if(ActiveIntToItem(prevActive-1)==b)
            {
                if(active==-1)
                {
                    active = prevActive - 1;
                }
                else
                {
                    active--;
                }
                prevActive = active;
                comb[Int32.Parse(b.Name[2].ToString())] = -1;
                b.Background = new SolidColorBrush(Color.FromRgb(255,255,255));
            }
        }

        #endregion

        #region utility
        private Button ActiveIntToItem(int active)
        {
            if (active == -1)             //4 signs added waiting for check
                return null;

            int row = active / 4;
            int col = active % 4;
            Button b = (Button)CombinationsPanel.FindName("b" + row.ToString() + col.ToString());
            return b;
        }

        public void SetupTimers()
        {
            timerGame = new DispatcherTimer();
            timerGame.Tick += TimerGame_Tick;
            timerGame.Interval = new TimeSpan(0, 0, 1);
        }

        private bool Exists(int[] arr, int n)
        {
            for(int i=0; i<arr.Length; i++)
            {
                if(arr[i]==n)
                {
                    return true;
                }
            }
            return false;
        }

        private int FirstIndexOf(int[] arr, int n)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] == n)
                {
                    return i;
                }
            }
            return -1;
        }
        #endregion

        
    }
}
