using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace MotsCache
{
    /// <summary>
    /// Logique d'interaction pour PageChooseGridSize.xaml
    /// </summary>
    public partial class PageChooseGridSize : Page
    {
        static private Random gen = new Random();
        static private GameWindow wndGame;

        /* Cette liste est un observable collection car on veut
         * update notre liste de mots caches automatiquement */
        public ObservableCollection<string> CurrentList { get; set; }
        Dictionary<int, List<Grille>> dictGridSize = new Dictionary<int, List<Grille>>();

        public PageChooseGridSize()
        {
            InitializeComponent();
        }

        private void GridSizeButton_Clicked(object sender, RoutedEventArgs e)
        {
            int gridSize = Convert.ToInt32((sender as Button).Tag);
            List<Grille> gridPool = GetGridPool(gridSize);
            Grille selectedGrid = gridPool[gen.Next(0, gridPool.Count)];


            wndGame = new GameWindow(selectedGrid);
            wndGame.Show();

            // Ferme notre premiere fenetre, le MainMenu window.
            Application.Current.Windows[0].Close();
        }

        private void ShowGame(object sender, EventArgs e)
        {
            //this.Content = wndGame.Content;
            //DoubleAnimation animFullOpacity = new DoubleAnimation(1, TimeSpan.FromMilliseconds(900))
            //{ EasingFunction = new QuarticEase() };

            //wndGame.grdMain.BeginAnimation(OpacityProperty, animFullOpacity);

        }

        //private void 
        private List<Grille> GetGridPool(int gridSize)
        {
            List<Grille> pool = new List<Grille>();
            foreach (Grille grid in GrillesDeMots.ArrayMotCache)
                if (grid.GridSize == gridSize)
                    pool.Add(grid);
            return pool;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            CurrentList = new ObservableCollection<string>();
            // Genere le nombre de mots cachés selon le gridsize
            foreach (Grille grid in GrillesDeMots.ArrayMotCache)
            {
                if (dictGridSize.ContainsKey(grid.GridSize))
                    dictGridSize[grid.GridSize].Add(grid);
                else
                    dictGridSize.Add(grid.GridSize, new List<Grille>(new Grille[] { grid }));
            }

            // Montre le nombre de mots cachés par gridsize
            foreach (int key in dictGridSize.Keys)
            {
                string numGrilles;
                if (dictGridSize[key].Count == 1) numGrilles = "1 mot caché";
                else numGrilles = dictGridSize[key].Count + " mots cachés";

                if (key == 6) lbl6.Content = numGrilles;
                else if (key == 10) lbl10.Content = numGrilles;
                else if (key == 12) lbl12.Content = numGrilles;
                else if (key == 15) lbl15.Content = numGrilles;
                else if (key == 16) lbl16.Content = numGrilles;
            }

            this.DataContext = this;
        }

        // On montre les grilles disponibles selon le grid size
        private void ShowListMotsCaches(object sender, EventArgs e)
        {
            List<Grille> currentList = new List<Grille>();

            Button btn = sender as Button;
            if ((string)btn.Tag == "6")
                currentList = dictGridSize[6];
            else if ((string)btn.Tag == "10")
                currentList = dictGridSize[10];
            else if ((string)btn.Tag == "12")
                currentList = dictGridSize[12];
            else if ((string)btn.Tag == "15")
                currentList = dictGridSize[15];
            else if ((string)btn.Tag == "16")
                currentList = dictGridSize[16];

            CurrentList.Clear();
            foreach (Grille grid in currentList)
                CurrentList.Add(grid.Title);
        }

        private void ClearListMotsCaches(object sender, EventArgs e)
        {
            CurrentList.Clear();
        }


        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current.Windows[0] as MainMenu).TransitionPage(MainMenu.IFRAME_PAGE.MAIN_MENU);
        }
    }
}
