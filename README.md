# Insurance API

## Overview

This project implements a hotel booking insurance system following DDD, CQRS, and clean architecture principles.  
It receives booking notifications from hotel systems, manages insurance policies, and exposes reporting endpoints.

## Architecture

- **API Layer:** Receives HTTP requests from hotel systems and reporting clients.
- **Application Layer:** Handles business use cases using MediatR (commands/queries).
- **Domain Layer:** Contains business entities (`Booking`, `InsuredBooking`) and business rules.
- **Infrastructure Layer:** Integrates with PostgreSQL (EF Core), RabbitMQ (MassTransit Outbox), and external insurance APIs.

![Architecture Diagram](Architecture%20Diagram.png)

## Main Features

- Receives booking confirmations, modifications, and cancellations via a single HTTP endpoint.
- Applies business rules and calculates insurance premiums.
- Uses Outbox Pattern with RabbitMQ for reliable integration with external insurance APIs.
- Exposes reporting endpoints for insured and cancelled bookings.

## How to Run

1. Build and run with Docker Compose:

2. The API will be available at `http://localhost:7097`.

## User Stories Implemented

- **US1:** Insure confirmed bookings.
- **US2:** Reinsure bookings on modification.
- **US3:** List all insured bookings with premium amount.
- **US4:** List all cancelled insured bookings.

## Assumptions & Design Decisions

- The insurance API endpoint is not yet available. The Outbox Pattern is used to persist integration messages until the endpoint is ready.
- RabbitMQ is used as the message broker, assuming the external endpoint will be implemented soon. If not, a database-persisted queue would be preferable to avoid broker overload.
- The system is designed as a modular monolith for simplicity, but can be splitinto microservices if needed.
- No authentication ordefensive programming is implemented, as requested in the challengestatement.
- The API expects all fields in the request body to be present and valid; otherwise, a domain exception is thrown.

## Pending Work & Next Steps

- **Observability:** Add structured logging for better monitoring and diagnostics.
- **Request Validation:** Implement input validation for all incoming HTTP requests.
- **Domain Events:** Add domain events for audit logging and integration with other modules.
- **Integration Tests:** Implement integration and functional tests for command handlers and API endpoints.
- **External API Integration:** Complete theconsumer logic to call the real insurance API when available.

## How to Test

- Use the provided `.http` file or Postman to send requests to:
- `POST /api/bookings/notifications` (actions: confirmation, modification, cancellation)
- `GET /api/invoices/insured-bookings`
- `GET /api/invoices/cancelled-bookings`
- Example request for confirmation:
- { "reference": "A83K1C", "action": "confirmation", "checkin": "2022-09-13", "checkout": "2022-09-17", "people": 3 }