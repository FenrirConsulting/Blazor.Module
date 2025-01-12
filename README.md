# Blazor.Module

## Overview
Blazor.Module is a template project designed for rapid application development within the Blazor Enterprise Architecture. It provides a standardized structure for creating micro-applications that can be seamlessly integrated into the Blazor.Shell environment.

## Features
- 🚀 Quick start template
- 🔄 Pre-configured dependencies
- 🔌 Shell integration ready
- 🎨 Standardized styling
- 📦 Common components
- 🔐 Built-in security

## Getting Started

### 1. Create New Module
```bash
# Clone the template
git clone https://github.com/FenrirConsulting/Blazor.Module YourModuleName

# Navigate to project
cd YourModuleName

# Install dependencies
dotnet restore
```

### 2. Configure Module
```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddBlazorModule(Configuration);
        services.AddModuleServices();
    }
}
```

## Project Structure
```
YourModule/
├── Components/           # Reusable UI components
├── Pages/               # Module pages
├── Services/            # Business logic
├── Models/              # Data models
├── wwwroot/            # Static assets
└── ModuleStartup.cs    # Module configuration
```

## Integration

### Module Registration
```csharp
services.AddModule(options => {
    options.Name = "YourModule";
    options.Route = "/your-module";
    options.Icon = "module-icon";
});
```

### Shell Integration
```json
{
  "Module": {
    "Name": "YourModule",
    "Description": "Module description",
    "Version": "1.0.0",
    "Dependencies": {
      "Blazor.RCL": "1.0.0"
    }
  }
}
```

## Development Guidelines

### 1. Component Development
- Use RCL components when available
- Follow naming conventions
- Implement error boundaries
- Use dependency injection
- Document public APIs

### 2. State Management
- Use provided stores
- Implement CRUD operations
- Handle side effects
- Manage subscriptions
- Clean up resources

### 3. Security
- Use authentication
- Implement authorization
- Validate inputs
- Handle errors
- Log security events

## Testing
```bash
# Run unit tests
dotnet test

# Run integration tests
dotnet test --filter Category=Integration
```

## Building
```bash
# Debug build
dotnet build

# Release build
dotnet build -c Release
```

## Deployment
1. Build module
2. Copy to modules directory
3. Register with shell
4. Configure IIS
5. Update permissions

## Best Practices
1. Follow coding standards
2. Use provided base classes
3. Implement interfaces
4. Handle errors appropriately
5. Write unit tests

## Customization
- Theme customization
- Layout modifications
- Component overrides
- Service extensions
- Custom middleware

## Troubleshooting
1. Build Issues
   - Check dependencies
   - Verify versions
   - Clear cache
2. Runtime Issues
   - Check logs
   - Verify configuration
   - Test connectivity

## Contributing
1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## License
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Author
Christopher Olson - Senior Security Engineer