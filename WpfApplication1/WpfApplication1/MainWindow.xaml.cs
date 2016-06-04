using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using System.IO;
using Microsoft.Win32;
using System.Windows.Controls.Primitives;

namespace adorners
{
    public class ResizingAdorner : Adorner
    {
        // Resizing adorner uses Thumbs for visual elements.  
        // The Thumbs have built-in mouse input handling.
        Thumb topLeft, topRight, bottomLeft, bottomRight;

        // To store and manage the adorner's visual children.
        VisualCollection visualChildren;
        //ContentControl c;
        

        // Initialize the ResizingAdorner.
        public ResizingAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            visualChildren = new VisualCollection(this);

            // Call a helper method to initialize the Thumbs
            // with a customized cursors.
            BuildAdornerCorner(ref topLeft, Cursors.SizeNWSE);
            BuildAdornerCorner(ref topRight, Cursors.SizeNESW);
            BuildAdornerCorner(ref bottomLeft, Cursors.SizeNESW);
            BuildAdornerCorner(ref bottomRight, Cursors.SizeNWSE);

            // Add handlers for resizing.
            bottomLeft.DragDelta += new DragDeltaEventHandler(HandleBottomLeft);
            bottomRight.DragDelta += new DragDeltaEventHandler(HandleBottomRight);
            topLeft.DragDelta += new DragDeltaEventHandler(HandleTopLeft);
            topRight.DragDelta += new DragDeltaEventHandler(HandleTopRight);
        }

        // Handler for resizing from the bottom-right.
        void HandleBottomRight(object sender, DragDeltaEventArgs args)
        {
            FrameworkElement adornedElement = this.AdornedElement as FrameworkElement;
            Thumb hitThumb = sender as Thumb;

            if (adornedElement == null || hitThumb == null) return;
            FrameworkElement parentElement = adornedElement.Parent as FrameworkElement;

            // Ensure that the Width and Height are properly initialized after the resize.
            EnforceSize(adornedElement);

            // Change the size by the amount the user drags the mouse, as long as it's larger 
            // than the width or height of an adorner, respectively.
            adornedElement.Width = Math.Max(adornedElement.Width + args.HorizontalChange, hitThumb.DesiredSize.Width);
            adornedElement.Height = Math.Max(args.VerticalChange + adornedElement.Height, hitThumb.DesiredSize.Height);
        }

        // Handler for resizing from the top-right.
        void HandleTopRight(object sender, DragDeltaEventArgs args)
        {
            FrameworkElement adornedElement = this.AdornedElement as FrameworkElement;
            Thumb hitThumb = sender as Thumb;

            if (adornedElement == null || hitThumb == null) return;
            FrameworkElement parentElement = adornedElement.Parent as FrameworkElement;

            // Ensure that the Width and Height are properly initialized after the resize.
            EnforceSize(adornedElement);

            // Change the size by the amount the user drags the mouse, as long as it's larger 
            // than the width or height of an adorner, respectively.
            adornedElement.Width = Math.Max(adornedElement.Width + args.HorizontalChange, hitThumb.DesiredSize.Width);
            //adornedElement.Height = Math.Max(adornedElement.Height - args.VerticalChange, hitThumb.DesiredSize.Height);

            double height_old = adornedElement.Height;
            double height_new = Math.Max(adornedElement.Height - args.VerticalChange, hitThumb.DesiredSize.Height);
            double top_old = Canvas.GetTop(adornedElement);
            adornedElement.Height = height_new;
            Canvas.SetTop(adornedElement, top_old - (height_new - height_old));
        }

        // Handler for resizing from the top-left.
        void HandleTopLeft(object sender, DragDeltaEventArgs args)
        {
            FrameworkElement adornedElement = AdornedElement as FrameworkElement;
            Thumb hitThumb = sender as Thumb;

            if (adornedElement == null || hitThumb == null) return;

            // Ensure that the Width and Height are properly initialized after the resize.
            EnforceSize(adornedElement);

            // Change the size by the amount the user drags the mouse, as long as it's larger 
            // than the width or height of an adorner, respectively.
            //adornedElement.Width = Math.Max(adornedElement.Width - args.HorizontalChange, hitThumb.DesiredSize.Width);
            //adornedElement.Height = Math.Max(adornedElement.Height - args.VerticalChange, hitThumb.DesiredSize.Height);

            double width_old = adornedElement.Width;
            double width_new = Math.Max(adornedElement.Width - args.HorizontalChange, hitThumb.DesiredSize.Width);
            double left_old = Canvas.GetLeft(adornedElement);
            adornedElement.Width = width_new;
            Canvas.SetLeft(adornedElement, left_old - (width_new - width_old));

            double height_old = adornedElement.Height;
            double height_new = Math.Max(adornedElement.Height - args.VerticalChange, hitThumb.DesiredSize.Height);
            double top_old = Canvas.GetTop(adornedElement);
            adornedElement.Height = height_new;
            Canvas.SetTop(adornedElement, top_old - (height_new - height_old));
        }

        // Handler for resizing from the bottom-left.
        void HandleBottomLeft(object sender, DragDeltaEventArgs args)
             {
            FrameworkElement adornedElement = AdornedElement as FrameworkElement;
            Thumb hitThumb = sender as Thumb;

            if (adornedElement == null || hitThumb == null) return;

            // Ensure that the Width and Height are properly initialized after the resize.
            EnforceSize(adornedElement);

            // Change the size by the amount the user drags the mouse, as long as it's larger 
            // than the width or height of an adorner, respectively.
            //adornedElement.Width = Math.Max(adornedElement.Width - args.HorizontalChange, hitThumb.DesiredSize.Width);
            adornedElement.Height = Math.Max(args.VerticalChange + adornedElement.Height, hitThumb.DesiredSize.Height);

            double width_old = adornedElement.Width;
            double width_new = Math.Max(adornedElement.Width - args.HorizontalChange, hitThumb.DesiredSize.Width);
            double left_old = Canvas.GetLeft(adornedElement);
            adornedElement.Width = width_new;            
            Canvas.SetLeft(adornedElement, left_old - (width_new - width_old));
        }

        // Arrange the Adorners.
        protected override Size ArrangeOverride(Size finalSize)
        {
            // desiredWidth and desiredHeight are the width and height of the element that's being adorned.  
            // These will be used to place the ResizingAdorner at the corners of the adorned elemet.  
            double desiredWidth = AdornedElement.DesiredSize.Width;
            double desiredHeight = AdornedElement.DesiredSize.Height;
            // adornerWidth & adornerHeight are used for placement as well.
            double adornerWidth = this.DesiredSize.Width;
            double adornerHeight = this.DesiredSize.Height;

            topLeft.Arrange(new Rect(-adornerWidth / 2, -adornerHeight / 2, adornerWidth, adornerHeight));
            topRight.Arrange(new Rect(desiredWidth - adornerWidth / 2, -adornerHeight / 2, adornerWidth, adornerHeight));
            bottomLeft.Arrange(new Rect(-adornerWidth / 2, desiredHeight - adornerHeight / 2, adornerWidth, adornerHeight));
            bottomRight.Arrange(new Rect(desiredWidth - adornerWidth / 2, desiredHeight - adornerHeight / 2, adornerWidth, adornerHeight));

            // Return the final size.
            return finalSize;
        }

        // Helper method to instantiate the corner Thumbs, set the Cursor property, 
        // set some appearance properties, and add the elements to the visual tree.
        void BuildAdornerCorner(ref Thumb cornerThumb, Cursor customizedCursor)
        {
            if (cornerThumb != null) return;

            cornerThumb = new Thumb();

            // Set some arbitrary visual characteristics.
            cornerThumb.Cursor = customizedCursor;
            cornerThumb.Height = cornerThumb.Width = 10;
            cornerThumb.Opacity = 0.40;
            cornerThumb.Background = new SolidColorBrush(Colors.MediumBlue);

            visualChildren.Add(cornerThumb);
        }

        // This method ensures that the Widths and Heights are initialized.  Sizing to content produces
        // Width and Height values of Double.NaN.  Because this Adorner explicitly resizes, the Width and Height
        // need to be set first.  It also sets the maximum size of the adorned element.
        void EnforceSize(FrameworkElement adornedElement)
        {
            if (adornedElement.Width.Equals(Double.NaN))
                adornedElement.Width = adornedElement.DesiredSize.Width;
            if (adornedElement.Height.Equals(Double.NaN))
                adornedElement.Height = adornedElement.DesiredSize.Height;

            FrameworkElement parent = adornedElement.Parent as FrameworkElement;
            if (parent != null)
            {
                adornedElement.MaxHeight = parent.ActualHeight;
                adornedElement.MaxWidth = parent.ActualWidth;
            }
        }
        // Override the VisualChildrenCount and GetVisualChild properties to interface with 
        // the adorner's visual collection.
        protected override int VisualChildrenCount { get { return visualChildren.Count; } }
        protected override Visual GetVisualChild(int index) { return visualChildren[index]; }
    }

    public partial class Window1 : Window
    {
        AdornerLayer aLayer;

        bool _isDown;
        bool _isDragging;
        bool selected = false;
        UIElement selectedElement = null;

        Point _startPoint;
        private double _originalLeft;
        private double _originalTop;

        public Window1()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.MouseLeftButtonDown += new MouseButtonEventHandler(Window1_MouseLeftButtonDown);
            this.MouseLeftButtonUp += new MouseButtonEventHandler(DragFinishedMouseHandler);
            this.MouseMove += new MouseEventHandler(Window1_MouseMove);
            this.MouseLeave += new MouseEventHandler(Window1_MouseLeave);

            paintCanvas.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(myCanvas_PreviewMouseLeftButtonDown);
            paintCanvas.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(DragFinishedMouseHandler);
            this.colorList.ItemsSource = typeof(Brushes).GetProperties();
            this.colorFill.ItemsSource = typeof(Brushes).GetProperties();
        }

        // Handler for drag stopping on leaving the window
        void Window1_MouseLeave(object sender, MouseEventArgs e)
        {
            StopDragging();
            e.Handled = true;
        }

        // Handler for drag stopping on user choise
        void DragFinishedMouseHandler(object sender, MouseButtonEventArgs e)
        {
            StopDragging();
            e.Handled = true;
        }

        // Method for stopping dragging
        private void StopDragging()
        {
            if (_isDown)
            {
                _isDown = false;
                _isDragging = false;
            }
        }

        // Hanler for providing drag operation with selected element
        void Window1_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDown)
            {
                if ((_isDragging == false) &&
                    ((Math.Abs(e.GetPosition(paintCanvas).X - _startPoint.X) > SystemParameters.MinimumHorizontalDragDistance) ||
                    (Math.Abs(e.GetPosition(paintCanvas).Y - _startPoint.Y) > SystemParameters.MinimumVerticalDragDistance)))
                    _isDragging = true;

                if (_isDragging)
                {
                    Point position = Mouse.GetPosition(paintCanvas);
                    Canvas.SetTop(selectedElement, position.Y - (_startPoint.Y - _originalTop));
                    Canvas.SetLeft(selectedElement, position.X - (_startPoint.X - _originalLeft));
                }
            }
        }

        // Handler for clearing element selection, adorner removal
        void Window1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (selected)
            {
                selected = false;
                if (selectedElement != null)
                {
                    aLayer.Remove(aLayer.GetAdorners(selectedElement)[0]);
                    selectedElement = null;
                }
            }
        }

        // Handler for element selection on the canvas providing resizing adorner
        void myCanvas_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Remove selection on clicking anywhere the window
            if (selected)
            {
                selected = false;
                if (selectedElement != null)
                {
                    // Remove the adorner from the selected element
                    aLayer.Remove(aLayer.GetAdorners(selectedElement)[0]);
                    selectedElement = null;
                }
            }

            // If any element except canvas is clicked, 
            // assign the selected element and add the adorner
            if (e.Source != paintCanvas)
            {
                _isDown = true;
                _startPoint = e.GetPosition(paintCanvas);

                selectedElement = e.Source as UIElement;

                _originalLeft = Canvas.GetLeft(selectedElement);
                _originalTop = Canvas.GetTop(selectedElement);

                aLayer = AdornerLayer.GetAdornerLayer(selectedElement);
                aLayer.Add(new ResizingAdorner(selectedElement));
                selected = true;
                e.Handled = true;
                
            }
        }


        bool isMouseDowned = false;
        bool clickedLine = false;
        bool clickedRect = false;
        bool clickedEll = false;
        bool clikedSqr = false;
        bool clickedCir = false;
        bool clickedArr = false;
        bool clickedHe = false;
        bool clickedSta = false;
        bool sl = false;
        bool Dragging;
        private double OLeft;
        private double OTop;
        AdornerLayer Layer;
        UIElement check_element = null;
        bool ins = false;
        Line lastLine, line;
        Ellipse el;
        Rectangle rec;
        Point startPoint;
        Polygon Arrow;
        Polygon Star;
        System.Windows.Shapes.Path Heart;
        Brush StrokeColor = null;
        Brush fill = null;
        double Thick = 1;
        int style_line;
        int dragMode;
        


        private void paintCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            startPoint = e.GetPosition(paintCanvas);
      
            if (check_element != null)
            {
                if (ins)
                {
                    startPoint = e.GetPosition(paintCanvas);
                    OLeft = Canvas.GetLeft(check_element);
                    OTop = Canvas.GetTop(check_element);
                    return;
                }
            }
            if (sl)
            {
                sl = false;
                if (check_element != null)
                {
                    Layer.Remove(Layer.GetAdorners(check_element)[0]);
                    check_element = null;
                }
                isMouseDowned = false;
                return;
            }

            if (e.LeftButton == MouseButtonState.Pressed)
            {

                if (clickedLine)
                {
                    lastLine = new Line();
                    if (StrokeColor == null)
                        lastLine.Stroke = System.Windows.Media.Brushes.Black;
                    else
                        lastLine.Stroke = StrokeColor;
                    if (fill != null)
                        lastLine.Fill = fill;
                    else
                        lastLine.Fill = System.Windows.Media.Brushes.White;
                    lastLine.StrokeThickness = Thick;
                    if (style_line == 2)
                        lastLine.StrokeDashArray = new DoubleCollection() { 1, 2 };
                    if (style_line == 3)
                        lastLine.StrokeDashArray = new DoubleCollection() { 8 };

                    lastLine.X1 = startPoint.X;
                    lastLine.Y1 = startPoint.Y;

                    paintCanvas.Children.Add(lastLine);

                }
            }

            if (clickedRect || clikedSqr)
            {
                rec = new Rectangle();
                if (StrokeColor == null)
                    rec.Stroke = System.Windows.Media.Brushes.Black;
                else
                    rec.Stroke = StrokeColor;
                if (fill != null)
                    rec.Fill = fill;
                else
                    rec.Fill = System.Windows.Media.Brushes.White;
                rec.StrokeThickness = Thick;
                if (style_line == 2)
                    rec.StrokeDashArray = new DoubleCollection() { 1, 2 };
                if (style_line == 3)
                    rec.StrokeDashArray = new DoubleCollection() { 8 };
                Canvas.SetLeft(rec, startPoint.X);
                Canvas.SetTop(rec, startPoint.X);
                paintCanvas.Children.Add(rec);
            }

            if (clickedEll || clickedCir)
            {
                el = new Ellipse();
                if (StrokeColor == null)
                    el.Stroke = System.Windows.Media.Brushes.Black;
                else
                    el.Stroke = StrokeColor;
                if (fill != null)
                    el.Fill = fill;
                else
                    el.Fill = System.Windows.Media.Brushes.White;
                el.StrokeThickness = Thick;

                if (style_line == 2)
                    el.StrokeDashArray = new DoubleCollection() { 1, 2 };
                if (style_line == 3)
                    el.StrokeDashArray = new DoubleCollection() { 8 };
                Canvas.SetLeft(el, startPoint.X);
                Canvas.SetTop(el, startPoint.X);
                paintCanvas.Children.Add(el);
            }

            if (clickedArr)
            {
                Arrow = new Polygon();
                if (StrokeColor == null)
                    Arrow.Stroke = System.Windows.Media.Brushes.Black;
                else
                    Arrow.Stroke = StrokeColor;
                if (fill != null)
                    Arrow.Fill = fill;
                else
                    Arrow.Fill = System.Windows.Media.Brushes.White;
                Arrow.StrokeThickness = Thick;

                if (style_line == 2)
                    Arrow.StrokeDashArray = new DoubleCollection() { 1, 2 };
                if (style_line == 3)
                    Arrow.StrokeDashArray = new DoubleCollection() { 8 };

                Canvas.SetLeft(Arrow, startPoint.X);
                Canvas.SetTop(Arrow, startPoint.Y);
                paintCanvas.Children.Add(Arrow);

            }

            if (clickedHe)
            {
                Heart = new System.Windows.Shapes.Path();
                if (StrokeColor == null)
                    Heart.Stroke = System.Windows.Media.Brushes.Black;
                else
                    Heart.Stroke = StrokeColor;
                if (fill != null)
                    Heart.Fill = fill;
                else
                    Heart.Fill = System.Windows.Media.Brushes.White;
                Heart.StrokeThickness = Thick;

                if (style_line == 2)
                    Heart.StrokeDashArray = new DoubleCollection() { 1, 2 };
                if (style_line == 3)
                    Heart.StrokeDashArray = new DoubleCollection() { 8 };

                Canvas.SetLeft(Heart, startPoint.X);
                Canvas.SetTop(Heart, startPoint.Y);
                paintCanvas.Children.Add(Heart);

            }

            if (clickedSta)
            {
                Star = new Polygon();
                if (StrokeColor == null)
                    Star.Stroke = System.Windows.Media.Brushes.Black;
                else
                    Star.Stroke = StrokeColor;
                if (fill != null)
                    Star.Fill = fill;
                else
                    Star.Fill = System.Windows.Media.Brushes.White;
                Star.StrokeThickness = Thick;

                if (style_line == 2)
                    Star.StrokeDashArray = new DoubleCollection() { 1, 2 };
                if (style_line == 3)
                    Star.StrokeDashArray = new DoubleCollection() { 8 };

                Canvas.SetLeft(Star, startPoint.X);
                Canvas.SetTop(Star, startPoint.Y);
                paintCanvas.Children.Add(Star);
            }
        }

        private void paintCanvas_MouseMove(object sender, MouseEventArgs e)
        {

            if (e.LeftButton != MouseButtonState.Pressed)
                return;
            if (isMouseDowned)
            {
                var endPoint = e.GetPosition(paintCanvas);
                if ((Dragging == false) &&
                    ((Math.Abs(endPoint.X - startPoint.X) > SystemParameters.MinimumHorizontalDragDistance) ||
                    (Math.Abs(endPoint.Y - startPoint.Y) > SystemParameters.MinimumVerticalDragDistance)))
                    Dragging = true;

                if (Dragging)
                {
                    endPoint = e.GetPosition(paintCanvas);

                    endPoint = e.GetPosition(paintCanvas);
                    if (dragMode != 1)
                    {
                        if (check_element != null)
                        {
                            Canvas.SetTop(check_element, endPoint.Y - (startPoint.Y - OTop));
                            Canvas.SetLeft(check_element, endPoint.X - (startPoint.X - OLeft));
                        }

                    }
                    else
                    {

                        Line tempLine = check_element as Line;
                        var deltaX = Math.Abs(endPoint.X - startPoint.X);
                        var deltaY = Math.Abs(endPoint.Y - startPoint.Y);
                        if (Math.Max(endPoint.X, startPoint.X) == startPoint.X)
                        {
                            tempLine.X1 -= deltaX;
                            tempLine.X2 -= deltaX;
                        }
                        else
                        {
                            tempLine.X1 += deltaX;
                            tempLine.X2 += deltaX;
                        }
                        if (Math.Max(endPoint.Y, startPoint.Y) == startPoint.Y)
                        {
                            tempLine.Y1 -= deltaX;
                            tempLine.Y2 -= deltaX;
                        }
                        else
                        {
                            tempLine.Y1 += deltaX;
                            tempLine.Y2 += deltaX;
                        }
                        startPoint = endPoint;
                    }

                }
                return;
            }

            if(clickedLine)
            {
                var endPoint = e.GetPosition(paintCanvas);
                if(lastLine != null)
                {
                    lastLine.X2 = endPoint.X;
                    lastLine.Y2 = endPoint.Y;
                }
            }

            if (clickedRect || clikedSqr)
            {
                if (e.LeftButton == MouseButtonState.Released || rec == null)
                    return;

                var pos = e.GetPosition(paintCanvas);

                var x = Math.Min(pos.X, startPoint.X);
                var y = Math.Min(pos.Y, startPoint.Y);

                var w = Math.Max(pos.X, startPoint.X) - x;
                var h = Math.Max(pos.Y, startPoint.Y) - y;

                if (clikedSqr)
                {
                    rec.Width = w;
                    rec.Height = rec.Width;
                }
                else
                {
                    rec.Width = w;
                    rec.Height = h;
                }

                rec.MinWidth = 0;
                rec.MinHeight = 0;

                Canvas.SetLeft(rec, x);
                Canvas.SetTop(rec, y);
            }

            if (clickedEll || clickedCir)
            {
                if (e.LeftButton == MouseButtonState.Released || el == null)
                    return;

                var pos = e.GetPosition(paintCanvas);

                var x = Math.Min(pos.X, startPoint.X);
                var y = Math.Min(pos.Y, startPoint.Y);

                var w = Math.Max(pos.X, startPoint.X) - x;
                var h = Math.Max(pos.Y, startPoint.Y) - y;

                if (clickedCir)
                {
                    el.Width = w;
                    el.Height = el.Width;
                }
                else
                {
                    el.Width = w;
                    el.Height = h;
                }

                el.MinWidth = 0;
                el.MinHeight = 0;
                Canvas.SetLeft(el, x);
                Canvas.SetTop(el, y);
            }

            if (clickedArr)
            {
                var EndPoint = e.GetPosition(paintCanvas);

                if (Arrow == null)
                    return;

                Canvas.SetLeft(Arrow, Math.Min(startPoint.X, EndPoint.X));
                Canvas.SetTop(Arrow, Math.Min(startPoint.Y, EndPoint.Y));

                Arrow.Height = Math.Abs(EndPoint.Y - startPoint.Y);
                Arrow.Width = Math.Abs(EndPoint.X - startPoint.X);
                Arrow.Stretch = Stretch.Fill;
                PointCollection arrow_point = new PointCollection();
                arrow_point.Add(new Point(1, 2)); 
                arrow_point.Add(new Point(5, 2)); 
                arrow_point.Add(new Point(5, 1)); 
                arrow_point.Add(new Point(9, 3)); 
                arrow_point.Add(new Point(5, 5)); 
                arrow_point.Add(new Point(5, 4)); 
                arrow_point.Add(new Point(1, 4));
                Arrow.Points = arrow_point;
            }

            if (clickedSta)
            {
                var EndPoint = e.GetPosition(paintCanvas);
                if (Star == null)
                    return;
                Canvas.SetLeft(Star, Math.Min(startPoint.X, EndPoint.X));
                Canvas.SetTop(Star, Math.Min(startPoint.Y, EndPoint.Y));

                Star.Height = Math.Abs(EndPoint.Y - startPoint.Y);
                Star.Width = Math.Abs(EndPoint.X - startPoint.X);
                Star.Stretch = Stretch.Fill;
                PointCollection star_point = new PointCollection();
                star_point.Add(new Point(0, 0));
                star_point.Add(new Point(-0.11226, 0.34549));
                star_point.Add(new Point(-0.47552, 0.34549));
                star_point.Add(new Point(-0.18163, 0.55901));
                star_point.Add(new Point(-0.29389, 0.90451));
                star_point.Add(new Point(0, 0.69097));
                star_point.Add(new Point(0.29389, 0.90451));
                star_point.Add(new Point(0.18163, 0.55901));
                star_point.Add(new Point(0.47552, 0.34549));
                star_point.Add(new Point(0.11226, 0.34549));
                Star.Points = star_point;
            }

            if (clickedHe)
            {
                var EndPoint = e.GetPosition(paintCanvas);
                if (Heart == null)
                    return;
                Canvas.SetLeft(Heart, Math.Min(startPoint.X, EndPoint.X));
                Canvas.SetTop(Heart, Math.Min(startPoint.Y, EndPoint.Y));

                Heart.Height = Math.Abs(EndPoint.Y - startPoint.Y);
                Heart.Width = Math.Abs(EndPoint.X - startPoint.X);

                Heart.Data = Geometry.Parse(@"M 241,200 
                                            A 20,20 0 0 0 200,240
                                            C 210,250 240,270 240,270
                                            C 240,270 260,260 280,240
                                            A 20,20 0 0 0 239,200");
                Heart.Stretch = Stretch.Fill;
            }
        }
        private void paintCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var endPoint = e.GetPosition(paintCanvas);
            if (rec != null || el != null || lastLine != null || Arrow != null || Star != null || Heart != null)
            {
                isMouseDowned = true;
                startPoint = e.GetPosition(paintCanvas);

                if (rec != null)
                    check_element = rec;
                else if (el != null)
                    check_element = el;
                else if (Arrow != null)
                    check_element = Arrow;
                else if (Star != null)
                    check_element = Star;
                else if (Heart != null)
                    check_element = Heart;
                else
                    check_element = lastLine;

                if (lastLine == null)
                {
                    OLeft = Canvas.GetLeft(check_element);
                    OTop = Canvas.GetTop(check_element);
                    Layer = AdornerLayer.GetAdornerLayer(check_element);
                    Layer.Add(new ResizingAdorner(check_element));
                }
                else
                {
                    OLeft = lastLine.X1;
                    OTop = lastLine.Y1;
                    Layer = AdornerLayer.GetAdornerLayer(check_element);
                    var a = new LineAdorner(check_element);
                    a.ClipToBounds = true;
                    if(Layer != null)
                        Layer.Add(a);
                }
                sl = true;

            }

            rec = null;
            el = null;
            lastLine = null;
            Arrow = null;
            Star = null;
            Heart = null;
        }

        private int Save_Object()
        {
            int n = paintCanvas.Children.Count;
            try
            {
                Line line = paintCanvas.Children[n - 1] as Line;
                if (line != null)
                    return 1;
            }
            catch { }
            try
            {
                Ellipse el = paintCanvas.Children[n - 1] as Ellipse;
                if (el != null) return 2;
            }
            catch { }

            try
            {
                Rectangle rec = paintCanvas.Children[n - 1] as Rectangle;
                if (rec != null) return 3;
            }
            catch { }

            try
            {
                Polygon arr = paintCanvas.Children[n - 1] as Polygon;
                if (arr != null) return 4;
            }
            catch { }

            try
            {
                System.Windows.Shapes.Path heart = paintCanvas.Children[n - 1] as System.Windows.Shapes.Path;
                if (heart != null) return 5;
            }
            catch { }

            try
            {
                Polygon star = paintCanvas.Children[n - 1] as Polygon;
                if (star != null) return 6;
            }
            catch { }


            return 0;
        }
        private void Line_shape_Click(object sender, RoutedEventArgs e)
        {
            clickedLine = true;
            clickedRect = false;
            clickedEll = false;
            clikedSqr = false;
            clickedCir = false;
            clickedArr = false;
            clickedHe = false;
            clickedSta = false;
        }
        private void Rectangle_shape_Click(object sender, RoutedEventArgs e)
        {
            clickedRect = true;
            clickedLine = false;
            clickedEll = false;
            clikedSqr = false;
            clickedCir = false;
            clickedArr = false;
            clickedHe = false;
            clickedSta = false;
        }
        private void Ellipse_shape_Click(object sender, RoutedEventArgs e)
        {
            clickedRect = false;
            clickedLine = false;
            clickedEll = true;
            clikedSqr = false;
            clickedCir = false;
            clickedArr = false;
            clickedHe = false;
            clickedSta = false;
        }
        private void Square_shape_Click(object sender, RoutedEventArgs e)
        {
            clickedLine = false;
            clickedRect = false;
            clickedEll = false;
            clikedSqr = true;
            clickedCir = false;
            clickedArr = false;
            clickedHe = false;
            clickedSta = false;
        }
        private void Circle_shape_Click(object sender, RoutedEventArgs e)
        {
            clickedLine = false;
            clickedRect = false;
            clickedEll = false;
            clikedSqr = false;
            clickedCir = true;
            clickedArr = false;
            clickedHe = false;
            clickedSta = false;
        }

        private void Arrow_shape_Click(object sender, RoutedEventArgs e)
        {
            clickedLine = false;
            clickedRect = false;
            clickedEll = false;
            clikedSqr = false;
            clickedCir = false;
            clickedArr = true;
            clickedHe = false;
            clickedSta = false;
        }

        private void Heart_shape_Click(object sender, RoutedEventArgs e)
        {
            clickedLine = false;
            clickedRect = false;
            clickedEll = false;
            clikedSqr = false;
            clickedCir = false;
            clickedArr = false;
            clickedHe = true;
            clickedSta = false;
        }

        private void Star_shape_Click(object sender, RoutedEventArgs e)
        {
            clickedLine = false;
            clickedRect = false;
            clickedEll = false;
            clikedSqr = false;
            clickedCir = false;
            clickedArr = false;
            clickedHe = false;
            clickedSta = true;
        }
       
        private void colorList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Brush selectedColor = (Brush)(e.AddedItems[0] as PropertyInfo).GetValue(null, null);
            StrokeColor = selectedColor;
        }

        private void colorFill_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Brush selectedColor = (Brush)(e.AddedItems[0] as PropertyInfo).GetValue(null, null);
            fill = selectedColor;
        }
        private void cboStyles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Line choose = (Line)cboStyles.SelectedItem;
            Thick = choose.StrokeThickness;
        }

        private void cbo_Linestyle(object sender, SelectionChangedEventArgs e)
        {
            ComboBox ComboBox = (ComboBox)sender;
            if (ComboBox == null)
                return;
            ComboBoxItem temp = ComboBox.SelectedItem as ComboBoxItem;
            style_line = int.Parse(temp.Tag.ToString());
        }

        private void button_save_Click(object sender, RoutedEventArgs e)
        {
            CreateSaveDialog();
            util.SaveCanvas(this, paintCanvas, 96, save_filename);
        }
        Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
        string save_filename;
        private void CreateSaveDialog()
        {
            dlg.FileName = "Canvas"; // Default file name
            dlg.DefaultExt = ".bmp"; // Default file extension
            dlg.Filter = "Canvas (.cvs)|*.cvs|Bitmap (.bmp)|*.bmp|Png Files (*.png)|*.png|Jpg Files (*.jpg)|*.jpg"; // Filter files by extension
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                string filename_tmp = dlg.FileName;
            }
            save_filename = dlg.FileName.ToString();
        }
        Microsoft.Win32.OpenFileDialog dlg_open = new Microsoft.Win32.OpenFileDialog();
        private void button_open_Click(object sender, RoutedEventArgs e)
        {
            dlg_open.FileName = "File Name";
            dlg_open.DefaultExt = ".png";
            dlg_open.Filter = "Canvas File(*.cvs)|*.cvs|Png Files (*.png)|*.png|Jpg Files (*.jpg)|*.jpg|Bmp Files (*.bmp)|*.bmp|All File(*.*)|*.*";
            Nullable<bool> result = dlg_open.ShowDialog();

            if (result == true)
            {
                Image img = new Image();
                string filename = dlg_open.FileName;
                img.Source = new BitmapImage(new Uri(@filename));
                paintCanvas.Children.Add(img);
            }
        }
        private void button_exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void button_new_Click(object sender, RoutedEventArgs e)
        {
           // paintCanvas.Children.RemoveRange(0, paintCanvas.Children.Count);
            paintCanvas.Children.RemoveRange(1, paintCanvas.Children.Count);
        }
        private void btn_save_clipboard(object sender, RoutedEventArgs e)
        {
            CopyUIElementToClipboard(selectedElement as FrameworkElement);
        }

        public static void CopyUIElementToClipboard(FrameworkElement element)
        {
            double width = element.ActualWidth;
            double height = element.ActualHeight;
            RenderTargetBitmap bmpCopied = new RenderTargetBitmap((int)Math.Round(width), (int)Math.Round(height), 96, 96, PixelFormats.Default);
            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(element);
                dc.DrawRectangle(vb, null, new Rect(new Point(), new Size(width, height)));
                
            }
            bmpCopied.Render(dv);
            Clipboard.SetImage(bmpCopied);
        }
        private void btn_paste(object sender, RoutedEventArgs e)
        {
           
        }

        int color, txtcolor;
        private void btn_addtext(object sender, RoutedEventArgs e)
        {
           
            ComboBoxItem typeItem = (ComboBoxItem)cbo_textsize.SelectedItem;
            string value = typeItem.Content.ToString();
            TextBox txtRun = new TextBox();
            txtRun.FontSize = double.Parse(value);
            //txtRun.FontFamily = new FontFamily("Time New Roman");

            switch (txtcolor)
            {
                case 1:
                    txtRun.Foreground = System.Windows.Media.Brushes.White;
                    break;
                case 2:
                    txtRun.Foreground = System.Windows.Media.Brushes.Black;
                    break;
                case 3:
                    txtRun.Foreground = System.Windows.Media.Brushes.Red;
                    break;
                case 4:
                    txtRun.Foreground = System.Windows.Media.Brushes.Orange;
                    break;
                case 5:
                    txtRun.Foreground = System.Windows.Media.Brushes.Yellow;
                    break;
                case 6:
                    txtRun.Foreground = System.Windows.Media.Brushes.Green;
                    break;
                case 7:
                    txtRun.Foreground = System.Windows.Media.Brushes.Blue;
                    break;
                case 8:
                    txtRun.Foreground = System.Windows.Media.Brushes.Purple;
                    break;
                case 9:
                    txtRun.Foreground = System.Windows.Media.Brushes.Gray;
                    break;
            }
            switch(color)
            {
                case 1:
                    txtRun.Background = System.Windows.Media.Brushes.White;
                    break;
                case 2:
                    txtRun.Background = System.Windows.Media.Brushes.Black;
                    break;
                case 3:
                    txtRun.Background = System.Windows.Media.Brushes.Red;
                    break;
                case 4:
                    txtRun.Background = System.Windows.Media.Brushes.Orange;
                    break;
                case 5:
                    txtRun.Background = System.Windows.Media.Brushes.Yellow;
                    break;
                case 6:
                    txtRun.Background = System.Windows.Media.Brushes.Green;
                    break;
                case 7:
                    txtRun.Background = System.Windows.Media.Brushes.Blue;
                    break;
                case 8:
                    txtRun.Background = System.Windows.Media.Brushes.Purple;
                    break;
                case 9:
                    txtRun.Background = System.Windows.Media.Brushes.Gray;
                    break;
            }
            txtRun.TextWrapping = TextWrapping.Wrap;
            txtRun.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            txtRun.AcceptsReturn = true;
            paintCanvas.Children.Add(txtRun);
            Canvas.SetLeft(txtRun, 16);
            Canvas.SetRight(txtRun, 32);
            
        }

        private void cbo_background(object sender, SelectionChangedEventArgs e)
        {
            ComboBox ComboBox = (ComboBox)sender;
            if (ComboBox == null)
                return;
            ComboBoxItem temp = ComboBox.SelectedItem as ComboBoxItem;
            color = int.Parse(temp.Tag.ToString());
        }
        private void btn_font(object sender, RoutedEventArgs e)
        {

            
        }
        private void cbo_size(object sender, SelectionChangedEventArgs e)
        {
           
        }
        private void cbo_textcolor(object sender, SelectionChangedEventArgs e)
        {
            ComboBox ComboBox = (ComboBox)sender;
            if (ComboBox == null)
                return;
            ComboBoxItem temp = ComboBox.SelectedItem as ComboBoxItem;
            txtcolor = int.Parse(temp.Tag.ToString());
            
        }
        private void btn_undo(object sender, RoutedEventArgs e)
        {
            int n = paintCanvas.Children.Count;

            if (n == 0)
            {
                Undo.IsEnabled = false;
                return;
            }

            switch (Save_Object())
            {               
                case 1:
                    Line line = paintCanvas.Children[n - 1] as Line;
                    Redo_Object.Ob.Add(line);
                    Redo_Object.L.Add(1);
                    break;
                case 2:
                    Ellipse el = paintCanvas.Children[n - 1] as Ellipse;
                    Redo_Object.Ob.Add(el);
                    Redo_Object.L.Add(2);
                    break;
                case 3:
                    Rectangle rtl = paintCanvas.Children[n - 1] as Rectangle;
                    Redo_Object.Ob.Add(rtl);
                    Redo_Object.L.Add(3);
                    break;
                case 4:
                    Polygon arr = paintCanvas.Children[n - 1] as Polygon;
                    Redo_Object.Ob.Add(arr);
                    Redo_Object.L.Add(4);
                    break;
                case 5:
                    System.Windows.Shapes.Path heart = paintCanvas.Children[n - 1] as System.Windows.Shapes.Path;
                    Redo_Object.Ob.Add(heart);
                    Redo_Object.L.Add(5);
                    break;
                case 6:
                    Polygon star = paintCanvas.Children[n - 1] as Polygon;
                    Redo_Object.Ob.Add(star);
                    Redo_Object.L.Add(6);
                    break;
            }
            Redo_Object.n++;
            paintCanvas.Children.RemoveAt(n - 1);
        }
        private void btn_redo(object sender, RoutedEventArgs e)
        {
            int n = Redo_Object.n - 1;
            if (n < 0)
            {
                Redo.IsEnabled = false;
                return;
            }
                
            switch (Redo_Object.L[n])
            {         
                case 1:
                    paintCanvas.Children.Add(Redo_Object.Ob[n] as Line);
                    break;
                case 2:
                    paintCanvas.Children.Add(Redo_Object.Ob[n] as Ellipse);
                    break;
                case 3:
                    paintCanvas.Children.Add(Redo_Object.Ob[n] as Rectangle);
                    break;
                case 4:
                    paintCanvas.Children.Add(Redo_Object.Ob[n] as Polygon);
                    break;
                case 5:
                    paintCanvas.Children.Add(Redo_Object.Ob[n] as System.Windows.Shapes.Path);
                    break;
                case 6:
                    paintCanvas.Children.Add(Redo_Object.Ob[n] as Polygon);
                    break;
            }

            Redo_Object.Ob.RemoveAt(Redo_Object.n - 1);
            Redo_Object.L.RemoveAt(Redo_Object.n - 1);
            Redo_Object.n--;
        }
    }
    public static class util
    {
        public static void SaveWindow(Window window, int dpi, string filename)
        {
            var rtb = new RenderTargetBitmap(
                (int)window.Width, //width 
                (int)window.Width, //height 
                dpi, //dpi x 
                dpi, //dpi y 
                PixelFormats.Pbgra32// pixelformat 
                );
            rtb.Render(window);
            SaveRTBAsPNG(rtb, filename);

        }
        public static void SaveCanvas(Window window, Canvas canvas, int dpi, string filename)
        {
            Size size = new Size(window.Width, window.Height);
            canvas.Measure(size);

            var rtb = new RenderTargetBitmap(
                (int)window.Width, //width 
                (int)window.Height, //height 
                dpi, //dpi x 
                dpi, //dpi y 
                PixelFormats.Pbgra32 // pixelformat 
                );
            rtb.Render(canvas);
            SaveRTBAsPNG(rtb, filename);
        }
        private static void SaveRTBAsPNG(RenderTargetBitmap bmp, string filename)
        {
            var enc = new System.Windows.Media.Imaging.PngBitmapEncoder();
            enc.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(bmp));

            using (var stm = System.IO.File.Create(filename))
            {
                enc.Save(stm);
            }
        }

    }
}