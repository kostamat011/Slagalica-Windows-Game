using System;
using System.Collections.Generic;
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
using System.IO;

namespace SlagalicaPC
{
    /// <summary>
    /// Interaction logic for Results.xaml
    /// </summary>
    public partial class Results : Window
    {
        int currentPts;
        public Results()
        {
            InitializeComponent();
        }

     

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string line;
            currentPts = MainWindow.points;
            string rankFile = Directory.GetCurrentDirectory() + "\\Data\\rang.slagalica";
            int[] points = new int[10];

            StreamReader sr = new StreamReader(rankFile);
            int i = 0;
            while ((line = sr.ReadLine())!=null)
            {
                string name = line.Substring(0,line.Length-(line.Substring(line.IndexOf('(')).Length));
                int pts = Int32.Parse(line.Substring(line.IndexOf('(')+1,line.Length-name.Length-2));
                points[i++] = pts;
                ScoreGrid.Items.Add(new { Name = name, Score=pts });
            }
            sr.Close();
            sr.Dispose();
        }
    }
}
