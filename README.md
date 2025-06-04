# WordCount API

A simple ASP.NET Core API that counts words in uploaded text files.

## What it does

- Upload one or multiple text files
- Counts alphanumeric words (case-insensitive)
- Returns word frequencies sorted by count (highest first)
- Stores results in SQLite database

## How to run

1. Navigate to the `api` folder
2. Run `dotnet run`
3. Open http://localhost:XXXX/swagger
4. Use the Swagger UI to test the API

## API Endpoint

**POST** `/api/WordCount`
- Upload files using the file input
- Returns JSON with word counts from all uploaded files combined
