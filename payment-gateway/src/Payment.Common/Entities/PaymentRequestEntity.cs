using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payment.Common.Entities;

[Table("PaymentRequest")]
public class PaymentRequestEntity : BaseEntity
{
    [Key] 
    public int PaymentId { get; set; }

    public string TransactionId { get; set; }

    public decimal Amount { get; set; }

    public string? VaultId { get; set; }

    public DateTime TransactionDate { get; set; }
}