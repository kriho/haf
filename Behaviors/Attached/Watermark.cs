using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace HAF {
  public partial class Behaviors {
    public static readonly DependencyProperty WatermarkProperty = DependencyProperty.RegisterAttached("Watermark", typeof(object), typeof(Behaviors), new FrameworkPropertyMetadata((object)null, new PropertyChangedCallback(OnWatermarkChanged)));

    private static readonly Dictionary<object, ItemsControl> itemsControls = new Dictionary<object, ItemsControl>();

    public static object GetWatermark(DependencyObject d) {
      return (object)d.GetValue(WatermarkProperty);
    }

    public static void SetWatermark(DependencyObject d, object value) {
      d.SetValue(WatermarkProperty, value);
    }

    private static void OnWatermarkChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      var control = (Control)d;
      control.Loaded += Control_Loaded;
      if (d is ComboBox || d is TextBox) {
        control.GotKeyboardFocus += Control_GotKeyboardFocus;
        control.LostKeyboardFocus += Control_Loaded;
      } else if (d is ItemsControl i) {
        i.ItemContainerGenerator.ItemsChanged += ItemsChanged;
        itemsControls.Add(i.ItemContainerGenerator, i);
        var prop = DependencyPropertyDescriptor.FromProperty(ItemsControl.ItemsSourceProperty, i.GetType());
        prop.AddValueChanged(i, ItemsSourceChanged);
      }
    }

    private static void Control_GotKeyboardFocus(object sender, RoutedEventArgs e) {
      var c = (Control)sender;
      if (ShouldShowWatermark(c)) {
        RemoveWatermark(c);
      }
    }

    private static void Control_Loaded(object sender, RoutedEventArgs e) {
      var control = (Control)sender;
      if (ShouldShowWatermark(control)) {
        ShowWatermark(control);
      }
    }

    private static void ItemsSourceChanged(object sender, EventArgs e) {
      var c = (ItemsControl)sender;
      if (c.ItemsSource != null) {
        if (ShouldShowWatermark(c)) {
          ShowWatermark(c);
        } else {
          RemoveWatermark(c);
        }
      } else {
        ShowWatermark(c);
      }
    }

    private static void ItemsChanged(object sender, ItemsChangedEventArgs e) {
      if (itemsControls.TryGetValue(sender, out var control)) {
        if (ShouldShowWatermark(control)) {
          ShowWatermark(control);
        } else {
          RemoveWatermark(control);
        }
      }
    }

    private static void RemoveWatermark(UIElement control) {
      var layer = AdornerLayer.GetAdornerLayer(control);
      // layer could be null if control is no longer in the visual tree
      if (layer != null) {
        var adorners = layer.GetAdorners(control);
        if (adorners == null) {
          return;
        }
        foreach (Adorner adorner in adorners) {
          if (adorner is WatermarkAdorner) {
            adorner.Visibility = Visibility.Hidden;
            layer.Remove(adorner);
          }
        }
      }
    }

    private static void ShowWatermark(Control control) {
      var layer = AdornerLayer.GetAdornerLayer(control);
      // layer could be null if control is no longer in the visual tree
      if (layer != null) {
        layer.Add(new WatermarkAdorner(control, GetWatermark(control)));
      }
    }

    private static bool ShouldShowWatermark(Control c) {
      if (c is ComboBox) {
        return (c as ComboBox).Text == string.Empty;
      } else if (c is TextBoxBase) {
        return (c as TextBox).Text == string.Empty;
      } else if (c is ItemsControl) {
        return (c as ItemsControl).Items.Count == 0;
      } else {
        return false;
      }
    }
  }

  internal class WatermarkAdorner : Adorner {
    private readonly ContentPresenter contentPresenter;

    public WatermarkAdorner(UIElement adornedElement, object watermark) :
      base(adornedElement) {
      this.IsHitTestVisible = false;

      this.contentPresenter = new ContentPresenter();
      this.contentPresenter.Content = watermark;
      this.contentPresenter.Opacity = 0.5;
      this.contentPresenter.Margin = new Thickness(Control.Margin.Left + Control.Padding.Left, Control.Margin.Top + Control.Padding.Top, 0, 0);

      if (this.Control is ItemsControl && !(this.Control is ComboBox)) {
        this.contentPresenter.VerticalAlignment = VerticalAlignment.Center;
        this.contentPresenter.HorizontalAlignment = HorizontalAlignment.Center;
      }
      // hide the control adorner when the adorned element is hidden
      var binding = new Binding("IsVisible") {
        Source = adornedElement,
        Converter = new BooleanToVisibilityConverter()
      };
      this.SetBinding(VisibilityProperty, binding);
    }

    protected override int VisualChildrenCount {
      get { return 1; }
    }

    private Control Control {
      get { return (Control)this.AdornedElement; }
    }

    protected override Visual GetVisualChild(int index) {
      return this.contentPresenter;
    }

    protected override Size MeasureOverride(Size constraint) {
      this.contentPresenter.Measure(this.Control.RenderSize);
      return this.Control.RenderSize;
    }

    protected override Size ArrangeOverride(Size finalSize) {
      this.contentPresenter.Arrange(new Rect(finalSize));
      return finalSize;
    }
  }
}
