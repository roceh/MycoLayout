﻿using SkiaSharp;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Myco
{
    public class MycoPagingView : MycoView
    {
        #region Fields

        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(nameof(ItemsSource), typeof(object), typeof(MycoPagingView), null,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                ((MycoPagingView)bindable).ItemsSourceChanged(oldValue, newValue);
            });

        public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create(nameof(ItemTemplate), typeof(DataTemplate), typeof(MycoPagingView), null);

        public static readonly BindableProperty SelectedIndexProperty = BindableProperty.Create(nameof(SelectedIndex), typeof(int), typeof(MycoPagingView), 0,
                defaultBindingMode: BindingMode.TwoWay,
                propertyChanged: async (bindable, oldValue, newValue) =>
                {
                    await ((MycoPagingView)bindable).AnimateToPage((int)newValue);
                });

        private MycoView _currentView;
        private MycoView _nextView;
        private int _visiblePageIndex = -1;

        #endregion Fields

        #region Constructors

        public MycoPagingView() : base()
        {
            IsClippedToBounds = true;

            var swipeGestureRecognizer = new MycoSwipeGestureRecognizer();
            swipeGestureRecognizer.SwipeLeft += SwipeGestureRecognizer_SwipeLeft;
            swipeGestureRecognizer.SwipeRight += SwipeGestureRecognizer_SwipeRight;
            GestureRecognizers.Add(swipeGestureRecognizer);
        }

        private async void SwipeGestureRecognizer_SwipeLeft(object sender, EventArgs e)
        {
            if (ItemsSource != null && SelectedIndex < ItemsSource.Count - 1)
            {
                await AnimateToPage(SelectedIndex + 1);
            }
        }

        private async void SwipeGestureRecognizer_SwipeRight(object sender, EventArgs e)
        {
            if (ItemsSource != null && SelectedIndex > 0)
            {
                await AnimateToPage(SelectedIndex - 1);
            }
        }

        #endregion Constructors

        #region Properties

        public IList ItemsSource
        {
            get
            {
                return (IList)GetValue(ItemsSourceProperty);
            }
            set
            {
                SetValue(ItemsSourceProperty, value);
            }
        }

        public DataTemplate ItemTemplate
        {
            get
            {
                return (DataTemplate)base.GetValue(ItemTemplateProperty);
            }
            set
            {
                base.SetValue(ItemTemplateProperty, value);
            }
        }

        public int SelectedIndex
        {
            get
            {
                return (int)GetValue(SelectedIndexProperty);
            }
            set
            {
                SetValue(SelectedIndexProperty, value);
            }
        }

        #endregion Properties

        #region Methods

        public async Task AnimateToPage(int index)
        {
            // really animate to new page ?
            if (_visiblePageIndex != index && _nextView == null && ItemsSource != null)
            {
                // get animation direction
                var translation = _visiblePageIndex < index ? Width : -Width;

                // indicate this is the one that is visible
                _visiblePageIndex = index;

                // save selected
                SelectedIndex = index;

                // get view to move to
                _nextView = (MycoView)ItemTemplate.CreateContent();
                _nextView.BindingContext = ItemsSource[index];
                _nextView.Parent = this;
                _nextView.Layout(new Rectangle(0, 0, Bounds.Width, Bounds.Height));

                // setup the next view offscreen
                _nextView.TranslationX = translation;

                // view to set to once complete
                var targetView = _nextView;
                var oldView = _currentView;

                // animate new in, ol dout
                await Task.WhenAll(_nextView.TranslateTo(0, 0, 250, Easing.CubicInOut), _currentView.TranslateTo(-translation, 0, 250, Easing.CubicInOut));

                // setup the current view
                _currentView = targetView;

                // null out old view
                oldView.Parent = null;
                oldView = null;

                // allow next animation
                _nextView = null;
            }
        }

        protected override void InternalDraw(SKCanvas canvas)
        {
            base.InternalDraw(canvas);

            if (_currentView != null)
            {
                _currentView.Draw(canvas);
            }

            if (_nextView != null)
            {
                _nextView.Draw(canvas);
            }
        }

        protected override void InternalLayout(Rectangle rectangle)
        {
            base.InternalLayout(rectangle);

            if (_currentView != null)
            {
                _currentView.Layout(new Rectangle(0, 0, rectangle.Width, rectangle.Height));
            }

            if (_nextView != null)
            {
                _nextView.Layout(new Rectangle(0, 0, rectangle.Width, rectangle.Height));
            }
        }

        private void ItemsSourceChanged(object oldValue, object newValue)
        {
            var oldValueNotifyChanged = oldValue as INotifyCollectionChanged;

            if (oldValueNotifyChanged != null)
            {
                oldValueNotifyChanged.CollectionChanged -= ItemsSource_OnItemChanged;
            }

            var newValueNotifyChanged = newValue as INotifyCollectionChanged;

            if (newValueNotifyChanged != null)
            {
                newValueNotifyChanged.CollectionChanged += ItemsSource_OnItemChanged;
            }

            SetupPage();
        }

        private void ItemsSource_OnItemChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (ItemsSource != null && SelectedIndex > ItemsSource.Count)
            {
                // currently indexing beyond the page list
                SelectedIndex = ItemsSource.Count - 1;
            }
            else if (ItemsSource != null && SelectedIndex < ItemsSource.Count && _currentView != ItemsSource[SelectedIndex])
            {
                // selected page is no longer what we originally had - select the page at the index we are at
                _visiblePageIndex = -1;
                SetupPage();
            }
            else if (ItemsSource == null)
            {
                // page source is all gone
                SetupPage();
            }
        }

        private void SetupPage()
        {
            if (_visiblePageIndex != SelectedIndex && ItemsSource != null)
            {
                _currentView = (MycoView)ItemTemplate.CreateContent();
                _currentView.BindingContext = ItemsSource[SelectedIndex];
                _currentView.Parent = this;
                _currentView.Layout(new Rectangle(0, 0, Bounds.Width, Bounds.Height));
                _currentView.TranslationX = 0;

                if (_nextView != null)
                {
                    _nextView.Parent = null;
                    _nextView = null;
                }

                _visiblePageIndex = SelectedIndex;

                Invalidate();
            }
            else if (ItemsSource == null)
            {
                _currentView = null;
                _nextView = null;

                Invalidate();
            }
        }

        #endregion Methods
    }
}