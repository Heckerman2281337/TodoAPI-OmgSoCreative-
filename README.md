## TodoAPI
Pet project that I use to practice ASP.NET dev. Project started as simple CRUD API. Project evolving with authentication, testing, Docker, etc.

## Stack
### Backend
- C#
- .NET 9
- ASP.NET core Web API
- Ef core
### DB
- PostgreSQL
### Auth
- JWT (refresh + access)
- Bcrpyt
### Testing
- xUnit
- Moq
### Logging
- Serilog
### Infrastructure
- Docker/Docker Compose
### Documentation
- Swagger UI

## Live Demo
https://todoapi-omgsocreative-production.up.railway.app/swagger

## How to run
1. git clone https://github.com/Heckerman2281337/TodoAPI-OmgSoCreative-.git
2. cd TodoAPI-OmgSoCreative-
#### Option 1: Run everything with docker
3. docker-compose up --build
4. Swagger: http://localhost:8080/swagger
#### Option 2: Run API locally
3. Start only postgres
4. docker-compose up -d db
5. dotnet watch run
## API Endpoints

| Method | Endpoint | Authentication | Description |
|--------|----------|----------------|-------------|
| POST | `/User/register` | ❌ | Register a new user |
| POST | `/User/login` | ❌ | Login and receive JWT |
| POST | `/User/refresh` | ❌ | Refresh token |
| POST | `/User/logout` | ✅ | Logout from account |
| GET | `/Task` | ✅ | Get all tasks |
| POST | `/Task` | ✅ | Create a new task |
| PATCH | `/Task/{id}` | ✅ | Update a task |
| DELETE | `/Task/{id}` | ✅ | Delete a task |
