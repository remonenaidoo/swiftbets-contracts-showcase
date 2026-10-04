// Generated from SwiftBets.Contracts. Do not edit; run `dotnet run --project tools/SwiftBets.Contracts.SchemaGen`.

export interface ResultPublishedV1 {
    fixtureId: string;
    resultVersion: number;
    status: ResultStatus;
    homeGoals: number;
    awayGoals: number;
    publishedAt: string;
    detail: ResultDetailV1 | null;
}

/** Ordered by precedence: a higher value outranks a lower one at the same result version. */
export type ResultStatus = "provisional" | "official" | "correction" | "void";

/** What a score alone cannot settle: tennis games (the score fields carry sets) and the winning selection ids of player and outright markets, such as the top batter or the competition winner. */
export interface ResultDetailV1 {
    homeGames: number | null;
    awayGames: number | null;
    winners: string[] | null;
}

export interface CouponRejectedV1 {
    couponId: string;
    punterId: string;
    reasonCode: string;
    rejectedAt: string;
}

/** Full snapshot of a fixture's offer; OfferVersion increases on every price or status change. */
export interface FixtureChangedV1 {
    fixtureId: string;
    competition: string;
    homeTeam: string;
    awayTeam: string;
    kickoffAt: string;
    status: FixtureStatus;
    offerVersion: number;
    markets: MarketV1[];
    changedAt: string;
    sport: string;
}

export type FixtureStatus = "scheduled" | "inPlay" | "finished" | "postponed";

/** A market and its prices; Line is the handicap or total for line markets, shown beside the market name. */
export interface MarketV1 {
    marketId: string;
    type: MarketType;
    status: MarketStatus;
    selections: SelectionV1[];
    line: number | null;
}

/** What a market prices; MarketIds maps each to the market id suffix settlement reads it from. */
export type MarketType = "matchResult" | "totalGoalsOverUnder25" | "matchWinner" | "setHandicap" | "totalGames" | "handicap" | "totalPoints" | "topBatter" | "competitionWinner";

export type MarketStatus = "open" | "suspended" | "closed";

export interface SelectionV1 {
    selectionId: string;
    name: string;
    odds: number;
}

/** Stage one of settlement: one leg judged against one version of its fixture's result. Keyed by coupon. */
export interface LegEvaluatedV1 {
    couponId: string;
    legId: string;
    fixtureId: string;
    resultVersion: number;
    outcome: LegOutcome;
    evaluatedAt: string;
}

export type LegOutcome = "won" | "lost" | "void";

export interface StuckCouponV1 {
    couponId: string;
    reason: string;
    evaluatedLegs: number;
    legCount: number;
    repaired: boolean;
    detectedAt: string;
}

/** A payout travelling the retry ladder (5s, 1m, 15m) or parked on the dead-letter topic. It carries the settlement outcome so every retry derives exactly the same wallet idempotency key as the first attempt. */
export interface PayoutAttemptV1 {
    couponId: string;
    punterId: string;
    settlementVersion: number;
    outcome: CouponOutcome;
    targetPayout: Money;
    step: PayoutStep;
    attempt: number;
    lastError: string | null;
    firstAttemptAt: string;
}

export type CouponOutcome = "won" | "lost" | "void" | "cashedOut";

/** An amount in minor units (cents) of an ISO 4217 currency. */
export interface Money {
    minorUnits: number;
    currency: string;
}

/** Named, not positional: a retried payout resumes at the step it failed on, whatever steps are added later. */
export type PayoutStep = "computeDelta" | "creditWallet" | "recordPayment";

export interface PayoutCompletedV1 {
    couponId: string;
    punterId: string;
    settlementVersion: number;
    delta: Money;
    paidToDate: Money;
    completedAt: string;
}

/** A detector opened an incident. Kind is one of stuckCoupon, walletOutage, poisonMessage, duplicateSettlement. */
export interface IncidentRaisedV1 {
    incidentId: string;
    kind: string;
    subject: string;
    summary: string;
    openedAt: string;
}

/** An incident changed status (diagnosing, awaitingApproval, diagnosisFailed, resolved). */
export interface IncidentUpdatedV1 {
    incidentId: string;
    kind: string;
    subject: string;
    status: string;
    rootCause: string | null;
    updatedAt: string;
}

/** An operator-approved remediation ran against the platform. */
export interface RemediationExecutedV1 {
    incidentId: string;
    actionId: string;
    actionType: string;
    target: string;
    succeeded: boolean;
    outcome: string;
    decidedBy: string;
    executedAt: string;
}

/** A customer registered and passed the age gate. Carries no personal details; those stay in identity. */
export interface UserRegisteredV1 {
    userId: string;
    brand: string;
    country: string;
    currency: string;
    registeredAt: string;
}

export interface EmailVerifiedV1 {
    userId: string;
    verifiedAt: string;
}

/** An account moved between active, suspended, closed and self-excluded. ChangedBy is a user id or a service name. */
export interface AccountStatusChangedV1 {
    userId: string;
    previousStatus: AccountStatus;
    status: AccountStatus;
    reason: string;
    changedBy: string;
    changedAt: string;
}

export type AccountStatus = "active" | "suspended" | "closed" | "selfExcluded";

/** Sessions to end at once. A null SessionId means every session of the user (suspension, self-exclusion, closure). */
export interface SessionRevokedV1 {
    userId: string;
    sessionId: string | null;
    reason: string;
    revokedAt: string;
}

/** One audited action, written through the owning service's outbox in the same transaction as the change. Before and After are JSON snapshots of the changed fields, null when not applicable. */
export interface AuditRecordedV1 {
    auditId: string;
    service: string;
    actor: string;
    action: string;
    subjectType: string;
    subjectId: string;
    before: string | null;
    after: string | null;
    correlationId: string;
    occurredAt: string;
}

/** Asks notifications to send one message. Recipient is the address for the channel (empty for in-app). Marketing messages are suppressed for restricted or self-excluded customers; service messages are not. */
export interface NotificationRequestedV1 {
    notificationId: string;
    userId: string;
    channel: NotificationChannel;
    template: string;
    recipient: string;
    data: { [key: string]: string; };
    isMarketing: boolean;
    requestedAt: string;
}

export type NotificationChannel = "email" | "push" | "inApp";

/** A money limit was set, lowered, raised or removed. Decreases take effect at once; increases and removals at EffectiveAt after the cooling period. A null Amount means no limit. */
export interface LimitChangedV1 {
    userId: string;
    kind: LimitKind;
    period: LimitPeriod;
    previousAmount: Money | null;
    amount: Money | null;
    effectiveAt: string;
    changedBy: string;
    changedAt: string;
}

/** What a responsible-gambling money limit caps over its period. */
export type LimitKind = "deposit" | "stake" | "loss";

/** Calendar periods in the customer's jurisdiction time zone; a week starts on Monday. */
export type LimitPeriod = "day" | "week" | "month";

/** The whole current responsible-gambling state of one account, keyed by user id on a compacted topic: the latest record is the truth. Revision only increases, so a consumer drops anything older than what it holds. */
export interface RestrictionsChangedV1 {
    userId: string;
    revision: number;
    limits: MoneyLimit[];
    restrictions: Restriction[];
    sessionLimitMinutes: number | null;
    realityCheckMinutes: number | null;
    kycStatus: KycStatus;
    changedAt: string;
}

/** Amount applies now. When PendingEffectiveAt has passed, PendingAmount applies instead (null: the limit is gone). */
export interface MoneyLimit {
    kind: LimitKind;
    period: LimitPeriod;
    amount: Money;
    pendingAmount: Money | null;
    pendingEffectiveAt: string | null;
}

/** Active from StartsAt until EndsAt; a null EndsAt lasts until an operator lifts it. */
export interface Restriction {
    kind: RestrictionKind;
    startsAt: string;
    endsAt: string | null;
    reason: string;
}

/** A block on part or all of an account. CoolingOff and SelfExclusion block everything, including marketing. */
export type RestrictionKind = "coolingOff" | "selfExclusion" | "noDeposits" | "noBetting" | "noWithdrawals" | "noMarketing";

export type KycStatus = "notStarted" | "pending" | "verified" | "rejected";

/** A cooling-off or self-exclusion began. Identity ends every session and notifications stops all marketing. */
export interface SelfExclusionStartedV1 {
    userId: string;
    kind: RestrictionKind;
    startsAt: string;
    endsAt: string | null;
    reason: string;
    changedBy: string;
}

/** A KYC case moved on. Provider names the verification adapter; Reason is set when a case is rejected. */
export interface KycStatusChangedV1 {
    userId: string;
    caseId: string;
    previousStatus: KycStatus;
    status: KycStatus;
    provider: string;
    reason: string | null;
    changedAt: string;
}

/** One provider callback, applied once: ProviderTransactionId is the provider's own id and the idempotency key, so a duplicate callback never produces a second event. A rollback for a bet never seen is stored and still published, with Amount zero, so reconciliation can account for it. */
export interface CasinoTransactionV1 {
    transactionId: string;
    providerId: string;
    providerTransactionId: string;
    roundId: string;
    punterId: string;
    gameId: string;
    kind: CasinoTransactionKind;
    amount: Money;
    occurredAt: string;
}

/** What a casino transaction did to the player's money. */
export type CasinoTransactionKind = "bet" | "win" | "rollback" | "freeSpinBet" | "transferIn" | "transferOut";

/** A lotto draw changed status. SeedHash is the SHA-256 of the house seed, published before the draw; Seed is revealed with the result so anyone can recompute the balls. Provider draws carry no seed. */
export interface LottoDrawChangedV1 {
    drawId: string;
    drawTypeId: string;
    status: LottoDrawStatus;
    drawAt: string;
    closesAt: string;
    balls: number[] | null;
    bonusBall: number | null;
    seedHash: string | null;
    seed: string | null;
    occurredAt: string;
}

/** Where a lotto draw is in its life: open for bets, closed, resulted, settled, or void. */
export type LottoDrawStatus = "open" | "closed" | "resulted" | "settled" | "void";

/** One lotto bet settled through the wallet; only its owner hears about it. */
export interface LottoBetSettledV1 {
    betId: string;
    drawId: string;
    punterId: string;
    won: boolean;
    payout: Money;
    settledAt: string;
}

/** One fixture's open liability after a coupon was placed or settled, keyed by fixture id. Version rises with every change, so a consumer keeps the highest it has seen; WorstCase is the largest liability over the outcomes. */
export interface LiabilityChangedV1 {
    fixtureId: string;
    version: number;
    outcomes: OutcomeLiability[];
    worstCase: Money;
    changedAt: string;
}

/** What the operator owes on one outcome if it wins: every open coupon backing it, at its full potential payout. */
export interface OutcomeLiability {
    marketId: string;
    selectionId: string;
    stake: Money;
    liability: Money;
    coupons: number;
}

/** Advisory: it refuses nothing. PunterIds and CouponIds are the bets that formed the pattern. */
export interface RiskAlertV1 {
    alertId: string;
    kind: RiskAlertKind;
    fixtureId: string;
    selectionId: string | null;
    punterIds: string[];
    couponIds: string[];
    totalStake: Money;
    summary: string;
    raisedAt: string;
}

/** A betting pattern worth a trader's look. */
export type RiskAlertKind = "repeatedBet" | "correlatedStake";

/** The exposure rule in force for one fixture, keyed by fixture id on a compacted topic: the latest record wins. Placement refuses new coupons on a Suspended fixture. CapMinorUnits is the worst-case liability that suspends it, null for the service default. */
export interface ExposureLimitV1 {
    fixtureId: string;
    capMinorUnits: number | null;
    suspended: boolean;
    reason: string;
    updatedAt: string;
}

/** The wallet refused a stake or a deposit because it would pass one of the customer's own limits. Published so the customer is told; Refused is "stake" or "deposit". */
export interface LimitReachedV1 {
    userId: string;
    kind: LimitKind;
    period: LimitPeriod;
    refused: string;
    attempted: Money;
    reachedAt: string;
}

/** A message put in one customer's in-app inbox, keyed by user id; realtime pushes it to that customer only. */
export interface InboxMessageV1 {
    messageId: string;
    userId: string;
    category: string;
    title: string;
    body: string;
    createdAt: string;
}

/** A provider's daily report compared with our ledger for the same business day. */
export interface ProviderReconciliationV1 {
    providerId: string;
    businessDate: string;
    ourNet: Money;
    providerNet: Money;
    drift: Money;
    missingOnOurSide: number;
    missingOnProviderSide: number;
    status: ReconciliationStatus;
    reconciledAt: string;
}

export type ReconciliationStatus = "matched" | "drift";

/** A provider confirmed a deposit and the wallet credited it. Keyed by user id. */
export interface DepositSucceededV1 {
    paymentId: string;
    userId: string;
    amount: Money;
    provider: string;
    providerReference: string;
    completedAt: string;
}

/** A deposit did not complete: the provider declined it or the wallet refused the credit (a limit or restriction). */
export interface DepositFailedV1 {
    paymentId: string;
    userId: string;
    amount: Money;
    provider: string;
    reason: string;
    failedAt: string;
}

/** A customer asked to withdraw; the amount is held in the wallet. RequiresApproval is set above the operator threshold. */
export interface WithdrawalRequestedV1 {
    withdrawalId: string;
    userId: string;
    amount: Money;
    requiresApproval: boolean;
    requestedAt: string;
}

/** An operator approved or rejected a withdrawal held for approval; a rejection returns the hold. */
export interface WithdrawalDecidedV1 {
    withdrawalId: string;
    userId: string;
    approved: boolean;
    decidedBy: string;
    reason: string | null;
    decidedAt: string;
}

/** The provider paid a withdrawal out and the wallet hold was captured. */
export interface WithdrawalPaidV1 {
    withdrawalId: string;
    userId: string;
    amount: Money;
    provider: string;
    providerReference: string;
    paidAt: string;
}

/** A withdrawal ended without paying out (provider failure or rejection); the hold went back to the customer. */
export interface WithdrawalFailedV1 {
    withdrawalId: string;
    userId: string;
    amount: Money;
    reason: string;
    failedAt: string;
}

/** The daily provider-against-ledger reconciliation found differences for a provider and day. Keyed by provider. */
export interface PaymentDriftDetectedV1 {
    runId: string;
    provider: string;
    day: string;
    driftCount: number;
    netDifference: Money;
    summary: string;
    detectedAt: string;
}

/** A placed coupon with one or more bets over its legs. */
export interface CouponPlacedV2 {
    couponId: string;
    punterId: string;
    totalStake: Money;
    potentialPayout: Money;
    legs: CouponLegV2[];
    bets: CouponBetV2[];
    placedAt: string;
}

/** A leg of a V2 coupon. A banker is in every line of every bet on the coupon. A bet builder leg has market id MarketId, its components in Builder, and settles as one leg. */
export interface CouponLegV2 {
    legId: string;
    fixtureId: string;
    marketId: string;
    selectionId: string;
    odds: number;
    offerVersion: number;
    isBanker: boolean;
    builder: BetBuilderLegV1 | null;
}

/** The pricing inputs a bet builder leg was accepted on: its components, the correlation factor of every pair (keyed by PairKey) and the margin. Settlement reprices from these when a component is void. */
export interface BetBuilderLegV1 {
    components: BuilderComponentV1[];
    pairFactors: { [key: string]: number; };
    marginPercent: number;
}

/** One selection inside a bet builder, at the price it had on its own when the builder was priced. */
export interface BuilderComponentV1 {
    marketId: string;
    selectionId: string;
    odds: number;
}

/** One bet on a coupon: every combination of each size in Folds taken from the non-banker legs, with the bankers added to each. A Trixie on three selections is folds [2, 3]; singles are [1]; an accumulator of n is [n]. Stake is UnitStake times Lines. AccaBoostPercent is the share of the winnings settlement adds when the bet wins, fixed at placement (see AccaBoost). */
export interface CouponBetV2 {
    betId: string;
    name: string;
    folds: number[];
    lines: number;
    unitStake: Money;
    stake: Money;
    potentialPayout: Money;
    accaBoostPercent: number;
}

/** A V2 coupon's settlement at SettlementVersion. The coupon is Won when it returns more than nothing but not only stakes back, Void when every line is void, otherwise Lost. Payout moves the difference between TargetPayout and what it has already paid. */
export interface CouponSettledV2 {
    couponId: string;
    punterId: string;
    settlementVersion: number;
    outcome: CouponOutcome;
    totalStake: Money;
    targetPayout: Money;
    bets: BetSettlementV2[];
    settledAt: string;
}

/** One bet's settlement: how its lines fell and what it returns. Return includes BoostBonus. */
export interface BetSettlementV2 {
    betId: string;
    outcome: CouponOutcome;
    winningLines: number;
    voidLines: number;
    losingLines: number;
    return: Money;
    boostBonus: Money | null;
}

/** One operational setting, keyed by Key on a compacted topic: the latest record is the value in force. Version only increases per key, so a consumer drops anything older than what it holds. Value is text; see ConfigKeys for the keys services read and how each is parsed. */
export interface ConfigEntryV1 {
    key: string;
    value: string;
    version: number;
    changedBy: string;
    reason: string;
    changedAt: string;
}

/** Site content changed: Kind is "home" (banners or quick links) or "page", with the page slug. It carries no content; realtime tells every open client, which refetches the public content API. */
export interface ContentChangedV1 {
    kind: string;
    slug: string | null;
    changedBy: string;
    changedAt: string;
}

/** A trader's result, keyed by ManualResultId so a retry is idempotent. MarketId is set for Market scope, CouponId for Coupon scope; WinningSelectionId for Settle and Override; VoidFrom for TimeVoid. */
export interface ManualResultV1 {
    manualResultId: string;
    scope: ManualResultScope;
    action: ManualResultAction;
    fixtureId: string;
    marketId: string | null;
    couponId: string | null;
    winningSelectionId: string | null;
    voidFrom: string | null;
    reason: string;
    operatorId: string;
    issuedAt: string;
}

/** What a manual result applies to: one coupon, one market, or every market of a fixture. */
export type ManualResultScope = "coupon" | "market" | "fixture";

export type ManualResultAction = "settle" | "void" | "override" | "timeVoid";

/** Settlement refused a manual result for one coupon; Code is stable, e.g. coupon_cashed_out. */
export interface ManualResultRejectedV1 {
    manualResultId: string;
    couponId: string;
    code: string;
    message: string;
    rejectedAt: string;
}

/** A market suspended, reopened or closed outside the feed. Source is trader, staleness or config. */
export interface MarketStatusChangedV1 {
    fixtureId: string;
    marketId: string;
    status: MarketStatus;
    source: string;
    reason: string;
    operatorId: string | null;
    changedAt: string;
}

/** RFC 7807 problem details plus a stable machine code and the request's correlation id. */
export interface ErrorEnvelope {
    type: string;
    title: string;
    status: number;
    code: string;
    correlationId: string;
    detail: string | null;
    instance: string | null;
    errors: FieldError[] | null;
}

export interface FieldError {
    field: string;
    code: string;
    message: string;
}
