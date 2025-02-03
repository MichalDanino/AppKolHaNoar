using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace AppKolHaNoar.Presentation;

public class CustomTemplateSelector : DataTemplateSelector
{
    public DataTemplate CheckBoxTemplate { get; set; }
    public DataTemplate InputTemplate { get; set; }
    public DataTemplate ButtonTemplate { get; set; }

    protected override DataTemplate SelectTemplateCore(object item)
    {
        return item switch
        {
            CheckBoxItem => CheckBoxTemplate,
            InputItem => InputTemplate,
            ButtonItem => ButtonTemplate,
            _ => base.SelectTemplateCore(item)
        };
    }
}
