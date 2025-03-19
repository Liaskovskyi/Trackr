# Trackr

Trackr is a music tracking application that monitors Spotify listening history. It automatically fetches and stores track data, artist information, and playback timestamps.

## Architecture

The project follows a layered architecture with four distinct layers:

- **Domain Layer** - Core business entities and interfaces
- **Application Layer** - Business logic and service orchestration
- **Infrastructure Layer** - Data access, external API clients, and technical implementations
- **API Layer** - HTTP endpoints and request handling

Each layer has a single responsibility, keeping database code, business logic, and HTTP concerns separate. The interface-based design in the Infrastructure layer enables swapping external data sources—for example, replacing Spotify with YouTube—without modifying the Application or Domain layers.

## Technologies

- **.NET 8** - Runtime framework
- **ASP.NET Core Web API** - REST API framework
- **Entity Framework Core** - ORM with SQL Server
- **ASP.NET Core Identity** - User authentication and authorization
- **JWT Authentication** - Token-based security
- **Redis** - Distributed caching for Spotify access tokens
- **RabbitMQ** - Message queue for asynchronous processing
- **Spotify Web API** - Music data source
- **AutoMapper** - Object mapping between layers
- **Docker** - Containerization support

## Key Features

### Spotify Integration

The application authenticates with Spotify via OAuth 2.0. Access tokens are cached in Redis and refreshed automatically, removing the need for users to re-authenticate on each request.

### Listening History Tracking

When a user connects their Spotify account, Trackr fetches their recently played tracks and incrementally builds a listening history. Tracks, artists, albums, and genres are deduplicated before persistence, so only new records are written.

### Asynchronous Processing

RabbitMQ implements a delayed message pattern with a one-hour delay. Each user has a dedicated message that triggers periodic data fetching from Spotify. Since Spotify only provides access to recently played tracks rather than full listening history, this mechanism ensures regular incremental updates without blocking user interactions.

### Data Model

The database models relationships between music entities:
- Tracks have many-to-many relationships with artists
- Albums have many-to-many relationships with artists
- Genres link to artists
- Listening history connects users to tracks with timestamps

All related entities are persisted within a single transaction.

### Error Handling

The application uses a Result pattern for error handling. Methods return typed results containing either a success value or error information, making error propagation explicit throughout the call chain.

### Background Processing

A hosted service runs continuously, consuming messages from RabbitMQ. On message arrival, it triggers the track fetching process for the target user.

## Project Structure

- `src/Trackr.APi` - HTTP controllers, authentication configuration, and dependency injection setup
- `src/Trackr.Application` - Service implementations, business logic, and background services
- `src/Trackr.Domain` - Domain models and interfaces
- `src/Trackr.Infrastructure` - Repository implementations, external API clients, and database context
- `src/Tests` - Tests

## Running the Application

Two deployment modes are supported: fully containerized via Docker Compose, or hybrid with RabbitMQ in Docker and the .NET application running locally. Required configuration values are documented in `.env.example`.