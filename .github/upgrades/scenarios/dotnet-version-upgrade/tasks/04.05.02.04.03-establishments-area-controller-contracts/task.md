# 04.05.02.04.03-establishments-area-controller-contracts: Migrate Establishments area controllers and their route/redirect contracts

## Objective
Migrate Establishments area controller surfaces to ASP.NET Core with route-name and action-result parity.

## Scope
- Web/Edubase.Web.UI/Areas/Establishments/Controllers/*.cs

## Steps
1. Convert base classes, attributes, and action signatures.
2. Preserve area route prefixes, named routes, and redirection targets.
3. Resolve compile blockers introduced by area-specific dependencies.
