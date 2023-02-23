using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payment.Common.Entities;

[Table("PaymentResponse")]
public class PaymentResponseEntity : BaseEntity
{
    [Key] public long PaymentResponseId { get; set; }

    public long PaymentId { get; set; }

    public string Response { get; set; }
    
}