# AI Generated API

## Architecture

### ChatGPT Prompt:

  > You are a .NET expert.
  >
  > Give me the overall architecture document (not the code) for an API in the latest version of .NET Core, it should: 
  > 
  > - Handle user Authentication and Authorization.
  > - Authenticated users can create, edit and delete events.
  >   - It should just allow users to edit or delete events if you created them
  > - Events have title, description, date, location and image. 
  > - Authenticated user can register and unregister for events

### Output (slightly modified by hand):

- Platform: .NET 8 (ASP.NET Core Web API)
- Authentication: JWT Bearer Tokens
- Authorization: Role-based and Ownership-based Access Control
- Database: Relational (Sqlite)
- Image Storage: Local file system or Cloud storage (e.g., Azure Blob Storage, Amazon S3)

#### 1. System Overview

- The Event Management API will allow users to:
  - Authenticate via a secure mechanism (JWT).
  - Perform CRUD operations on events they own.
- Register and unregister for events.
  - Enforce that only event owners can modify or delete their own events.

#### 2. High-Level Architecture

```
[ASP.NET Core Web API] --> [Authentication Server / JWT Provider]
                      |--> [SQL Database]
                      |--> [File Storage Service]
```

#### 3. Modules and Components

##### 3.1 Authentication & Authorization

- Authentication:
  - Use ASP.NET Core Identity with JWT Bearer tokens.
- Authorization:
  - Role-based: e.g. "User", "Admin" (if needed for scalability)
  - Ownership-based: Users can only edit or delete events they have created.
  - JWT Token Handling:
    - Tokens will be issued upon login and validated on every request using middleware.

##### 3.2 Event Management

- Event Entity:
  - Id (Guid)
  - Title
  - Description
  - Date
  - Location
  - ImageUrl
  - OwnerId (Foreign Key to User)
- Endpoints:
  - `POST /api/events - Create Event (Authenticated)`
  - `GET /api/events - List All Events (Public)`
  - `GET /api/events/{id} - Get Event Details (Public)`
  - `PUT /api/events/{id} - Update Event (Owner only)`
  - `DELETE /api/events/{id} - Delete Event (Owner only)`

##### 3.3 Event Registration

- EventRegistration Entity:
  - Id (Guid)
  - EventId (Foreign Key)
  - UserId (Foreign Key)
- Endpoints:
  - `POST /api/events/{id}/register - Register for Event (Authenticated)`
  - `DELETE /api/events/{id}/register - Unregister from Event (Authenticated)`

##### 3.4 User Management

- User Entity:
  - Extend ASP.NET Core Identity User (includes Id, Email, PasswordHash, etc.)
- Endpoints:
  - `POST /api/auth/register - Register New User`
  - `POST /api/auth/login - Authenticate User & Return JWT`

##### 3.5 Image Uploading

- Endpoints:
  - `POST /api/events/{id}/image - Upload or update image for event (Owner only)`
- Storage Options:
  - Store image locally on server with a URL reference in the Event entity.
  - (Optional) Integrate cloud storage (Azure, AWS, etc.) for scalability.

#### 4. Security Considerations

- Passwords securely hashed using ASP.NET Core Identity.
  - JWT tokens with short expiry + refresh tokens if scalability needed.
- Endpoint protection using [Authorize] attributes.
  - Custom authorization handler to verify event ownership.
  - Input validation to prevent SQL injection and XSS.

#### 5. Error Handling and Validation

- Centralized exception handling using ASP.NET Core’s middleware.
  - Return appropriate HTTP status codes:
    - `401` `Unauthorized` for unauthenticated requests.
    - `403` `Forbidden` for unauthorized (non-owner) access.
    - `400` `Bad Request` for validation errors.
    - `404` `Not Found` for non-existing resources.
  - Model validation using Data Annotations and FluentValidation (optional).

#### 6. Technology Stack

- Component	Technology
  - **Backend Framework:** ASP.NET Core 8 Web API
  - **Authentication:** ASP.NET Core Identity + JWT
  - **Database:** Sqlite
  - **ORM:** Entity Framework Core
  - **Image Storage:** Local/Cloud
  - **API Documentation:** Swagger (Swashbuckle)
  - **Validation:** Data Annotations / FluentValidation
  - **Dependency Injection:** Built-in ASP.NET Core DI
  - **Logging:** Serilog (optional)

#### 7. Deployment

- Containerized using Docker.
- CI/CD via GitHub Actions.
- Host on Azure App Service.

#### 8. Potential Extensions

- Pagination and filtering on event listing.
- Role-based admin users to manage all events.
- Email notifications for event registration.
- User profile management.
- Event categories/tags.