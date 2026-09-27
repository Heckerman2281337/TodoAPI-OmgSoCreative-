FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env
WORKDIR /app

COPY TodoAPI.WebAPI/*.csproj TodoAPI.WebAPI/
COPY TodoAPI.Application/*.csproj TodoAPI.Application/
COPY TodoAPI.Domain/*.csproj TodoAPI.Domain/
COPY TodoAPI.Infrastructure/*.csproj TodoAPI.Infrastructure/

RUN dotnet restore TodoAPI.WebAPI/TodoAPI.WebAPI.csproj

COPY . ./
RUN dotnet publish TodoAPI.WebAPI/TodoAPI.WebAPI.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:9.0

RUN apt-get update && \
    apt-get install -y libgssapi-krb5-2 && \
    rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY --from=build-env /app/out .

EXPOSE 8080
ENTRYPOINT ["dotnet", "TodoAPI.WebAPI.dll"]