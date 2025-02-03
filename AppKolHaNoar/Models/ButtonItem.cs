using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppKolHaNoar.Models;
public class ButtonItem : ListItemBase
{
    public string ButtonText { get; set; }
    public Action Command { get; set; }
}
