using System.ComponentModel.DataAnnotations;

namespace CustomerRestServer.Models;

public class CustomerModel : IComparable<CustomerModel>
{
    [Required]
    [Range(1, uint.MaxValue)]
    public uint Id { get; set; }
    
    [Required]
    [MinLength(2)]
    public string LastName { get; set; }

    [Required]
    [MinLength(2)]
    public string FirstName { get; set; }
    
    [Required]
    [Range(18, 90)]
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