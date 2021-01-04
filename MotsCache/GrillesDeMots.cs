﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotsCache
{
    // Les mots cachés ont tous ces variables similaires.
    // On n'a pas besoin que d'une structure et non d'une classe, car leurs usages seront motivés selon les fonctions de notre jeu.
    public struct Grille
    {
        private string title;
        private string[] wordList;
        private string letters;
        public int gridSize;


        public string Title
        {
            get
            {
                if (title == null || title == "")
                    return "Mot caché sans nom.";
                else
                    return title;
            }

            set
            {title = value;}
        }
        public string[] WordList
        {
            get { return wordList; }
            set { wordList = value; }
        }

        public string Letters
        {
            get { return letters; }
            set { letters = value; }
        }
        public int GridSize
        {
            get { return gridSize; }
            set { gridSize = value; }
        }
    }

    // Liste des mots cachés
    class GrillesDeMots
    {

        public static Grille[] ArrayMotCache = new Grille[]
        {

        new Grille()
        {
            Title = "La tempête",
            WordList = new string[] { "AMI", "AVEU", "AVERSE", "NUAGE", "NUIT", "ORAGE", "SAISON", "SALETE" },
            Letters = "SAISONAVEUPOLEASRTERCAAITSGLMUEEGAUN",
            GridSize = 6
        },

        new Grille()
        {
            Title = "Composition",
            WordList = new string[] {"BAIE", "BOIRE", "NON", "PAIN", "PERE", "POUTRE",  "PRET", "PRUNE", "TERRE", "TOI"},
            Letters = "PAINPTBOIREEABURRRINPTEREOROREANVIOE",
            GridSize = 6
        },

        new Grille()
        {
            Title = "Villes de Provence",
            WordList = new string[] { "AIX", "APT", "ARLES", "AUBAGNE", "AUPS", "AVIGNON", "BANDOL", "CANNES", "CASSIS", "DIGNE", "FREJUS", "HYERES", "LUNEL", "MIRAMAS", "NICE", "ORANGE", "SALON", "SORGUES", "TOULON", "VENCE" },
            Letters = "SSSAUBAGNEEPAAMVENCELULMIRAMASRAOGEXACOEALNEGNARORTOULONGLRENDSTNUEIEYINPEENILDHCASSUJERFLEBELSISSAC",
            GridSize = 10
        },

        new Grille()
        {
            Title = "L'espace",
            WordList = new string[] { "ANGLE", "APESANTEUR", "COMETE", "COMPUT", "COSMOS", "ESPACE", "ETOILE", "FUSEE", "GALAXIE", "JUPITER", "LUNE", "MARS", "MATIERE", "MERCURE", "METEORE", "NAVETTE", "NEBULEUSE", "NEPTUNE", "PHASE", "PLANETE", "SAROS", "SATELLITE", "SATURNE", "SOLEIL", "UNIVERS", "VENUS" },
            Letters = "MCONSOMSOCSTSAMERCURESOUGTREUETENALPAPESANTEUREMLNHTAOIITOIOAUGAITSVPSLCXFNLSLUUEULOIUEEEELRNRJMESUELUBENESELEANAVETTEVTMETEORESPACETIEREITAMOSN",
            GridSize = 12
        },

        new Grille()
        {
            Title = "Noël",
            WordList = new string[] {"AMOUR","AVENT","BARBE","BOULE","BUCHE","CADEAU","CHANT","CHEMINEE","CLOCHE","CRECHE","DECEMBRE","DINDE","ETOILE","EXCES","FETE","FRAIS","GUIRLANDE","HOTTE","HOUX","JERUSALEM","JOUET","LUTIN","MAGES","MESSE","NEIGE","NOEL","RENNE","REVEILLON","SANTON","SAPIN","TRAINEAU","VOEUX"},
            Letters = "SAPINNEIGEPEAAMELASUREJNNDREVEILLONNTPIEBRABXICEORNNLBRBURTRNTAADMFUETLEEENIMEHCOLCTSDBHNCHHVLOEETOILETEUOJFCUUAEDACHANTXTLENITULEONEMESSEGAMOUR",
            GridSize = 12
        },

        new Grille()
        {
            Title = "Le chocolat",
            WordList = new string[] { "AMER", "ANTIOXYDANT", "AZTEQUES", "BEURRE", "BLANC", "BROWNIES", "CABOSSE", "CHARLOTTE", "CONCHAGE", "COTEDIVOIRE", "FORASTERO", "GANACHE", "GHANA", "GIANDUJA", "NOIR", "NOUGATINE", "OEUF", "PAQUES", "PATE", "STIMULANT", "THEOBROMINE", "TRINITARIO" },
            Letters = "TTEGANACHENCSRNORETSAROFTHIAEIOEBIUCICMNDUOILOGSMOOEIYFNAVAEUNROSTXWNITULCBSASAOCDIQAHOLEMORIENENAERRUEBITETTGHANAQRAONZAETTOLRAHCTAGIANDUJAPATE",
            GridSize = 12
        },

        new Grille()
        {
            Title = "Au bord de la mer",
            WordList = new string[] {"ALGUE","AMERRIR","BAIE","BARRAGE","CRABE","DIGUE","ECLUSE","ECUME","ELEMENT","ESTRAN","ETALE","HOULE","ISTHME","JETEE","JUSANT","LAGON","LAISSE","LITTORAL","MAREE","MARNAGE","MASCARET","MEDUSE","MER","PASSE","PHARE","POLDER","SABLE","TEMPETE","VAGUES","VASIERE"},
            Letters = "TEBARCELBASENSTFHOULEREUETTESUDEMTIGMRNMPOLDERALEAAAEMAJGLBALNSHEREISAVEESUCPSITRIIMEIJNAMURLSSULITTORALRSTCMARNAGERCEHEVAGUESATEEMEDIGUETALEEEA",
            GridSize = 12
        },

        new Grille()
        {
            Title = "Les insectes",
            WordList = new string[] { "ABEILLE", "AGRION", "AILES", "ANTENNE", "BLATTE", "BOURDON", "CAFARD", "CHENILLE", "CIGALE", "COCCINELLE", "COLONIE", "CRIQUET", "DARD", "DIPTERES", "ELYTRE", "FOURMI", "FRELON", "GRILLON", "GUEPE", "HANNETON", "IMAGO", "INSECTE", "LARVE", "LIBELLULE", "LOCUSTE", "LUCANE", "LUNE", "MANTE", "MOUCHE", "MOUSTIQUE", "PAPILLON", "PATTE", "PIQUE", "POUX", "PUCE", "PUNAISE", "SAUTERELLE", "TAON", "TEIGNE", "TERMITE", "VACCINS" },
            Letters = "ENGIETIMRETTALBPIMAGOIMRUOFDEPEINOLOCPIQUEITAUNOLLIRGEISNPSTGODNFNIMTTNOTUTOIRPRSQEOSIAECELRUUEEUCAUCTRODIGONLCEUNOCHELABABAOTTPTMAHSERELLINEHCENVMEVDLOLSROMANTELNRXLABEILLENGAIAAUUULELYTREGSFCLOLLNTNOLLIPAPUEPELLENICCOCSELIA",
            GridSize = 15
        },

        new Grille()
        {
            Title = "Capitales du monde",
            WordList = new string[] { "ALGER", "AMMAN", "ATHENES", "BAGDAD", "BALE", "BAMAKO", "BERLIN", "BOGOTA", "BRASILIA", "CANBERRA", "CARACAS", "DAKAR", "DAMAS", "DUBLIN", "EREVAN", "KABOUL", "KATMANDOU", "LILLE", "LIMA", "LISBONNE", "LOME", "LONDRES", "MADRID", "MANILLE", "MONACO", "MOSCOU", "PANAMA", "PARIS", "PEKIN", "PRAGUE", "RABAT", "RIYAD", "ROME", "SANAA", "SEOUL", "SINGAPOUR", "SOFIA", "TEHERAN", "TIRANA", "TRIPOLI", "TUNIS", "VARSOVIE", "VIENNE", "VILLE", "VILNIUS" },
            Letters = "UMLSVIENNELLILAOOVEIBALIDTUNISDNANLRDVAULOPSANALEEAOAIBEEEBMACGHVSLQYLKSUOAMOETRIPOLININNDTTRASLLINNRIDNAASVNUINLEDLIUEMKIDOCANBERRAOSMSRBKMOAGEDBECOADAKARGVBAUORSFNKPNMDTEMJPGAOIEAAMANARITAOAMAMPIDBASEMORTURIOSACARACRABATRPL",
            GridSize = 15
        },

        new Grille()
        {
            Title = "Le football",
            WordList = new string[] { "AMORTI", "BLEUS", "BRESIL", "BUTEUR", "BUTS", "CAMP", "CLUB", "COMPETITION", "CORNER", "DIVISION", "DOPAGE", "DRIBBLE", "FAUTE", "FIFA", "GOAL", "ITALIE", "JAUNE", "LIBERO", "LOB", "LUCARNE", "MAIN", "MARADONA", "MATCH", "MILIEU", "MITEMPS", "ONZE", "PARC", "PASSE", "RONALDO", "ROUGE", "SAISON", "SCORE", "SHORT", "SIFFLET", "SPORT", "SURFACE", "TACLE", "TERRAIN", "TOUCHE", "TRIBUNES", "UEFA", "VERTS" },
            Letters = "CAMPEOTBHRFLMSLBSIANIUAOCUNITATLAZRTLUCCTBTROTREISAGAALUAEEGCEOUSEDRTTEEMVROOLHSONOEICLPAOMRMFSENUNAABSMNTIEPFNRPAAFBEOANRLBEIOFAJRIHRLLIIIITSIISURCTDIEABELISSFSDUIOSLRRUUCTPIAEOBPEECORNERIOVUTOARBULCEEFAORITLGBMAINSTSAPNTDEE",
            GridSize = 15
        },

        new Grille()
        {
            Title = "Science fiction",
            WordList = new string[] {"ALIEN","ANDROIDE","ASTRONEF","AVATAR","CYBORG","BILBO","DYSTOPIE","ESPACE","ESPRIT","ETOILE","FANTASY","FORCE","FUSEE","FUTUR","GALAXIE","GOLEM","LASER","LUMIERE","MACHINE","MAGIE","MARTIEN","MONSTRE","OASIS","OCCULTE","ORDINATEUR","PLANETE","POTION","ROBOT","SABRE","SORCIER","SOUCOUPE","STASE","SUPERHEROS","SURNATUREL","TEMPS","TERREUR","UCHRONIE","UNIVERS","VOYAGE","ZOMBIES"},
            Letters = "TOBORRUERRETSMEFUSEEEREIMULUENOTYNSEETENALPLIRISPETLUCCOUEOHDRAOCIBILBORRGCIPTTFSTCOCUIHEANSNIEAYRUTUFECMAEAONBNOANEDRRUTGFNORSNEMICOETEAERRERICOGVSIEUYGFTULIRNACECMRORESALDOSMSARPAVATARNTFTIPIOSONGALAXIERSTASEDYSTOPIESEIBMOZ",
            GridSize = 15
        },

        new Grille()
        {
            Title = "Mots du Japon",
            WordList = new string[] {"ALLURE","ARCHIPEL","BANZAI","BONSAI","BONZE","CATALPA","EMPEREUR","ESTAMPE","FEMUR","FUTON","GEISHA","GOMASIO","HAIKU","HIROSHIMA","HONSHU","HOKKAIDO","IKEBANA","JEUDI","JUDOKA","KAMIKAZE","KARAOKE","KOBE","KOURILES","KYOTO","LOTUS","MANGA","MIKADO","NOUILLE","ORIGAMI","OSAKA","PACIFIQUE","SAMOURAI","SEISME","SHOGUN","SOJA","SUDOKU","SUMO","SURIMI","SUSHI","TATAMI","TOFU","TOKYO","TSUNAMI","WASABI"},
            Letters = "SOJAKODUJHONSHUSTOFUAPACIFIQUEEUSRUEREPMEAMFOLIDUEJZALUPFAEYIEGOMASIOLIENZKRKPRKOODAKIMGNOUIMIRUSTEZEUAOTOSMGHUABSPNROBSKAKAHCAEMGFAINEKMYMNNRAIUEIBUIOOOIAUTATAMIAGSSUTOLSSOIHSUSOMAROLENNTMUKIAHEKAAAODIAKKOHWSAAIEBOKAMIHSORIH",
            GridSize = 15
        },

        new Grille()
        {
            Title = "Monstre et compagnie",
            WordList = new string[] {"CENTAURE","CERBERE","CHIMERE","CYCLOPE","DJINN","DRACULA","DRAGON","ELFE","FANTOME","FARFADET","GARGANTUA","GARGOUILLE","GEANT","GOBELIN","GOLEM","GORGONE","GOULE","GRIFFON","HOBBIT","HYDRE","KORRIGAN","LICORNE","LOUP","MELUSINE","MINOTAURE","MONSTRE","NAGA","NAIN","NIXE","NYMPHE","OGRE","ONDINE","PEGASE","PSYCHE","SATAN","SIRENE","SORCIERE","SPHINX","TROLL","VAMPIRE","VOUIVRE","YETI","ZOMBIE"},
            Letters = "NIXESAGEPDJINNCIGELLIUOGRAGRGMLORRHERTSNOMOECEEUIUYRENISULEMBFARFADETALORKQOLTELFTRIEGBOGZGENRNOOOECERGOOUEEMORUNNRRIARMPTCYCLOPEIEOAGBSPHINXLCGRMISOIYDRACULAISIRENECNEMOTNAFLHPIEYHOBBITNAEGCAMEEENIDNOGARDNATASAGANYMPHERVIUOV",
            GridSize = 15
        },

        new Grille()
        {
            Title = "Les montagnes",
            WordList = new string[] { "ALPAGE", "ALPES", "ALPINISME", "ALTITUDE", "ANDES", "APLOMB", "ARMOR", "ASCENSION", "BALISAGE", "CAMPING", "CANYONING", "CASCADE", "CHALET", "DEGEL", "ESCALADE", "EVEREST", "FORET", "HIMALAYA", "GLACIER", "MASSIF", "MONTAGNE", "NEIGE", "OISANS", "PANORAMA", "PISTE", "RANDONNEE", "RAQUETTES", "REMONTEE", "ROCHEUSES", "SIERRA", "SOMMET", "TELEPHERIQUE", "TERTRE", "TOURISME", "TRANSAT", "TREKKING" },
            Letters = "LSPISTEEDALACSEEEOAEXMEEAMARONAPGMPUGOGETRANSATSEMLQLNANETTOEREEDEOIATPNMEAITCRDATMRCALOSLMSTATNCABEIGADIAANEMRASLAHENRNRHSEUPEHAPLPREMAUCSCQIKICIIEORORODISANKMANSLFORETIFARGIARIAECANYONINGNNLRSGTEDUTITLAEAGAEMEIRREMONTEEIEYIESEPLAEVERESTGASNASIOROCHEUSESE",
            GridSize = 16
        },
    };

    }
}
