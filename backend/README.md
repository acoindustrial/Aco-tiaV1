# Backend

NestJS + TypeORM API for Gestiune Stoc.

## Setup

```bash
cp .env.example .env
npm install
```

Ensure PostgreSQL is running (see `docker-compose.yml`). Then run migrations (synchronize used in this demo).

## Run

```bash
npm run start:dev
```

The API is served at http://localhost:3000.
