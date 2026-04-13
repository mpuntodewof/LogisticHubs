namespace Domain.Enums
{
    public enum UserRole
    {
        Admin,
        Manager,
        Viewer
    }

    // Catalog
    public enum ProductStatus
    {
        Draft,
        Active,
        Discontinued
    }

    // Inventory
    public enum StockMovementType
    {
        In,
        Out,
        Transfer,
        Adjustment
    }

    public enum StockMovementReason
    {
        Purchase,
        CustomerReturn,
        TransferIn,
        InitialStock,
        Sale,
        Damaged,
        Expired,
        TransferOut,
        StockTake,
        Correction,
        Other
    }

    // Finance
    public enum AccountType
    {
        Asset,
        Liability,
        Equity,
        Revenue,
        Expense
    }

    public enum JournalEntryStatus
    {
        Draft,
        Posted,
        Voided
    }

    // Tax
    public enum TaxType
    {
        PPN,
        PPnBM,
        Exempt
    }

    public enum InvoiceStatus
    {
        Draft,
        Issued,
        Paid,
        Cancelled,
        CreditNoted
    }

    // Payment
    public enum PaymentMethod
    {
        Cash,
        BankTransfer,
        CreditCard,
        DebitCard,
        EWallet,
        QRIS,
        Other
    }

    public enum PaymentStatus
    {
        Pending,
        Paid,
        Failed,
        Refunded
    }

    // Audit
    public enum AuditAction
    {
        Create, Update, Delete, StatusChange, Login, Logout, Export
    }

    public enum SystemLogLevel
    {
        Trace, Debug, Information, Warning, Error, Critical
    }
}
