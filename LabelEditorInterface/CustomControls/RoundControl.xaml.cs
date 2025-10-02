
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LabelEditorInterface.CustomControls;

public partial class RoundControl : UserControl
{
    public event RoutedEventHandler? Click;

    public string? Text { get; set; }
    public new double Height { get; set; }
    public new double Width { get; set; }
    public ImageSource? ImageLeaveButton { get; set; }
    public ImageSource? ImageEnterButton { get; set; }

    public RoundControl()
    {
        InitializeComponent();
        DataContext = this;
    }

    public void ChangeFullSize(double newSize, double newSizeFont)
    {
        Height = newSize;
        Width = newSize;
        RoundButton.Height = newSize;
        RoundButton.Width = newSize;
        RoundButton.FontSize = newSizeFont;
        Height = newSize;
        Width = newSize;
    }

    private void RoundButton_MouseLeftButtonDown(object sender, RoutedEventArgs e) 
        => Click?.Invoke(this, e);
}

