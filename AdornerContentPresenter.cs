using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace HAF {
  public class AdornerContentPresenter: Adorner {
    public enum AdornerPlacement {
      Unspecified = 0,
      In = 1,
      OutHorizontally = 2,
      OutVertically = 3
    }

    public enum AdornerPosition {
      Unspecified = 0,
      TopLeft = 1,
      TopCenter = 2,
      TopRight = 3,
      MiddleLeft = 4,
      MiddleCenter = 5,
      MiddleRight = 6,
      BottomLeft = 7,
      BottomCenter = 8,
      BottomRight = 9
    }

    private readonly AdornerPlacement placement;

    private readonly AdornerPosition position;

    private readonly VisualCollection visuals;

    private readonly ContentPresenter contentPresenter;

    public AdornerContentPresenter(UIElement adornedElement, AdornerPlacement placement, AdornerPosition position) : base(adornedElement) {
      this.placement = placement;
      this.position = position;
      this.visuals = new VisualCollection(this);
      this.contentPresenter = new ContentPresenter();
      this.visuals.Add(contentPresenter);
    }

    public AdornerContentPresenter(UIElement adornedElement, AdornerPlacement placement, AdornerPosition position, Visual content) : this(adornedElement, placement, position) {
      this.Content = content;
    }

    protected override Size MeasureOverride(Size constraint) {
      this.contentPresenter.Measure(constraint);
      return this.contentPresenter.DesiredSize;
    }

    protected override Size ArrangeOverride(Size finalSize) {
      var adorningPoint = new Point();
      var pos = (int)position - 1;
      adorningPoint.X = (((pos % 3) / 2d) * this.AdornedElement.DesiredSize.Width)
                        - (((pos % 3) / 2d) * this.contentPresenter.DesiredSize.Width);
      adorningPoint.Y = (((pos / 3) / 2d) * this.AdornedElement.DesiredSize.Height)
                        - (((pos / 3) / 2d) * this.contentPresenter.DesiredSize.Height);
      if(placement == AdornerPlacement.OutHorizontally) {
        switch(pos % 3) {
          default:
          case 1: {
              //center column do nothing
              break;
            }
          case 0: {
              adorningPoint.X -= this.contentPresenter.DesiredSize.Width;
              break;
            }
          case 2: {
              adorningPoint.X += this.contentPresenter.DesiredSize.Width;
              break;
            }
        }
      } else if(placement == AdornerPlacement.OutVertically) {
        switch(pos / 3) {
          default:
          case 1: {
              //center row do nothing
              break;
            }
          case 0: {
              adorningPoint.Y -= this.contentPresenter.DesiredSize.Height;
              break;
            }
          case 2: {
              adorningPoint.Y += this.contentPresenter.DesiredSize.Height;
              break;
            }
        }
      }
      this.contentPresenter.Arrange(new Rect(adorningPoint, finalSize));
      return this.contentPresenter.RenderSize;
    }

    protected override Visual GetVisualChild(int index) {
      return visuals[index];
    }

    protected override int VisualChildrenCount => this.visuals.Count;

    public object Content {
      get => this.contentPresenter.Content;
      set => this.contentPresenter.Content = value;
    }
  }
}
