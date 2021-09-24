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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SlagalicaPC
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static bool gameON;
        public static int points;
      
        public MainWindow()
        {
            InitializeComponent();
        }

        #region game_methods
        protected void StartGame()
        {
            gameON = true;

             points = 0;

            if (gameON)
            {
                Slagalica game1 = new Slagalica();
                game1.ShowDialog();
            }

            if (gameON)
            {
                MojBroj game2 = new MojBroj();
                game2.ShowDialog();
            }

            if (gameON)
            {
                Skocko game3 = new Skocko();
                game3.ShowDialog();
            }

            if (gameON)
            {
                Spojnice game4 = new Spojnice();
                game4.ShowDialog();
            }

            if (gameON)
            {
                KoZnaZna game5 = new KoZnaZna();
                game5.ShowDialog();
            }
            
            if (gameON)
            {
                Asocijacije game6 = new Asocijacije();
                game6.ShowDialog();
            }

            if (gameON)
            {
                UpdateHighscores();
            }

            gameON = false;
            

        }

        protected void EndGame()
        {

        }
        #endregion

        #region events
        #endregion

        #region utility
       
        #endregion

        private void BtnNewGame_Click(object sender, RoutedEventArgs e)
        {
            StartGame();
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void BtnScoreboard_Click(object sender, RoutedEventArgs e)
        {
            Results res = new Results();
            res.ShowDialog();
        }

        private void UpdateHighscores()
        {
            string line;
            int currentPts = MainWindow.points;
            string rankFile = Directory.GetCurrentDirectory() + "\\Data\\rang.slagalica";
            int[] points = new int[10];
            string[] names = new string[10];

            StreamReader sr = new StreamReader(rankFile);
            int i = 0, position;
            while ((line = sr.ReadLine()) != null)
            {
                string name = line.Substring(0, line.Length - (line.Substring(line.IndexOf('(')).Length));
                int pts = Int32.Parse(line.Substring(line.IndexOf('(') + 1, line.Length - name.Length - 2));
                names[i] = name;
                points[i++] = pts;
            }
            sr.Close();
            sr.Dispose();
            for(position=0; position<=i; position++)
            {
                if (points[position] < currentPts)
                    break;
            }
            if(position<=i) //postignut je novi highscore
            {
                string name="";
                Dialog d = new Dialog();
                d.ShowDialog();
                if(d.DialogResult.HasValue && d.DialogResult.Value)
                {
                    name = d.getName();
                }
                StreamWriter sw = new StreamWriter(rankFile);
                for(i=0; i<10; i++)
                {
                    if (i != position)
                        sw.WriteLine(names[i] + "(" + points[i].ToString() + ")");
                    else
                        sw.WriteLine(name + "(" + currentPts.ToString() + ")");
                }
                sw.Close();
                sw.Dispose();
            }
        }
    }
}
