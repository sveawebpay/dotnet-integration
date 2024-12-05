using AdminWS;

namespace Sample.AspNetCore.Models;

public class SelectableNumberedOrderRow : NumberedOrderRow
{
    public bool IsSelected { get; set; }
}
