using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace Grafik
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        double Xmin, Xmax, Ymin, Ymax;
        double Ysredina;
        Point lokacijaPointera;
        bool pomeraj;
        bool prvi;
        Label xyPokazivac;
        Label xyStatusBar;
        ContextMenu meni;

        public MainWindow()
        {
            InitializeComponent();

            Xmin = Ymin = -5;
            Xmax = Ymax = 5;
            Ysredina = canvas.Height / (double)2;

            prvi = true;
            xyStatusBar = new Label();
            canvas.Children.Add(xyStatusBar);

            ContextMenu meni = new ContextMenu();
            MenuItem mi = new MenuItem();
            mi.Header = "File";
            meni.Items.Add(mi);
            canvas.ContextMenu = meni;

            inicMeni();

            CrtajFunkciju();
        }

        void inicMeni()
        {
            meni = new ContextMenu();
            MenuItem item = new MenuItem();
            item.Header = "Save to file";
            meni.Items.Add(item);
            item.Click += new RoutedEventHandler(item_Click);
            canvas.ContextMenu = meni;
        }

        void BrisiSveLinije()
        {
            Type put = typeof(Path);
            Type linija = typeof(Line);
            Type label = typeof(Label);
            for (int i = canvas.Children.Count - 1; i >= 0; i--)
            {
                if (canvas.Children[i].GetType() == put || canvas.Children[i].GetType() == linija)
                    canvas.Children.RemoveAt(i);
            }
        }

        double funkcija(double x)
        {
            return Math.Sin(x);
        }

        void CrtajFunkciju()
        {
            BrisiSveLinije();

            List<double> niz = new List<double>();
            double korak = (Xmax - Xmin) / canvas.Width;
            for (double i = Xmin; i <= Xmax; i += korak)
            {
                niz.Add(funkcija(i));
            }

            PathFigureCollection figureKolekcija = new PathFigureCollection();

            double odnosX = 1;
            double odnosY = canvas.Height / (Ymax - Ymin);
            int j = 0;
            for (double i = 0; i <= canvas.Width && j < niz.Count; j++, i += odnosX)
            {
                if (!Double.IsNaN(niz[j]) && !Double.IsInfinity(niz[j]))
                {
                    Point pocetna = new Point(i, Ysredina - (niz[j++] * odnosY));
                    i += odnosX;
                    PathSegmentCollection segmentiKolekcija = new PathSegmentCollection();

                    for (; i <= canvas.Width && j < niz.Count && !Double.IsNaN(niz[j]) && !Double.IsInfinity(niz[j]); j++, i += odnosX)
                    {
                        LineSegment tacka = new LineSegment();
                        tacka.Point = new Point(i, Ysredina - (niz[j] * odnosY));
                        segmentiKolekcija.Add(tacka);
                    }
                    PathFigure figura = new PathFigure(pocetna, segmentiKolekcija, false);
                    figureKolekcija.Add(figura);
                }
            }

            PathGeometry myPathGeometry = new PathGeometry();
            myPathGeometry.Figures = figureKolekcija;

            Path grafik = new Path();
            grafik.Stroke = Brushes.Red;
            grafik.StrokeThickness = 0.7;
            grafik.Data = myPathGeometry;
            canvas.Children.Add(grafik);

            CrtajKordinatni();
            crtajKordinate();
        }

        void CrtajKordinatni()
        {
            if (0 > Xmin && 0 < Xmax)
            {
                double podela = canvas.Width / (Xmax - Xmin);
                Line vertikalna = new Line();
                vertikalna.X1 = podela * (0 - Xmin);
                vertikalna.X2 = podela * (0 - Xmin);
                vertikalna.Y1 = 0;
                vertikalna.Y2 = canvas.Height;
                vertikalna.Stroke = System.Windows.Media.Brushes.Black;
                vertikalna.StrokeThickness = 0.3;
                canvas.Children.Add(vertikalna);
            }
            Line horizontala = new Line();
            horizontala.X1 = 0;
            horizontala.X2 = canvas.Width;
            horizontala.Y1 = Ysredina;
            horizontala.Y2 = Ysredina;
            horizontala.Stroke = System.Windows.Media.Brushes.Black;
            horizontala.StrokeThickness = 0.3;
            canvas.Children.Add(horizontala);
        }


        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            canvas.Width = this.Width - 22;
            canvas.Height = this.Height - 39;
            if (prvi)
            {
                Ysredina = canvas.Height / (double)2;
                prvi = false;
            }
            else
            {
                Point p = (Point)e.PreviousSize;
                Point n = (Point)e.NewSize;
                Ysredina += n.Y - p.Y;
            }
            CrtajFunkciju();
        }

        private void Window_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                if (e.Delta > 0)
                {
                    double smanji = ((Xmax - Xmin) / (double)2) / (double)2;
                    Xmax -= smanji;
                    Xmin += smanji;
                    CrtajFunkciju();
                }
                else if (e.Delta < 0)
                {
                    double povecaj = ((Xmax - Xmin) / (double)2);
                    Xmax += povecaj;
                    Xmin -= povecaj;
                    CrtajFunkciju();
                }
            }
            else if (Keyboard.IsKeyDown(Key.LeftAlt))
            {
                if (e.Delta > 0)
                {
                    double smanji = ((Ymax - Ymin) / (double)2) / (double)2;
                    Ymax -= smanji;
                    Ymin += smanji;
                    CrtajFunkciju();
                }
                else if (e.Delta < 0)
                {
                    double povecaj = ((Ymax - Ymin) / (double)2);
                    Ymax += povecaj;
                    Ymin -= povecaj;
                    CrtajFunkciju();
                }
            }
            else
            {
                if (e.Delta > 0)
                {
                    double smanji = ((Xmax - Xmin) / (double)2) / (double)2;
                    Xmax -= smanji;
                    Xmin += smanji;
                    smanji = ((Ymax - Ymin) / (double)2) / (double)2;
                    Ymax -= smanji;
                    Ymin += smanji;
                    CrtajFunkciju();
                }
                else if (e.Delta < 0)
                {
                    double povecaj = ((Xmax - Xmin) / (double)2);
                    Xmax += povecaj;
                    Xmin -= povecaj;
                    povecaj = ((Ymax - Ymin) / (double)2);
                    Ymax += povecaj;
                    Ymin -= povecaj;
                    CrtajFunkciju();
                }
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            lokacijaPointera = e.GetPosition(this);
            pomeraj = true;
        }

        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            pomeraj = false;
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (pomeraj && (e.GetPosition(this).X - lokacijaPointera.X != 0 || e.GetPosition(this).Y - lokacijaPointera.Y != 0))
            {
                double x = (e.GetPosition(this).X - lokacijaPointera.X) / canvas.Width * (Xmax - Xmin);
                Xmin -= x;
                Xmax -= x;

                Ysredina += e.GetPosition(this).Y - lokacijaPointera.Y;

                CrtajFunkciju();
            }
            lokacijaPointera = e.GetPosition(this);

            Point m = Mouse.GetPosition(this);
            xyStatusBar.Margin = new Thickness(0, 0, 0, 0);
            double w = Math.Round(Xmin + m.X * ((Xmax - Xmin) / canvas.Width), 4);
            double h = Math.Round((Ysredina - m.Y) * (Ymax - Ymin) / canvas.Height, 4);
            xyStatusBar.Content = "(" + w + ", " + h + ")";
        }

        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            pomeraj = false;
            xyStatusBar.Content = "";
        }

        private void btnZoomIn_Click(object sender, RoutedEventArgs e)
        {
            double smanji = ((Xmax - Xmin) / (double)2) / (double)2;
            Xmax -= smanji;
            Xmin += smanji;
            smanji = ((Ymax - Ymin) / (double)2) / (double)2;
            Ymax -= smanji;
            Ymin += smanji;
            CrtajFunkciju();
        }

        private void btnZoomOut_Click(object sender, RoutedEventArgs e)
        {
            double povecaj = ((Xmax - Xmin) / (double)2);
            Xmax += povecaj;
            Xmin -= povecaj;
            povecaj = ((Ymax - Ymin) / (double)2);
            Ymax += povecaj;
            Ymin -= povecaj;
            CrtajFunkciju();
        }

        private void prozor_KeyDown(object sender, KeyEventArgs e)
        {
            if(Keyboard.IsKeyDown(Key.P))
                prikaziKordinate(Mouse.GetPosition(this));
            if (Mouse.LeftButton == MouseButtonState.Released)
                pomeraj = false;
        }

        void crtajKordinate()
        {
            for (int i = canvas.Children.Count - 1; i >= 0; i--)
            {
                if (canvas.Children[i].GetType() == typeof(Label) && canvas.Children[i]!=xyStatusBar)
                    canvas.Children.RemoveAt(i);
            }

            double j = Xmin + (Xmax - Xmin) / 4;
            double odnosx = (Xmax - Xmin) / 4;
            double xosa = (canvas.Width) / 4;
            for (int i = 0; i < 3;  i++)
            {
                Label l = new Label();
                l.Margin = new Thickness(xosa-8, canvas.Height-20, 0, 0);
                xosa += (canvas.Width) / 4;
                l.Content = Math.Round(j, 4);
                j += odnosx;
                canvas.Children.Add(l);
            }

            j = canvas.Height / 4;
            double odnosy = (Ymax - Ymin) / 4;
            double yosa = (canvas.Height) / 4;
            for (int i = 0; i < 3; i++)
            {
                Label l = new Label();
                l.Margin = new Thickness(0, yosa-14, 0, 0);
                yosa += (canvas.Height) / 4;
                l.Content = Math.Round((Ysredina - j) * (Ymax - Ymin) / canvas.Height, 4);
                j += canvas.Height / 4;
                canvas.Children.Add(l);
            }
        }

        void prikaziKordinate(Point p)
        {
            canvas.Children.Remove(xyPokazivac);
            xyPokazivac = new Label();
            double x = Math.Round(Xmin + p.X * ((Xmax - Xmin) / canvas.Width), 4);
            double y = Math.Round((Ysredina - p.Y) * (Ymax - Ymin) / canvas.Height, 4);
            xyPokazivac.Content = "("+x+", "+ y+")";
            xyPokazivac.Margin = new Thickness(p.X, p.Y - 20, 0, 0);
            canvas.Children.Add(xyPokazivac);
        }

        private void prozor_KeyUp(object sender, KeyEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Released)
                pomeraj = false;
            canvas.Children.Remove(xyPokazivac);
        }

        private void item_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog();
            save.FileName = "Slika";
            save.DefaultExt = ".png";
            save.Filter = "Bitmap|*.png";
            save.ShowDialog();
            String str = @save.FileName;
            Uri adresa = new Uri(str);
            ExportToPng(adresa, canvas);
        }

        public void ExportToPng(Uri path, Canvas surface)
        {
            if (path == null) return;

            Transform transform = surface.LayoutTransform;
            surface.LayoutTransform = null;

            Size size = new Size(surface.Width, surface.Height);

            surface.Measure(size);
            surface.Arrange(new Rect(size));

            RenderTargetBitmap renderBitmap =
              new RenderTargetBitmap(
                (int)size.Width,
                (int)size.Height,
                96d,
                96d,
                PixelFormats.Pbgra32);
            renderBitmap.Render(surface);

            using (System.IO.FileStream outStream = new System.IO.FileStream(path.LocalPath, System.IO.FileMode.Create))
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
                encoder.Save(outStream);
            }
            surface.LayoutTransform = transform;
        }

        private void prozor_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            meni.PlacementTarget = this;
            meni.IsOpen = true;
        }

    }
}
