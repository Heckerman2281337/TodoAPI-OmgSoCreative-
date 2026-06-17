## TodoAPI
REST API for task management with JWT authentication

## Stack
Runtime: ASP.NET Core 9
Database: EF Core, PostgreSQL
Containerization: Docker
Auth: JWT
API Documentaion: Swagger UI

## How to run
1. git clone https://github.com/Heckerman2281337/TodoAPI-OmgSoCreative-.git
2. docker-compose up --build
3. Swagger: http://localhost:8080/swagger
 
## List of endpoints
- POST /User/register - user registration
- POST /User/login - user login, return JWT
- Endpoint below requires JWT:
- GET /Task - returns all task for user
- POST /Task - create task
- PATCH /Task/{id} - update task by its id
- DELETE /Task/{id} - delete task by its id

## Project Development
- [ ] **Frontend Integration:** Developing an SPA application (React/Next.js) for a full-fledged user interface.
- [ ] **Expanding Task Functionality:** Adding categories (tags), priorities, and deadlines for To-Do items.
- [ ] **Authorization Update:** Implementing Refresh Tokens for secure session updates.
- [ ] **CI/CD:** Setting up GitHub actions for deployment and deployment of the device to a remote server.
- [ ] **Test Coverage:** Adding Unit tests for business logic (xUnit + Moq).
