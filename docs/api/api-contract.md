# API Contract Draft

> Example contract for analysis; finalize names and fields with the implementation.

| Method | Endpoint | Purpose |
|---|---|---|
| POST | `/api/reviews` | Create and analyze a review |
| GET | `/api/reviews/{id}` | Get one review and prediction |
| GET | `/api/reviews` | List/filter reviews |
| GET | `/api/products/{productId}/sentiment-summary` | Product sentiment summary |
| GET | `/api/admin/dashboard/summary` | Admin aggregate summary |
| GET | `/api/health` | Health/status endpoint |

## Example Request

```json
{
  "productId": 42,
  "reviewText": "The product quality is excellent and delivery was fast."
}
```

## Example Response

```json
{
  "reviewId": 123,
  "sentiment": "Positive",
  "score": 0.94,
  "modelVersion": "v1"
}
```

The exact prediction fields must match the selected ML.NET model output rather than being invented by the API layer.
