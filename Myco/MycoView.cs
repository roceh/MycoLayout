﻿using SkiaSharp;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Myco
{
    public class MycoView : BindableObject, IAnimatable
    {
        #region Fields

        public static readonly BindableProperty FrameColorProperty = BindableProperty.Create(nameof(FrameColor), typeof(Color), typeof(MycoView), Color.Transparent);
        public static readonly BindableProperty FrameThicknessProperty = BindableProperty.Create(nameof(FrameThickness), typeof(double), typeof(MycoView), 0.0);

        public static readonly BindableProperty IsClippedToBoundsProperty = BindableProperty.Create(nameof(IsClippedToBounds), typeof(bool), typeof(MycoView), false, BindingMode.OneWay);
        public static readonly BindableProperty BackgroundColorProperty = BindableProperty.Create(nameof(BackgroundColor), typeof(Color), typeof(MycoView), Color.Transparent);                
        public static readonly BindableProperty HorizontalOptionsProperty = BindableProperty.Create(nameof(HorizontalOptions), typeof(LayoutOptions), typeof(MycoView), LayoutOptions.Fill);
        public static readonly BindableProperty IsVisibleProperty = BindableProperty.Create(nameof(IsVisible), typeof(bool), typeof(MycoView), true, BindingMode.OneWay);
        public static readonly BindableProperty MarginProperty = BindableProperty.Create(nameof(Margin), typeof(Thickness), typeof(MycoView), new Thickness(0, 0, 0, 0));
        public static readonly BindableProperty ScaleProperty = BindableProperty.Create(nameof(Scale), typeof(double), typeof(MycoView), 1.0);
        public static readonly BindableProperty TranslationXProperty = BindableProperty.Create(nameof(TranslationX), typeof(double), typeof(MycoView), 0.0);
        public static readonly BindableProperty TranslationYProperty = BindableProperty.Create(nameof(TranslationY), typeof(double), typeof(MycoView), 0.0);
        public static readonly BindableProperty VerticalOptionsProperty = BindableProperty.Create(nameof(VerticalOptions), typeof(LayoutOptions), typeof(MycoView), LayoutOptions.Fill);
        public static readonly BindableProperty WidthRequestProperty = BindableProperty.Create(nameof(WidthRequest), typeof(double), typeof(MycoView), -1.0, BindingMode.OneWay);
        public static readonly BindableProperty HeightRequestProperty = BindableProperty.Create(nameof(HeightRequest), typeof(double), typeof(MycoView), -1.0, BindingMode.OneWay);

        public static readonly BindablePropertyKey XPropertyKey = BindableProperty.CreateReadOnly(nameof(X), typeof(double), typeof(MycoView), 0.0, BindingMode.OneWayToSource);
        public static readonly BindableProperty XProperty = XPropertyKey.BindableProperty;

        public static readonly BindablePropertyKey YPropertyKey = BindableProperty.CreateReadOnly(nameof(Y), typeof(double), typeof(MycoView), 0.0, BindingMode.OneWayToSource);
        public static readonly BindableProperty YProperty = YPropertyKey.BindableProperty;

        public static readonly BindablePropertyKey WidthPropertyKey = BindableProperty.CreateReadOnly(nameof(Width), typeof(double), typeof(MycoView), 0.0, BindingMode.OneWayToSource);
        public static readonly BindableProperty WidthProperty = WidthPropertyKey.BindableProperty;

        public static readonly BindablePropertyKey HeightPropertyKey = BindableProperty.CreateReadOnly(nameof(Height), typeof(double), typeof(MycoView), 0.0, BindingMode.OneWayToSource);
        public static readonly BindableProperty HeightProperty = HeightPropertyKey.BindableProperty;

        public static readonly BindableProperty OpacityProperty = BindableProperty.Create(nameof(OpacityProperty), typeof(double), typeof(MycoView), 1.0);
                
        private readonly List<MycoGestureRecognizer> _gestureRecognizers = new List<MycoGestureRecognizer>();
        private MycoContainer _container;
        private MycoView _parent;

        #endregion Fields

        #region Properties

        public Action<SKCanvas> Drawing { get; set; }
        
        public Color BackgroundColor
        {
            get
            {
                return (Color)GetValue(BackgroundColorProperty);
            }
            set
            {
                SetValue(BackgroundColorProperty, value);
            }
        }

        public Color FrameColor
        {
            get
            {
                return (Color)GetValue(FrameColorProperty);
            }
            set
            {
                SetValue(FrameColorProperty, value);
            }
        }

        public Rectangle Bounds
        {
            get
            {
                return new Rectangle(X, Y, Width, Height);
            }
        }

        public MycoContainer Container
        {
            get
            {
                return _container;
            }

            set
            {
                _container = value;
            }
        }

        public double FrameThickness
        {
            get
            {
                return (double)GetValue(FrameThicknessProperty);
            }
            set
            {
                SetValue(FrameThicknessProperty, value);
            }
        }

        public IList<MycoGestureRecognizer> GestureRecognizers
        {
            get { return _gestureRecognizers; }
        }

        public double Height
        {
            get
            {
                return (double)GetValue(HeightProperty);
            }
            private set
            {
                SetValue(HeightPropertyKey, value);
            }
        }

        public double HeightRequest
        {
            get
            {
                return (double)GetValue(MycoView.HeightRequestProperty);
            }
            set
            {
                SetValue(MycoView.HeightRequestProperty, value);
            }
        }

        public LayoutOptions HorizontalOptions
        {
            get
            {
                return (LayoutOptions)GetValue(HorizontalOptionsProperty);
            }
            set
            {
                SetValue(HorizontalOptionsProperty, value);
            }
        }

        public bool IsVisible
        {
            get
            {
                return (bool)GetValue(MycoView.IsVisibleProperty);
            }
            set
            {
                SetValue(MycoView.IsVisibleProperty, value);
            }
        }

        public bool IsClippedToBounds
        {
            get
            {
                return (bool)GetValue(MycoView.IsClippedToBoundsProperty);
            }
            set
            {
                SetValue(MycoView.IsClippedToBoundsProperty, value);
            }
        }

        public Thickness Margin
        {
            get
            {
                return (Thickness)GetValue(MarginProperty);
            }
            set
            {
                SetValue(MarginProperty, value);
            }
        }

        public MycoView Parent
        {
            get
            {
                return _parent;
            }

            set
            {
                _parent = value;
            }
        }

        public double InheritiedScale
        {
            get
            {
                if (Parent != null)
                {
                    return Scale * Parent.InheritiedScale;
                }

                return 1;
            }
        }

        public double InheritiedTranslationX
        {
            get
            {
                if (Parent != null)
                {
                    return TranslationX + Parent.InheritiedTranslationX;
                }

                return 0;
            }
        }

        public double InheritiedTranslationY
        {
            get
            {
                if (Parent != null)
                {
                    return TranslationY + Parent.InheritiedTranslationY;
                }

                return 0;
            }
        }

        public double InheritedX
        {
            get
            {
                if (Parent != null)
                {
                    return X + Parent.InheritedX;
                }

                return 0;
            }
        }

        public double InheritedY
        {
            get
            {
                if (Parent != null)
                {
                    return Y + Parent.InheritedY;
                }

                return 0;
            }
        }

        public double Opacity
        {
            get

            {
                return (double)GetValue(OpacityProperty);
            }
            set
            {
                SetValue(OpacityProperty, value);
            }
        }

        public bool IsOpaque
        {
            get
            {
                return Math.Abs(Opacity - 1.0) < 0.0001;
            }
        }

        public bool IsScaled
        {
            get
            {
                return Math.Abs(Scale - 1.0) > 0.0001;
            }
        }

        public Rectangle RenderBounds
        {
            get
            {
                var totalTranslationX = InheritiedTranslationX;
                var totalTranslationY = InheritiedTranslationY;
                var totalX = InheritedX;
                var totalY = InheritedY;
                
                return new Rectangle(
                    totalX + totalTranslationX,
                    totalY + totalTranslationY,
                    Width,
                    Height);
            }
        }

        public double Scale
        {
            get
            {
                return (double)GetValue(ScaleProperty);
            }
            set
            {
                SetValue(ScaleProperty, value);
            }
        }

        public double TranslationX
        {
            get
            {
                return (double)GetValue(TranslationXProperty);
            }
            set
            {
                SetValue(TranslationXProperty, value);
            }
        }

        public double TranslationY
        {
            get
            {
                return (double)GetValue(TranslationYProperty);
            }
            set
            {
                SetValue(TranslationYProperty, value);
            }
        }

        public LayoutOptions VerticalOptions
        {
            get
            {
                return (LayoutOptions)GetValue(VerticalOptionsProperty);
            }
            set
            {
                SetValue(VerticalOptionsProperty, value);
            }
        }

        public double Width
        {
            get
            {
                return (double)GetValue(WidthProperty);
            }
            private set
            {
                SetValue(WidthPropertyKey, value);
            }
        }

        public double WidthRequest
        {
            get
            {
                return (double)GetValue(MycoView.WidthRequestProperty);
            }
            set
            {
                SetValue(MycoView.WidthRequestProperty, value);
            }
        }

        public double X
        {
            get
            {
                return (double)GetValue(XProperty);
            }
            private set
            {
                SetValue(XPropertyKey, value);
            }
        }

        public double Y
        {
            get
            {
                return (double)GetValue(YProperty);
            }
            private set
            {
                SetValue(YPropertyKey, value);
            }
        }

        #endregion Properties

        #region Methods

        public void BatchBegin()
        {
        }

        public void BatchCommit()
        {
            Invalidate();
        }

        public void Draw(SKCanvas canvas)
        {
            if (!IsVisible)
                return;

            int canvasState = int.MaxValue;

            var renderBounds = RenderBounds;

            if (IsScaled || IsClippedToBounds)
            {
                canvasState = canvas.Save();
            }

            if (IsScaled)
            {
                canvas.Translate((float)renderBounds.Center.X, (float)renderBounds.Center.Y);
                canvas.Scale((float)Scale, (float)Scale);
                canvas.Translate((float)-renderBounds.Center.X, (float)-renderBounds.Center.Y);
            }

            if (IsClippedToBounds)
            {
                canvas.ClipRect(renderBounds.ToSKRect());
            }

            if (IsOpaque)
            {
                InternalDraw(canvas);
            }
            else
            {
                using (var surface = GetSKContainer().CreateOpacitySurface())
                {
                    if (IsClippedToBounds)
                    {
                        surface.Canvas.ClipRect(renderBounds.ToSKRect());
                    }

                    InternalDraw(surface.Canvas);

                    using (var paint = new SKPaint())
                    {
                        paint.Color = new SKColor(255, 255, 255, (byte)(255.0 * Opacity));
                        canvas.DrawImage(surface.Snapshot(), 0, 0, paint);
                    }
                }
            }

            if (canvasState != int.MaxValue)                
            {
                canvas.RestoreToCount(canvasState);
            }
        }

        protected virtual void InternalDraw(SKCanvas canvas)
        {
            using (var paint = new SKPaint())
            {
                if (BackgroundColor.A > 0)
                {
                    paint.Color = BackgroundColor.ToSKColor();
                    canvas.DrawRect(RenderBounds.ToSKRect(), paint);
                }

                if (FrameColor.A > 0)
                {
                    paint.Color = FrameColor.ToSKColor();
                    paint.IsStroke = true;
                    paint.StrokeWidth = (float) FrameThickness;
                    canvas.DrawRect(RenderBounds.ToSKRect(), paint);
                }
            }        

            if (Drawing != null)
            {
                Drawing(canvas);
            }
        }

        public virtual void GetGestureRecognizers(Point gestureStart, IList<Tuple<MycoView, IMycoGestureRecognizerController>> matches)
        {
            if (RenderBounds.Contains(gestureStart))
            {
                foreach (var gesture in _gestureRecognizers)
                {
                    matches.Add(new Tuple<MycoView, IMycoGestureRecognizerController>(this, gesture));
                }
            }
        }

        public MycoContainer GetSKContainer()
        {
            if (_parent != null)
            {
                return _parent.GetSKContainer();
            }

            return _container;
        }

        public void Invalidate()
        {
            var container = GetSKContainer();

            if (container != null)
            {
                container.Invalidate();
            }
        }

        public void Layout(Rectangle rectangle)
        {
            if (!IsVisible)
                return;

            InternalLayout(rectangle);
        }

        public Size SizeRequest(double widthConstraint, double heightConstaint)
        {
            if (!IsVisible)
                return new Size(0, 0);

            if (!Double.IsPositiveInfinity(widthConstraint))
            {
                widthConstraint = Math.Max(0, widthConstraint - (Margin.Left + Margin.Right));
            }

            if (!Double.IsPositiveInfinity(heightConstaint))
            {
                heightConstaint = Math.Max(0, heightConstaint - (Margin.Top + Margin.Bottom));
            }

            var result = InternalSizeRequest(widthConstraint, heightConstaint);

            if (!Double.IsPositiveInfinity(result.Width))
            {
                result.Width += Margin.Left + Margin.Right;
            }

            if (!Double.IsPositiveInfinity(result.Height))
            {
                result.Height += Margin.Top + Margin.Bottom;
            }

            return result;
        }

        protected virtual void InternalLayout(Rectangle rectangle)
        {
            X = rectangle.X;
            Y = rectangle.Y;
            Width = rectangle.Width;
            Height = rectangle.Height;
        }

        protected virtual Size InternalSizeRequest(double widthConstraint, double heightConstaint)
        {
            return new Size(
                WidthRequest >= 0 ? (Double.IsPositiveInfinity(widthConstraint) ? WidthRequest  : Math.Min(WidthRequest, widthConstraint)) : widthConstraint,
                HeightRequest >= 0 ? (Double.IsPositiveInfinity(heightConstaint) ? HeightRequest : Math.Min(HeightRequest, heightConstaint)) : heightConstaint);
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            Invalidate();
        }

        #endregion Methods
    }
}