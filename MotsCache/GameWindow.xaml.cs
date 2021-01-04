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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MotsCache
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class GameWindow : Window
    {
        #region VARIABLES COMMUNES
        // Prefixes pour le nom des lettres
        private const string gridLetterNamePrefix = "GridChildLetter";

        /* Cette liste est un ObservableCollection car on veut que
        * notre liste de mots a droite soit updated automatiquement
        * car cette collection donne des alertes de changements.
        * est est public car elle doit être accesible au XAML (pour databind).
        */
        /* Ce { get; set; } reste sans contraintes écrites car cette liste porte deja ses propres contraines:
         il est tout simplement écrit comme cela pour que notre lsvCheats du XAML puisse référer à cette listre! */
        public ObservableCollection<string> ListWordsToFind { get; set; } // Ce get

        //  Custom grille de pettres
        private Grille gameGrid;
        private List<Label> listUsedLetters = new List<Label>(); // Liste des lettres qui ont été utilisées
        private List<Label> listNonUsedLetters = new List<Label>(); // Liste des qui n'ont pas été utilisées
        private List<Label> listLettersFormingWord = new List<Label>(); // Liste des lettres qui forment un mot
        private List<TextBlock> listCompletedWords = new List<TextBlock>(); // Liste des mots qui sont complétés
        private List<Polygon> listPolygons = new List<Polygon>(); // Liste des polygones dans notre canvas
        Dictionary<string, TextBlock> dictWords = new Dictionary<string, TextBlock>(); // Dictionnaire des mots

        // Polygone qu'on trace immediatement
        private Polygon currentPolygon;
        // Verifier si il faut dessiner le polygone
        private bool isCellFirstLetterSelected = false;

        // ************************************************************ //
        /* Notez que ces variables ne font pas parties de la structure
         * de Grille, cela est parce que les quatres valeurs varient
         * selon la grosseur de notre jeu dans le XAML.
         */
        private double cellSize =0;
        private double sizeOfLetter =0;
        private double heightOfLetter =0;
        private double sizeOfWord = 0;
        // ************************************************************ //
        #endregion

        #region VARAIBLE D'ANIMATIONS
        // Mots du wrap panel
        ColorAnimation animTurnWordGreen = new ColorAnimation(Color.FromArgb(255, 10, 160, 10), TimeSpan.FromMilliseconds(300))
        { EasingFunction = new QuadraticEase()};
        // Blink du polygone
        ColorAnimation animBlinkPolygon = new ColorAnimation(Color.FromArgb(255, 0, 0, 0), TimeSpan.FromMilliseconds(600))
        { RepeatBehavior= RepeatBehavior.Forever, AutoReverse = true};
        // Reveal un element
        DoubleAnimation animRevealElement = new DoubleAnimation(1, TimeSpan.FromMilliseconds(2000));
        // Cache un element
        DoubleAnimation animHideElement = new DoubleAnimation(0, TimeSpan.FromMilliseconds(700));
       
        #endregion


        public GameWindow(Grille customGrid)
        {

            // Assigne notre game grid au customGrid
            this.gameGrid = customGrid;

            InitializeComponent();

            // Met le titre approprié
            lblTitle.Content = gameGrid.Title;

            ListWordsToFind = new ObservableCollection<string>();
            cellSize = grdLettres.Height / gameGrid.GridSize;
            StrikethroughDrawing.SizeOfCell = cellSize;

            // Remplis notre grid et les listes.
            FillGridWithCells();
            sizeOfLetter = DetermineSizeOfLetters();
            heightOfLetter = DetermineHeightOfLetters();
            sizeOfWord = DetermineSizeOfWords();
            FillGridWithLetters();
            FillListMots();

            // Crerr notre premier polygone
            currentPolygon = new Polygon() { Stroke = new SolidColorBrush(Colors.Black), StrokeThickness = 2 };
            // Rajoute-le dans notre canvas.
            cnvPolygone.Children.Add(currentPolygon);

            // Pour databind notre OvservableCollection.
            this.DataContext = this;

            StartArrowAnimation();
        }

        #region FONCTIONS: REMPLISSAGE DU GRID

        // Remplir le WrapPanel et notre Dictionarry
        private void FillListMots()
        {
            // Reset
            wrpMots.Children.Clear();
            dictWords.Clear();
            ListWordsToFind.Clear();

            foreach (string mot in gameGrid.WordList)
            {
                // Add au observable collection

                TextBlock txbMot = new TextBlock
                {
                    Text = mot,
                    Foreground = Brushes.DarkBlue,
                    FontWeight = FontWeights.Bold,
                    FontSize = sizeOfWord,
                    Margin = new Thickness(10, 0, 10, 0)
                };

                // Add au wrap panel
                wrpMots.Children.Add(txbMot);
                // Add au dictionnaire
                dictWords.Add(mot, txbMot);
                // Add mot dans la liste
                ListWordsToFind.Add(mot);
            }
        }
        // Creer et tracer les cellules.
        private void FillGridWithCells()
        {
            // Reset
            grdLettres.Children.Clear();
            grdLettres.ColumnDefinitions.Clear();
            grdLettres.RowDefinitions.Clear();


            double sizeOfLines = 3 / (gameGrid.GridSize - 1) + 0.5;
            // Creer la grile
            for (int i = 0; i < gameGrid.GridSize; ++i)
            {
                grdLettres.RowDefinitions.Add(new RowDefinition());
                grdLettres.ColumnDefinitions.Add(new ColumnDefinition());

                // Draw the columns of the grid
                Rectangle lineCol = new Rectangle
                {
                    Fill = Brushes.Black,
                    Width = sizeOfLines,
                    HorizontalAlignment = HorizontalAlignment.Left
                };
                Grid.SetColumn(lineCol, i + 1);
                Grid.SetRowSpan(lineCol, gameGrid.GridSize);


                // Draw the rows of the grid
                Rectangle rowCol = new Rectangle
                {
                    Fill = Brushes.Black,
                    Height = sizeOfLines,
                    VerticalAlignment = VerticalAlignment.Bottom
                };
                Grid.SetRow(rowCol, i);
                Grid.SetColumnSpan(rowCol, gameGrid.GridSize);

                // Rajoute les lignes dans notre grille
                grdLettres.Children.Add(lineCol);
                grdLettres.Children.Add(rowCol);
            }
        }
        // Replir la grilles avec les lettres.
        private void FillGridWithLetters()
        {
            for (int row = 0; row < gameGrid.GridSize; ++row)
                for (int col = 0; col < gameGrid.GridSize; ++col)
                {
                    Label letter = new Label
                    {
                        // Content, Size and Height of the letter
                        Name = string.Format("{0}{1:00}{2:00}", gridLetterNamePrefix, row, col),
                        Content = gameGrid.Letters[row * gameGrid.GridSize + col],
                        FontSize = sizeOfLetter,
                        FontFamily = new FontFamily("Microsoft YaHei UI Light"),
                        // Center Letter
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Stretch,
                        HorizontalContentAlignment = HorizontalAlignment.Center,

                        // Corrige pour faitre rentere la lettre
                        Height = heightOfLetter,
                        // Tentative de centrer les lettres
                        Padding = new Thickness(0, heightOfLetter * 0.1, 0, heightOfLetter * 0.1),
                        Background = Brushes.Transparent,
                        BorderBrush = Brushes.Transparent,
                    };

                    // Add les events de LMB click and release
                    letter.MouseDown += SetStartingLetter;
                    letter.MouseMove += SetEndingLetter;

                    // Assignes les dans les bonnes ranges et colonnes
                    Grid.SetRow(letter, row);
                    Grid.SetColumn(letter, col);

                    // Rajoute dans notre grid
                    grdLettres.Children.Add(letter);
                    // Rajoute a notre list de lettres pas utilises
                    listNonUsedLetters.Add(letter);
                }
        }
        // Determiner la grosseur des lettres dans notre grille.
        private double DetermineSizeOfLetters()
        {
            return grdLettres.Height/grdLettres.ColumnDefinitions.Count*0.66;
        }
        // Determine la longueur du rectangle de la lettre pour la centrer dans la cellule.
        private double DetermineHeightOfLetters()
        {
            return grdLettres.Height / gameGrid.GridSize * 1.1;
        }
        // Determiner la grosseur des mots dans notre wrap panel.
        private double DetermineSizeOfWords()
        {
            /* Je fais un calcul avec une base qui diminue lineairement
             * afin d'avoir de bonnes grosseurs de mots selon le nombre de mots
             * pourque cela soit logiquement plus petit si on a plusieurs mots.
             * J'ai fait essaie/erreur sur la valeur de la variable baseSize
             * pour avoir des grosseurs de fonts raisonnables.
             */ 

            double baseSize = 165;
            double baseSub = 2;

            for (int i = 0; i < gameGrid.WordList.Length; ++i)
            {
                baseSize -= baseSize / baseSub;
                baseSub += 2;
            }

            return baseSize;
        }
        #endregion

        #region FONCTIONS: DESSINER LE POLYGONE

        // Calcul chaque point du polygone.
        private void AssignPolygonToPoints()
        {
            PointCollection points = StrikethroughDrawing.GetPolygonPoints();
            currentPolygon.Points = points;
        }

        // Signaler l'indice avec une animation.
        private void BlinkPolygon()
        {
            // Desinne notre polygone
            AssignPolygonToPoints();

            // Anime le blink
            currentPolygon.Stroke = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            currentPolygon.Stroke.BeginAnimation(SolidColorBrush.ColorProperty, animBlinkPolygon);

        }
        #endregion

        #region FONCTIONS: CLICK, DRAG, RELEASE
        // (Click) Assigner un point de depart le moment d'un clic.
        private void SetStartingLetter(object sender, MouseButtonEventArgs e)
        {
            Label startingLetter = sender as Label;

            // Annule l'animation du "hint" si il est en loop
            currentPolygon.Stroke = new SolidColorBrush(Colors.Black);


            // Assigne la premiere coordonne de notre polygone
            StrikethroughDrawing.CellFirstLetter = new StrikethroughDrawing.CellPosition() { Row = Grid.GetRow(startingLetter), Col = Grid.GetColumn(startingLetter) };
            StrikethroughDrawing.CellLastLetter = new StrikethroughDrawing.CellPosition() { Row = Grid.GetRow(startingLetter), Col = Grid.GetColumn(startingLetter) }; ;
            // Dit a l'autre fonction qu'il doit dessiner le polygone
            isCellFirstLetterSelected = true;
            AssignPolygonToPoints(); // Creer le cercle :)
        }
        // (Mouse Move) Assigner un point final lors du mouvement de la souris sur une case.
        private void SetEndingLetter(object sender, MouseEventArgs e) 
        {
            Label endLetter = sender as Label;

            // Sauvegarde le endpos au cas que celui qu'on prend est invalide (un encerclement qui n'est pas exactement 90 degres)
            StrikethroughDrawing.CellPosition prevEndpos = new StrikethroughDrawing.CellPosition()
            { Row = StrikethroughDrawing.CellLastLetter.Row, Col = StrikethroughDrawing.CellLastLetter.Col };

            StrikethroughDrawing.CellLastLetter = new StrikethroughDrawing.CellPosition()
            { Row = Grid.GetRow(endLetter), Col = Grid.GetColumn(endLetter) };

            // Verifie qu'on a peser sur une case
            if (isCellFirstLetterSelected)
                if (StrikethroughDrawing.ValidatePoints()) // Verifie l'angle de l'entourage
                    AssignPolygonToPoints(); // Si c'est bon, dessine le polygone
                else
                    StrikethroughDrawing.CellLastLetter = prevEndpos;
        }
        // (Left Mouse Released) Griller le mot et recreer un nouveau polygone au besoin.
        public void ReleaseMouse_StrikeThroughWord(object sender, MouseButtonEventArgs e)
        {
            // Retounr si on a pas fait d'encerclement
            if (!isCellFirstLetterSelected) return;

            // Verifier si on peut encercler le mot
            if (StrikeTrhoughWord())
            {
                // Si oui, creer une nouvelle instance d'un polygone
                listPolygons.Add(currentPolygon);
                currentPolygon = new Polygon() { Stroke = Brushes.Black, StrokeThickness = 2 };
                cnvPolygone.Children.Add(currentPolygon);

                // Reset les deux points
                StrikethroughDrawing.CellFirstLetter = new StrikethroughDrawing.CellPosition() { Row =0, Col=0};
                StrikethroughDrawing.CellLastLetter = new StrikethroughDrawing.CellPosition() { Row=0, Col=0};

                // Si le mot croise est fini
                if (listCompletedWords.Count == gameGrid.WordList.Length)
                    RevealSecretWord();
            }
            else
                currentPolygon.Points = new PointCollection();
            isCellFirstLetterSelected = false;
        }
        #endregion

        #region FONCTIONS: TROUVER/VERIFIER LES MOTS

        // Griller un mot si on trouve un. Retourne false si ce n'est pas possible
        private bool StrikeTrhoughWord()
        {
            // Contstruit le mot entouré
            string word = AssembleWord();
            // Si c'est vide, retourne
            if (word == "")
                return false;

            // Regarde si le mot est dans notre dictionnaire de mots
            if (IsWordInTheDictionary(word))
            {
                // Enleve le mot de notre observable collection
                if (ListWordsToFind.Contains(word))
                    ListWordsToFind.Remove(word);
                else
                    ListWordsToFind.Remove(ReverseString(word));

                // Rajoute les lettres, qui ont formé notre mot, dans la liste des lettres utilisés
                AddLettersToUsedList();


                // Trouve le mot dans notre wrap panel
                TextBlock txbWord = GetTextBlockFromDictionary(word);

                // Rajoute dans la liste des mots trouves
                listCompletedWords.Add(txbWord);

                // Transforme le mot en vert (le mot est donc complété)
                txbWord.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
                txbWord.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, animTurnWordGreen);
                return true;
            }
            else
                return false;
        }
        // Former le mot qui est encerclé
        private string AssembleWord()
        {
            listLettersFormingWord.Clear();

            int wordLength = StrikethroughDrawing.GetWordLength();

            //Determiner le saut requis en Y (soit 1 ou 0)
            int jumpY = (StrikethroughDrawing.CellFirstLetter.Row == StrikethroughDrawing.CellLastLetter.Row ? 0 : 1);
            if (StrikethroughDrawing.CellFirstLetter.Row > StrikethroughDrawing.CellLastLetter.Row)
                jumpY *= -1; // Voir si le saut est à l'envers

            //Determiner le saut requis en X (soit 1 ou 0)
            int jumpX = (StrikethroughDrawing.CellFirstLetter.Col == StrikethroughDrawing.CellLastLetter.Col ? 0 : 1);
            if (StrikethroughDrawing.CellFirstLetter.Col > StrikethroughDrawing.CellLastLetter.Col)
                jumpX *= -1; // Voir si le saut est à l'envers

            string mot = "";

            // Forme le mot
            for (int indexLettre = 0; indexLettre <= wordLength; ++indexLettre)
            {
                // Contruit le nom avec le row et col
                string nom = string.Format("{0}{1:00}{2:00}", 
                    gridLetterNamePrefix, StrikethroughDrawing.CellFirstLetter.Row + indexLettre * jumpY,
                    StrikethroughDrawing.CellFirstLetter.Col + indexLettre * jumpX);

                // Trouve le nom dans notre grille
                Label letter = (Label)(LogicalTreeHelper.FindLogicalNode(grdLettres, nom));

                //  Retourne rien si on cherche quelquechose d'invalide
                if (letter == null)
                {
                    listLettersFormingWord.Clear(); // Empty la liste.
                    return ""; // Retounrne du vide.
                }

                // Rajoute la lettre dans notre mot
                mot += letter.Content;

                // Rajoute le dans notre liste
                listLettersFormingWord.Add(letter);
            }
            return mot;
        }
        // Ajoute les lettres dans notre liste de lettres utilises
        private void AddLettersToUsedList()
        {
            foreach (Label letter in listLettersFormingWord)
            {

                // Rajoute dans notre liste des lettres used
                if (!listUsedLetters.Contains(letter))
                    listUsedLetters.Add(letter);

                // Enleve de notre liste des lettres non used
                if (listNonUsedLetters.Contains(letter))
                    listNonUsedLetters.Remove(letter);
            }
        }
        // Verifier si le mot est dans le dictionaire
        private bool IsWordInTheDictionary(string word)
        {
            return dictWords.ContainsKey(word) || dictWords.ContainsKey(ReverseString(word));
        }
        // Inverser un string (pour verifier si l'inverse donne le bon mot)
        private string ReverseString(string word)
        {
            string reverse = "";
            for (int i = word.Length - 1; i >= 0; --i)
                reverse += word[i];
            return reverse;
        }
        // Trouver notre TextBlock dans notre dictionnaire
        private TextBlock GetTextBlockFromDictionary(string word)
        {
            if (dictWords.ContainsKey(word))
                return dictWords[word];
            else
                return dictWords[ReverseString(word)];
        }
        #endregion

        #region FONCTIONS: ANIMATIONS
        // Releve le mot secret lorsqu'on finit la liste de mots
        private void RevealSecretWord()
        {
            grdGameScreen.IsEnabled = false; // Eviter qu'on puise toujours drag
            stackCheats.BeginAnimation(OpacityProperty, animHideElement);

            AnimateHideUsedLettersAndPolygons();
            StartAnimateShiftSecretWord();
        }
        // Cache tout les lettres utilises
        private void AnimateHideUsedLettersAndPolygons()
        {
            DoubleAnimation animHideLetterOrPolygon = new DoubleAnimation(0, TimeSpan.FromMilliseconds(1500));
            DoubleAnimation animReduceGrdLetresOpacity = new DoubleAnimation(0.3, TimeSpan.FromMilliseconds(1500));

            grdLettres.BeginAnimation(OpacityProperty, animReduceGrdLetresOpacity);
            foreach (Polygon polygon in listPolygons)
                polygon.BeginAnimation(OpacityProperty, animHideLetterOrPolygon);
        }
        // Debute le shift des lettres
        private void StartAnimateShiftSecretWord()
        {
            // Margins pour positioner les lettres un par un
            double marginLeftCenter = 0;
            double marginTop = 0;
            double marginCurrentLeft = 0;
            const double letterSpacing = 0.60;
            // Calcul MarginLeftcenter
            foreach (Label letter in listNonUsedLetters)
            {
                /* Je fais le width et le height egal a son current width et height
                * pour que la grosseur des bordures des lettres ne changent pas apres mon calcul.
                */
                letter.Width = letter.ActualWidth;
                letter.Height = letter.ActualHeight;

                // Rajoute le width
                marginLeftCenter += letter.ActualWidth;
            }
            /* Je cherhce le margin qui va centrer les lettres puis je soustrait le dixieme
             * du de beut et la fin des lettre a cause que le content des labels sont Centered.
             */
            marginLeftCenter = (grdGameScreen.ActualWidth - marginLeftCenter*letterSpacing) /2 -
                (listNonUsedLetters[0].Width + listNonUsedLetters[listNonUsedLetters.Count -1].Width) *0.1;

            // Calcul MarginTop
            marginTop = (grdGameScreen.ActualHeight - cellSize) / 2;
            marginCurrentLeft = 0;

            for (int i = 0; i < listNonUsedLetters.Count; ++i)
            {
                Label letter = listNonUsedLetters[i];

                Point relativePos = letter.TransformToAncestor(grdGameScreen).Transform(new Point(0, 0));

                // Enleves les lettres de la grille
                grdLettres.Children.Remove(letter);
                // Mets les dans le gamescreen
                grdGameScreen.Children.Add(letter);

                // Je mets ces alignements car on calcul selon le margin
                letter.HorizontalContentAlignment = HorizontalAlignment.Center;
                letter.HorizontalAlignment = HorizontalAlignment.Left;
                letter.VerticalAlignment = VerticalAlignment.Top;

                letter.Margin = new Thickness(relativePos.X, relativePos.Y, 0, 0);

                // Shift les lettres dans leur ordre

                ThicknessAnimation anim = new ThicknessAnimation(new Thickness(marginLeftCenter + marginCurrentLeft, marginTop, 0, 0), TimeSpan.FromMilliseconds(3000 +i*60))
                { EasingFunction = new QuarticEase() { EasingMode = EasingMode.EaseIn } };

                // Met les after effects apres la fin de la derniere animation
                if (i == listNonUsedLetters.Count -1)
                    anim.Completed += StartAnimateBounceWord;

                letter.BeginAnimation(MarginProperty, anim);

                // Rajoute un margin par lettre selon leur width
                marginCurrentLeft += letter.Width * letterSpacing;
            }
        }
        // Debute le bounce des lettres
        private void StartAnimateBounceWord(object sender, EventArgs e)
        {
            // Dit bravo au user
            CongratulateUser();


            double intervalPerLetter = 1000 / gameGrid.WordList.Length;
            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(intervalPerLetter),
            };

            timer.Tick += AnimateBounceLetter;
            timer.Start();
        }
        // Faire bondir une letre
        private void AnimateBounceLetter(object sender, EventArgs e)
        {
            if (listNonUsedLetters.Count > 0)
            {
                Label letter = listNonUsedLetters[0];

                Thickness newMargin = new Thickness(letter.Margin.Left, letter.Margin.Top - 8, letter.Margin.Right, letter.Margin.Bottom);
                ThicknessAnimation animUpDown = new ThicknessAnimation(newMargin, TimeSpan.FromMilliseconds(1000))
                { AutoReverse = true, EasingFunction = new ElasticEase(), RepeatBehavior = RepeatBehavior.Forever };

                letter.BeginAnimation(MarginProperty, animUpDown);
                listNonUsedLetters.Remove(letter);
            }
            else
                (sender as DispatcherTimer).Stop();
        }
        // Dit bravo!
        private void CongratulateUser()
        {
            Label congrats = new Label()
            {
                Content = "Bravo! Vous avez trouvé le mot!",
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Center,
                FontSize = 22,
                Opacity = 0,
                Margin = new Thickness(0,0,0,50)
            };

            grdGameScreen.Children.Add(congrats);
            congrats.BeginAnimation(OpacityProperty, animRevealElement);
        }
        // Montre le button "cheat"
        private void StartArrowAnimation()
        {
            imgArrow.Opacity = 0;

            DispatcherTimer timerShowArrow = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(2000),
            };

            timerShowArrow.Tick += ShowArrow;
            timerShowArrow.Start();


            DispatcherTimer timerHideArrow = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(6000),
            };

            timerHideArrow.Tick += HideArrow;
            timerHideArrow.Start();
        }
        // Rend la fleche visible
        private void ShowArrow(object sender, EventArgs e)
        {
            DoubleAnimation animShow = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(3));

            imgArrow.BeginAnimation(OpacityProperty, animShow);

            (sender as DispatcherTimer).Stop();
        }
        // Rend la fleche invisible
        private void HideArrow(object sender, EventArgs e)
        {
            DoubleAnimation animShow = new DoubleAnimation(0, TimeSpan.FromSeconds(2))
            { EasingFunction = new QuadraticEase() };

            imgArrow.BeginAnimation(OpacityProperty, animShow);

            (sender as DispatcherTimer).Stop();
        }
        #endregion

        #region CHEATS
        // Trouve un mot et montre le avec le polygone
        private void BtnCheat_Click(object sender, RoutedEventArgs e)
        {
            if (lsvCheat.SelectedIndex == -1) return;
            string selectedWord = lsvCheat.SelectedValue as string;

            FindWord(selectedWord);
            AssignPolygonToPoints();
            BlinkPolygon();
        }
        // Montre les cheats
        private void BtnShowCheat_Checked(object sender, RoutedEventArgs e)
        {
            DoubleAnimation animPullUpCheats = new DoubleAnimation(350, TimeSpan.FromMilliseconds(2000))
            { EasingFunction = new QuarticEase() };

            lsvCheat.BeginAnimation(HeightProperty, animPullUpCheats);
            btnCheat.BeginAnimation(OpacityProperty, animRevealElement);
            btnInstaSolve.BeginAnimation(OpacityProperty, animRevealElement);

            // Cache la fleche
            if (cnvArrow.Opacity != 0)
            {
                DoubleAnimation animHide = new DoubleAnimation(0, TimeSpan.FromMilliseconds(150))
                { EasingFunction = new QuadraticEase() };

                cnvArrow.BeginAnimation(OpacityProperty, animHide);
            }
        }
        // Cache les cheats
        private void BtnShowCheat_Unchecked(object sender, RoutedEventArgs e)
        {
            DoubleAnimation animPullDownCheats = new DoubleAnimation(0, TimeSpan.FromMilliseconds(700));

            lsvCheat.BeginAnimation(HeightProperty, animPullDownCheats);
            btnCheat.BeginAnimation(OpacityProperty, animHideElement);
            btnInstaSolve.BeginAnimation(OpacityProperty, animHideElement);
        }
        // Resous le mot caché
        private void BtnInstaSolve_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                "Are you sure you want to automatically solve le mot caché?", "Auto Solve", 
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                DispatcherTimer timer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromMilliseconds(40),
                };

                timer.Tick += AutoSolveNextWord;
                timer.Start();
            }
        }
        // Trouver un mot dans notre grille
        private void FindWord(string searchWord)
        {
            string currentWord;
            int maxInterval = gameGrid.GridSize - searchWord.Length + 1;

            // Horizontal
            for (int col = 0; col < maxInterval; ++col)
                for (int row = 0; row < gameGrid.GridSize; ++row)
                {
                    StrikethroughDrawing.CellFirstLetter = new StrikethroughDrawing.CellPosition
                    {
                        Col = col,
                        Row = row,
                    };

                    StrikethroughDrawing.CellLastLetter = new StrikethroughDrawing.CellPosition
                    {
                        Col = col + (searchWord.Length - 1),
                        Row = row,
                    };

                    currentWord = AssembleWord();
                    if (currentWord == searchWord || ReverseString(currentWord) == searchWord)
                        return;
                }


            // Vertical
            for (int row = 0; row < maxInterval; ++row)
                for (int col = 0; col < gameGrid.GridSize; ++col)
                {
                    StrikethroughDrawing.CellFirstLetter = new StrikethroughDrawing.CellPosition
                    {
                        Col = col,
                        Row = row,
                    };

                    StrikethroughDrawing.CellLastLetter = new StrikethroughDrawing.CellPosition
                    {
                        Col = col,
                        Row = row + (searchWord.Length - 1),
                    };

                    currentWord = AssembleWord();
                    if (currentWord == searchWord || ReverseString(currentWord) == searchWord)
                        return;
                }

            // Diagonale
            for (int row = 0; row < maxInterval; ++row)
                for (int col = 0; col < maxInterval; ++col)
                {
                    // Gauche -> Droite
                    StrikethroughDrawing.CellFirstLetter = new StrikethroughDrawing.CellPosition
                    {
                        Col = col,
                        Row = row,
                    };

                    StrikethroughDrawing.CellLastLetter = new StrikethroughDrawing.CellPosition
                    {
                        Col = col + (searchWord.Length - 1),
                        Row = row + (searchWord.Length - 1),
                    };

                    currentWord = AssembleWord();
                    if (currentWord == searchWord || ReverseString(currentWord) == searchWord)
                        return;

                    // Droite -> Gauche
                    StrikethroughDrawing.CellFirstLetter = new StrikethroughDrawing.CellPosition
                    {
                        Col = col + (searchWord.Length - 1),
                        Row = row,
                    };

                    StrikethroughDrawing.CellLastLetter = new StrikethroughDrawing.CellPosition
                    {
                        Col = col,
                        Row = row + (searchWord.Length - 1),
                    };

                    currentWord = AssembleWord();
                    if (currentWord == searchWord || ReverseString(currentWord) == searchWord)
                        return;
                }

            // Au cas si ca ne le trouves pas pour x raisons
            throw new Exception("The word was not found. Something is wrong :(");
        }
        // Trouver et griller le prochain mot de notre liste
        private void AutoSolveNextWord(object sender, EventArgs e)
        {
            if (ListWordsToFind.Count != 0)
            {
                FindWord(ListWordsToFind[0]);
                AssignPolygonToPoints();
                isCellFirstLetterSelected = true;
                ReleaseMouse_StrikeThroughWord(null, null);
            }
            else
                (sender as DispatcherTimer).Stop();

        }
        #endregion

        #region BUTTONS
        // Recommencer le jeu
        private void BtnRestart_Click(object sender, RoutedEventArgs e)
        {
            var response = MessageBox.Show("Voulez-vous vraiment recommencer?", "Restart", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (response == MessageBoxResult.Yes)
            {
                GameWindow gameWindow = new GameWindow(this.gameGrid);
                this.Close();
                gameWindow.Show();
            }
        }
        // Aller en arriere
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            var response = MessageBox.Show("Voulez-vous vraiment retourner?", "Retourner", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (response == MessageBoxResult.Yes)
            {
                MainMenu mainWindow = new MainMenu();
                mainWindow.TransitionPage(MainMenu.IFRAME_PAGE.PLAY);
                this.Close();
                mainWindow.Show();
            }
        }
        #endregion
    }
}
