using System.Collections.ObjectModel;
using System.Linq;
using System.ComponentModel;

public class AutoSuggestViewModels : INotifyPropertyChanged
{
    private string _selectedText;
    public ObservableCollection<string> Items { get; set; }
    public ObservableCollection<string> baseItems { get; set; }


    public string SelectedText
    {
        get => _selectedText;
        set
        {
            _selectedText = value;
            OnPropertyChanged(nameof(SelectedText));
        }
    }

    public AutoSuggestViewModels()
    {
        Items = new ObservableCollection<string> { "Apple", "Banana", "Cherry", "Date", "Fig", "Grape" };
        baseItems = new ObservableCollection<string> (Items);
    }

    public ObservableCollection<string> GetFilteredItems(string query)
    {
       Items.Clear();
        if (!string.IsNullOrEmpty(query))
        {
            var filteredItems = baseItems.Where(i => i.Contains(query,System.StringComparison.OrdinalIgnoreCase)).ToList();
            foreach (var item in filteredItems)
            {
                Items.Add(item);
            }
        }
        else
        {
            foreach(var item in baseItems)
            {
                Items.Add(item);    
            }
        }
        return Items;
    }

    public void ValidateSelection()
    {
        if (!Items.Contains(SelectedText))
        {
            SelectedText = ""; // מנקה אם אין התאמה
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
