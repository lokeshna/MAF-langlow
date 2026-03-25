# MAF Langflow Bot (ASP.NET Core 8 MVC + CopilotKit UI)

This project is an ASP.NET Core 8 MVC app with a **CopilotKit floating bot UI**.

## What it does
- Uses an MVC front-end with CopilotKit popup UI mounted from a browser ESM module.
- Exposes:
  - `POST /api/bot/chat` for simple custom chat requests.
  - `POST /api/copilotkit` as a lightweight compatibility shim for CopilotKit payloads.
- Uses a Microsoft Agent Framework `Agent` instance in the service layer.
- Sends user messages to a Langflow flow endpoint and returns the response.

## Configure
Update `appsettings.json`:
- `Langflow:Endpoint` - your Langflow run endpoint.
- `Langflow:ApiKey` - optional bearer token.

## Run
```bash
dotnet restore
dotnet run
```

Then open the app URL shown in logs and use the floating CopilotKit button in the bottom-right.

## Notes
- The CopilotKit UI is loaded from CDN (`unpkg` + `esm.sh`).
- For production, pin exact package versions and consider self-hosting frontend assets.
