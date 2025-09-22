using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using MenuGenerator.ViewModel;

namespace MenuGenerator.ViewLocator;

public class ViewLocator : IDataTemplate
{
    public Control? Build(object? param)
    {
        if (param is null)
        {
            return null;
        }

        var paramType = param.GetType();
        
        var viewAttribute = paramType
            .GetCustomAttributes(typeof(ViewAttribute), true)
            .FirstOrDefault();

        if (viewAttribute is not ViewAttribute viewAttributeInstance)
        {
            return new TextBlock { Text = "Not Found: " + paramType.FullName };
        }

        var type = viewAttributeInstance.ViewType;

        return (Control)Activator.CreateInstance(type)!;

    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}