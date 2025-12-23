# Pastebin Demo Application (.NET)

A simple Pastebin-like application built with **React, TypeScript**, and a **.NET minimal API backend** using **Entity Framework Core + SQLite**.
This project is intentionally built with **minimal security measures** for educational purposes in security courses.


## Features

* Code snippet creation and editing
* Support for multiple programming languages:

  * TypeScript
  * JavaScript
  * Python
  * Java
  * C++
  * C#
* Syntax highlighting using CodeMirror
* File upload functionality
* Basic user authentication
* Unique URLs for each saved snippet
* SQLite database with Entity Framework Core



## ⚠️ Security Notice

This application is deliberately built **WITHOUT security measures** for educational purposes.
It contains various vulnerabilities including (but not limited to):

* SQL Injection possibilities
* No input validation
* Weak authentication
* No CSRF protection
* Potential XSS vulnerabilities

**DO NOT USE THIS IN PRODUCTION.**
This project is intended strictly for learning and demonstration.



## Prerequisites

* .NET 9 SDK
* Node.js (v14 or higher) and npm



## Installation

### Backend (.NET)

```bash
cd learn_paste-bin-dotnet/backend/PasteBinBackend
dotnet restore
dotnet run
```

The backend runs on:

```
http://localhost:3001
```

A `pastebin.db` SQLite database will be created in this folder automatically.



### Frontend (Vite + React)

```bash
cd learn_paste-bin-dotnet/frontend
npm install
npm run dev
```

The frontend runs on:

```
http://localhost:3000
```

Requests are proxied to the backend.



## Running with Docker (Optional)

This repository includes an **optional Docker setup** for the backend.

### Backend (Docker)

From the repository root:

```bash
docker compose build
docker compose up
```

The backend will be available at:

```
http://localhost:3001
```

> **Note:** The backend port (`3001`) is intentionally hardcoded in the application code for educational clarity and to ensure consistent behavior between local and Docker environments.

To stop the container:

```bash
docker compose down
```

Docker is optional and **not required** for local development.



## Usage

* Access the application at:

  ```
  http://localhost:3000
  ```

* Login with default credentials:

  ```
  Username: admin
  Password: admin
  ```

### Create new snippets

1. Enter a title
2. Select a programming language
3. Write or paste your code
4. Click **Save** to generate a unique URL

### Upload files

1. Click the file upload button
2. Select a text file
3. The content will be automatically loaded into the editor

### Access saved snippets

* Use the generated URL:

  ```
  /snippet/:id
  ```
* Edit and save changes as needed



## API Endpoints

* `POST /api/auth/login` — User authentication
* `POST /api/auth/register` — Register a new user
* `POST /api/snippets` — Create/update snippets (auth required)
* `GET /api/snippets/:id` — Retrieve a specific snippet
* `GET /api/snippets` — List snippets for the logged-in user
* `DELETE /api/snippets/:id` — Delete a snippet



## Development Notes

* Backend runs on port **3001**
* Frontend dev server runs on port **3000** and proxies to the backend
* Several vulnerabilities are **intentional** and used for teaching secure coding practices



## License

This project is licensed under the **Elastic License 2.0**.

You may use, copy, and modify this software for educational and internal purposes.
You may **not** provide this software as a hosted or managed service.

See the [LICENSE](./LICENSE) file for full details.



## About

This repository is used as part of security-focused educational material and demonstrations.

