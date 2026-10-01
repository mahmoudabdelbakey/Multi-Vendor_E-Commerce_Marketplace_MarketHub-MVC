# Smart Sentiment Analysis API for E-Commerce Reviews

> A business-analysis and system-design package for an ASP.NET Core Web API integrated with ML.NET for sentiment analysis of e-commerce reviews.

## Project Overview

**Goal:** receive customer reviews, validate and persist them, run sentiment prediction through an ML.NET model, expose results through REST APIs, and give store administrators useful review/sentiment insights.

### Core Stack
- ASP.NET Core Web API
- C# / .NET
- ML.NET Sentiment Analysis
- Entity Framework Core
- SQL Server
- REST / JSON
- Swagger / OpenAPI
- Hugging Face sentiment datasets (training/evaluation source, subject to dataset license and suitability)
- Microsoft ML.NET documentation/tutorials

## Business Analysis & System Design

The project is designed around a realistic workflow rather than starting directly with code:

**Business Problem → Requirements → Scope → Actors → Use Cases → User Stories → Business Rules → Data Model → Architecture → API Design → ML Pipeline → Testing → Deployment**

## Repository Documentation

| Area | Location |
| --- | --- |
| Full Business Analysis | `docs/business-analysis/business-analysis.md` |
| Requirements | `docs/requirements/requirements.md` |
| User Stories & Acceptance Criteria | `docs/requirements/user-stories.md` |
| Business Rules | `docs/requirements/business-rules.md` |
| Risk Register | `docs/business-analysis/risk-register.md` |
| Assumptions & Constraints | `docs/business-analysis/assumptions-constraints.md` |
| Traceability Matrix | `docs/business-analysis/requirements-traceability.md` |
| API Contract Draft | `docs/api/api-contract.md` |
| Database Design | `docs/database/database-design.md` |
| Data Dictionary | `docs/database/data-dictionary.md` |

## Diagrams

Each diagram is available as `.png` for quick preview, `.svg` for editable/vector use, and `.pdf` for print-ready documentation.

| Diagram | Files |
| --- | --- |
| System Context Diagram | `diagrams/system-context-diagram` |
| Use Case Diagram — Customer + Store Admin | `diagrams/use-case-diagram-full` |
| Activity Diagram — Review Submission & Analysis | `diagrams/review-analysis-activity-diagram` |
| Sequence — Customer Submits Review | `diagrams/sequence-diagrams/01-customer-submits-review` |
| Sequence — ML Model Sentiment Prediction | `diagrams/sequence-diagrams/02-ml-sentiment-prediction` |
| Sequence — Store Admin Views Dashboard | `diagrams/sequence-diagrams/03-admin-views-dashboard` |
| Sequence — Admin Manages Review | `diagrams/sequence-diagrams/04-admin-review-management` |
| Sequence — Authentication | `diagrams/sequence-diagrams/05-authentication-flow` |
| DFD Level 0 — System | `diagrams/dfd-level-0` |
| DFD Level 1 — Review Analysis Pipeline | `diagrams/dfd-level-1-review-analysis` |
| DFD Level 2 — Sentiment Prediction | `diagrams/dfd-level-2-sentiment-prediction` |
| Domain Model / Class Diagram | `diagrams/domain-model-class-diagram` |
| Deployment Diagram | `diagrams/deployment-diagram` |
| Conceptual ERD | `diagrams/erd/conceptual-erd` |
| Logical ERD | `diagrams/erd/logical-erd` |
| System Architecture — API + ML.NET | `diagrams/system-architecture-diagram` |
| Component Diagram | `diagrams/component-diagram` |
| ML Training & Prediction Pipeline | `diagrams/ml-training-prediction-pipeline` |
| Review Lifecycle / State Diagram | `diagrams/review-lifecycle-state-diagram` |
| Admin Dashboard Flow | `diagrams/admin-dashboard-flow` |
| Error Handling Flow | `diagrams/error-handling-flow` |
| CI/CD & Release Flow | `diagrams/cicd-release-flow` |

## Suggested Implementation Order

1. Confirm scope and requirements.
2. Finalize actors, use cases, and business rules.
3. Finalize conceptual and logical data model.
4. Create ASP.NET Core solution and project structure.
5. Implement EF Core entities/configuration and migrations.
6. Implement review ingestion and validation.
7. Add ML.NET model training/evaluation workflow.
8. Integrate the trained model into the prediction service.
9. Expose review, prediction, and admin analytics APIs.
10. Add authentication/authorization if required by the final scope.
11. Test API, business rules, and model behavior.
12. Add logging, monitoring, security, and deployment configuration.

## Important Scope Note

The diagrams intentionally describe a practical MVP plus extensible features. They are analysis artifacts, not a claim that every feature must be implemented in the first release. The team should convert the approved scope into GitHub Issues before implementation.

## Sources

- Hugging Face datasets: use a suitable sentiment dataset and verify its license, labels, language, and redistribution terms before inclusion.
- Microsoft ML.NET documentation/tutorials: use official Microsoft guidance for model training and inference.
