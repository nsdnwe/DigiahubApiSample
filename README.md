# Digia Hub API

Sample code for a C# console app that integrates a backend system with the Digiahub.com partner API.

The code is simple on purpose, so the same approach works in Java, Node.js or any other environment.

## What the sample covers

| Endpoint | Purpose |
|---|---|
| `GET/POST/PUT/DELETE /api/resource` | Resource basic data: name, contact details, location, availability, price request, currency, summary |
| `PUT /api/availability/{id}` | Availability and starting date |
| `GET/POST/DELETE /api/resourceFile/{id}` | CV file as base64 (pdf, doc, docx, rtf, txt, jpg, png; max 10 MB) |
| `GET/POST/DELETE /api/profileImage/{id}` | Profile image as base64 (jpg, png; max 3 MB) |
| `GET/POST /api/competence/{id}` | Competences with years of experience |
| `GET/POST /api/workHistory/{id}` | Work history |
| `GET/POST /api/language/{id}` | Language skills, levels 1 to 4 |
| `GET/POST /api/education/{id}` | Educations |
| `GET /api/competenceList`, `/api/languageList`, `/api/educationTypeList`, `/api/currencyList` | Lookup lists for the fields above |

Every request sends the company `apikey` in the `apikey` HTTP header. Use the value `sandbox-mode` to test: sandbox data is isolated per caller and removed automatically after 24 hours.

The full API specification is available at https://digiahub.com/swagger

## Running

Requires the .NET 8 SDK or newer.

```
dotnet run --project DigiahubApiSample
```

Set the environment variable `DIGIAHUB_API_URI` to point the sample at another environment.

For more information, contact niko.wessman@digia.com
