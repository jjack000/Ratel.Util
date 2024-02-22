using Ratel.Vision.Mura;
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


//using RaRect = System.Drawing.Rectangle;
//using RaPoint = System.Drawing.Point;
//using RaSize = System.Drawing.Size;

//using CvSize = OpenCvSharp.Size;
//using CvPoint = OpenCvSharp.Point;
//using CvRect = OpenCvSharp.Rect;
//using Mat = OpenCvSharp.Mat;
//using MatType = OpenCvSharp.MatType;
//using Size2f = OpenCvSharp.Size2f;
//using Point2f = OpenCvSharp.Point2f;
using OpenCvSharp.WpfExtensions;
using System.Diagnostics;
using Ratel.Vision.WPF;
using Ratel;
using Ratel.Vision;
using Cv = OpenCvSharp;
using Cv2 = OpenCvSharp.Cv2;

namespace RatelMura
{
    /// <summary>
    /// MuraConfigEditWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MuraConfigEditWindow : Window
    {
        public MuraConfig MuraConfig { get; set; }
        public List<string> CompareT => Enum.GetNames(typeof(CompareOption)).ToList();
        Cv.Mat mat = new Cv.Mat(1000, 1000, Cv.MatType.CV_8UC1, 128);
        Point ClickPoint = new Point();
        string filename = "";
        public MuraConfigEditWindow()
        {
            Ratel.Vision.Init.InitActiPro();
            InitializeComponent();
            Loaded += MuraConfigEditWindow_Loaded;
            viewer.ImageMouseDown += Viewer_ImageMouseDown;
        }

        private void Viewer_ImageMouseDown(object sender, ImageMouseArgs e)
        {
            ClickPoint = e.ImagePoint;
            MakeFiltersInImage();
            try
            {
                this.surPosList.CommitEdit(DataGridEditingUnit.Row, true);
                this.defectPosList.CommitEdit(DataGridEditingUnit.Row, true);
                surPosList.Items.Refresh();
                defectPosList.Items.Refresh();
            }
            catch(Exception ex)
            {
                RatelLib.Log.Error(ex);
            }
        }

        private void MuraConfigEditWindow_Loaded(object sender, RoutedEventArgs e)
        {
            helpPage.Visibility = Visibility.Collapsed;
            viewer.Mat = mat;
            viewer.AllowDrop = true;
            viewer.DragEnter += (s, ee) =>
            {
                if (ee.Data.GetDataPresent(DataFormats.FileDrop))
                    ee.Effects = DragDropEffects.All;
                else
                    ee.Effects = DragDropEffects.None;
            };
            viewer.Drop += (s, ee) =>
            {
                string[] ss = (string[])ee.Data.GetData(DataFormats.FileDrop, false);
                filename = ss[0];
                mat = viewer.Mat = new Cv.Mat(ss[0], OpenCvSharp.ImreadModes.Grayscale);

            };
            InitBinding();
        }

        void InitBinding()
        {
            DataContext = MuraConfig;
            //filterProertyGrid.DataObject = MuraConfig;
            filterProperty.DataContext = MuraConfig;
            defectPosList.ItemsSource = MuraConfig.DefectPoints;
            surPosList.ItemsSource = MuraConfig.SurPoints;

        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            defectPosList.ItemsSource = MuraConfig.DefectPoints;
        }

        // 영상위에 필터를 그린다.
        void MakeFiltersInImage()
        {
            if (mat == null)
                return;
            viewer.RemoveElements();
            int defSel = defectPosList.SelectedIndex;
            int surSel = surPosList.SelectedIndex;
            var centerx = (int)ClickPoint.X;
            var centery = (int)ClickPoint.Y;
            
            foreach(var (i, p) in MuraConfig.DefectPoints.Select((p, i)=> (i, p )))
            {
                int thickness = 1;
                if (i == defSel)
                    thickness = 4;

                var cx = -p.X * MuraConfig.SizeX * p.X;
                var cy = -p.Y * MuraConfig.SizeY * p.Y;
                var rot = new RotateTransform(MuraConfig.Angle, cx, cy);
                var drawRect = new System.Drawing.Rectangle((int)(centerx + p.X * MuraConfig.SizeX + 0.5),
                    (int)(centery + p.Y * MuraConfig.SizeY + 0.5),
                    MuraConfig.SizeX, MuraConfig.SizeY);
                if (mat.Rect().Contains(drawRect.ToCV()) == false)
                {
                    continue;
                }
                var avg = mat.SubMat(drawRect.ToCV()).Mean()[0];
                p.Level = avg;
                if (MuraConfig.AverageMethod == AverageMethod.Gaussian)
                {
                    var shape = new Ellipse
                    {
                        Stroke = new SolidColorBrush(Colors.Red),
                        StrokeThickness = thickness,
                        Width = MuraConfig.SizeX,
                        Height = MuraConfig.SizeY
                    };
                    shape.RenderTransform = rot;
                    viewer.AddEllipse(shape, drawRect);
                }
                else
                {
                    var shape = new Rectangle
                    {
                        Stroke = new SolidColorBrush(Colors.Red),
                        StrokeThickness = thickness,
                        Width = MuraConfig.SizeX,
                        Height = MuraConfig.SizeY
                    };
                    shape.RenderTransform = rot;
                    viewer.AddRectangle(shape, drawRect);
                }

            }
            double defAvg = 0;
            try
            {
                defAvg = MuraConfig.DefectPoints.Where(d => d.Level > 0).Average(d => d.Level);
            }
            catch
            {
                defAvg = 0;
            }

            foreach (var (i, p) in MuraConfig.SurPoints.Select((p, i) =>(i,p)))
            {
                int thickness = 1;
                if (i == surSel)
                    thickness = 4;
                Shape shape = null;
                if (MuraConfig.AverageMethod == AverageMethod.Gaussian)
                {
                    shape = new Ellipse
                    {
                        Stroke = new SolidColorBrush(Colors.Green),
                        StrokeThickness = thickness,
                        Width = MuraConfig.SizeX,
                        Height = MuraConfig.SizeY
                    };
                }
                else
                {
                    shape = new Rectangle
                    {
                        Stroke = new SolidColorBrush(Colors.Green),
                        StrokeThickness = thickness,
                        Width = MuraConfig.SizeX,
                        Height = MuraConfig.SizeY
                    };
                }
                (shape.Fill, shape.Opacity, shape.Stroke) = p.CompareOption switch
                {
                    CompareOption.MustNoCount or CompareOption.MustCount => (Brushes.Transparent, 1, Brushes.Lime),
                    CompareOption.Count => (Brushes.Transparent, 1, Brushes.Green),
                    CompareOption.NoCount => (Brushes.Transparent, 1, Brushes.LightGray),
                    CompareOption.MustNot => (Brushes.Red, 0.5, Brushes.Green),
                    _ => (Brushes.Green, 1, Brushes.Green),
                };

                var cx = -MuraConfig.SizeX * p.X;
                var cy = -MuraConfig.SizeY * p.Y;
                var rot = new RotateTransform(MuraConfig.Angle, cx, cy);
                shape.RenderTransform = rot;

                var drawRect = new System.Drawing.Rectangle((int)(centerx + p.X * MuraConfig.SizeX + 0.5),
    (int)(centery + p.Y * MuraConfig.SizeY + 0.5),
    MuraConfig.SizeX, MuraConfig.SizeY);
                if (mat.Rect().Contains(drawRect.ToCV()) == false)
                {
                    p.Level = defAvg;
                    p.DiffValue = 0;
                    continue;
                }
                var avg = mat.SubMat(drawRect.ToCV()).Mean()[0];
                p.Level = avg;
                p.DiffValue = defAvg - avg;
                if (MuraConfig.AverageMethod == AverageMethod.Gaussian)
                {
                    viewer.AddEllipse((Ellipse)shape, drawRect);
                }
                else
                {
                    viewer.AddRectangle((Rectangle)shape, drawRect);
                }
                //viewer.AddRectangle(rect, new RaRect((int)(centerx + p.X * MuraConfig.Size.Width * MuraConfig.PitchFactor + 0.5), 
                //    (int)(centery + p.Y * MuraConfig.Size.Height * MuraConfig.PitchFactor + 0.5), 
                //    MuraConfig.Size.Width, MuraConfig.Size.Height));
            }
            try
            {
                MuraConfig.Level = MuraConfig.SurPoints.Where(d => d.Level > 0).Average(d => d.Level) - defAvg;
            }
            catch
            {
                MuraConfig.Level = 0;
            }
            MuraConfig.UpdateData();
        }

        private void Draw_Click(object sender, RoutedEventArgs e)
        {
            MakeFiltersInImage();
            surPosList.Items.Refresh();
            defectPosList.Items.Refresh();
        }

        private void MakeNewFilterButton_Click(object sender, RoutedEventArgs e)
        {
            var checkedButton = radioContainer.Children.OfType<RadioButton>()
                                                  .FirstOrDefault(r => r.IsChecked == true);

            int cx = int.Parse(newFilterWidth.Text);
            int cy = int.Parse(newFilterHeight.Text);
            double pitch = double.Parse(newFilterPitch.Text);
            double percent = double.Parse(newFilterPercent.Text);
            var size = new System.Drawing.Size(cx, cy);
            switch (checkedButton.Content.ToString())
            {
                case "Circle":
                    MuraConfig = FilterHelpers.MakeCir8Config(cx, cy, MuraConfig.BlackFactor, MuraConfig.WhiteFactor, dpitch: pitch, minLevelPercent: percent);
                    break;

                case "Circle4":
                    MuraConfig = FilterHelpers.MakeCir4Config(cx, cy, MuraConfig.BlackFactor, MuraConfig.WhiteFactor, dpitch: pitch, minLevelPercent: percent);
                    break;

                case "Horizontal":
                    MuraConfig = FilterHelpers.MakeHoriConfig(cx, cy, MuraConfig.BlackFactor, MuraConfig.WhiteFactor, dpitch: pitch, minLevelPercent: percent);
                    break;

                case "Vertical":
                    MuraConfig = FilterHelpers.MakeVertConfig(cx, cy, MuraConfig.BlackFactor, MuraConfig.WhiteFactor, dpitch: pitch, minLevelPercent: percent);
                    break;
            }

            InitBinding();
        }

        private void NewFilterType_Checked(object sender, RoutedEventArgs e)
        {
        }

        private void TestButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void surPosList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MakeFiltersInImage();
        }

        private void defectPosList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MakeFiltersInImage();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void ShowInformationButton_Click(object sender, RoutedEventArgs e)
        {
            if (helpPage.Visibility == Visibility.Visible)
            {
                helpPage.Visibility = Visibility.Collapsed;
            }
            else
            {
                helpPage.Visibility = Visibility.Visible;
            }
        }
    }
}
