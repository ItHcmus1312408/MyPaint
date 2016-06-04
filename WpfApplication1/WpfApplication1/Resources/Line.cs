using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace adorners
{
    class LineAdorner : Adorner
    {
        // Resizing adorner uses Thumbs for visual elements. The Thumbs have built-in mouse input handling.
        private Thumb thumbLeft, thumbRight;

        Line selectedLine;
        // To store and manage the adorner's visual children.
        private VisualCollection visualChildren;

        public LineAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            this.visualChildren = new VisualCollection(this);

            // Call a helper method to initialize the Thumbs with a customized cursors.
            BuildAdorner(ref this.thumbLeft, Cursors.SizeWE);
            BuildAdorner(ref this.thumbRight, Cursors.SizeWE);

            // Add handlers for resizing.
            this.thumbLeft.DragDelta += new DragDeltaEventHandler(HandleLeft);
            this.thumbRight.DragDelta += new DragDeltaEventHandler(HandleRight);
        }

        // Helper method to instantiate the thumbs, set the Cursor property, 
        // set some appearance properties, and add the elements to the visual tree.
        private void BuildAdorner(ref Thumb thumb, Cursor cursor)
        {
            if (thumb != null) return;

            thumb = new Thumb
            {
                Width = 10,
                Height = 10,
                Opacity = 0.40,
                Cursor = cursor,
                Background = new SolidColorBrush(Colors.MediumBlue)
            };

            this.visualChildren.Add(thumb);
        }

        // Arrange the Adorners.
        protected override Size ArrangeOverride(Size finalSize)
        {
            selectedLine = AdornedElement as Line;

            var startRect = new Rect(selectedLine.X1 - (this.thumbLeft.Width / 2), selectedLine.Y1 - (this.thumbLeft.Width / 2), this.thumbLeft.Width, this.thumbLeft.Height);
            this.thumbLeft.Arrange(startRect);

            var endRect = new Rect(selectedLine.X2 - (this.thumbRight.Width / 2), selectedLine.Y2 - (this.thumbRight.Height / 2), this.thumbRight.Width, this.thumbRight.Height);
            this.thumbRight.Arrange(endRect);

            return finalSize;
        }

        // Handler for resizing from the left.
        private void HandleLeft(object sender, DragDeltaEventArgs args)
        {
            Thumb hitThumb = sender as Thumb;
            if (hitThumb == null) return;

            selectedLine = AdornedElement as Line;
            if (selectedLine == null) return;

            //selectedLine.X1 = selectedLine.X1 + args.HorizontalChange;

            Point position = Mouse.GetPosition(this);
            selectedLine.X1 = position.X;
            selectedLine.Y1 = position.Y;

        }

        // Handler for resizing from the right.
        private void HandleRight(object sender, DragDeltaEventArgs args)
        {
            Thumb hitThumb = sender as Thumb;
            if (hitThumb == null) return;

            selectedLine = this.AdornedElement as Line;
            if (selectedLine == null) return;

            //selectedLine.X2 = selectedLine.X2 + args.HorizontalChange;

            Point position = Mouse.GetPosition(this);
            selectedLine.X2 = position.X;
            selectedLine.Y2 = position.Y;
        }

        // Override the VisualChildrenCount and GetVisualChild properties to interface with the adorner's visual collection.
        protected override int VisualChildrenCount { get { return visualChildren.Count; } }
        protected override Visual GetVisualChild(int index) { return visualChildren[index]; }
    }
}
