# Student Profile

The **Student Profile** application is a microservice-based system. It features an ASP.NET Core 7 API with Entity Framework Core and a React (Vite) client using Material UI. It includes JWT authentication, user management, personal details, projects, skills, chat, and file/image uploads.

## Prerequisites

- [.NET SDK 7](https://dotnet.microsoft.com/download/dotnet/7.0)
- [Node.js](https://nodejs.org/) (LTS recommended) for the client.
- **SQL Server** (optional for development; see below).

## Project Structure

| Folder | Description |
|--------|-------------|
| Root   | Web API (`Program.cs`, `Controllers/`, `BLL/`, `Data/`) |
| `Client/` | React + Vite application |

## API Setup

### Connection String and JWT
Edit `appsettings.json` (or `appsettings.Development.json` if it exists):

- **ConnectionStrings:DefaultConnection**: Your SQL Server connection string.
- **Jwt**: You must set `Issuer`, `Audience`, and `Key` (use a long symmetric key; in production, use a strong random secret).

If the connection string is empty or matches the local machine name (`DESKTOP-IV6MTF1\RUTHB`) during **Development**, the server automatically uses an **InMemory database**—useful for quick testing without SQL Server.

### Running the API
```bash
cd /path/to/Microservice_Profile-main
dotnet run --launch-profile http

```

The default settings (from `Properties/launchSettings.json`) are:

* HTTP: `http://localhost:5290`
* HTTPS: `https://localhost:7182`

In **Development** mode, **Swagger UI** and **OpenAPI** are available (usually at `/swagger`).

### Authentication

* `POST /api/auth/register` — Register
* `POST /api/auth/login` — Login
* `POST /api/auth/dev-token` — Generate a JWT for development (only in `Development` mode).

Other controllers are mapped under `api/...` (e.g., `api/Users`, `api/Projects`, `api/Chat`, `api/Skills`).

## React Client Setup

### Environment Variables

Copy `Client/.env.example` to `Client/.env` (or `.env.local`) and configure:

* **VITE_API_BASE_URL**: Must end with `/api` (e.g., `http://127.0.0.1:5290/api`).
* Optional: **VITE_DEV_PROXY_TARGET**: The Vite proxy target (default is `http://127.0.0.1:5290`).

### Running the Client

```bash
cd Client
npm install
npm run dev

```

For production builds, use `npm run build`. The output will be in `Client/dist` (the server serves static files via `UseStaticFiles` if configured).

## Additional Integrations

The `appsettings.json` file includes a **Staff6Events** section for event publishing. Replace the placeholder values in a real environment.

## Quick Start Summary

1. Set `Jwt:Key` and connection string (or rely on InMemory for development).
2. Run `dotnet run --launch-profile http` in the project root.
3. In `Client/`: `npm install && npm run dev`, with `VITE_API_BASE_URL` pointing to `http://127.0.0.1:5290/api`.

---

*Note: This is a demo project. Before deploying, ensure you strengthen JWT secrets, CORS, and security policies.*

```
