using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using NC = NCalc;

namespace dmp1
{
    class Graf
    {
        private List<List<Point>> fce; //Jednotlivé body funkcí

        #region Rozdělení obrazovky na + a -
        int minW;
        int maxW;

        int minH;
        int maxH;
        #endregion

        #region Posunutí
        int posX = 0;
        int posY = 0;
        #endregion

        float koeficient = 32; //Přiblížení / Oddálení

        NC.Expression priklad = new NC.Expression("2 * Sin(x)"); //Předpis funkce

        Canvas Plocha;
        Window Okno;

        BackgroundWorker Casovac;

        //Přiřazení základních hodnot
        public Graf(Canvas plocha, Window okno)
        {
            Okno = okno;
            Plocha = plocha;
            Casovac = new BackgroundWorker()
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            NakresliGraf();

            Casovac.DoWork += MerInterval;
            Casovac.ProgressChanged += IntervalUplynul;

            Plocha.SizeChanged += SizeChanged;
            Plocha.MouseWheel += Zoom;
            Okno.KeyDown += StisklKlavesu;
            Okno.KeyUp += PustilKlavesu;
        }

        bool[] klavesy = new bool[] { false, false, false, false }; //Které klávesy jsou stisknuty (W, D, S, A)
        private void MerInterval(object sender, DoWorkEventArgs e)
        {
            while (!Casovac.CancellationPending)
            {
                Thread.Sleep(50);
                Casovac.ReportProgress(0);
            }
        }

        private void IntervalUplynul(object sender, ProgressChangedEventArgs e)
        {
            if (klavesy[0])
            {
                posY += 16;
            }
            if (klavesy[1])
            {
                posX -= 16;
            }
            if (klavesy[2])
            {
                posY -= 16;
            }
            if (klavesy[3])
            {
                posX += 16;
            }

            NakresliGraf();
        }

        private void StisklKlavesu(object sender, KeyEventArgs e)
        {
            switch(e.Key)
            {
                case Key.W:
                    klavesy[0] = true;
                    break;
                case Key.D:
                    klavesy[1] = true;
                    break;
                case Key.S:
                    klavesy[2] = true;
                    break;
                case Key.A:
                    klavesy[3] = true;
                    break;
                default:
                    return;
            }

            if(!Casovac.IsBusy)
            {
                Casovac.RunWorkerAsync();
            }
        }

        private void PustilKlavesu(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.W:
                    klavesy[0] = false;
                    break;
                case Key.D:
                    klavesy[1] = false;
                    break;
                case Key.S:
                    klavesy[2] = false;
                    break;
                case Key.A:
                    klavesy[3] = false;
                    break;
            }

            if (!klavesy.Contains(true))
            {
                Casovac.CancelAsync();
            }
        }

        private void Zoom(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                Priblizeni();
            }
            else if (e.Delta < 0)
            {
                Oddaleni();
            }
        }

        //Výpočet bodů funkce
        private void Calc()
        {
            fce = new List<List<Point>>();
            fce.Add(new List<Point>());
            for (int x = minW - posX; x < maxW - posX; x++)
            {
                priklad.Parameters["x"] = x / koeficient;
                double v = Convert.ToDouble(priklad.Evaluate());
                if (double.IsInfinity(v))
                {
                    fce.Last().Add(new Point(x + maxW, v == double.PositiveInfinity ? (maxH * 2) : (maxH * -2)));
                    fce.Add(new List<Point>());
                    continue;
                }
                double res = -Convert.ToDouble(priklad.Evaluate()) * koeficient + maxH + posY;
                fce.Last().Add(new Point(x + maxW + posX, (float)(res > Plocha.Height + 3 ? Plocha.Height + 3 : res < 0 ? -3 : res)));
            }
        }

        //Vytvoření části grafu
        private void NovaCara(double x1, double y1, double x2, double y2, Brush stroke, double strokeThickness)
        {
            Line cara = new Line()
            {
                X1 = x1,
                Y1 = y1,
                X2 = x2,
                Y2 = y2,
                Stroke = stroke,
                StrokeThickness = strokeThickness
            };

            Plocha.Children.Add(cara);
        }
        
        //Vykreslení celého grafu
        public void NakresliGraf()
        {
            Calc();

            if (fce.Count > 0)
            {
                Plocha.Children.Clear();
                NovaCara(0, maxH + posY, Plocha.ActualWidth, maxH + posY, Brushes.Red, 1);
                NovaCara(maxW + posX, 0, maxW + posX, Plocha.ActualHeight, Brushes.Red, 1);

                for (float i = maxW + posX % koeficient + (int)Math.Ceiling(maxW / koeficient) * koeficient; i >= 0; i -= koeficient)
                {
                    NovaCara(i, maxH - koeficient / 4 + posY, i, maxH + koeficient / 4 + posY, Brushes.Black, 1);
                }

                for (float i = maxH + posY % koeficient + (int)Math.Ceiling(maxH / koeficient) * koeficient; i >= 0; i -= koeficient)
                {
                    NovaCara(maxW - koeficient / 4 + posX, i, maxW + koeficient / 4 + posX, i, Brushes.Black, 1);
                }


                foreach (List<Point> body in fce)
                {
                    for (int i = 0; i < body.Count - 1; ++i)
                    {
                        NovaCara(body[i].X, body[i].Y, body[i + 1].X, body[i + 1].Y, Brushes.Blue, 1);
                    }
                }
            }
            else
            {
                SizeChanged(null, null);
            }
        }

        private void Priblizeni()
        {
            koeficient *= 2;
            NakresliGraf();
        }

        private void Oddaleni()
        {
            if (koeficient > 4)
            {
                koeficient /= 2;
                NakresliGraf();
            }
        }

        //Výpočet posunutí
        private void SizeChanged(object sender, EventArgs e)
        {
            minW = (int)(-Plocha.ActualWidth / 2);
            maxW = (int)(Plocha.ActualWidth / 2);

            minH = (int)(-Plocha.ActualHeight / 2);
            maxH = (int)(Plocha.ActualHeight / 2);
            NakresliGraf();
        }
    }
}
