# OrgaNestApi

**OrgaNestApi** is a RESTful API built using .NET 8 MVC. It provides endpoints for managing categories, expenses, shopping lists, and users for the OrgaNest application.

## Overview

The API follows the OpenAPI 3.0 specification and includes features such as:

- Creating, retrieving, updating, and deleting categories.
- Managing expenses with support for user and family filtering.
- Handling shopping lists and their details.
- Creating and retrieving users.

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- A supported database (or use the in‑memory provider for testing)
- (Optional) Swagger UI for interactive API documentation

### Installation

1. **Clone the repository:**

   ```bash
   git clone https://github.com/YOUR_USERNAME/OrgaNest.git
   cd OrgaNestApi
   ```

2. **Restore dependencies and build the solution:**



  ```bash
  dotnet restore
  dotnet build --configuration Release
   ```
3. **Run the API:**

  ```bash
  dotnet run --configuration Release
   ```
