# Pastebin Demo Application (.NET)

A simple Pastebin-like application built with React, TypeScript, and a .NET minimal API backend using EF Core + SQLite. This is intentionally built with minimal security measures for educational purposes in security courses.

## Features
- Code snippet creation and editing
- Support for multiple programming languages:
  - TypeScript
  - JavaScript
  - Python
  - Java
  - C++
  - C#
- Syntax highlighting using CodeMirror
- File upload functionality
- Basic user authentication
- Unique URLs for each saved snippet
- SQLite database with Entity Framework Core

## ⚠️ Security Notice
This application is deliberately built WITHOUT security measures for educational purposes. It contains various vulnerabilities including but not limited to:
- SQL Injection possibilities
- No input validation
- Weak authentication
- No CSRF protection
- Potential XSS vulnerabilities

DO NOT USE THIS IN PRODUCTION!

## Prerequisites
- .NET 9 SDK
- Node.js (v14 or higher) and npm

## Installation
1. Backend (.NET):
```bash
cd learn_paste-bin-dotnet/backend/PasteBinBackend
dotnet restore
dotnet run   # runs on http://localhost:3001 and creates pastebin.db in this folder
```

2. Frontend (Vite React):
```bash
cd learn_paste-bin-dotnet/frontend
npm install
npm run dev   # runs on http://localhost:3000 with proxy to backend
```

## Usage
1. Access the application at `http://localhost:3000`
2. Login with default credentials:
   - Username: `admin`
   - Password: `admin`
3. Create new snippets:
   - Enter a title
   - Select a programming language
   - Write or paste your code
   - Click "Save" to generate a unique URL
4. Upload files:
   - Click the file upload button
   - Select a text file
   - The content will be automatically loaded into the editor
5. Access saved snippets:
   - Use the generated URL (format: `/snippet/:id`)
   - Edit and save changes as needed

## API Endpoints
- `POST /api/auth/login` - User authentication
- `POST /api/auth/register` - Register a new user
- `POST /api/snippets` - Create/update snippets (auth required)
- `GET /api/snippets/:id` - Retrieve a specific snippet
- `GET /api/snippets` - List snippets for the logged-in user
- `DELETE /api/snippets/:id` - Delete a snippet

## Development
Backend runs on port 3001, frontend dev server on port 3000 (proxied to backend).
