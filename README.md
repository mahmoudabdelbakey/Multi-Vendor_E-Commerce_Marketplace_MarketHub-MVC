HEAD

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

| Area                               | Location                                              |
| ---------------------------------- | ----------------------------------------------------- |
| Full Business Analysis             | `docs/business-analysis/business-analysis.md`         |
| Requirements                       | `docs/requirements/requirements.md`                   |
| User Stories & Acceptance Criteria | `docs/requirements/user-stories.md`                   |
| Business Rules                     | `docs/requirements/business-rules.md`                 |
| Risk Register                      | `docs/business-analysis/risk-register.md`             |
| Assumptions & Constraints          | `docs/business-analysis/assumptions-constraints.md`   |
| Traceability Matrix                | `docs/business-analysis/requirements-traceability.md` |
| API Contract Draft                 | `docs/api/api-contract.md`                            |
| Database Design                    | `docs/database/database-design.md`                    |
| Data Dictionary                    | `docs/database/data-dictionary.md`                    |

## Diagrams

Each diagram is available as `.png` for quick preview, `.svg` for editable/vector use, and `.pdf` for print-ready documentation.

| Diagram                                   | Files                             |
| ----------------------------------------- | --------------------------------- |
| System Context Diagram                    | `diagrams/system-context-diagram` |
| Use Case Diagram — Customer + Store Admin | `diagrams/use-case-diagram-full`  |

=======

# SentiReview — Smart Sentiment Analysis API for E-Commerce Reviews

Graduation Project — ASP.NET Core + ML.NET

---

## 📄 Main document

`docs/SentiReview-SAD-Document.docx` — the full System Analysis & Design document (vision, requirements, business rules, diagrams embedded, data structures, testing strategy, development order, etc.)

> _Not created yet — first deliverable of Week 1._

---

## 🎯 Project overview

An AI-powered API that automatically analyzes e-commerce product reviews and classifies them as **Positive**, **Negative**, or **Neutral** in real time, removing the need for store owners to read through thousands of reviews manually.

|                     |                                                                                             |
| ------------------- | ------------------------------------------------------------------------------------------- |
| **Team size**       | 5 members                                                                                   |
| **Timeline**        | 6 weeks                                                                                     |
| **Core stack**      | ASP.NET Core Web API, ML.NET (Binary Classification), SQL Server / SQLite, Swagger          |
| **Related project** | Designed to plug into [Emart](#) (our e-commerce platform) as a reviews-intelligence module |

---

## 📊 Diagrams (source-quality exports: PNG, SVG, PDF)

| Diagram                                             | Files                             |
| --------------------------------------------------- | --------------------------------- |
| System Context Diagram                              | `diagrams/system-context-diagram` |
| Use Case Diagram (Customer + Store Admin, combined) | `diagrams/use-case-diagram-full`  |

> > > > > > > 97f9a8562edec7f076549378f96fba1c914e3b15
> > > > > > > | Activity Diagram — Review Submission & Analysis | `diagrams/review-analysis-activity-diagram` |
> > > > > > > | Sequence — Customer Submits Review | `diagrams/sequence-diagrams/01-customer-submits-review` |
> > > > > > > | Sequence — ML Model Sentiment Prediction | `diagrams/sequence-diagrams/02-ml-sentiment-prediction` |
> > > > > > > | Sequence — Store Admin Views Dashboard | `diagrams/sequence-diagrams/03-admin-views-dashboard` |
> > > > > > > HEAD
> > > > > > > | Sequence — Admin Manages Review | `diagrams/sequence-diagrams/04-admin-review-management` |
> > > > > > > | Sequence — Authentication | `diagrams/sequence-diagrams/05-authentication-flow` |
> > > > > > > | DFD Level 0 — System | `diagrams/dfd-level-0` |
> > > > > > > | DFD Level 1 — Review Analysis Pipeline | `diagrams/dfd-level-1-review-analysis` |
> > > > > > > | DFD Level 2 — Sentiment Prediction | `diagrams/dfd-level-2-sentiment-prediction` |
> > > > > > > | Domain Model / Class Diagram | `diagrams/domain-model-class-diagram` |
> > > > > > > | Deployment Diagram | `diagrams/deployment-diagram` |
> > > > > > > | Conceptual ERD | `diagrams/erd/conceptual-erd` |
> > > > > > > | Logical ERD | `diagrams/erd/logical-erd` |
> > > > > > > | System Architecture — API + ML.NET | `diagrams/system-architecture-diagram` |
> > > > > > > | Component Diagram | `diagrams/component-diagram` |
> > > > > > > | ML Training & Prediction Pipeline | `diagrams/ml-training-prediction-pipeline` |
> > > > > > > | Review Lifecycle / State Diagram | `diagrams/review-lifecycle-state-diagram` |
> > > > > > > | Admin Dashboard Flow | `diagrams/admin-dashboard-flow` |
> > > > > > > | Error Handling Flow | `diagrams/error-handling-flow` |
> > > > > > > | CI/CD & Release Flow | `diagrams/cicd-release-flow` |

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

| DFD Level 0 | `diagrams/dfd-level-0` |
| DFD Level 1 — Review Analysis Pipeline | `diagrams/dfd-level-1-review-analysis` |
| Domain Model / Class Diagram | `diagrams/domain-model-class-diagram` |
| Deployment Diagram | `diagrams/deployment-diagram` |
| Conceptual ERD (crow's-foot) | `diagrams/conceptual-erd` |
| System Architecture Diagram (API + ML.NET integration) | `diagrams/system-architecture-diagram` |

Each diagram will be available as `.png` (quick preview), `.svg` (vector, editable), and `.pdf` (print-ready) — same file name, different extension.

> _Diagrams are planned for Week 1 (Planning & Design phase) — none exported yet._

---

## 🖥️ UI Dashboard

**Admin / Analytics Dashboard** — displays per-product sentiment stats (positive vs. negative %), trend over time, and a searchable reviews table.

> _Not built yet — scheduled for Week 5 (Frontend / Dashboard phase)._

To add it here once built: export as image/PDF and drop into `diagrams/ui/`.

---

## ✅ Status — what's done vs. still pending

**Done:**

- Core idea finalized (Smart Sentiment Analysis API for E-Commerce Reviews)
- Tech stack decided (ASP.NET Core Web API + ML.NET)
- Team roles assigned (5 members)
- 6-week execution plan drafted

**Still pending:**

- Full SAD document (vision, requirements, diagrams, data structures, testing strategy)
- All diagrams listed above
- Dataset collection & cleaning (Hugging Face sentiment datasets)
- ML.NET model training
- API build & Swagger docs
- Admin dashboard UI
- Full test suite + final documentation

---

## 👥 Team & roles

| Member   | Role                           | Core responsibilities                                                     |
| -------- | ------------------------------ | ------------------------------------------------------------------------- |
| Member 1 | Backend / API Lead             | ASP.NET Core Web API structure, Endpoints, database setup & integration   |
| Member 2 | ML / Data Engineer             | Training data prep, ML.NET sentiment model training & tuning              |
| Member 3 | Integration Engineer           | Wiring the ML.NET model into the API, error handling, performance testing |
| Member 4 | Frontend / Dashboard Developer | Admin dashboard UI showing sentiment stats                                |
| Member 5 | QA & Documentation Lead        | Test cases, full test pass, final documentation & presentation            |

---

## 🚀 Recommended development order (6-week plan)

1. **Week 1 — Planning & Environment**: finalize scope, set up GitHub repo & branches, design DB schema, install ML.NET
2. **Week 2 — Backend Core**: build ASP.NET Core Web API project, EF Core + DB, core Endpoints, enable Swagger
3. **Weeks 2–3 (parallel) — ML Model Training**: collect/clean training data, train Binary Classification model, validate accuracy, export `.zip` model
4. **Week 4 — Integration**: load ML.NET model into the API via `PredictionEngine`, auto-analyze new reviews, store results, test response speed
5. **Week 5 — Dashboard & Stats**: build the admin dashboard page with a simple chart (e.g. Chart.js) for sentiment breakdown per product
6. **Week 6 — Final Testing & Docs**: full test pass (positive / negative / ambiguous / empty text), code cleanup, write `README` + SAD doc, prepare demo & presentation

---

## 🔧 Tech stack details

| Layer          | Technology                                          |
| -------------- | --------------------------------------------------- |
| API            | ASP.NET Core Web API                                |
| AI / ML        | ML.NET — Sentiment Analysis (Binary Classification) |
| Database       | SQL Server / SQLite                                 |
| Docs & Testing | Swagger                                             |
| Training data  | Hugging Face sentiment datasets                     |

> > > > > > > 97f9a8562edec7f076549378f96fba1c914e3b15
