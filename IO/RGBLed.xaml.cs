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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Ratel.Util
{
    /// <summary>
    /// RGBLed.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class RGBLed : UserControl
    {
        public RGBLed()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty LedColorProperty =
            DependencyProperty.Register("LedColor", typeof(Color), 
                typeof(RGBLed), new PropertyMetadata(Colors.Gray, OnLedColorChanged));

        public static readonly DependencyProperty LedOpacityProperty =
            DependencyProperty.Register("LedOpacity", typeof(double), 
                               typeof(RGBLed), new PropertyMetadata(1.0, OnLedOpacityChanged));

        public Color LedColor { get => (Color)GetValue(LedColorProperty); set => SetValue(LedColorProperty, value); }
        public double LedOpacity { get => (double)GetValue(LedOpacityProperty); set => SetValue(LedOpacityProperty, value); }

        private static void OnLedColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var led = d as RGBLed;
            if (led != null)
            {
                if (e.NewValue is Color color)
                    led.stopColor.Color = color;
            }
        }

        private static void OnLedOpacityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var led = d as RGBLed;
            if (led != null)
            {
                if (e.NewValue is double opacity)
                    led.ledColor.Opacity = opacity;
            }
        }



    }
}
