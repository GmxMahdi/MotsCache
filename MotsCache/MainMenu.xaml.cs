using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MotsCache
{
    /// <summary>
    /// Logique d'interaction pour MainMenu.xaml
    /// </summary>
    public partial class MainMenu : Window
    {
        public enum IFRAME_PAGE
        {
            MAIN_MENU,
            PLAY
        };

        static private Random gen = new Random();
        static IFRAME_PAGE transitionPage;

        public MainMenu()
        {
            InitializeComponent();
        }

        private void GridSizeButton_Clicked(object sender, RoutedEventArgs e)
        {
            int gridSize = Convert.ToInt32((sender as Button).Tag);
            List<Grille> gridPool = GetGridPool(gridSize);
            Grille selectedGrid = gridPool[gen.Next(0, gridPool.Count)];


            GameWindow wndGame = new GameWindow(selectedGrid);
            wndGame.Show();
            this.Close();
        }

        // Trouver notre liste de Grille, selon la grosseur qu'on veut
        private List<Grille> GetGridPool(int gridSize)
        {
            List<Grille> pool = new List<Grille>();
            foreach (Grille grid in GrillesDeMots.ArrayMotCache)
                if (grid.GridSize == gridSize)
                    pool.Add(grid);
            return pool;
        }

        // Animation de page
        public void TransitionPage(IFRAME_PAGE page)
        {
            transitionPage = page;

            DoubleAnimation opacityFrame = new DoubleAnimation(0, TimeSpan.FromMilliseconds(300))
            { EasingFunction = new QuarticEase()};

            opacityFrame.Completed += SwitchPage;
            ifrPage.BeginAnimation(OpacityProperty, opacityFrame);
        }

        // Vrai transition de page
        private void SwitchPage(object sender, EventArgs e)
        {
            if (transitionPage == IFRAME_PAGE.PLAY)
                ifrPage.Source = new Uri("Pages/PageChooseGridSize.xaml", UriKind.Relative);
            else if (transitionPage == IFRAME_PAGE.MAIN_MENU)
                ifrPage.Source = new Uri("Pages/PageMainMenu.xaml", UriKind.Relative);
            else
                MessageBox.Show("La page n'etais pas trouvé :(");

            DoubleAnimation opacityFrame = new DoubleAnimation(1, TimeSpan.FromMilliseconds(300))
            { EasingFunction = new ExponentialEase() };
            ifrPage.BeginAnimation(OpacityProperty, opacityFrame);
        }
    }
}
