# Diagram Index

All diagrams have source `.dot` plus generated `.svg`, `.png`, and `.pdf`. The DOT source is included so the team can edit the diagrams with Graphviz or translate them to another diagramming tool.

## Modeling Notes

- The architecture separates HTTP/API concerns from application logic, data access, and ML.NET integration.
- The prediction relationship is shown as one current result per review for the MVP, while the logical model documents an optional history/versioning approach.
- Authentication is modeled as an optional but recommended admin capability if the API is exposed beyond a trusted development environment.
