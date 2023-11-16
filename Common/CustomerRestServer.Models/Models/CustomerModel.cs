namespace CustomerRestServer.Models;

public class CustomerModel : IComparable<CustomerModel>
{
    public uint Id { get; set; }
    public string LastName { get; set; }
    public string FirstName { get; set; }
    public byte Age { get; set; }

    public int CompareTo(CustomerModel? other)
    {
        if (other is null)
            return -1;

        var me = $"{LastName}|{FirstName}";
        var another = $"{other.LastName}|{other.FirstName}";

        return string.Compare(me, another, StringComparison.OrdinalIgnoreCase);
    }
}