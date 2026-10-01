# Risk Register

| Risk | Impact | Mitigation |
|---|---|---|
| Poor model accuracy | Incorrect sentiment classification | Evaluate on representative validation data; track metrics. |
| Dataset mismatch | Weak real-world performance | Check language/domain distribution and validate on e-commerce reviews. |
| Model file unavailable | Prediction outage | Validate model on startup and provide controlled failure handling. |
| Database outage | Review persistence failure | Health checks, logging, retry strategy where appropriate. |
| Sensitive data leakage | Privacy/security issue | Minimize stored personal data; protect logs and secrets. |
| Scope creep | Delivery delay | Maintain MVP/Phase 2 boundary. |
| License mismatch | Legal/compliance issue | Review dataset/model license before use and redistribution. |
