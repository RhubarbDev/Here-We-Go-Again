using System;
using System.Diagnostics;
using System.Threading.Tasks;
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
        private MainWindow? previousWindow = null;
        private MainWindow? nextWindow = null;
        public bool pleaseIgnoreMe = false;

        public MainWindow() : this(1000, 1000, null) { Topmost = true; }

        public MainWindow(double width, double height, MainWindow? previousWindow)
        {
            InitializeComponent();
            this.Width = width;
            this.Height = height;
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
            double width = this.ActualWidth - margin;
            double height = this.ActualHeight - borderSize - margin;

            if (height < 0 || width < 0) return;

            MainCanvas.Height = height;
            MainCanvas.Width = width;
            double totalMargin = 2 * margin;

            double rectWidth = width - totalMargin;
            double rectHeight = height - totalMargin;

            // Main window rectangle with curved corners
            Border windowRect = new Border
            {
                Tag = "this",
                Width = rectWidth,
                Height = rectHeight,
                Background = new SolidColorBrush(Color.FromRgb(220, 220, 220)), // Light grey color
                CornerRadius = new CornerRadius(8) // Adjust this value for the main window curvature
            };

            // Title bar rectangle with curved top corners
            Border titleBarRect = new Border
            {
                Width = rectWidth,
                Height = borderSize,
                Background = new SolidColorBrush(Color.FromRgb(169, 169, 169)), // Grey color
                CornerRadius = new CornerRadius(8, 8, 0, 0) // Adjust this value for the title bar curvature
            };

            // Three dots on the right side
            TextBlock dotsText = new TextBlock
            {
                Text = "...",
                Foreground = new SolidColorBrush(Colors.Black),
                FontSize = 18,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right
            };

            // Calculate the position to center the window rectangle
            double left = (width - rectWidth) / 2;
            double top = (height - rectHeight - borderSize) / 2;

            // Set the position of the window rectangle
            Canvas.SetLeft(windowRect, left);
            Canvas.SetTop(windowRect, top);
            MainCanvas.Children.Add(windowRect);

            // Set the position of the title bar rectangle
            Canvas.SetLeft(titleBarRect, left);
            Canvas.SetTop(titleBarRect, top);
            MainCanvas.Children.Add(titleBarRect);

            // Set the position of the dots text
            Canvas.SetLeft(dotsText, left + rectWidth - margin);
            Canvas.SetTop(dotsText, top - 1);
            MainCanvas.Children.Add(dotsText);

            // Register mouse events for dragging
            titleBarRect.MouseLeftButtonDown += WindowRect_MouseLeftButtonDown;
            titleBarRect.MouseMove += WindowRect_MouseMove;
            dotsText.MouseLeftButtonDown += WindowRect_MouseLeftButtonDown;
            dotsText.MouseMove += WindowRect_MouseMove;
            this.LocationChanged += WindowMoved;
            // Do windows stuffs
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
            Point point = new Point(Left, Top);
            if (previousWindow != null)
            {
                previousWindow.UpdatePosition(previousWindow.MainCanvas.PointFromScreen(point));
            }
        }

        private void UpdatePosition(Point point)
        {
            foreach (UIElement child in MainCanvas.Children)
            {
                // ... will be set to the left (fix this)
                if (child != null)
                {
                    Canvas.SetLeft(child, point.X);
                    Canvas.SetTop(child, point.Y);
                }
            }
        }

        private void UpdatePosition(double deltaX, double deltaY)
        {
            foreach (UIElement child in MainCanvas.Children)
            {
                // ... will be set to the left (fix this)
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

                // Move the next window relative to the main window
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

    }
}