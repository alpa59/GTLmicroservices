# System Patterns: GTL Marketplace Microservices Architecture

## Core Architectural Overview

### Event-Driven Microservices Architecture
The GTL marketplace uses domain decomposition with four core microservices communicating asynchronously through a service bus for eventual consistency:

### Core Microservices

#### 1. Search Service
- **Domain Responsibility**: Full-text search across all textbooks with live stock counts
- **Technology Pattern**: CQRS Read Model using Redis for search optimization
- **Performance Requirements**: Sub-1 second response, 1000+ req/min throughput
- **Interfaces**: REST API for queries, Event consumers for stock/optical updates

#### 2. Warehouse Service
- **Domain Responsibility**: Centralized inventory management across all sellers and GTL stock
- **Technology Pattern**: Event-Sourcing for inventory changes, PostgreSQL for persistence
- **Business Logic**: Track stock levels, manage shipments, coordinate inventory updates
- **Interfaces**: REST for admin operations, Event publishers for stock changes

#### 3. Order Service
- **Domain Responsibility**: Process multi-vendor purchases with coordinated fulfillment
- **Technology Pattern**: Saga Orchestration for distributed transaction management
- **Complex Logic**: Single order distributed across multiple sellers with shipping coordination
- **Interfaces**: REST for checkout, Events for fulfillment orchestration

#### 4. Book Add Service
- **Domain Responsibility**: Handle graduate sellers listing textbooks for sale
- **Technology Pattern**: Command/Event separation with async publishing
- **Business Rules**: Book validation, duplicate prevention, seller verification
- **Interfaces**: REST for listings, Events for system-wide notifications

## Distributed Systems Patterns

### 1. CQRS (Command Query Responsibility Segregation)
**Problem**: Optimize read performance while maintaining write model complexity
**Implementation**:
- **Command Side (Write Model)**: Warehouse/Order/BookAdd services handle business rules
- **Query Side (Read Model)**: SearchService uses denormalized Redis for fast lookups
- **Synchronization**: Events published by command services update query models
- **GTL Benefit**: Search performs at database-level speed regardless of catalog size

### 2. Saga Pattern - Distributed Transactions
**Problem**: Complex multi-vendor order fulfillment requires coordination across services
**Implementation**:
- **Order Saga Orchestration**:
  ```
  Step 1: OrderCreated → Reserve stock in Warehouse Service
  Step 2: PaymentConfirmed → Deduct stock, publish OrderFulfillmentRequired
  Step 3: SellerNotifications → Individual shipping events to each seller
  Compensation: OrderCancelled → Release reserved stock and notify sellers
  ```
- **Saga State**: Persisted in Order Service with timeout handling
- **MassTransit Support**: State machine library for reliable saga execution

### 3. Event Sourcing Pattern
**Problem**: Provide complete audit trail and ability to rebuild state from business events
**Implementation**:
- **Business Events as Ledger**: BookAdded, StockChanged, OrderPaid stored as append-only streams
- **State Reconstruction**: Services rebuild current state from event history
- **PostgreSQL Event Store**: Tables for event streams with global ordering
- **Audit Capabilities**: Complete business transaction history for compliance/troubleshooting

### 4. Circuit Breaker Pattern
**Problem**: Prevent cascading failures during service outages or network issues
**Implementation**:
- **Polly Library**: HTTP client with circuit breaker, retry, timeout policies
- **Service Bus Resilience**: Handle message broker disconnections gracefully
- **Fail-Fast Strategy**: Quick degradation rather than extended timeouts
- **Semester Peak Protection**: Automatic circuit breaking during load spikes

### 5. API Gateway Pattern (Future Implementation)
**Problem**: Provide single entry point for external clients and admin interfaces
**Implementation**:
- **Rate Limiting**: Protect against request flooding
- **Authentication**: JWT token validation and user context passing
- **Service Discovery**: Dynamic routing to microservices
- **Cross-Cutting**: Logging, metrics, request tracing

## Communication Patterns

### Async Messaging (Primary Inter-Service)
- **Service Bus**: RabbitMQ/MassTransit for reliable event delivery
- **Event Contracts**: Strictly typed messages (BookEvents, OrderEvents, InventoryEvents)
- **Delivery Guarantees**: At-least-once delivery with idempotent processing
- **Routing**: Fan-out patterns for broadcast notifications (stock changes to search)

### REST APIs (External Communications)
- **Client Interfaces**: Standard REST endpoints with JSON payloads
- **Admin Interfaces**: Warehouse management, seller dashboards
- **API Versioning**: URL versioning strategy for backward compatibility
- **Swagger Documentation**: Auto-generated API exploration

## Data Consistency Patterns

### Eventual Consistency
- **Definition**: System accepts temporary inconsistency for availability/performance
- **Acceptance Criteria**: Search may lag up to 30 seconds during peak loads
- **User Communication**: Transparent messaging about fulfillment times
- **Business Trade-Off**: Prefer commerce flow over 100% consistency

### Strong Consistency (Selective)
- **Payment Processing**: Use database transactions for order payment validation
- **Inventory Reservation**: ACID operations for stock holding during order window
- **Fulfillment Coordination**: Orchestrated consistency across shipping notifications

## Fault Tolerance Patterns

### Bulkhead Pattern
- **Service Isolation**: Resource limits per service to prevent cascade failures
- **Database Connection Pooling**: Separate pools per service
- **Thread Pools**: Async operation isolation to prevent resource exhaustion

### Health Monitoring Pattern
- **Health Checks**: /health endpoints returning service liveness/readiness
- **Metrics Collection**: Performance counters, error rates, throughput
- **Distributed Tracing**: Request correlation across service boundaries
- **Automated Actions**: Load balancer removal of unhealthy instances

## Scalability Patterns

### Horizontal Scaling
- **Stateless Services**: All services designed for instance multiplication
- **Data Partitioning**: Event streams partitioned by aggregate IDs
- **Read Scaling**: Multiple Redis replicas for search load distribution
- **Service Bus Scaling**: Message broker clustering for event throughput

### Load Distribution
- **Rate Limiting**: Token bucket algorithms at API gateway
- **Queue Buffering**: Async event processing handles traffic spikes
- **Auto-Scaling**: Kubernetes HPA based on CPU/memory or custom metrics
- **Geographic Distribution**: CDN for static content, regional service deployments

## Pattern Benefits for GTL Marketplace

### Technical Advantages
- **Search Performance**: CQRS enables sub-second queries at scale
- **Fault Tolerance**: Circuit breakers maintain platform availability
- **Scalability**: Async patterns handle semester peak loads
- **Debuggability**: Event sourcing provides complete transaction history

### Business Advantages
- **Marketplace Reliability**: Saga orchestration ensures order fulfillment
- **Operational Visibility**: Event streams enable real-time analytics
- **Future Evolution**: Patterns support adding new features/verticals
- **Competitive Edge**: Architecture scales globally like Netflix/Airbnb vision

This pattern-centric approach ensures the GTL marketplace scales from initial campus launch to worldwide operation while maintaining reliability and performance.
