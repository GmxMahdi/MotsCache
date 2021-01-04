using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace MotsCache
{
    /// <summary>
    /// Logique d'interaction pour PageMainMenu.xaml
    /// </summary>
    public partial class PageMainMenu : Page
    {
        public PageMainMenu()
        {
            InitializeComponent();
        }

        // Play!
        private void BtnPlay_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current.Windows[0] as MainMenu).TransitionPage(MainMenu.IFRAME_PAGE.PLAY);
        }

        // Montre l'aide :)
        private void BtnHelp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ProcessStartInfo process = new ProcessStartInfo
                {
                    FileName = "Aide_Mots_Caches.pdf",
                    WorkingDirectory = Directory.GetCurrentDirectory() + "\\Resources"
                };
                Process.Start(process);
            }
            catch (System.ComponentModel.Win32Exception)
            {
                MessageBox.Show("Le fichier est introuvable :/", "Oh no!", MessageBoxButton.OK);
            }
        }
    }
}
