namespace Payment.Common;

public enum ResponseCodes
{
    BankProcessingPending,
    BankProcessingApproved,
    BankProcessingDeclined,
    Approved,
    Deferred,
    Declined
}