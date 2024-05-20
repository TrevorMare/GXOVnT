namespace GXOVnT.Services.Models;

public class ListItem
{

    #region Properties

    public string Id { get; set; } = string.Empty;

    public string Display { get; set; } = string.Empty;

    #endregion


}

public class ListItem<T> : ListItem
{

    public T? Data { get; set; }
    
}

