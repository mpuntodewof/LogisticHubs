namespace Domain.Enums
{
    public enum UserRole
    {
        Admin,
        Manager,
        Driver,
        Viewer
    }

    public enum DriverStatus
    {
        Available,
        OnDuty,
        OffDuty,
        Suspended
    }

    public enum VehicleStatus
    {
        Available,
        InUse,
        UnderMaintenance,
        Retired
    }

    public enum ShipmentStatus
    {
        Pending,
        Assigned,
        PickedUp,
        InTransit,
        OutForDelivery,
        Delivered,
        Failed,
        Cancelled
    }

    public enum ProductStatus
    {
        Draft,
        Active,
        Discontinued
    }

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

    public enum CustomerType
    {
        Individual,
        Company
    }

    public enum AddressType
    {
        Billing,
        Shipping,
        Both
    }

    public enum SalesOrderStatus
    {
        Draft,
        Confirmed,
        Processing,
        Shipped,
        Delivered,
        Completed,
        Cancelled,
        Refunded
    }

    public enum SalesOrderType
    {
        POS,
        Online
    }

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

    // Payment Gateway
    public enum PaymentGatewayProvider
    {
        Midtrans,
        Xendit,
        Doku,
        Other
    }

    public enum PaymentTransactionStatus
    {
        Pending,
        Processing,
        Success,
        Failed,
        Expired,
        Refunded,
        Cancelled
    }

    // E-commerce
    public enum DiscountType
    {
        Percentage,
        FixedAmount
    }

    public enum BannerPosition
    {
        Homepage,
        Category,
        Sidebar,
        Popup
    }

    public enum PageStatus
    {
        Draft,
        Published,
        Archived
    }

    public enum ReviewStatus
    {
        Pending,
        Approved,
        Rejected
    }

    // Logistics Enhancements
    public enum DeliveryRateType
    {
        FlatRate,
        PerKg,
        WeightRange
    }

    public enum ShipmentNoteType
    {
        General,
        DriverInstruction,
        WarehouseNote,
        CustomerNote,
        Issue
    }

    // Promotions
    public enum PromotionType
    {
        BuyXGetY,
        BundleDiscount,
        FlashSale,
        FreeShipping,
        PercentageDiscount,
        FixedAmountDiscount
    }

    public enum PromotionStatus
    {
        Draft,
        Active,
        Paused,
        Expired,
        Cancelled
    }

    public enum PromotionRuleType
    {
        MinQuantity,
        MinOrderAmount,
        SpecificProducts,
        SpecificCategories,
        CustomerGroup,
        FirstOrder
    }

    // Loyalty
    public enum LoyaltyTransactionType
    {
        Earn,
        Redeem,
        Expire,
        Adjust,
        Bonus
    }

    public enum LoyaltyProgramStatus
    {
        Active,
        Paused,
        Archived
    }

    // Purchase
    public enum PurchaseOrderStatus
    {
        Draft,
        Submitted,
        Approved,
        Received,
        Cancelled
    }

    public enum GoodsReceiptStatus
    {
        Draft,
        Confirmed,
        Cancelled
    }

    // Notification
    public enum NotificationChannel
    {
        Email,
        SMS,
        Push
    }

    public enum NotificationStatus
    {
        Unread,
        Read,
        Archived
    }

    // HRM
    public enum LeaveRequestStatus
    {
        Pending,
        Approved,
        Rejected
    }

    public enum LeaveType
    {
        Annual,
        Sick,
        Maternity,
        Paternity,
        Unpaid,
        Other
    }

    public enum EmploymentStatus
    {
        Active,
        OnLeave,
        Terminated,
        Probation
    }

    // Reporting
    public enum ReportType
    {
        Sales, Inventory, Finance, Tax, Purchase, HRM, Loyalty, Custom
    }

    public enum ReportExecutionStatus
    {
        Queued, Running, Completed, Failed, Cancelled
    }

    public enum ReportScheduleFrequency
    {
        None, Daily, Weekly, Monthly
    }

    public enum DashboardWidgetType
    {
        KPI, Chart, Table, List, Summary
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

    // API / Webhooks
    public enum WebhookEventType
    {
        Created, Updated, Deleted, StatusChanged
    }

    public enum WebhookDeliveryStatus
    {
        Pending, Success, Failed, Expired
    }
}
