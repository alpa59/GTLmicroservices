# Tech Context: GTL Marketplace Technology Ecosystem

## Technology Stack Overview

### Backend Microservices Framework
- **ASP.NET Core (.NET 8)**: Cross-platform, high-performance framework
- **Minimal APIs**: Streamlined for REST endpoint development
- **C# Language**: Pattern matching, records, nullable reference types
- **Dependency Injection**: Built-in container with service lifetime management

### Data Storage & Caching

#### Search Service (Read-Optimized)
- **Redis Database**: In-memory data structure store for search optimization
- **StackExchange.Redis**: High-performance C# Redis client
- **Search Commands**: FT.SEARCH for full-text queries, ZRANGE for pagination
- **Replication**: Redis Cluster for availability and read scaling

#### Relational Services (Write-Optimized)
- **PostgreSQL**: ACID-compliant database for transactional data
- **Entity Framework Core**: ORM with migrations and code-first development
- **Event Store Tables**: Append-only event sourcing implementation
- **Connection Pooling**: PgBouncer for efficient resource management

#### Message Bus
- **RabbitMQ**: Industry-standard message broker
- **MassTransit**: C# distributed application framework
- **Event-Driven Patterns**: Publish-subscribe, request-response, saga orchestration
- **Dead Letter Queues**: Handle failed message processing

### Infrastructure & DevOps

#### Containerization
- **Docker**: Container images for all microservices
- **Multi-Stage Builds**: Optimized production images
- **Docker Compose**: Local development environment orchestration
- **Kubernetes**: Production orchestration with service mesh

#### Development Tools
- **Visual Studio Code**: Primary IDE with C# extensions
- **.NET CLI**: Build, test, and publish commands
- **Git/GitHub**: Version control and pull request workflows
- **Postman**: API testing and documentation

#### CI/CD Pipeline
- **GitHub Actions**: Automated build, test, and deploy
- **Container Registry**: Azure Container Registry for image storage
- **Environment Promotion**: Dev → Staging → Production pipelines
- **Integration Testing**: Automated end-to-end service testing

### Libraries & Frameworks

#### CQRS & Event Sourcing
- **MediatR**: In-process messaging for command/query separation
- **MassTransit Saga**: Distributed saga state machine implementation
- **EventStore.ClientAPI**: Specialized event store operations

#### Resilience & Fault Tolerance
- **Polly**: Circuit breaker, retry, and bulkhead patterns
- **HealthChecks**: Microsoft.Extensions.Diagnostics.HealthChecks
- **Serilog**: Structured logging with sinks for Kibana/Seq

#### Testing & Quality
- **xUnit**: Unit testing framework
- **Moq**: Mocking library for dependency isolation
- **FluentAssertions**: Readable assertion syntax
- **Integration Test Host**: TestServer for API testing

#### Security
- **ASP.NET Core Identity**: User authentication and authorization
- **JWT Bearer Tokens**: Stateless authentication for service APIs
- **Azure Key Vault**: Secret management and certificate storage
- **Data Protection**: Encrypted cookies and tokens

## Development Environment Setup

### Local Development
```
GTLmicroservices/
├── memory-bank/          # Documentation
├── SearchService/        # .NET microservice projects
├── WarehouseService/
├── OrderService/
├── BookAddService/
├── docker-compose.yml    # Multi-service local setup
├── .github/workflows/    # CI/CD pipelines
└── tests/               # Integration tests
```

### Local Infrastructure (Docker Compose)
```yaml
services:
  redis:
    image: redis:7-alpine
  postgres:
    image: postgres:15-alpine
  rabbitmq:
    image: masstransit/rabbitmq:latest
  search-service:
    build: ./SearchService
  warehouse-service:
    build: ./WarehouseService
  # ... other services
```

## Technical Constraints & Performance Requirements

### Non-Functional Requirements

#### Performance Targets
- **Search Response Time**: < 1 second for any query (95th percentile)
- **Throughput Baseline**: 1000 requests/minute across all services
- **Peak Handling**: 10x+ load during semester starts (October, January)
- **API Availability**: 99.9% uptime SLA

#### Scalability Targets
- **Service Instances**: Horizontal scaling to 10+ instances per service
- **Data Growth**: Handle 100,000+ books, 10,000+ active sellers
- **Global Deployment**: Multi-region deployment within 5 years
- **Cost Efficiency**: Optimize cloud resource utilization

#### Security Requirements
- **Authentication**: User identity verification for all operations
- **Authorization**: Role-based access (Student, Graduate, Admin)
- **Data Privacy**: GDPR/CCPA compliance for EU expansion
- **Payment Security**: PCI compliance for transaction processing

### Development Constraints
- **Cross-Platform Compatibility**: Windows, macOS, Linux development support
- **Team Structure**: 10-person functional organization, possible restructuring
- **Skill Evolution**: .NET expertise evolution to microservices patterns
- **Maintenance**: Logically structured codebase for long-term maintainability

## Tool Usage Patterns & Workflows

### Development Workflow
1. **Feature Branch**: git checkout -b feature/search-redis-integration
2. **TDD Approach**: Red → Green → Refactor cycle
3. **Memory Bank Updates**: Document architectural decisions immediately
4. **Code Review**: Pull request process with automated CI checks
5. **Incremental Delivery**: Small, testable changes with feature flags

### Testing Strategy
- **Unit Tests**: 70%+ code coverage on business logic
- **Integration Tests**: Service endpoint validation
- **Contract Tests**: Event message compatibility verification
- **Performance Tests**: Load testing against performance targets
- **Chaos Testing**: Fault injection for resilience validation

### Quality Gates
- **Code Quality**: SonarQube analysis for code smells/security issues
- **Security Scanning**: Container image scanning and dependency vulnerability checks
- **Performance Regression**: Automated performance testing in CI pipeline
- **Documentation**: Updated memory-bank files for significant changes

## Integration Patterns

### Service Communication Matrix
| Service From/To | Search | Warehouse | Order | BookAdd | Messaging |
|----------------|--------|-----------|-------|---------|-----------|
| Search        | -      | REST      | -     | -       | Consumer  |
| Warehouse     | Events | -        | Events| Events  | Publisher |
| Order         | -      | Events    | -     | -       | Publisher |
| BookAdd       | Events | Events    | -     | -       | Publisher |

### Database Schema Distribution
- **Search DB (Redis)**: Denormalized read models, populated by events
- **Warehouse DB (PostgreSQL)**: Inventory tables, event sourcing tables
- **Order DB (PostgreSQL)**: Orders, saga states, shipping tracking
- **BookAdd DB (PostgreSQL)**: Listings, user profiles, seller verification

This technology ecosystem provides the foundation for building a scalable, reliable marketplace that can grow from campus pilot to global platform while maintaining development efficiency and operational excellence.
