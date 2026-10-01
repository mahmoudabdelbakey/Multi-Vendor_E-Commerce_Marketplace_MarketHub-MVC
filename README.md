# SentiReview — Smart Sentiment Analysis API for E-Commerce Reviews
Graduation Project — ASP.NET Core + ML.NET

---

## 📄 Main document

`docs/SentiReview-SAD-Document.docx` — the full System Analysis & Design document (vision, requirements, business rules, diagrams embedded, data structures, testing strategy, development order, etc.)

> *Not created yet — first deliverable of Week 1.*

---

## 🎯 Project overview

An AI-powered API that automatically analyzes e-commerce product reviews and classifies them as **Positive**, **Negative**, or **Neutral** in real time, removing the need for store owners to read through thousands of reviews manually.

| | |
|---|---|
| **Team size** | 5 members |
| **Timeline** | 6 weeks |
| **Core stack** | ASP.NET Core Web API, ML.NET (Binary Classification), SQL Server / SQLite, Swagger |
| **Related project** | Designed to plug into [Emart](#) (our e-commerce platform) as a reviews-intelligence module |

---

## 📊 Diagrams (source-quality exports: PNG, SVG, PDF)

| Diagram | Files |
|---|---|
| System Context Diagram | `diagrams/system-context-diagram` |
| Use Case Diagram (Customer + Store Admin, combined) | `diagrams/use-case-diagram-full` |
| Activity Diagram — Review Submission & Analysis | `diagrams/review-analysis-activity-diagram` |
| Sequence — Customer Submits Review | `diagrams/sequence-diagrams/01-customer-submits-review` |
| Sequence — ML Model Sentiment Prediction | `diagrams/sequence-diagrams/02-ml-sentiment-prediction` |
| Sequence — Store Admin Views Dashboard | `diagrams/sequence-diagrams/03-admin-views-dashboard` |
| DFD Level 0 | `diagrams/dfd-level-0` |
| DFD Level 1 — Review Analysis Pipeline | `diagrams/dfd-level-1-review-analysis` |
| Domain Model / Class Diagram | `diagrams/domain-model-class-diagram` |
| Deployment Diagram | `diagrams/deployment-diagram` |
| Conceptual ERD (crow's-foot) | `diagrams/conceptual-erd` |
| System Architecture Diagram (API + ML.NET integration) | `diagrams/system-architecture-diagram` |

Each diagram will be available as `.png` (quick preview), `.svg` (vector, editable), and `.pdf` (print-ready) — same file name, different extension.

> *Diagrams are planned for Week 1 (Planning & Design phase) — none exported yet.*

---

## 🖥️ UI Dashboard

**Admin / Analytics Dashboard** — displays per-product sentiment stats (positive vs. negative %), trend over time, and a searchable reviews table.

> *Not built yet — scheduled for Week 5 (Frontend / Dashboard phase).*

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

| Member | Role | Core responsibilities |
|---|---|---|
| Member 1 | Backend / API Lead | ASP.NET Core Web API structure, Endpoints, database setup & integration |
| Member 2 | ML / Data Engineer | Training data prep, ML.NET sentiment model training & tuning |
| Member 3 | Integration Engineer | Wiring the ML.NET model into the API, error handling, performance testing |
| Member 4 | Frontend / Dashboard Developer | Admin dashboard UI showing sentiment stats |
| Member 5 | QA & Documentation Lead | Test cases, full test pass, final documentation & presentation |

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

| Layer | Technology |
|---|---|
| API | ASP.NET Core Web API |
| AI / ML | ML.NET — Sentiment Analysis (Binary Classification) |
| Database | SQL Server / SQLite |
| Docs & Testing | Swagger |
| Training data | Hugging Face sentiment datasets |
