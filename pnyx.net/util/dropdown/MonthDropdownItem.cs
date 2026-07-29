using System.Collections.Generic;
using pnyx.net.util.dates;

namespace pnyx.net.util.dropdown;

public class MonthDropdownItem : DropdownItem<string>
{
    public MonthDropdownItem(string text, string value) : base(text, value)
    {
    }

    public MonthDropdownItem()
    {
    }

    public static List<MonthDropdownItem> buildMonthDropdownItems(LocalDay today, int months = 12)
    {
        List<MonthDropdownItem> items = new();
        LocalDay month = today.startOfMonth();
        while (items.Count < months)
        {
            items.Add(new MonthDropdownItem(month.local.ToString(DateFormats.MonthTextAndYear), month.ToString()));
            month = month.addMonth(-1);
        }
        return items;
    }
    
    public static MonthDropdownItem buildMonthDropdownItem(LocalDay month)
    {
        return new MonthDropdownItem(month.local.ToString(DateFormats.MonthTextAndYear), month.ToString());
    }
}