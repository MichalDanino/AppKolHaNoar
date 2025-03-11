using System.Collections.ObjectModel;
using System.Linq;
using System.ComponentModel;
using DTO;
using AppKolHaNoar.Services;
using System.Collections;
using System.Runtime.CompilerServices;


public class AutoSuggestViewModels : INotifyPropertyChanged
{
   public  Campaign _selectedText = new Campaign() { Campaign_Name="", Campaign_Number=""};
    public ObservableCollection<Campaign> Items { get; set; }
    public ObservableCollection<Campaign> baseItems { get; set; }
    private ServiceUI serviceUI= new ServiceUI();

    public Campaign SelectedText
    {
        get => _selectedText;
        set
        {
            if (_selectedText != value)
            {
                _selectedText = value;
                OnPropertyChanged();
            }
        }
    }
    public string SelectedTextName
    {
        get => _selectedText.Campaign_Name;
        
    }

    public AutoSuggestViewModels()
    {
        Items = new ObservableCollection<Campaign> ( serviceUI.GetCampaignsTable().OrderBy(a=> a.Campaign_Name)?.ToList()?? new List<Campaign>());
       
        baseItems = new ObservableCollection<Campaign> (Items);
    }

    public ObservableCollection<Campaign> GetFilteredItems(string query)
    {
       Items.Clear();
        //if (!string.IsNullOrEmpty(query))
        //{
            var filteredItems = baseItems.Where(i => i.Campaign_Name.Contains(query,System.StringComparison.OrdinalIgnoreCase)).ToList();
            if (filteredItems.Any()) 
            { 
                  foreach (var item in filteredItems)
                  {
                      Items.Add(item);
                  }

                  return Items;
            }
       // }
        ValidateSelection();

        return Items;

    }


    public void ValidateSelection()
    {
        var isValid = baseItems.FirstOrDefault(i => i.Campaign_Name.Contains(SelectedText.Campaign_Name));
        if (isValid == null)
        {
            SelectedText.Campaign_Name = ""; // מנקה אם אין התאמה
            SelectedText.Campaign_Number = "";
        }
    }
    public void isGetDataFromService()
    {
        if (!Items.Any())
        {
            GenericMessage message = new GenericMessage() { MessageContent = "נרא שיש בעיה בבקשה לשרת, אנא וודא את חיבור האינטרנט ונכונות פרטי ההתחברות" };
            ServiceUI.ShowException(message);
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
