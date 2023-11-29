using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Here_we_go_again
{
    public partial class MainWindow : Window
    {
        private static readonly int margin = 20;
        private Point startPoint;
        private readonly MainWindow? previousWindow = null;
        private MainWindow? nextWindow = null;
        public bool pleaseIgnoreMe = false;
        private double rectangleWidth = 0.0;

        public MainWindow() : this(1000, 1000, null) { Topmost = true; }
        public MainWindow(double width, double height, MainWindow? previousWindow)
        {
            InitializeComponent();
            Width = width;
            Height = height;
            Loaded += delegate
            {
                InitialiseWindow();
            };
            this.previousWindow = previousWindow;
        }

        private Point GetWindowPoint()
        {
            Point windowPoint = new Point();
            foreach (UIElement child in MainCanvas.Children)
            {
                if (child is Border border && border.Tag != null)
                {
                    double currentLeft = Canvas.GetLeft(border);
                    double currentTop = Canvas.GetTop(border);
                    Point canvasPoint = new Point(currentLeft, currentTop);
                    windowPoint = MainCanvas.PointToScreen(canvasPoint);
                    break;
                }
            }
            return windowPoint;
        }

        private void InitialiseWindow()
        {
            if (previousWindow != null)
            {
                Point point = previousWindow.GetWindowPoint();
                Left = point.X;
                Top = point.Y;
            }

            double borderSize = SystemParameters.WindowNonClientFrameThickness.Top;
            double width = ActualWidth - margin;
            double height = ActualHeight - borderSize - margin;

            if (height < 0 || width < 0) return;

            MainCanvas.Height = height;
            MainCanvas.Width = width;
            double totalMargin = 2 * margin;
            double rectWidth = width - totalMargin;
            rectangleWidth = rectWidth;
            double rectHeight = height - totalMargin;

            Border windowRect = new()
            {
                Tag = "this",
                Width = rectWidth,
                Height = rectHeight,
                Background = new SolidColorBrush(Color.FromRgb(220, 220, 220)),
                CornerRadius = new CornerRadius(4)
            };

            Border titleBarRect = new()
            {
                Width = rectWidth,
                Height = borderSize,
                Background = new SolidColorBrush(Color.FromRgb(169, 169, 169)),
                CornerRadius = new CornerRadius(4, 4, 0, 0)
            };

            TextBlock dotsText = new()
            {
                Text = "...",
                Foreground = new SolidColorBrush(Colors.Black),
                FontSize = 18,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            double left = (width - rectWidth) / 2;
            double top = (height - rectHeight - borderSize) / 2;
            Canvas.SetLeft(windowRect, left);
            Canvas.SetTop(windowRect, top);
            MainCanvas.Children.Add(windowRect);
            Canvas.SetLeft(titleBarRect, left);
            Canvas.SetTop(titleBarRect, top);
            MainCanvas.Children.Add(titleBarRect);
            Canvas.SetLeft(dotsText, left + rectWidth - margin);
            Canvas.SetTop(dotsText, top - 1);
            MainCanvas.Children.Add(dotsText);
            titleBarRect.MouseLeftButtonDown += WindowRect_MouseLeftButtonDown;
            titleBarRect.MouseMove += WindowRect_MouseMove;
            dotsText.MouseLeftButtonDown += WindowRect_MouseLeftButtonDown;
            dotsText.MouseMove += WindowRect_MouseMove;
            LocationChanged += WindowMoved;
            height = rectWidth;
            nextWindow = new MainWindow(rectWidth, rectHeight, this);
            Point nextWindowPoint = GetWindowPoint();
            nextWindow.Left = nextWindowPoint.X;
            nextWindow.Top = nextWindowPoint.Y;
            nextWindow.Show();
        }

        private void WindowMoved(object? sender, EventArgs e)
        {
            if (pleaseIgnoreMe) return;
            previousWindow?.UpdatePosition(previousWindow.MainCanvas.PointFromScreen(new(Left, Top)));
        }

        private void UpdatePosition(Point point)
        {
            foreach (UIElement child in MainCanvas.Children)
            {
                if (child != null)
                {
                    Canvas.SetLeft(child, point.X);
                    Canvas.SetTop(child, point.Y);
                    if (child is TextBlock)
                    {
                        Canvas.SetLeft(child, point.X + rectangleWidth - margin);
                    }
                }
            }
        }

        private void UpdatePosition(double deltaX, double deltaY)
        {
            foreach (UIElement child in MainCanvas.Children)
            {
                if (child != null)
                {
                    double currentLeft = Canvas.GetLeft(child);
                    double currentTop = Canvas.GetTop(child);
                    Canvas.SetLeft(child, currentLeft + deltaX);
                    Canvas.SetTop(child, currentTop + deltaY);
                }
            }
        }

        private void WindowRect_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            startPoint = e.GetPosition(null);
        }

        private void WindowRect_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point endPoint = e.GetPosition(null);
                double deltaX = endPoint.X - startPoint.X;
                double deltaY = endPoint.Y - startPoint.Y;
                UpdatePosition(deltaX, deltaY);
                Point windowPoint = GetWindowPoint();
                if (nextWindow != null)
                {
                    nextWindow.pleaseIgnoreMe = true;
                    nextWindow.Left = windowPoint.X;
                    nextWindow.Top = windowPoint.Y;
                    nextWindow.pleaseIgnoreMe = false;
                }
                startPoint = endPoint;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            nextWindow?.Close();
        }
    }
}