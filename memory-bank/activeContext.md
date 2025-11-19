# Active Context: GTL Marketplace Development Focus

## Current Work Focus

### Primary Emphasis: Search Service Implementation & Memory Bank Inauguration
The immediate development focus is establishing the Search Service as the first functional microservice while simultaneously creating the memory bank documentation foundation that will guide the entire project.

### Search Service MVP Development
- **Objective**: Deliver operational book search with REST API and basic Redis integration
- **Scope**: Start with title substring matching, expand to full-text search later
- **Requirements**: Sub-1 second responses, display stock counts from warehouse events
- **Quality Gate**: Integrated with Warehouse mock events for testing

## Recent Changes & Decisions

### Memory Bank Creation (Completed)
- **Delivered**: Complete documentation structure with 6 core files
- **Achievement**: Established project foundation and architectural patterns
- **Immediate Value**: Team alignment on event-driven microservices approach

### Project Structure Evolution
- **Clean Start**: Removed previous incomplete implementations
- **Salvation Choices**: Settled on ASP.NET Core .NET 8, Docker, Redis/RabbitMQ/MassTransit
- **Pattern Commitment**: Explicit adoption of CQRS, Saga, Event Sourcing patterns

### Architectural Decisions Made
- **CQRS Implementation**: Search service as dedicated read model
- **Saga Coordination**: Order service orchestrates multi-vendor fulfillment
- **Eventual Consistency**: Acceptable lag for performance optimization
- **Service Boundaries**: Clear domain separation (Search/Write, Warehouse/Order/BookAdd)

## Next Steps (Short Term: 1-2 Weeks)

### Phase 1: Search Service Core (Immediate Priority)
1. **Data Models**: Define BookSearchResult, SearchRequest DTOs
2. **Redis Integration**: StackExchange.Redis client with connection pooling
3. **Search Logic**: Implement title.Contains() matching with pagination
4. **Event Consumption**: Mock Warehouse event handlers for stock updates
5. **Performance Testing**: Load testing against 1000 req/min target
6. **Docker Containerization**: Multi-stage build with health checks

### Phase 2: Cross-Service Infrastructure (Next Week)
7. **Message Bus Setup**: RabbitMQ/MassTransit configuration for all services
8. **Event Contracts**: Strongly-typed messages (BookAdded, StockChanged, etc.)
9. **Service Templates**: ASP.NET Core project scaffolding for Warehouse/Order/BookAdd
10. **Docker Compose**: Multi-service development environment
11. **Integration Testing**: Event flow validation between services

## Active Decisions & Current Considerations

### CQRS Read Model Strategy
- **Decision**: Redis Hash/SortedSet for searchable book data
- **Event Handler Structure**: Consume StockChanged events to update book:{isbn} hashes
- **Denormalization**: Store complete book data (title, stock, seller info) in Redis
- **Trade-Off**: Memory usage vs. query performance (acceptable for this domain)

### Saga Orchestration Complexity
- **Decision**: Order Service maintains saga state for fulfillment coordination
- **State Management**: Persist saga progress in database with timeout handling
- **Compensation Logic**: OrderCancel events trigger stock release notifications
- **MassTransit Selection**: Chosen for mature C# distributed patterns support

### Eventual Consistency Acceptance
- **User Impact**: Search results may lag 1-5 seconds during peak periods
- **Communication**: "Results may be updating..." messaging in UI
- **Business Justification**: Commerce continuity preferred over perfect consistency
- **Monitoring Needs**: Event processing lag metrics and alerting

## Important Patterns & Standards

### Code Architecture Patterns
- **Minimal APIs**: Preferred over MVC Controllers for service endpoints
- **Result Pattern**: Use Results<T> instead of exceptions for domain operations
- **Dependency Injection**: Strict interface segregation for testability
- **Async Everywhere**: TAP pattern for non-blocking operations

### Event Design Standards
- **Immutable Events**: All event properties read-only, timestamp/version included
- **Strong Typing**: Message contracts in shared assembly
- **Global Ordering**: Event sequence numbers for potential reconstruction
- **Idempotent Handling**: Event handlers tolerate duplicate processing

### Development Standards
- **Memory Bank Maintenance**: Update documentation for all architectural changes
- **TDD Approach**: Red-Green-Refactor cycle with high test coverage
- **Feature Flags**: Gradual rollout of new capabilities
- **Semantic Versioning**: API versioning strategy for consumer compatibility

### Testing Strategy
- **Unit Tests**: Business logic isolation with Moq/xUnit
- **Integration Tests**: Event flow validation between services
- **Contract Tests**: Message compatibility verification
- **Performance Tests**: JMeter/K6 for load simulation

## Current Questions & Destination Open Items

### Technical Clarifications Needed
- **Redis Data Structure**: Hash vs. JSON for book storage?
- **Pagination Strategy**: Cursor-based vs. offset for search results?
- **Event Replay**: Need for complete state reconstruction during deployments?

### Business Clarifications Pending
- **Stock Display**: Show individual seller stock or aggregate availability?
- **Seller Notifications**: Push notifications vs. email/dashboard for orders?
- **Authentication**: User roles (Student/Graduate/Admin) and UI experience?

### Risk Assessments
- **Saga Complexity**: Multi-step orchestration may require specialized testing
- **Eventual Consistency**: User acceptance of temporary data discrepancies
- **Performance Targets**: Realistic Redis throughput for 1000 req/min search loads

## Learnings from Memory Bank Process

### Early Documentation Impact
- **Architectural Clarity**: Writing patterns forced systematic thinking
- **Team Alignment**: All services designed consistently with event patterns
- **Future Planning**: Saga orchestration insight prevents future integration issues

### .NET Microservices Maturity
- **Framework Support**: ASP.NET Core provides excellent building blocks
- **Ecosystem Richness**: MediatR, Polly, MassTransit provide distributed patterns
- **Hosting Options**: Docker/Kubernetes ready for scaling requirements

### Pattern Complexity Recognition
- **CQRS Benefits**: Distinct read/write concerns enable performance optimization
- **Event Sourcing Overhead**: Append-only logs for audit trail worth the complexity
- **Saga Reliability**: Distributed transaction coordination requires careful design

The active context establishes current development priorities while maintaining awareness of broader architectural implications. Next focus remains on Search Service delivery with confidence in the established patterns foundation.
