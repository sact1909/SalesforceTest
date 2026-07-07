# Salesforce Transactional Entities for Account Context

This document is the master reference of all Salesforce API objects considered transactional
within the context of an Account. It combines two sources:

1. **The official transactional objects reference** (Xari Integration Reference · June 2026)
2. **The Explorer dynamic discovery** — standard, queryable, non-deprecated objects actually found
   in the connected org via `GET /services/data/v59.0/sobjects`
   (`Queryable=true`, `DeprecatedAndHidden=false`, `CustomSetting=false`, no `__c` suffix)

> **Availability note:** Not all objects are present in every org. Depends on licensed products
> (Sales Cloud, Revenue Cloud / Billing, Commerce Cloud, Service Cloud). Always validate via
> dynamic discovery before querying.

Legend — **Source** column:
- `PDF` — from the official reference document
- `Explorer` — found in the connected org's dynamic discovery cache
- `PDF + Explorer` — present in both

---

## Sales Pipeline & Opportunity Management

| API Object Name       | Label               | Source          | Relation to Account              | Already Queried |
|-----------------------|---------------------|-----------------|----------------------------------|-----------------|
| `Opportunity`         | Opportunity         | PDF + Explorer  | Direct `AccountId` field         | No              |
| `OpportunityLineItem` | OpportunityLineItem | PDF + Explorer  | Via `Opportunity.AccountId`      | No              |
| `OpportunityHistory`  | Opportunity History | Explorer        | Via `Opportunity.AccountId`      | No              |

**Not included from this group:**
- `Lead` — pre-transaction, not yet linked to an Account
- `Campaign` / `CampaignMember` — marketing attribution, no financial value
- `OpportunityStage` — metadata/picklist definition, not a data record
- `OpportunityContactRole` — junction object, no financial value
- `OpportunityFieldHistory` — audit trail, not a transaction

---

## Product Catalog & Pricing

| API Object Name          | Label                   | Source          | Purpose                                              | Already Queried |
|--------------------------|-------------------------|-----------------|------------------------------------------------------|-----------------|
| `Product2`               | Product                 | PDF + Explorer  | Master product catalog                               | No              |
| `Pricebook2`             | Pricebook               | PDF + Explorer  | Price list applied to Orders/Quotes                  | No              |
| `PricebookEntry`         | PricebookEntry          | PDF + Explorer  | Specific product price within a Pricebook            | No              |
| `ProductCatalog`         | Product Catalog         | Explorer        | Commerce Cloud catalog grouping products             | No              |
| `ProductCategory`        | Product Category        | Explorer        | Category hierarchy within a ProductCatalog           | No              |
| `ProductCategoryProduct` | Product Category Product| Explorer        | Junction: product assigned to a category             | No              |
| `ProductRelationshipType`| Product Relationship Type| Explorer       | Defines how products relate (bundles, substitutes)   | No              |
| `TaxTreatment`           | Tax Treatment           | Explorer        | Tax rules applied to order/invoice line items        | No              |

---

## Quoting & Order Processing

| API Object Name  | Label         | Source          | Relation to Account                    | Already Queried |
|------------------|---------------|-----------------|----------------------------------------|-----------------|
| `Quote`          | Quote         | PDF             | Via `Opportunity.AccountId`            | No              |
| `QuoteLineItem`  | QuoteLineItem | PDF             | Child of `Quote`                       | No              |
| `Contract`       | Contract      | PDF             | Direct `AccountId` field               | No              |
| `Order`          | Order         | PDF + Explorer  | Direct `AccountId` field               | **Yes**         |
| `OrderItem`      | OrderItem     | PDF             | Child of `Order`                       | No              |
| `OrderHistory`   | Order History | Explorer        | Audit trail of Order field changes     | No              |

---

## Billing, Invoicing & Payments

The `blng__*` objects use the `__c` suffix and are **excluded by the Explorer's current
standard-object filter**. They require dedicated hardcoded queries (same pattern as `Order`
and `Invoice` today) if the org has Revenue Cloud / Billing enabled.

| API Object Name        | Label                  | Source          | Relation to Account               | Already Queried |
|------------------------|------------------------|-----------------|-----------------------------------|-----------------|
| `Invoice`              | Invoice                | PDF + Explorer  | Via `OrderSummary` or direct      | **Yes**         |
| `InvoiceLine`          | Invoice Line           | Explorer        | Child of `Invoice`                | No              |
| `LegalEntity`          | Legal Entity           | Explorer        | Entity issuing invoices/payments  | No              |
| `blng__Invoice__c`     | Invoice (Billing)      | PDF             | Via Order/Contract → Account      | No ⚠️          |
| `blng__InvoiceLine__c` | Invoice Line (Billing) | PDF             | Child of `blng__Invoice__c`       | No ⚠️          |
| `blng__Payment__c`     | Payment (Billing)      | PDF             | Via `blng__Invoice__c` → Account  | No ⚠️          |
| `blng__CreditNote__c`  | Credit Memo (Billing)  | PDF             | Via Order/Account                 | No ⚠️          |
| `blng__Refund__c`      | Refund (Billing)       | PDF             | Via payment/credit memo → Account | No ⚠️          |
| `PaymentLineInvoice`   | PaymentLineInvoice     | PDF             | Junction: Payment ↔ Invoice Lines | No              |

> ⚠️ `blng__*` objects end in `__c` and are filtered out by the Explorer. Add dedicated
> hardcoded SOQL queries or adjust the filter for known Billing package prefixes.

---

## Commerce Cloud & Order Management

| API Object Name            | Label                      | Source          | Relation to Account               | Already Queried |
|----------------------------|----------------------------|-----------------|-----------------------------------|-----------------|
| `OrderSummary`             | OrderSummary               | PDF             | Via `BillingAccountId`            | No              |
| `OrderItemSummary`         | OrderItemSummary           | PDF             | Child of `OrderSummary`           | No              |
| `FulfillmentOrder`         | FulfillmentOrder           | PDF             | Via `OrderSummaryId` → Account    | No              |
| `FulfillmentOrderLineItem` | FulfillmentOrderLineItem   | PDF             | Child of `FulfillmentOrder`       | No              |
| `WebCart`                  | WebCart                    | PDF             | Via `AccountId` (B2B storefronts) | No              |
| `CartItem`                 | CartItem                   | PDF             | Child of `WebCart`                | No              |
| `WebStore`                 | Store                      | Explorer        | B2B/B2C storefront configuration  | No              |
| `WebStoreCatalog`          | Store Catalog              | Explorer        | Catalog assigned to a WebStore    | No              |
| `WebStorePricebook`        | Store Pricebook            | Explorer        | Pricebook assigned to a WebStore  | No              |
| `WebStoreBuyerGroup`       | Store Buyer Group          | Explorer        | Buyer groups for a WebStore       | No              |
| `SalesStore`               | Sales Store                | Explorer        | B2B Sales store entity            | No              |
| `SalesStoreCatalog`        | Sales Store Catalog        | Explorer        | Catalog linked to a Sales Store   | No              |
| `BuyerAccount`             | Buyer Account              | Explorer        | B2B buyer linked to an Account    | No              |
| `BuyerGroup`               | Buyer Group                | Explorer        | Groups buyers for pricing/entitlements | No         |
| `BuyerGroupPricebook`      | Buyer Group Pricebook      | Explorer        | Pricebook assigned to a BuyerGroup| No              |
| `CommerceEntitlementPolicy`| Entitlement Policy         | Explorer        | Access rules for B2B commerce     | No              |
| `CommerceEntitlementProduct`| Entitlement Product       | Explorer        | Products covered by an entitlement policy | No       |
| `CommerceEntitlementBuyerGroup`| Entitlement Buyer Group| Explorer        | Buyer groups under an entitlement policy  | No       |
| `ShippingConfigurationSet` | Shipping Configuration Set | Explorer        | Shipping rules applied to orders  | No              |
| `ShippingRateGroup`        | Shipping Rate Group        | Explorer        | Groups of shipping rates          | No              |
| `ShippingRateArea`         | Shipping Rate Area         | Explorer        | Geographic area for shipping rates| No              |
| `StandardShippingRate`     | Standard Shipping Rate     | Explorer        | Per-rate cost applied to orders   | No              |
| `Location`                 | Location                   | Explorer        | Fulfillment/warehouse location    | No              |

> All Commerce Cloud objects are license-dependent. Validate presence via dynamic discovery
> before implementing.

---

## Post-Sale & Customer Service

| API Object Name       | Label               | Source          | Relation to Account              | Already Queried |
|-----------------------|---------------------|-----------------|----------------------------------|-----------------|
| `Case`                | Case                | PDF             | Direct `AccountId` field         | No              |
| `ServiceContract`     | ServiceContract     | PDF             | Direct `AccountId` field         | No              |
| `Entitlement`         | Entitlement         | PDF             | Via `ServiceContract.AccountId`  | No              |
| `ReturnOrder`         | ReturnOrder         | PDF             | Linked to original `Order`       | No              |
| `ReturnOrderLineItem` | ReturnOrderLineItem | PDF             | Child of `ReturnOrder`           | No              |

---

## Core Context Objects (Already Queried)

These are not transactions themselves but are the root context required for all account-scoped
transaction queries. Already implemented with dedicated DTOs.

| API Object Name | Label   | Source          | Relation to Account         | Already Queried |
|-----------------|---------|-----------------|------------------------------|-----------------|
| `Account`       | Account | Explorer        | Root entity                  | **Yes**         |
| `Contact`       | Contact | Explorer        | Direct `AccountId` field     | **Yes**         |

---

## Recommended Implementation Order

1. `Contract` — direct `AccountId`, legal context for all orders and billing
2. `Opportunity` (filter: `StageName = 'Closed Won'`) — upstream source of every Order
3. `OrderItem` + `InvoiceLine` — sub-selects within existing `Order` and `Invoice` queries
4. `Quote` + `QuoteLineItem` — pre-order pricing history per account
5. `Case` — service transaction history; direct `AccountId`
6. `ServiceContract` + `Entitlement` — support coverage linked to account
7. `ReturnOrder` + `ReturnOrderLineItem` — return/refund lifecycle
8. `blng__Payment__c` + `blng__CreditNote__c` + `blng__Refund__c` — if Revenue Cloud active, add as hardcoded queries
9. `BuyerAccount` + `BuyerGroup` + `WebStore` — if Commerce Cloud B2B is active
10. `OrderSummary` + `FulfillmentOrder` — if Order Management is active
