# 04.05.02.04.04-groups-governors-area-controller-contracts: Migrate Groups/Governors area controllers and contract-sensitive flows

## Objective
Migrate Groups and Governors area controllers to ASP.NET Core while preserving contract-sensitive workflow behaviors and route dependencies.

## Scope
- Web/Edubase.Web.UI/Areas/Groups/Controllers/*.cs
- Web/Edubase.Web.UI/Areas/Governors/Controllers/*.cs

## Steps
1. Migrate controller and attribute contracts to ASP.NET Core APIs.
2. Preserve route-area conventions and RedirectToRoute behavior.
3. Maintain anti-forgery/auth attributes and validation flow semantics.
4. Build and resolve compile issues for this slice.
