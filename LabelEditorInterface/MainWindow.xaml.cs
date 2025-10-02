
using BarcodeLib.Barcode;
using LabelEditorInterface.CustomControls;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Image = System.Windows.Controls.Image;

namespace LabelEditorInterface;

public partial class MainWindow : Window
{
    private const double DefaultHeight = 200;
    private const double DefaultWidth = 350;

    private bool IsOpenMenu = false;
    private DraggableItem? _selectedItem;

    public MainWindow()
    {
        InitializeComponent();
    }

    private void InteractionMenu(object sender, RoutedEventArgs e)
       => InteractionSliderButton(MenuSlider, MenuButton);

    private void InteractionSliderButton(Slider slider, RoundControl button)
    {
        DoubleAnimation animationSlider;
        DoubleAnimation animationButton;
        int sliderOpenTo, buttonOpenTo;

        if (IsOpenMenu)
        {
            sliderOpenTo = 0;
            buttonOpenTo = 50;
        }
        else
        {
            sliderOpenTo = 300;
            buttonOpenTo = 0;
        }

        animationSlider = new DoubleAnimation
        {
            From = slider.ActualWidth,
            To = sliderOpenTo,
            Duration = TimeSpan.FromSeconds(0.3)
        };
        animationButton = new DoubleAnimation
        {
            From = button.ActualWidth,
            To = buttonOpenTo,
            Duration = TimeSpan.FromSeconds(0.3)
        };

        Storyboard storyboard = new();
        storyboard.Children.Add(animationSlider);
        storyboard.Children.Add(animationButton);

        Storyboard.SetTarget(animationSlider, slider);
        Storyboard.SetTarget(animationButton, button);

        Storyboard.SetTargetProperty(animationSlider, new PropertyPath("Width"));
        Storyboard.SetTargetProperty(animationButton, new PropertyPath("Width"));
        storyboard.Begin();
        IsOpenMenu = !IsOpenMenu;
    }

    private void MainCanvas_Drop(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent("TileType"))
        {
            var type = (ElementType)e.Data.GetData("TileType");
            UIElement? element = null;

            switch (type)
            {
                case ElementType.Barcode:
                    var inputWindow = new BarcodeInputWindow
                    {
                        Owner = Application.Current.MainWindow
                    };

                    bool? result = inputWindow.ShowDialog();
                    if (result == true)
                    {
                        string code = inputWindow.BarcodeText;
                        var bitmapSource = GenerateBarcodeImage(code, (int)DefaultWidth, (int)DefaultHeight);

                        element = new Image
                        {
                            Source = bitmapSource,
                            Width = DefaultWidth,
                            Height = DefaultHeight,
                            Stretch = Stretch.Uniform
                        };
                    }
                    break;

                case ElementType.Image:
                    var openFileDialog = new Microsoft.Win32.OpenFileDialog
                    {
                        Filter = "Image Files (*.bmp;*.jpg;*.jpeg;*.png;*.gif)|*.bmp;*.jpg;*.jpeg;*.png;*.gif",
                        Title = "Выберите изображение"
                    };

                    if (openFileDialog.ShowDialog() == true)
                    {
                        string imagePath = openFileDialog.FileName;

                        var bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.UriSource = new Uri(imagePath);
                        bitmapImage.EndInit();

                        element = new Image
                        {
                            Source = bitmapImage,
                            Width = DefaultWidth,
                            Height = DefaultHeight,
                            Stretch = Stretch.Uniform
                        };
                    }
                    break;

                case ElementType.Ellipse:
                    element = new Ellipse
                    {
                        Width = DefaultWidth - 4,
                        Height = DefaultHeight - 4,
                        Stroke = System.Windows.Media.Brushes.DarkRed,
                        StrokeThickness = 2,
                        Fill = System.Windows.Media.Brushes.LightCoral
                    };
                    break;

                case ElementType.Line:
                    element = new Line
                    {
                        X1 = 0,
                        Y1 = 0,
                        X2 = DefaultWidth,
                        Y2 = 0,
                        Stroke = System.Windows.Media.Brushes.DarkRed,
                        VerticalAlignment = VerticalAlignment.Center,
                        StrokeThickness = 3
                    };
                    break;

                case ElementType.Rectangle:
                    element = new System.Windows.Shapes.Rectangle
                    {
                        Height = DefaultHeight - 4,
                        Width = DefaultWidth - 4,
                        Stroke = System.Windows.Media.Brushes.DarkBlue,
                        StrokeThickness = 2,
                        Fill = System.Windows.Media.Brushes.LightPink
                    };
                    break;

                case ElementType.RichTextbox:
                    element = new TextBox
                    {
                        FontSize = 16,
                        Foreground = System.Windows.Media.Brushes.Black,
                        AcceptsReturn = true,
                        TextWrapping = TextWrapping.Wrap,
                        IsReadOnly = true,
                        Background = System.Windows.Media.Brushes.Transparent,
                        Width = DefaultWidth - 2,
                        Height = DefaultHeight - 2
                    };
                    break;
            }

            if (element != null)
            {
                var draggable = new DraggableItem
                {
                    Content = element,
                    Width = DefaultWidth,
                    Height = DefaultHeight,
                    Click = SelectedItem
                };

                Canvas.SetLeft(draggable, e.GetPosition(MainCanvas).X);
                Canvas.SetTop(draggable, e.GetPosition(MainCanvas).Y);

                MainCanvas.Children.Add(draggable);
            }
        }
    }

    public static BitmapSource GenerateBarcodeImage(string data, int width, int height)
    {
        var barcode = new Linear
        {
            Type = BarcodeType.CODE128,
            Data = data,
            BarWidth = width,
            BarHeight = height
        };

        using var bmp = barcode.drawBarcode();
        using var ms = new MemoryStream();
        bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
        ms.Position = 0;

        var bitmap = new BitmapImage();
        bitmap.BeginInit();
        bitmap.CacheOption = BitmapCacheOption.OnLoad;
        bitmap.StreamSource = ms;
        bitmap.EndInit();
        bitmap.Freeze();
        return bitmap;
    }

    public void SelectedItem(DraggableItem draggableItem)
    {
        if (_selectedItem != null)
            _selectedItem.Deselect();

        _selectedItem = draggableItem;
    }

    private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
        if (e.Key == System.Windows.Input.Key.Delete && _selectedItem != null)
        {
            MainCanvas.Children.Remove(_selectedItem);
            _selectedItem = null;
        }
    }

    private void MainCanvas_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        foreach (var child in MainCanvas.Children)
        {
            if (child is DraggableItem draggableItem)
                draggableItem.Deselect();
        }
    }
}
