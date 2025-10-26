# AuditLog Feature Documentation

## Overview
The AuditLog feature provides comprehensive audit logging capabilities for tracking user actions, system events, and data changes throughout the application.

## Architecture
The implementation follows Clean Architecture and CQRS patterns:
- **Domain Layer**: `AuditLog` entity with fields for tracking actions
- **Application Layer**: Queries, Commands, and Handlers for business logic
- **Infrastructure Layer**: Repository implementation with LLBLGen Pro ORM
- **API Layer**: RESTful controller with comprehensive endpoints

## Features Implemented

### Repository (IAuditLogRepository)
The repository provides the following capabilities:
- Create, Read, Delete operations for audit logs
- Query by user, entity, action, and date range
- Pagination and sorting support
- Statistics and analytics methods
- Caching for improved performance

### API Endpoints

#### Query Endpoints

1. **Get Audit Log by ID**
   - `GET /api/auditlog/{auditId}`
   - Returns a single audit log entry

2. **Get Paged Audit Logs**
   - `GET /api/auditlog/paged?page=1&pageSize=20`
   - Returns paginated audit logs with filtering and sorting

3. **Get Audit Logs by User**
   - `GET /api/auditlog/user/{userId}?page=1&pageSize=20`
   - Returns audit logs for a specific user

4. **Get Audit Logs by Entity**
   - `GET /api/auditlog/entity/{entity}?entityId={guid}&page=1&pageSize=20`
   - Returns audit logs for a specific entity type and optional entity ID

5. **Get Audit Logs by Action**
   - `GET /api/auditlog/action/{action}?page=1&pageSize=20`
   - Returns audit logs filtered by action type

6. **Get Audit Logs by Date Range**
   - `GET /api/auditlog/daterange?fromDate={date}&toDate={date}&page=1&pageSize=20`
   - Returns audit logs within a date range

7. **Get Audit Logs by User and Date Range**
   - `GET /api/auditlog/user/{userId}/daterange?fromDate={date}&toDate={date}&page=1&pageSize=20`
   - Returns audit logs for a user within a date range

8. **Get Recent Audit Logs**
   - `GET /api/auditlog/recent?count=100`
   - Returns the most recent audit logs

9. **Get Count by User**
   - `GET /api/auditlog/count/user/{userId}`
   - Returns the count of audit logs for a specific user

10. **Get Count by Action**
    - `GET /api/auditlog/count/action/{action}`
    - Returns the count of audit logs for a specific action

11. **Get Action Statistics**
    - `GET /api/auditlog/statistics?fromDate={date}&toDate={date}`
    - Returns statistics grouped by action type

#### Command Endpoints

1. **Create Audit Log**
   - `POST /api/auditlog`
   - Body: `{ "userId": "guid", "action": "string", "entity": "string", "entityId": "guid", "detail": "json" }`

2. **Delete Audit Log**
   - `DELETE /api/auditlog/{auditId}`
   - Deletes a specific audit log entry

## Data Model

### AuditLog Entity
```csharp
public class AuditLog
{
    public Guid AuditId { get; set; }
    public Guid? UserId { get; set; }
    public string Action { get; set; }  // Required, max 200 chars
    public string? Entity { get; set; }  // Optional, max 100 chars
    public Guid? EntityId { get; set; }
    public string? Detail { get; set; }  // JSON format
    public DateTime CreatedAt { get; set; }
    public User? User { get; set; }  // Navigation property
}
```

### AuditLogDto
```csharp
public class AuditLogDto
{
    public Guid AuditId { get; set; }
    public Guid? UserId { get; set; }
    public string? UserName { get; set; }
    public string Action { get; set; }
    public string? Entity { get; set; }
    public Guid? EntityId { get; set; }
    public string? Detail { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

## Usage Examples

### Creating an Audit Log
```csharp
var command = new CreateAuditLogCommand(
    UserId: userId,
    Action: "CREATE",
    Entity: "Product",
    EntityId: productId,
    Detail: JsonSerializer.Serialize(new { Name = "New Product", Price = 99.99 })
);
var result = await mediator.Send(command);
```

### Querying Audit Logs
```csharp
// Get recent logs
var query = new GetRecentAuditLogsQuery(
    Request: new PagedRequest { Page = 1, PageSize = 50 },
    Count: 100
);
var result = await mediator.Send(query);

// Get logs by user
var userQuery = new GetAuditLogsByUserIdQuery(
    Request: new PagedRequest { Page = 1, PageSize = 20 },
    UserId: userId
);
var userResult = await mediator.Send(userQuery);

// Get statistics
var statsQuery = new GetActionStatisticsQuery(
    FromDate: DateTime.UtcNow.AddDays(-30),
    ToDate: DateTime.UtcNow
);
var stats = await mediator.Send(statsQuery);
```

## Features

### Pagination
All list endpoints support pagination with the following parameters:
- `page`: Page number (default: 1)
- `pageSize`: Items per page (default: 20, max: 100)

### Filtering
The paged endpoint supports advanced filtering:
- Filter by any field using operators (equals, contains, greater than, etc.)
- Combine multiple filters
- Range filters for dates

### Sorting
Sortable fields:
- `Action`
- `Entity`
- `CreatedAt` (default, descending)
- `UserId`

### Caching
The repository implements caching for:
- Individual audit log lookups (1 hour TTL)
- Paged results (15 minutes TTL)
- Automatic cache invalidation on create/delete

### Search
Full-text search across:
- Action
- Entity
- Detail

## Implementation Notes

1. **Transaction Support**: The repository respects the Unit of Work pattern and will use the transaction adapter when available.

2. **Performance**: Queries are optimized with:
   - Database indices on frequently queried fields
   - Caching layer for read operations
   - Efficient pagination

3. **Security**: Consider adding authorization attributes to the controller endpoints based on your security requirements.

4. **Retention**: The Delete operation is available but consider implementing a scheduled job for automatic cleanup of old audit logs based on your retention policy.

## Dependencies
- MediatR for CQRS pattern
- AutoMapper for object mapping
- LLBLGen Pro for ORM
- ASP.NET Core for Web API

## Files Created
- **Queries**: 11 query classes in `GroceryEcommerce.Application/Features/AuditLog/Queries/`
- **Commands**: 2 command classes in `GroceryEcommerce.Application/Features/AuditLog/Commands/`
- **Handlers**: 13 handler classes in `GroceryEcommerce.Application/Features/AuditLog/Handlers/`
- **Controller**: `GroceryEcommerce.API/Controllers/AuditLogController.cs`
- **DI Registration**: Updated `GroceryEcommerce.Infrastructure/DependencyInjection.cs`
