# Base stage for runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
#ARG BASE_PATH=back-end/microservices
ARG PROJECT_NAME=ensek-spark
#ARG PROJECT_PATH=$BASE_PATH/$PROJECT_NAME
ARG PROJECT_PATH=ensek-spark
WORKDIR /src

# Copy and restore dependencies in one step to leverage Docker layer caching
COPY ${PROJECT_PATH}/${PROJECT_NAME}.csproj ${PROJECT_NAME}/
RUN dotnet restore ${PROJECT_NAME}/${PROJECT_NAME}.csproj

# Copy the rest of the source code
COPY ${PROJECT_PATH}/ ${PROJECT_NAME}/

# Build and publish the service
RUN dotnet publish ${PROJECT_NAME}/${PROJECT_NAME}.csproj -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage for runtime
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

# Set environment variable for project name
ENV PROJECT_NAME=ensek-spark

# Use the environment variable in the ENTRYPOINT
ENTRYPOINT dotnet $PROJECT_NAME.dll