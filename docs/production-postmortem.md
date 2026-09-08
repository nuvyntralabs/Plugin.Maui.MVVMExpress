# Production post-mortem (Phase 10)

A polished sample is not social proof. This file is the slot for a real app you control.

Until that write-up exists:

- Playground + `dotnet new mvvmexpress` remain the public proof surface
- 1.0 APIs stay locked by `Contract.Tests`
- Do not cite a clone app as production

When you have a shipped app, record: package version, platform mix, what broke, and what the host adapters (`UseAuth`, `UseDeepLinks`, `UseSecureSessionAuth`) actually did.
