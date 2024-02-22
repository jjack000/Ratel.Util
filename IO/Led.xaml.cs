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
    /// Led.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Led : UserControl
    {
        public Led()
        {
            InitializeComponent();
        }

        //// LedColor 의 종류
        //public enum LedColorType
        //{
        //    Gray,
        //    Green,
        //    Red,
        //    Yellow
        //}

        //// LedColor 의 종류에 따라 색을 변경한다.
        //public LedColorType LedColor
        //{
        //    get => (LedColorType)GetValue(LedColorProperty);
        //    set => SetValue(LedColorProperty, value);
        //}
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool?), typeof(Led), new PropertyMetadata(false, IsCheckedChanged));
        public static readonly DependencyProperty OnColorProperty =
            DependencyProperty.Register("OnColor", typeof(Color), typeof(Led), new PropertyMetadata(Colors.Green, OnColorChanged));
        public static readonly DependencyProperty OffColorProperty =
            DependencyProperty.Register("OffColor", typeof(Color), typeof(Led), new PropertyMetadata(Colors.Black, OffColorChanged));
        public static readonly DependencyProperty OnOpacityProperty =
            DependencyProperty.Register("OnOpacity", typeof(double), typeof(Led), new PropertyMetadata(1.0, OnOpacityChanged));
        public static readonly DependencyProperty OffOpacityProperty =
            DependencyProperty.Register("OffOpacity", typeof(double), typeof(Led), new PropertyMetadata(0.3, OffOpacityChanged));

        public bool IsChecked { get => (bool)GetValue(IsCheckedProperty); set => SetValue(IsCheckedProperty, value); }
        public Color OnColor { get => (Color)GetValue(OnColorProperty); set => SetValue(OnColorProperty, value); }
        public Color OffColor { get => (Color)GetValue(OffColorProperty); set => SetValue(OffColorProperty, value); }
        public double OnOpacity { get => (double)GetValue(OnOpacityProperty); set => SetValue(OnOpacityProperty, value); }
        public double OffOpacity { get => (double)GetValue(OffOpacityProperty); set => SetValue(OffOpacityProperty, value); }

        private static void OnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var led = d as Led;
            if (led != null)
            {
                if (led.IsChecked == true)
                    led.stopColor.Color = (Color)e.NewValue;
                led.stopColor.Color = (Color)e.NewValue;
            }
        }
        private static void OffColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var led = d as Led;
            if (led != null)
            {
                if (led.IsChecked == false)
                    led.stopColor.Color = (Color)e.NewValue;
            }
        }
        private static void OnOpacityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var led = d as Led;
            if (led != null)
            {
                if (led.IsChecked == true)
                    led.ledColor.Opacity = (double)e.NewValue;
            }
        }
        private static void OffOpacityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var led = d as Led;
            if (led != null)
            {
                if (led.IsChecked == false)
                    led.ledColor.Opacity = (double)e.NewValue;
            }
        }
        private static void IsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var led = d as Led;
            if (led != null)
            {
                var isChecked = (bool?)e.NewValue;
                if (isChecked == true)
                {
                    led.stopColor.Color = led.OnColor;
                    led.ledColor.Opacity = led.OnOpacity;
                }
                else
                {
                    led.stopColor.Color = led.OffColor;
                    led.ledColor.Opacity = led.OffOpacity;
                }
            }
        }
    }
}
