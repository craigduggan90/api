# api

## Migrations

We use separate startup and data projects, so running a migration requires a couple of extra parameters:

```sh
# Commands relative to /src directory

# Add Migration
dotnet ef migrations add <name> --project ./Sol.Data --startup-project ./Sol.Api --output-dir Context/Migrations

# Remove Migration
dotnet ef migrations remove --project ./Sol.Data --startup-project ./Sol.Api

# Apply Migration
dotnet ef database update --project ./Sol.Data --startup-project ./Sol.Api
```

> ℹ️ We use separate read and write contexts, if that is enforced by connection string (as is the case for the default 
> SQLite), be sure to update the read string to be writable.  For example:
> 
> Before: ```"Reader": "Data Source=../../sol.db;mode=ReadOnly",```
> 
> After: ```"Reader": "Data Source=../../sol.db",```