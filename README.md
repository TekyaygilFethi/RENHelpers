# 🚨 THIS PACKAGE IS NOW OBSOLETE! 🚨

> **RenHelpers.DataAccessHelpers is deprecated and will no longer receive updates or support.**
>
> Please migrate to [REN.Kit.DataKit](https://www.nuget.org/packages/REN.Kit.DataKit) and [REN.Kit.CacheKit](https://www.nuget.org/packages/REN.Kit.CacheKit) for modern, enterprise-grade data access and caching in .NET projects.

---

## ❌ End of Life

- **No more updates, bug fixes, or new features will be provided.**
- This package is NOT recommended for new projects.
- All users should upgrade to REN.Kit as soon as possible.

---

## 🚚 Migration Guide

Moving from RenHelpers.DataAccessHelpers to REN.Kit is quick and easy!

### 1. Remove the Old Package

Uninstall the legacy package from your project:

```shell
dotnet remove package RenHelpers.DataAccessHelpers
```

### 2. Install REN.Kit Packages
- For **database operations** (Unit of Work, Repository, transactions, etc.), install:

```shell
dotnet add package REN.Kit.DataKit
```

- For **caching (In-Memory, Redis, etc.)**, install:
```shell
dotnet add package REN.Kit.CacheKit
```

If you use both, install both packages!

### 3. Update Usings & Service Registrations
Update your code to use the new namespaces and service registrations:
```csharp
// For DataKit
builder.Services.RegisterRENDataServices();

// For CacheKit (example: InMemory)
builder.Services.AddRENCaching(RegisterRENCaching.CacheType.InMemory);
```

4. Review the Documentation
Find full guides and examples here: [REN.Kit Documentation](https://fethis-organization.gitbook.io/ren.kit-documentation)


Thank you for using RenHelpers!
Upgrade to REN.Kit for a faster, cleaner, and future-proof .NET experience.

------------------------------------------
# REN (Regular Everyday Normal) Helper

Have you ever thought a world without repeating yourself? A world that you can focus only the important parts of the code instead of rewriting all the processes over and over again? A world that does not make you feel that you are trapped in the Matrix.

Well, I have. And I've created this library to help all of us with that. Ladies and gentlements, here is REN!

![reindeer.png](reindeer.png)

## What does REN Helper do?
REN Helper is a nuget package that streamlines and standardizes common development tasks, such as creating a Repository layer and implementing essential utility functions. It enables developers to adhere to best practices, save time, and focus on solving problems efficiently.

# NuGet Page

- [NuGet Page](https://www.nuget.org/packages/RENHelpers.DataAccessHelpers)

# Components
- [Data Access Helper](src/RENHelpers.DataAccessHelpers/)


## Documentation

[Documentation](https://fethis-organization.gitbook.io/ren-regular-everyday-normal-helper/)

