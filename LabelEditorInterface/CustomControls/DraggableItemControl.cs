
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LabelEditorInterface.CustomControls;

public class DraggableItem : ContentControl
{
    private bool _isDragging;
    private Point _startPoint;
    public Action<DraggableItem>? Click;

    public DraggableItem()
    {
        BorderBrush = Brushes.Transparent;
        BorderThickness = new Thickness(1);

        AddHandler(PreviewMouseLeftButtonDownEvent,
            new MouseButtonEventHandler(OnPreviewMouseLeftButtonDown), true);

        MouseLeftButtonDown += OnMouseDown;
        MouseLeftButtonUp += OnMouseUp;
        MouseMove += OnMouseMove;
    }

    private void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.Source is DraggableItem)
            return;

        if (Content is TextBox tb)
        {
            if (e.ClickCount >= 2)
                EnterTextEditMode(tb);
            else
                OnMouseDown(sender, e);
        }
    }

    private void EnterTextEditMode(TextBox tb)
    {
        tb.IsReadOnly = false;
        tb.Focus();
        tb.CaretIndex = tb.Text?.Length ?? 0;

        tb.LostKeyboardFocus += Tb_LostKeyboardFocus;
        tb.PreviewKeyDown += Tb_PreviewKeyDown;
    }

    private void Tb_PreviewKeyDown(object? sender, KeyEventArgs e)
    {
        if (sender is not TextBox tb)
            return;

        if ((!tb.AcceptsReturn && e.Key == Key.Enter) ||
            (tb.AcceptsReturn && e.Key == Key.Enter && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control) ||
            e.Key == Key.Escape)
        {
            Focus();
            e.Handled = true;
        }
    }

    private void Tb_LostKeyboardFocus(object? sender, KeyboardFocusChangedEventArgs e)
    {
        if (sender is TextBox tb)
        {
            tb.IsReadOnly = true;
            tb.LostKeyboardFocus -= Tb_LostKeyboardFocus;
            tb.PreviewKeyDown -= Tb_PreviewKeyDown;
        }
    }

    private void OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        _isDragging = true;
        _startPoint = e.GetPosition(Parent as Canvas);
        CaptureMouse();
        Click?.Invoke(this);
        Select();
        e.Handled = true;
    }

    private void OnMouseUp(object sender, MouseButtonEventArgs e)
    {
        _isDragging = false;
        ReleaseMouseCapture();
    }

    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        if (_isDragging && Parent is Canvas canvas)
        {
            Point pos = e.GetPosition(canvas);
            double offsetX = pos.X - _startPoint.X;
            double offsetY = pos.Y - _startPoint.Y;

            Canvas.SetLeft(this, Canvas.GetLeft(this) + offsetX);
            Canvas.SetTop(this, Canvas.GetTop(this) + offsetY);

            _startPoint = pos;
        }
    }

    private void Select()
    {
        BorderBrush = Brushes.Blue;
        BorderThickness = new Thickness(2);
        if (GetTemplateChild("ResizeThumbRB") is Thumb trb)
            trb.Visibility = Visibility.Visible;
        if (GetTemplateChild("ResizeThumbLB") is Thumb tlb)
            tlb.Visibility = Visibility.Visible;
        if (GetTemplateChild("ResizeThumbRT") is Thumb trt)
            trt.Visibility = Visibility.Visible;
        if (GetTemplateChild("ResizeThumbLT") is Thumb tlt)
            tlt.Visibility = Visibility.Visible;
    }

    public void Deselect()
    {
        BorderBrush = Brushes.Transparent;
        BorderThickness = new Thickness(1);

        if (GetTemplateChild("ResizeThumbRB") is Thumb trb)
            trb.Visibility = Visibility.Collapsed;
        if (GetTemplateChild("ResizeThumbLB") is Thumb tlb)
            tlb.Visibility = Visibility.Collapsed;
        if (GetTemplateChild("ResizeThumbRT") is Thumb trt)
            trt.Visibility = Visibility.Collapsed;
        if (GetTemplateChild("ResizeThumbLT") is Thumb tlt)
            tlt.Visibility = Visibility.Collapsed;
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        if (GetTemplateChild("ResizeThumbRB") is Thumb rb)
        {
            rb.DragDelta += (s, e) =>
            {
                Width = Math.Max(Width + e.HorizontalChange, 20);
                Height = Math.Max(Height + e.VerticalChange, 20);

                ChangeSizeContentElement();
            };
        }

        if (GetTemplateChild("ResizeThumbLB") is Thumb lb)
        {
            lb.DragDelta += (s, e) =>
            {
                double oldWidth = Width;
                Width = Math.Max(Width - e.HorizontalChange, 20);

                double dx = oldWidth - Width;
                Canvas.SetLeft(this, Canvas.GetLeft(this) + dx);

                Height = Math.Max(Height + e.VerticalChange, 20);

                ChangeSizeContentElement();
            };
        }

        if (GetTemplateChild("ResizeThumbRT") is Thumb rt)
        {
            rt.DragDelta += (s, e) =>
            {
                double oldHeight = Height;
                Height = Math.Max(Height - e.VerticalChange, 20);

                double dy = oldHeight - Height;
                Canvas.SetTop(this, Canvas.GetTop(this) + dy);

                Width = Math.Max(Width + e.HorizontalChange, 20);

                ChangeSizeContentElement();
            };
        }

        if (GetTemplateChild("ResizeThumbLT") is Thumb lt)
        {
            lt.DragDelta += (s, e) =>
            {
                double oldWidth = Width;
                double oldHeight = Height;

                Width = Math.Max(Width - e.HorizontalChange, 20);
                Height = Math.Max(Height - e.VerticalChange, 20);

                double dx = oldWidth - Width;
                double dy = oldHeight - Height;

                Canvas.SetLeft(this, Canvas.GetLeft(this) + dx);
                Canvas.SetTop(this, Canvas.GetTop(this) + dy);

                ChangeSizeContentElement();
            };
        }

        void ChangeSizeContentElement()
        {
            if (Content is Image im)
            {
                im.Width = Width;
                im.Height = Height;
            }

            else if (Content is Ellipse ellipse)
            {
                ellipse.Width = Width - 4;
                ellipse.Height = Height - 4;
            }

            else if (Content is Line line)
            {
                line.X2 = Width;
            }

            else if (Content is Rectangle rect)
            {
                rect.Width = Width - 4;
                rect.Height = Height - 4;
            }

            else if (Content is TextBox tb)
            {
                tb.Width = Width - 2;
                tb.Height = Height - 2;
            }
        }
    }
}
