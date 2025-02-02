# Block Country IP

A .NET application that blocks IP addresses based on their country of origin using in-memory storage and IP geolocation.

## Features

- Block/unblock countries permanently or temporarily.
- Check if an IP address is blocked based on its country.
- Paginated listing of blocked countries.
- Automatic cleanup of expired temporary blocks.
- Integrates with [IPGeolocation.io](https://ipgeolocation.io/) for IP-to-country lookup.

## Technologies

- **.NET 6+**
- **ConcurrentDictionary** for thread-safe in-memory storage.
- **IPGeolocation.io API** for IP geolocation.
- **BackgroundService** for automated cleanup.
- **Swagger** for API documentation.

## Installation

1. **Prerequisites**:
   - [.NET 6+ SDK](https://dotnet.microsoft.com/download)
   - [IPGeolocation.io API Key](https://ipgeolocation.io/pricing.html) (Free tier available)
