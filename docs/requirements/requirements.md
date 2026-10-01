# Requirements

## Functional Requirements

| ID | Requirement | Priority |
|---|---|---|
| FR-01 | Submit review | Must |
| FR-02 | Validate review payload | Must |
| FR-03 | Analyze sentiment | Must |
| FR-04 | Store prediction | Must |
| FR-05 | Retrieve reviews | Must |
| FR-06 | Filter reviews | Should |
| FR-07 | Sentiment statistics | Should |
| FR-08 | Admin authentication | Should |
| FR-09 | Model version tracking | Could |
| FR-10 | Feedback/correction workflow | Could |

## Non-Functional Requirements

- API should expose consistent JSON contracts.
- Database schema must preserve referential integrity.
- ML model should be loaded efficiently and not retrained per request.
- Secrets must be externalized from source control.
- Logs must be useful without leaking review/customer-sensitive data.
