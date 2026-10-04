using SwiftBets.Contracts.Audit;
using SwiftBets.Contracts.Compliance;
using SwiftBets.Contracts.Config;
using SwiftBets.Contracts.Errors;
using SwiftBets.Contracts.Identity;
using SwiftBets.Contracts.Notifications;
using SwiftBets.Contracts.Offer;
using SwiftBets.Contracts.Casino;
using SwiftBets.Contracts.Risk;
using SwiftBets.Contracts.Payments;
using SwiftBets.Contracts.Payout;
using SwiftBets.Contracts.Placement;
using SwiftBets.Contracts.Settlement;
using SwiftBets.Contracts.Steward;
using SwiftBets.Contracts.Trading;

namespace SwiftBets.Contracts.SchemaGen;

public static class ContractCatalog
{
    public static IReadOnlyList<(string Name, Type Type)> Contracts { get; } =
    [
        ($"{ResultPublishedV1.EventType}.v{ResultPublishedV1.EventVersion}", typeof(ResultPublishedV1)),
        ($"{CouponRejectedV1.EventType}.v{CouponRejectedV1.EventVersion}", typeof(CouponRejectedV1)),
        ($"{FixtureChangedV1.EventType}.v{FixtureChangedV1.EventVersion}", typeof(FixtureChangedV1)),
        ($"{LegEvaluatedV1.EventType}.v{LegEvaluatedV1.EventVersion}", typeof(LegEvaluatedV1)),
        ($"{StuckCouponV1.EventType}.v{StuckCouponV1.EventVersion}", typeof(StuckCouponV1)),
        ($"{PayoutAttemptV1.EventType}.v{PayoutAttemptV1.EventVersion}", typeof(PayoutAttemptV1)),
        ($"{PayoutCompletedV1.EventType}.v{PayoutCompletedV1.EventVersion}", typeof(PayoutCompletedV1)),
        ($"{IncidentRaisedV1.EventType}.v{IncidentRaisedV1.EventVersion}", typeof(IncidentRaisedV1)),
        ($"{IncidentUpdatedV1.EventType}.v{IncidentUpdatedV1.EventVersion}", typeof(IncidentUpdatedV1)),
        ($"{RemediationExecutedV1.EventType}.v{RemediationExecutedV1.EventVersion}", typeof(RemediationExecutedV1)),
        ($"{UserRegisteredV1.EventType}.v{UserRegisteredV1.EventVersion}", typeof(UserRegisteredV1)),
        ($"{EmailVerifiedV1.EventType}.v{EmailVerifiedV1.EventVersion}", typeof(EmailVerifiedV1)),
        ($"{AccountStatusChangedV1.EventType}.v{AccountStatusChangedV1.EventVersion}", typeof(AccountStatusChangedV1)),
        ($"{SessionRevokedV1.EventType}.v{SessionRevokedV1.EventVersion}", typeof(SessionRevokedV1)),
        ($"{AuditRecordedV1.EventType}.v{AuditRecordedV1.EventVersion}", typeof(AuditRecordedV1)),
        ($"{NotificationRequestedV1.EventType}.v{NotificationRequestedV1.EventVersion}", typeof(NotificationRequestedV1)),
        ($"{LimitChangedV1.EventType}.v{LimitChangedV1.EventVersion}", typeof(LimitChangedV1)),
        ($"{RestrictionsChangedV1.EventType}.v{RestrictionsChangedV1.EventVersion}", typeof(RestrictionsChangedV1)),
        ($"{SelfExclusionStartedV1.EventType}.v{SelfExclusionStartedV1.EventVersion}", typeof(SelfExclusionStartedV1)),
        ($"{KycStatusChangedV1.EventType}.v{KycStatusChangedV1.EventVersion}", typeof(KycStatusChangedV1)),
        ($"{CasinoTransactionV1.EventType}.v{CasinoTransactionV1.EventVersion}", typeof(CasinoTransactionV1)),
        ($"{LiabilityChangedV1.EventType}.v{LiabilityChangedV1.EventVersion}", typeof(LiabilityChangedV1)),
        ($"{RiskAlertV1.EventType}.v{RiskAlertV1.EventVersion}", typeof(RiskAlertV1)),
        ($"{ExposureLimitV1.EventType}.v{ExposureLimitV1.EventVersion}", typeof(ExposureLimitV1)),
        ($"{LimitReachedV1.EventType}.v{LimitReachedV1.EventVersion}", typeof(LimitReachedV1)),
        ($"{InboxMessageV1.EventType}.v{InboxMessageV1.EventVersion}", typeof(InboxMessageV1)),
        ($"{ProviderReconciliationV1.EventType}.v{ProviderReconciliationV1.EventVersion}", typeof(ProviderReconciliationV1)),
        ($"{DepositSucceededV1.EventType}.v{DepositSucceededV1.EventVersion}", typeof(DepositSucceededV1)),
        ($"{DepositFailedV1.EventType}.v{DepositFailedV1.EventVersion}", typeof(DepositFailedV1)),
        ($"{WithdrawalRequestedV1.EventType}.v{WithdrawalRequestedV1.EventVersion}", typeof(WithdrawalRequestedV1)),
        ($"{WithdrawalDecidedV1.EventType}.v{WithdrawalDecidedV1.EventVersion}", typeof(WithdrawalDecidedV1)),
        ($"{WithdrawalPaidV1.EventType}.v{WithdrawalPaidV1.EventVersion}", typeof(WithdrawalPaidV1)),
        ($"{WithdrawalFailedV1.EventType}.v{WithdrawalFailedV1.EventVersion}", typeof(WithdrawalFailedV1)),
        ($"{PaymentDriftDetectedV1.EventType}.v{PaymentDriftDetectedV1.EventVersion}", typeof(PaymentDriftDetectedV1)),
        ($"{CouponPlacedV2.EventType}.v{CouponPlacedV2.EventVersion}", typeof(CouponPlacedV2)),
        ($"{CouponSettledV2.EventType}.v{CouponSettledV2.EventVersion}", typeof(CouponSettledV2)),
        ($"{ConfigEntryV1.EventType}.v{ConfigEntryV1.EventVersion}", typeof(ConfigEntryV1)),
        ($"{ContentChangedV1.EventType}.v{ContentChangedV1.EventVersion}", typeof(ContentChangedV1)),
        ($"{ManualResultV1.EventType}.v{ManualResultV1.EventVersion}", typeof(ManualResultV1)),
        ($"{ManualResultRejectedV1.EventType}.v{ManualResultRejectedV1.EventVersion}", typeof(ManualResultRejectedV1)),
        ($"{MarketStatusChangedV1.EventType}.v{MarketStatusChangedV1.EventVersion}", typeof(MarketStatusChangedV1)),
        ("error-envelope", typeof(ErrorEnvelope)),
    ];
}
