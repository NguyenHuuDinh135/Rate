---
name: rate-domain-booking
description: Work on Rate movie booking domain flows across movies, genres, persons, theaters, seats, shows, bookings, payments, and reviews. Use for tasks touching backend Domain entities/enums, Application feature folders for Movies/Theaters/Shows/Bookings/Payments/Genres/Persons, Web endpoints for those resources, related seed data, or frontend user flows for discovery and booking.
---

# Rate Domain Booking

## Orientation

Core business features are organized by resource:

- Domain: `Movie`, `Theater`, `TheaterSeat`, `Show`, `Booking`, `Payment`, `Review`, `Genre`, `Person`.
- Application: feature folders under `src/Application/<Resource>/Commands` and `Queries`.
- Web: `src/Web/Endpoints/*Endpoints.cs`.
- Data: EF configurations and JSON seed files under `src/Infrastructure/Data`.

## Domain Workflow

1. Start from the domain relationship: movie/genre/person metadata, theater seats, scheduled shows, bookings, then payments/reviews.
2. Keep state changes in commands and read models in queries.
3. Make endpoints thin; delegate behavior to MediatR requests.
4. Update seed data and functional tests when changing required fields or relationships.
5. Preserve authorization boundaries: public discovery endpoints may be anonymous, but create/update/delete and user booking operations should be explicit.

## Invariants To Check

- Booking logic should respect show, seat, user, and payment relationships.
- Show changes can affect booking availability and filtered movie/show queries.
- Payment changes can affect booking status and user-facing history.
- Movie/person/genre changes can affect search, AI recommendations, and seed data consistency.

## Verification

Run focused functional tests for the affected resource, then build the backend solution.
