
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LabelEditorInterface.CustomControls;

public partial class TileControl : UserControl
{
    public static readonly DependencyProperty SvgPathProperty =
        DependencyProperty.Register(nameof(SvgPathProperty), typeof(string), typeof(TileControl), new PropertyMetadata(null));

    public string? SvgPath
    {
        get => (string?)GetValue(SvgPathProperty);
        set => SetValue(SvgPathProperty, value);
    }

    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(TileControl), new PropertyMetadata(string.Empty));

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly DependencyProperty TileTypeProperty =
        DependencyProperty.Register(nameof(TileType), typeof(ElementType), typeof(TileControl), new PropertyMetadata(null));

    public ElementType TileType
    {
        get => (ElementType)GetValue(TileTypeProperty);
        set => SetValue(TileTypeProperty, value);
    }

    public event RoutedEventHandler? Click;

    public TileControl()
    {
        InitializeComponent();
        DataContext = this;
    }

    private void TileControl_MouseMove(object sender, MouseEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            DragDrop.DoDragDrop((DependencyObject)sender,
                new DataObject("TileType", TileType),
                DragDropEffects.Copy);
        }
    }
}
