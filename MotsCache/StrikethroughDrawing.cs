using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MotsCache
{
    // Cette class va sevir à calculer les points de notre encerclage afin qu'il soir beau :)
    // Cette classe est statique, car elle ne va permettre que de calculer nos points
    class StrikethroughDrawing
    {
        public struct CellPosition
        {
            public int Row;
            public int Col;
        }

        // Circonference d'un cercle
        private const double circleCircum = 2 * Math.PI;

        static private double sizeOfCell = 0; // Grosseur par defaut, elle devra etre changé selon la grosseur de la grille
        static private CellPosition cellFirstLetter = new CellPosition() { Row = 0, Col = 0 };
        static private CellPosition cellLastLetter = new CellPosition() { Row = 0, Col = 0 };

        // Access pour sizeOfCell
        static public double SizeOfCell
        {
            set
            {
                if (value > 0)
                    sizeOfCell = value;
                else
                    sizeOfCell = 1;
            }
        }

        // Access pour cellFirstLetter
        static public CellPosition CellFirstLetter
        {
            get
            { return cellFirstLetter; }
            set
            {
                if (value.Row >= 0)
                    cellFirstLetter.Row = value.Row;
                if (value.Col >= 0)
                    cellFirstLetter.Col = value.Col;
            }
        }

        // Access pour cellLastLetter
        static public CellPosition CellLastLetter
        {
            get
            { return cellLastLetter; }
            set
            {
                if (value.Row >= 0)
                    cellLastLetter.Row = value.Row;
                if (value.Col >= 0)
                    cellLastLetter.Col = value.Col;
            }
        }

        // Calcul chaque point du polygone.
        static public PointCollection GetPolygonPoints()
        {
            List<Point> listSeriesOfPoints = new List<Point>();
            Point firstCellPosition = CalculateEndpoints(cellFirstLetter);
            Point lastCellPosition = CalculateEndpoints(cellLastLetter);

            // Direction du cercle
            double circleDirection = DetermineCircleAngle();

            // Rayon
            double radius = sizeOfCell / 2 * 0.8; // 0.8 pour pas encadrer la cellule au complet

            // Nombre de points
            double numPoints = 20;

            // Sauts
            double jump = circleCircum / numPoints / 2;

            // Premiere demi du cercle
            for (int pointInd = 0; pointInd <= numPoints; ++pointInd)
                listSeriesOfPoints.Add(new Point(
                    firstCellPosition.X + Math.Cos(pointInd * jump + circleDirection) * radius,
                    firstCellPosition.Y + (Math.Sin(pointInd * jump + circleDirection) * radius) * -1));

            // Deuxieme demi du cercle
            for (int pointInd = 0; pointInd <= numPoints; ++pointInd)
                listSeriesOfPoints.Add(new Point(
                    lastCellPosition.X + Math.Cos(pointInd * jump + circleDirection + Math.PI) * radius,
                    lastCellPosition.Y + (Math.Sin(pointInd * jump + circleDirection + Math.PI) * radius) * -1));

            // Assigne la series de points à notre polygone
            return new PointCollection(listSeriesOfPoints);
        }

        // Trouver l'angle du demi-cercle.
        static private double DetermineCircleAngle()
        {
            // En bas
            if ((cellFirstLetter.Row < cellLastLetter.Row) && (cellFirstLetter.Col == cellLastLetter.Col))
                return 0;

            // En bas, coin droit
            else if ((cellFirstLetter.Row < cellLastLetter.Row) && (cellFirstLetter.Col < cellLastLetter.Col))
                return Math.PI / 4;

            // Egal, droit
            else if ((cellFirstLetter.Row == cellLastLetter.Row) && (cellFirstLetter.Col < cellLastLetter.Col))
                return Math.PI / 2;

            // En haut, coin droit
            else if ((cellFirstLetter.Row > cellLastLetter.Row) && (cellFirstLetter.Col < cellLastLetter.Col))
                return Math.PI * 3 / 4;

            // En haut
            else if ((cellFirstLetter.Row > cellLastLetter.Row) && (cellFirstLetter.Col == cellLastLetter.Col))
                return Math.PI;

            // En haut, coin gauche
            else if ((cellFirstLetter.Row > cellLastLetter.Row) && (cellFirstLetter.Col > cellLastLetter.Col))
                return Math.PI * (5.0 / 4.0);

            // Egal, gauche
            else if ((cellFirstLetter.Row == cellLastLetter.Row) && (cellFirstLetter.Col > cellLastLetter.Col))
                return Math.PI * (6.0 / 4.0);

            // En bas, coin gauche
            else if ((cellFirstLetter.Row < cellLastLetter.Row) && (cellFirstLetter.Col > cellLastLetter.Col))
                return Math.PI * (7.0 / 4.0);

            // Meme Pooint
            return 0;
        }

        // Generere la position des points (debut ou fin).
        static private Point CalculateEndpoints(CellPosition One)
        {
            return new Point()
            {
                X = One.Col * sizeOfCell + sizeOfCell / 2,
                Y = One.Row * sizeOfCell + sizeOfCell / 2
            };
        }

        static public bool ValidatePoints()
        {
            // Si c'est une ligne droite
            if (cellFirstLetter.Row == cellLastLetter.Row || cellFirstLetter.Col == cellLastLetter.Col)
                return true;

            // Si c'est une diagonale parfaite
            else if (Math.Abs(cellFirstLetter.Row - cellLastLetter.Row) == Math.Abs(cellFirstLetter.Col - cellLastLetter.Col))
                return true;

            // Si c'est mauvais
            else
                return false;
        }

        static public int GetWordLength()
        {
            // Trouver la longueur du mot formé en row
            int lengthX = Math.Abs(cellFirstLetter.Col - cellLastLetter.Col);

            // Trouver la longueur du mot formé en col
            int lengthY = Math.Abs(cellFirstLetter.Row - cellLastLetter.Row);

            // Determiner la longueur du mot.
            return Math.Max(lengthX, lengthY);
        }

    }
}
