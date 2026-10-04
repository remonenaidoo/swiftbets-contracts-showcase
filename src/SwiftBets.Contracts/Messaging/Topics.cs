namespace SwiftBets.Contracts.Messaging;

public static class Topics
{
    public const string FixtureChanged = "offer.fixture-changed.v1";
    public const string PriceChanged = "offer.price-changed.v1";
    public const string ResultPublished = "offer.result-published.v1";
    public const string CouponRejected = "placement.coupon-rejected.v1";
    public const string LedgerPosted = "wallet.ledger-posted.v1";
    public const string LegEvaluated = "settlement.leg-evaluated.v1";

    /// <summary>Coupons as bets over legs, with bankers and system bets; the V1 coupon topics were removed in 2.0.0.</summary>
    public const string CouponPlacedV2 = "placement.coupon-placed.v2";
    public const string CouponSettledV2 = "settlement.coupon-settled.v2";
    public const string StuckCoupon = "settlement.stuck-coupon.v1";
    public const string PayoutRetry5Seconds = "payout.retry-5s.v1";
    public const string PayoutRetry1Minute = "payout.retry-1m.v1";
    public const string PayoutRetry15Minutes = "payout.retry-15m.v1";
    public const string PayoutDeadLetter = "payout.dead-letter.v1";
    public const string PayoutCompleted = "payout.payout-completed.v1";
    public const string IncidentRaised = "steward.incident-raised.v1";
    public const string IncidentUpdated = "steward.incident-updated.v1";
    public const string RemediationExecuted = "steward.remediation-executed.v1";
    public const string LiabilityChanged = "risk.liability-changed.v1";
    public const string RiskAlert = "risk.risk-alert.v1";

    /// <summary>Compacted, keyed by fixture id: the latest record per fixture is the exposure rule in force.</summary>
    public const string ExposureLimits = "risk.exposure-limits.v1";
    public const string LimitReached = "wallet.limit-reached.v1";
    public const string InboxMessage = "notifications.inbox-message.v1";
    public const string UserRegistered = "identity.user-registered.v1";
    public const string EmailVerified = "identity.email-verified.v1";
    public const string AccountStatusChanged = "identity.account-status-changed.v1";
    public const string SessionRevoked = "identity.session-revoked.v1";
    public const string AuditRecorded = "audit.audit-recorded.v1";
    public const string NotificationRequested = "notifications.notification-requested.v1";
    public const string LimitChanged = "compliance.limit-changed.v1";

    /// <summary>Compacted, keyed by user id: the latest record per account is its whole responsible-gambling state.</summary>
    public const string RestrictionsChanged = "compliance.restrictions-changed.v1";
    public const string SelfExclusionStarted = "compliance.self-exclusion-started.v1";
    public const string KycStatusChanged = "compliance.kyc-status-changed.v1";
    public const string DepositSucceeded = "payments.deposit-succeeded.v1";
    public const string DepositFailed = "payments.deposit-failed.v1";
    public const string WithdrawalRequested = "payments.withdrawal-requested.v1";
    public const string WithdrawalDecided = "payments.withdrawal-decided.v1";
    public const string WithdrawalPaid = "payments.withdrawal-paid.v1";
    public const string WithdrawalFailed = "payments.withdrawal-failed.v1";
    public const string PaymentDriftDetected = "payments.drift-detected.v1";
    public const string CasinoTransaction = "casino.transaction.v1";
    public const string ProviderReconciliation = "casino.provider-reconciliation.v1";

    /// <summary>Compacted, keyed by setting key: the latest record per key is the value in force.</summary>
    public const string ConfigEntries = "config.entries.v1";

    /// <summary>Keyed by content kind: a banner, quick link or page changed, so clients refetch.</summary>
    public const string ContentChanged = "config.content-changed.v1";
    public const string ManualResult = "trading.manual-result.v1";
    public const string ManualResultRejected = "trading.manual-result-rejected.v1";
    public const string MarketStatusChanged = "trading.market-status-changed.v1";

    public static IReadOnlyList<string> All { get; } =
    [
        FixtureChanged, PriceChanged, ResultPublished, CouponRejected, LedgerPosted,
        LegEvaluated, StuckCoupon, PayoutRetry5Seconds, PayoutRetry1Minute,
        PayoutRetry15Minutes, PayoutDeadLetter, PayoutCompleted, IncidentRaised, IncidentUpdated,
        RemediationExecuted, LiabilityChanged, RiskAlert, UserRegistered, EmailVerified, AccountStatusChanged,
        SessionRevoked, AuditRecorded, NotificationRequested, LimitChanged, RestrictionsChanged, SelfExclusionStarted,
        KycStatusChanged, DepositSucceeded, DepositFailed, WithdrawalRequested, WithdrawalDecided, WithdrawalPaid,
        WithdrawalFailed, PaymentDriftDetected, ConfigEntries, CouponPlacedV2, CouponSettledV2, ManualResult, ManualResultRejected, MarketStatusChanged, CasinoTransaction, ProviderReconciliation, ExposureLimits, LimitReached, InboxMessage, ContentChanged,
    ];
}
