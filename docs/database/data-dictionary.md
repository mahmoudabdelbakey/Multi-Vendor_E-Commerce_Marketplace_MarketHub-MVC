# Data Dictionary

| Entity | Field | Purpose |
|---|---|---|
| Product | ProductId | Primary key |
| Product | Name | Product display name |
| Review | ReviewId | Primary key |
| Review | ProductId | Product relationship |
| Review | ReviewText | Original review text |
| Review | CreatedAt | Submission timestamp |
| SentimentPrediction | PredictionId | Prediction record key |
| SentimentPrediction | ReviewId | Related review |
| SentimentPrediction | Label | Predicted sentiment label |
| SentimentPrediction | Score/Probability | Model output when available |
| SentimentPrediction | ModelVersionId | Model lineage when enabled |
| SentimentPrediction | CreatedAt | Prediction timestamp |
