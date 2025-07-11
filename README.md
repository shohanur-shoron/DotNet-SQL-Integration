# Database Integration in Your .NET Project

This guide explains how your .NET application connects to and interacts with a SQL Server database using Entity Framework (EF) Core.

## Understanding the Key Parts

Connecting your application to a database involves four main parts working together:

1.  **The Model:** A C# class that represents a table in your database.
2.  **The Database Context:** A C# class that manages the connection to the database and lets you query it.
3.  **The Connection String:** A line in a configuration file that tells your app where to find the database.
4.  **The Program File:** The main file that "wires up" all the pieces.

---

### Step 1: The Model - Defining Your Data

This is a simple C# class where each property represents a column in your database table.

**File:** `Models/ItemModel.cs`
```csharp
namespace Bookverse.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
    }
}
```

---

### Step 2: The Database Context - Your Database's Representative

The "Context" file is the bridge between your C# code and the database. It inherits from `DbContext` (a special class from Entity Framework) and contains a `DbSet<T>` for each model you want to have a table for.

**File:** `Data/BookverseContext.cs`
```csharp
using Microsoft.EntityFrameworkCore;
using Bookverse.Models;

namespace Bookverse.Data
{
    public class BookverseContext : DbContext
    {
        public BookverseContext(DbContextOptions<BookverseContext> options) : base(options) { }

        // This tells EF Core that you want a table called "Items"
        // based on your "Item" model.
        public DbSet<Item> Items { get; set; }
    }
}
```

---

### Step 3: The Connection String - The Database Address

Your application needs to know the server and name of the database. You store this information in a "connection string" inside the `appsettings.json` file.

**File:** `appsettings.json`
```json
{
  "ConnectionStrings": {
    "DefaultConnectionStrings": "Data Source=.;Initial Catalog=Bookverse;Integrated Security=True;Pooling=False;Encrypt=False;Trust Server Certificate=True"
  }
}
```
*   **Data Source=.:** The database is on the local machine.
*   **Initial Catalog=Bookverse:** The name of the database is "Bookverse".
*   **Integrated Security=True:** Connect using your Windows login (no password needed).

---

### Step 4: Wiring It All Up in `Program.cs`

Finally, you need to tell your application to actually use the Database Context and the connection string. You do this in the main `Program.cs` file.

**File:** `Program.cs`
```csharp
// Find this line in Program.cs
builder.Services.AddDbContext<BookverseContext>(options =>
    // This tells the app to use SQL Server and get the connection details
    // from the "DefaultConnectionStrings" in appsettings.json
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionStrings")));
```

---

### Step 5: Installing the Required Packages

Before you can use Entity Framework Core, you need to install a few packages from NuGet. These packages provide the core functionality, the command-line tools, and the specific provider for SQL Server.

Run these commands in your terminal, in the root directory of your project:

```bash
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
```

---

## Creating and Updating the Database (The Commands)

You use a tool called `dotnet-ef` to create and update your database based on your C# models. This process is called **"migrations"**.

### First-Time Setup

If you haven't already, you need to install the `dotnet-ef` tool. Open a terminal or command prompt in your project's main folder and run this command once:
```bash
dotnet tool install --global dotnet-ef
```

Now, to create the database for the first time:

**1. Create a Migration:** This command looks at your models and creates a set of instructions to build the database schema.
```bash
dotnet ef migrations add InitialCreate
```
This will create a new `Migrations` folder in your project.

**2. Update the Database:** This command runs the migration instructions and actually builds the database and tables.
```bash
dotnet ef database update
```
Your database "Bookverse" with the "Items" table should now exist on your local SQL Server.

### What To Do When You Change a Model

Let's say you want to add an "Author" to your `Item` model.

**1. Change the C# Model:**
Update `Models/ItemModel.cs`:
```csharp
public class Item
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public double Price { get; set; }
    public string Author { get; set; } // Your new property
}
```

**2. Create a New Migration:** You need to create a new migration to tell the database how to change. **Give it a descriptive name!**
```bash
dotnet ef migrations add AddAuthorToItem
```

**3. Update the Database:** Apply the new migration to your database.
```bash
dotnet ef database update
```
The "Items" table in your database will now have a new "Author" column, and your existing data will be safe. You repeat these two commands every time you change your models.
