# User Stories & Acceptance Criteria

## US-01 Submit Review
As a customer, I want to submit a product review so that the system can analyze its sentiment.

**Acceptance Criteria**
- Given valid text, when the request is submitted, then the API accepts it.
- The review is stored.
- A prediction is generated or a controlled processing error is returned.

## US-02 View Review Result
As a client application, I want the review and sentiment result in a predictable JSON response.

## US-03 Admin Filter Reviews
As a store admin, I want to filter reviews by product and sentiment so that I can investigate trends.

## US-04 Admin View Statistics
As a store admin, I want aggregate sentiment counts so that I can monitor feedback at a glance.

## US-05 Handle Model Failure
As an operator, I want model failures to be logged and returned as controlled errors so that failures are diagnosable.
