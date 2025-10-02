
namespace PrintInterfaceComponent;

public class PrintOptions
{
    /// <summary>
    /// Тип бумаги. Например, "Label", "Sticker", "A4"
    /// </summary>
    public string? PaperType { get; set; }

    /// <summary>
    /// Двусторонняя печать.
    /// </summary>
    public bool Duplex { get; set; }

    /// <summary>
    /// Цветная печать.
    /// </summary>
    public bool Color { get; set; }

    /// <summary>
    /// Количество копий.
    /// </summary>
    public int Copies { get; set; } = 1;
}
