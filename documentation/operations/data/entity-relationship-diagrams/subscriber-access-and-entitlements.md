# Subscriber Access And Entitlements

This page records the retired subscriber access and commercial subscription model.

The subscriber subdomain has been confirmed as no longer in use. The model is included to explain the legacy schema and to avoid mistaking these tables for active access-control concepts.

## Scope

This model covers:

- subscriber user group context;
- subscription information;
- subscription orders, packages and schedules;
- additional dataset orders;
- invoices and payment terms.

## How To Read This Model

- This is a retired commercial subscription model.
- It is closer to order management than ordinary application authorisation.
- Shared user and group tables are not retired just because they appear in this model.
- Subscription-specific tables should not be treated as active access-control dependencies without renewed business confirmation.

## Application-Derived Insights

- The model describes packages, orders and invoices rather than simple entitlements.
- The schema does not contain a clean bridge saying that a live subscription grants access to a dataset.
- Future access-control design should not reuse this model as a general entitlement pattern.
- Retired subscriber tables should be separated from active identity and user-group concepts.

## Subscriber Access And Entitlements

```mermaid
erDiagram
    UserRole {
        nvarchar authority PK
        nvarchar caption
    }
    UserGroup {
        nvarchar code PK
        nvarchar name
        nvarchar role FK
    }
    SystemUser {
        nvarchar username PK
        nvarchar UserGroupCode FK
        numeric subscriptionInfo_id FK
    }
    SubscriptionInfo {
        numeric id PK
        tinyint privateUse
        tinyint notForMarketing
        nvarchar purpose
        nvarchar postcode
    }
    SubscriptionOrder {
        numeric id PK
        nvarchar user_username FK
        nvarchar subscriptionPackage_code FK
        numeric schedule_id FK
        nvarchar paymentTerm_code FK
        nvarchar extractType
        datetime endSubscriptionDate
        datetime expiredDate
        numeric sum
    }
    SubscriptionPackage {
        nvarchar code PK
        nvarchar name
        numeric price
    }
    SubscriptionSchedule {
        numeric id PK
        nvarchar type
        int dayOfMonth
        int dayOfWeek
    }
    AdditionalDatasetOrder {
        numeric id PK
        numeric subscription_order_id FK
        nvarchar datasetName_code FK
        numeric sum
    }
    DatasetName {
        nvarchar code PK
        nvarchar displayName
    }
    SubscriptionInvoice {
        numeric id PK
        numeric subscriptionOrder_id FK
        numeric invoiceDocument_id FK
        numeric proformaDocument_id FK
        nvarchar invoiceStatus
        int instalment
        numeric total
        datetime paidDate
    }
    PaymentTerm {
        nvarchar code PK
        nvarchar name
    }
    InvoiceDocument {
        numeric id PK
        nvarchar number
    }

    UserRole ||--o{ UserGroup : broad_role
    UserGroup ||--o{ SystemUser : has_users
    SubscriptionInfo o|--o{ SystemUser : subscriber_details
    SystemUser ||--o{ SubscriptionOrder : places_orders
    SubscriptionPackage o|--o{ SubscriptionOrder : package
    SubscriptionSchedule o|--o{ SubscriptionOrder : delivery_schedule
    PaymentTerm o|--o{ SubscriptionOrder : payment_terms
    SubscriptionOrder ||--o{ AdditionalDatasetOrder : adds_datasets
    DatasetName o|--o{ AdditionalDatasetOrder : selected_dataset
    SubscriptionOrder ||--o{ SubscriptionInvoice : invoices
    InvoiceDocument o|--o{ SubscriptionInvoice : invoice_document
    InvoiceDocument ||--o{ SubscriptionInvoice : proforma_document
```

### SubscriptionInfo

Business-friendly pattern:

```text
For this subscriber user,
what purpose, address and use declarations were recorded?
```

### SubscriptionOrder

Business-friendly pattern:

```text
For this subscriber user,
what subscription package, delivery schedule and payment term were ordered?
```

### AdditionalDatasetOrder

Business-friendly pattern:

```text
For this subscription order,
which additional datasets were ordered?
```

### SubscriptionInvoice

Business-friendly pattern:

```text
For this subscription order,
what invoice or proforma document was raised,
and what payment status was recorded?
```

## Reading This Diagram

Use this model as a retired-subdomain reference. It should not drive current access-control design unless the subscriber business process is explicitly reinstated.
