# Security Policy

## Supported versions

MVVMExpress has not shipped a stable NuGet package. Until 1.0.0, report issues against `main`.

## Reporting a vulnerability

Do not open a public issue for vulnerabilities that could leak tokens, personal data, or allow unexpected code execution in consumer apps.

Use GitHub Security Advisories on the repository, or contact the maintainer listed on [https://github.com/NiladriPadhy](https://github.com/NiladriPadhy).

## What this library will not do

- Store secrets in ViewModels
- Log passwords, tokens, or raw personal data
- Ship insecure default storage for `[PersistState]`
- Implement authentication providers (use Plugin.Maui.SecureSession or your own)

## What we will do

Acknowledge actionable reports and fix confirmed issues in a patch release after 1.0.0, or in the next 0.x drop before then.
