using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace AppKolHaNoar.Presentation.Controller;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class ListPage : Page
{
    public ObservableCollection<ListItemBase> Items { get; set; }

    public ListPage()
    {
        this.InitializeComponent();
        Items = new ObservableCollection<ListItemBase>
        {
            new CheckBoxItem { Text = "Enable Notifications", IsChecked = true },
            new InputItem { Placeholder = "Enter your name", Value = "" },
            new ButtonItem { ButtonText = "Submit", Command = () => SubmitAction() }
        };
        DataContext = this;
    }

    private void Back_Click(object sender, RoutedEventArgs e)
    {
        if (Frame.CanGoBack)
        {
            Frame.GoBack();
        }
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.DataContext is ButtonItem item)
        {
            item.Command?.Invoke();
        }
    }

    private void SubmitAction()
    {
        // כאן אפשר להוסיף קוד לביצוע פעולת הכפתור
    }
}
