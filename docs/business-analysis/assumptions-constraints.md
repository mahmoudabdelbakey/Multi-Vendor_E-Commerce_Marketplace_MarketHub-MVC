# Assumptions & Constraints

## Assumptions
- Reviews are primarily text-based.
- The first model targets the sentiment labels supported by the selected training dataset/model.
- The API is the primary integration boundary.
- SQL Server is the initial relational database.

## Constraints
- Model quality depends on dataset quality and domain fit.
- ML.NET API details depend on the chosen ML.NET version.
- External dataset terms must be respected.
- The system should not make business decisions solely from an automated sentiment label without appropriate human/business rules.
