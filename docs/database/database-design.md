# Database Design

## Candidate Core Entities

- Product
- Review
- SentimentPrediction
- AdminUser (if authentication is implemented)
- ModelVersion (recommended when model versioning is required)

## Relationship Summary

- Product 1 — N Review
- Review 1 — 1 or N SentimentPrediction depending on whether re-analysis/history is required.
- ModelVersion 1 — N SentimentPrediction when model versioning is enabled.

## Design Decision

For a simple MVP, one current prediction per review can be enough. If retraining/re-analysis is part of the roadmap, use a prediction-history model so old predictions are not overwritten.
