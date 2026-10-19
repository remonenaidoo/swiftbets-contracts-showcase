# swiftbets-contracts

[![ci](https://github.com/remonenaidoo/swiftbets-contracts-showcase/actions/workflows/ci.yml/badge.svg)](https://github.com/remonenaidoo/swiftbets-contracts-showcase/actions/workflows/ci.yml)

The shared, versioned contracts of the SwiftBets platform: every event that crosses Kafka, the single API error envelope, the gRPC wallet service, and the TypeScript types the dashboard and mobile app compile against.

| Package | Contents |
|---|---|
| `SwiftBets.Contracts` (NuGet) | `EventEnvelope<T>`, event records (`CouponPlacedV1`, `ResultPublishedV1`, ...), `TopicName` / `Topics`, `ErrorEnvelope`, `Result<T>`, `Money`, `ContractJson` serializer options |
| `SwiftBets.Contracts.Grpc` (NuGet) | `swiftbets.wallet.v1.Wallet` client and server stubs, generated from [`protos/`](protos/swiftbets/wallet/v1/wallet.proto) |
| `@swiftbets/contracts` (npm) | TypeScript types generated from the same records, plus topic names |

## Rules

- Records are immutable. A breaking change is a new type and a new topic major (`CouponPlacedV2`, `...v2`), never an edit.
- `schemas/*.schema.json` and `ts/src/generated.ts` are generated and checked in. `SchemaSnapshotTests` fails when a contract changes without regenerating, so every contract change is visible in review.
- Topic names are `swiftbets.<domain>.<event>.v<N>.<env>`, built only through `TopicName.For`.
- Every mutating wallet RPC takes an idempotency key; expected failures come back as a typed `WalletFailure`, not as a status code.

## Build

```bash
dotnet test
dotnet run --project tools/SwiftBets.Contracts.SchemaGen -- .   # regenerate schemas and TS types
(cd ts && npm ci && npm run build)
```

## Releases

Tagging `vX.Y.Z` publishes the `.nupkg` files and the npm tarball as assets of the GitHub release. Consumers pin the exact version in `Directory.Packages.props` and CI fetches it into a local feed (`scripts/fetch-shared-packages.sh` in each service).

## Part of SwiftBets

SwiftBets is a sportsbook and casino platform built as independent .NET services. It runs live at [swiftbets.remonenaidoo.com](https://swiftbets.remonenaidoo.com). These repositories show a selection of it: [Settlement](https://github.com/remonenaidoo/swiftbets-settlement-showcase) · [Wallet](https://github.com/remonenaidoo/swiftbets-wallet-showcase) · [Originals](https://github.com/remonenaidoo/swiftbets-originals-showcase) · [Steward](https://github.com/remonenaidoo/swiftbets-steward-showcase) · [Building blocks](https://github.com/remonenaidoo/swiftbets-building-blocks-showcase).

## License

MIT
