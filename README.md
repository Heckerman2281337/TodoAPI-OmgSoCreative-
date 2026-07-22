## TodoAPI
REST API for task management with JWT authentication

## Stack
- Runtime: ASP.NET Core 9
- Database: EF Core, PostgreSQL
- Containerization: Docker/Docker Compose
- Auth: JWT
- API Documentaion: Swagger UI

## Live Demo
https://todoapi-omgsocreative-production.up.railway.app/swagger

## How to run
1. git clone https://github.com/Heckerman2281337/TodoAPI-OmgSoCreative-.git
2. cd TodoAPI-OmgSoCreative-
### Choose your execution mode:
#### Mode A: Full Stack (For Frontend devs / Testing)
3. docker-compose up --build
4. Swagger: http://localhost:8080/swagger
#### Mode B: Hybrid Infrastructure (For Backend Development)
3. docker-compose up -d db
4. dotnet watch run
## API Endpoints

| Method | Endpoint | Authentication | Description |
|--------|----------|----------------|-------------|
| POST | `/User/register` | ❌ | Register a new user |
| POST | `/User/login` | ❌ | Login and receive JWT |
| GET | `/Task` | ✅ | Get all tasks |
| POST | `/Task` | ✅ | Create a new task |
| PATCH | `/Task/{id}` | ✅ | Update a task |
| DELETE | `/Task/{id}` | ✅ | Delete a task |

## Project Development
- [ ] **Frontend Integration:** Developing an SPA application (React/Next.js) for a full-fledged user interface.
- [ ] **Expanding Task Functionality:** Adding categories (tags), priorities, and deadlines for To-Do items.
- [ ] **Authorization Update:** Implementing Refresh Tokens for secure session updates.
- [ ] **CI/CD:** Setting up GitHub actions for deployment and deployment of the device to a remote server.
- [ ] **Test Coverage:** Adding Unit tests for business logic (xUnit + Moq).

- [x] **Docker Multi-stage Build:** Optimized Dockerfile.
- [x] **Automated Migrations:** Database automatically migrates on startup.

## Notes for me
- [x] Add AsNoTracking to GetAllTasks method
- [x] Host on railway
