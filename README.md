# NCacheTestClient

NCacheTestClient is a C# test client designed to interact with and test features of [Alachisoft NCache](https://www.alachisoft.com/ncache/), an in-memory distributed caching solution for .NET applications. This project provides a modular, extensible way to exercise and validate a variety of NCache features, making it useful for developers and DevOps engineers working with NCache clusters.

## Features

- **Connection Testing:** Easily configure and connect to different NCache servers and caches.
- **Cache Operations:** Test fundamental cache operations including add, get, update, remove, clear, and bulk operations.
- **ReadThru/WriteThru:** Demonstrates cache-through patterns for reading/writing data to/from backend data stores.
- **Pub/Sub Messaging:** Implements publish/subscribe messaging patterns using NCache topics, including durable and shared subscriptions.
- **Tags and Named Tags:** Test tag-based cache item organization and querying using SQL/OQL for flexible data retrieval.
- **Continuous Query:** Set up continuous queries and receive notifications for cache data changes in real time.
- **Dependency and Locking:** Test cache dependencies and distributed locking mechanisms.
- **Logging:** Integrated with log4net for configurable logging of cache operations and events.

## Getting Started

1. **Clone the repository:**
   ```sh
   git clone https://github.com/shoaib-diyatech/NCacheTestClient.git
   ```
2. **Set up dependencies:**
   - Ensure you have [NCache](https://www.alachisoft.com/ncache/) and .NET installed.
   - Update `log4net.config` and other configuration files as needed.
3. **Configure connection parameters:**
   - Edit the `Program.cs` or use configuration files to provide your NCache server IPs, ports, and cache names.
4. **Run the client:**
   - Build and run the project from your IDE or command line.

## Example Usage

The main client (`Program.cs`) allows you to instantiate different test classes (e.g., `CacheThrough`, `PubSubClient`, `TagClient`) and call their `Test()` methods to exercise specific NCache functionalities.

```csharp
// Example: Test ReadThru functionality
CacheThrough nCacheClient = new CacheThrough(serverIps, port, cacheName);
nCacheClient.Initialize();
nCacheClient.Test();
```

## Project Structure

- `NCacheTestClient/Program.cs`: Application entry point and configuration.
- `NCacheTestClient/NCacheClient/`: Contains various test clients for different NCache features:
  - `CacheThrough.cs`
  - `PubSubClient.cs`
  - `TagClient.cs`
  - `InProcClient.cs`
  - and more.

## Coding Standards

Please refer to the [C# Coding Standards](https://github.com/shoaib-diyatech/NCacheTestClient/blob/main/_new.md) for guidelines on contributing to this project.

## Target Users

- **.NET Developers:** Integrate, test, and explore NCache features via code.
- **DevOps/Admins:** Validate and monitor cache cluster health and configuration.

---

For more details on NCache and its features, visit the [official NCache documentation](https://www.alachisoft.com/resources/docs/).