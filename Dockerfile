# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy everything (dockerignore will skip obj/bin/tests)
COPY . ./

# Restore & publish
RUN dotnet restore MiniShare.Web/MiniShare.Web.csproj
RUN dotnet publish MiniShare.Web/MiniShare.Web.csproj -c Release -o /app/out

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy published files
COPY --from=build /app/out .

# Expose ports
EXPOSE 80
EXPOSE 443

ENTRYPOINT ["dotnet", "MiniShare.Web.dll"]
