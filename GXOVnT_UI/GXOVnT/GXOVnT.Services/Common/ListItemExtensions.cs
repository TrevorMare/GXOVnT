using System.ComponentModel.DataAnnotations;
using GXOVnT.Services.Models;

namespace GXOVnT.Services.Common;

public static class ListItemExtensions
{

    #region List Item Extension Methods
    
    /// <summary>
    /// Converts an enum into a list object to display
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static List<ListItem<T>> ConvertEnumToListItems<T>() where T: struct 
    {
        var result = new List<ListItem<T>>();
       
        // For each of the values in the enum
        foreach (T value in Enum.GetValues(typeof(T)))
        {
            var fieldInfo = value.GetType().GetField(value.ToString() ?? "");
            if (fieldInfo == null) continue;
            var displayAttribute = fieldInfo.GetCustomAttributes(typeof(DisplayAttribute), false).Cast<DisplayAttribute>().FirstOrDefault();
            result.Add(new ListItem<T> 
            {
                Display = displayAttribute?.Name ?? value.ToString() ?? "",
                Id = Convert.ToInt32(value).ToString(),
                Data = value
            });
        }

        return result.OrderBy(i => i.Display).ToList();

    }

    #endregion
    
    
}