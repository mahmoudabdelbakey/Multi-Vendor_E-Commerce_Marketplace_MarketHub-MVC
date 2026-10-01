# Business Analysis — Smart Sentiment Analysis API for E-Commerce Reviews

## 1. Executive Summary

The system is a backend platform that accepts e-commerce product reviews and determines their sentiment using a machine-learning model implemented with ML.NET. It stores review and prediction data and exposes REST APIs for customer-facing applications and store administration.

The primary business value is converting unstructured review text into structured sentiment information that can support product monitoring, customer-experience analysis, and administrative dashboards.

## 2. Problem Statement

E-commerce stores may receive large volumes of written reviews. Manually reading every review is slow and inconsistent. The proposed system automates an initial sentiment classification so a store can quickly identify positive and negative feedback and investigate product-level trends.

## 3. Objectives

- Accept and validate customer reviews.
- Persist review text and relevant metadata.
- Predict sentiment using ML.NET.
- Return prediction results through REST APIs.
- Allow administrators to query and filter reviews.
- Provide aggregated sentiment statistics.
- Keep the design modular so the model can be retrained or replaced.
- Produce auditable and testable business flows.

## 4. Stakeholders

| Stakeholder | Interest |
|---|---|
| Customer | Submit and view review-related information |
| Store Admin | Monitor reviews and sentiment insights |
| Product/Business Owner | Understand customer feedback trends |
| Developer Team | Build, test, deploy, and maintain the system |
| ML/AI Contributor | Prepare, evaluate, and improve the sentiment model |

## 5. Actors

### Customer
Can submit a review, optionally associate it with a product/order, and receive a processing result.

### Store Admin
Can authenticate, browse/filter reviews, inspect predictions, and view aggregate sentiment analytics.

### ML.NET Model
A system component rather than a human actor. It receives prepared text/features and returns a prediction and confidence/probability information supported by the selected model.

### External Dataset Source
Used during model-development/training activities. It is not necessarily called at runtime.

## 6. Scope

### MVP
- Review submission
- Input validation
- Product association
- Sentiment prediction
- Review persistence
- Prediction persistence
- Review retrieval/filtering
- Admin summary statistics
- Swagger/OpenAPI
- Structured error handling and logging

### Possible Phase 2
- Authentication and role-based authorization
- Product-level trend analytics
- Pagination and advanced filtering
- Model version tracking
- Re-training workflow
- Feedback loop for incorrect predictions
- Multi-language models
- Dashboard frontend

### Out of Scope Unless Explicitly Approved
- Automated moderation decisions
- Fully autonomous customer support
- Financial/refund decisions
- Production model training directly from unverified user input

## 7. Functional Requirements

FR-01 The system shall accept a review submission.
FR-02 The system shall validate required fields and length limits.
FR-03 The system shall associate a review with a product when product context is provided.
FR-04 The system shall invoke the sentiment prediction service for eligible reviews.
FR-05 The system shall persist the review and prediction result.
FR-06 The system shall expose review retrieval endpoints.
FR-07 The system shall support filtering by sentiment and product.
FR-08 The system shall expose aggregate sentiment statistics.
FR-09 Authorized administrators shall be able to inspect review details.
FR-10 The system shall return meaningful HTTP status codes and error responses.
FR-11 The system shall log operational failures without exposing sensitive information.
FR-12 The system shall allow the prediction implementation to be changed without rewriting API contracts.

## 8. Non-Functional Requirements

- Maintainability: separation of API, business logic, data access, and ML integration.
- Performance: avoid unnecessary database queries and model reloads per request.
- Reliability: predictable error handling and transactional persistence where appropriate.
- Security: validate input, protect credentials/secrets, and enforce authorization for admin operations.
- Observability: structured logs and useful diagnostics.
- Testability: business logic and prediction integration should be independently testable.
- Scalability: keep stateless API behavior where practical.

## 9. Main Business Flow

1. Customer submits review.
2. API validates request.
3. System checks product context if supplied.
4. Review is persisted or prepared for persistence according to the selected transaction strategy.
5. Prediction service loads/uses the ML.NET model.
6. Model returns sentiment and supported confidence/probability values.
7. Prediction is persisted with model/version metadata if implemented.
8. API returns a standardized response.
9. Admin can later query the review and aggregate sentiment data.

## 10. Key Business Rules

- A review cannot be processed when required review text is missing or invalid.
- A prediction must be traceable to the review that produced it.
- A failed prediction must not silently appear as a successful sentiment result.
- Admin-only analytics endpoints must not be publicly writable.
- Model output is an automated classification and should not be presented as human certainty.
- Dataset licensing and provenance must be checked before using external training data.

## 11. Key Edge Cases

- Empty review text.
- Review exceeds maximum length.
- Unknown product ID.
- Duplicate submission.
- ML model unavailable.
- Invalid/corrupt model file.
- Prediction service timeout/failure.
- Database unavailable.
- Unsupported language or text outside the training distribution.
- Prediction confidence/probability is low or not exposed by the selected model.

## 12. Success Criteria

The project is ready for MVP review when a valid review can be submitted through the API, validated, analyzed with the configured ML.NET model, stored with its prediction, retrieved later, and represented in admin-level aggregate queries, with automated tests covering the principal success and failure paths.
