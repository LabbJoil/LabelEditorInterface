
namespace PrintInterfaceComponent;

public interface IPrinter
{
    /// <summary>
    /// Уникальное имя или идентификатор принтера.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Модель или тип принтера.
    /// </summary>
    string Model { get; }

    /// <summary>
    /// Флаг готовности принтера к печати.
    /// </summary>
    bool IsReady { get; }

    /// <summary>
    /// Подключение к принтеру.
    /// </summary>
    void Connect();

    /// <summary>
    /// Отключение от принтера.
    /// </summary>
    void Disconnect();

    /// <summary>
    /// Отправка данных на печать.
    /// </summary>
    /// <param name="data">Данные для печати.</param>
    /// <param name="options">Настройки печати.</param>
    void Print(byte[] data, PrintOptions? options = null);

    /// <summary>
    /// Проверка состояния принтера (бумага, ошибки, температура головки и пр.).
    /// Возвращает описание статуса в текстовом виде.
    /// </summary>
    string GetStatus();

    /// <summary>
    /// Получение списка текущих заданий в очереди печати.
    /// </summary>
    /// <returns>Список идентификаторов или объектов заданий.</returns>
    IEnumerable<string> GetPrintQueue();

    /// <summary>
    /// Отмена конкретного задания по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор задания.</param>
    void CancelPrintJob(string id);

    /// <summary>
    /// Очистка всей очереди печати.
    /// </summary>
    void ClearQueue();
}
