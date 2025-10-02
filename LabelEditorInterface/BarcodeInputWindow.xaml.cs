
using System.Windows;

namespace LabelEditorInterface;

public partial class BarcodeInputWindow : Window
{
    public string BarcodeText { get; private set; } = string.Empty;

    public BarcodeInputWindow()
    {
        InitializeComponent();
        BarcodeTextBox.Focus();
    }

    private void Ok_Click(object sender, RoutedEventArgs e)
    {
        BarcodeText = BarcodeTextBox.Text.Trim();
        if (!string.IsNullOrEmpty(BarcodeText))
        {
            DialogResult = true;
            Close();
        }
        else
            MessageBox.Show("Введите код для штрихкода!");
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}

