# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a **Lotofácil prediction system** written in C# using .NET 9.0. The system uses advanced machine learning models and statistical analysis to predict Lotofácil lottery numbers. The project consists of two main components:

1. **LotoLibrary** - Core library containing prediction models, data processing, and business logic
2. **Dashboard** - WPF application providing user interface for the prediction system

## Build and Development Commands

### Building the Solution
```bash
# Build the entire solution
dotnet build GraphFacil.sln

# Build specific projects
dotnet build Library/LotoLibrary.csproj
dotnet build Dashboard/Dashboard.csproj

# Build for release
dotnet build GraphFacil.sln --configuration Release
```

### Running the Application
```bash
# Run the WPF Dashboard application
dotnet run --project Dashboard/Dashboard.csproj

# Run from executable after build
cd Dashboard/bin/Debug/net9.0-windows
./Dashboard.exe
```

### Testing
```bash
# Run all tests
dotnet test

# Run specific test projects
dotnet test Tests/Unit/
dotnet test Tests/Integration/
dotnet test Tests/Performance/
```

## Code Architecture

### Core Data Models
- **Lance** (`Library/Models/Core/Lance.cs`): Represents a single lottery game/bet with numbers and analysis metrics
- **Lances** (`Library/Models/Core/Lance.cs`): Collection of Lance objects with specialized methods
- **Lotofacil** (`Library/Models/Lotofacil.cs`): Main entity representing lottery game data

### Prediction Models Architecture
The system uses a modular prediction architecture with multiple specialized models:

**Base Architecture:**
- `IPredictionModel` - Core interface for all prediction models
- `PredictionModelBase` - Base class providing common functionality
- `IConfigurableModel` - Interface for models with configurable parameters

**Model Categories:**
- **Individual Models** (`Library/PredictionModels/Individual/`):
  - `MetronomoModel` - Rhythm-based prediction
  - `DeepLearningModel` - Neural network predictions
- **Anti-Frequency Models** (`Library/PredictionModels/AntiFrequency/`):
  - `AntiFrequencySimpleModel` - Simple frequency-based predictions
  - `SaturationModel` - Saturation point analysis
  - `StatisticalDebtModel` - Statistical debt calculations
- **Ensemble Models** (`Library/PredictionModels/Ensemble/`):
  - `MetaLearningModel` - Meta-learning combining multiple models

### Engine Layer
- **PredictionEngine** (`Library/Engines/PredictionEngine.cs`): Orchestrates predictions across models
- **OscillatorEngine** (`Library/Engines/OscillatorEngine.cs`): Handles oscillation pattern analysis
- **MetronomoEngine** (`Library/Engines/MetronomoEngine.cs`): Manages rhythm-based predictions

### Services Layer
- **LotofacilService** (`Library/Services/LotofacilService.cs`): Business logic for lottery operations
- **ValidationService** (`Library/Services/Validation/ValidationService.cs`): Model validation and testing
- **DiagnosticService** (`Library/Services/DiagnosticService.cs`): System diagnostics and health checks

### UI Architecture (WPF/MVVM)
- **ViewModels** (`Dashboard/ViewModels/`): 
  - `MainWindowViewModel` - Main application orchestrator
  - Specialized ViewModels in `Specialized/` folder for different features
  - `ViewModelBase` - Base class with common MVVM functionality
- **Views** (`Dashboard/Views/`): WPF user controls and windows
- **Services** (`Dashboard/Services/`): UI-specific services

## Key Development Patterns

### Dependency Injection
The system uses Microsoft.Extensions.Hosting for dependency injection. Services are registered in the Dashboard application startup.

### MVVM Pattern
The WPF UI follows strict MVVM pattern using CommunityToolkit.Mvvm:
- ViewModels inherit from `ViewModelBase`
- Use `[ObservableProperty]` for data binding
- Use `[RelayCommand]` for command binding

### Repository Pattern
Data access is handled through repository pattern:
- `IRepository` interface defines data operations
- `LotofacilRepository` implements lottery-specific data access

### Factory Pattern
- `ModelFactory` creates appropriate prediction models
- `ViewModelFactory` creates specialized ViewModels

## Dependencies and Packages

### Library Dependencies
- **Microsoft.ML** (4.0.0) - Machine learning framework
- **Microsoft.ML.FastTree** (4.0.0) - Fast tree algorithms
- **Newtonsoft.Json** (13.0.3) - JSON serialization
- **NumSharp** (0.30.0) - Numerical computing
- **CommunityToolkit.Mvvm** (8.2.1) - MVVM toolkit
- **Microsoft.Extensions.Logging** (9.0.0) - Logging framework

### Dashboard Dependencies
- **CommunityToolkit.Mvvm** (8.2.1) - MVVM framework
- **Microsoft.Extensions.Hosting** (9.0.0) - Dependency injection

## Data Files and Configuration

### Data Files
- **Lotofacil.json** - Historical lottery data (root directory)
- **Saida/** - Output directory for predictions and analysis results

### Configuration
- No external configuration files - settings are handled through code
- Historical data is loaded from `Lotofacil.json` on application startup

## Testing Strategy

### Test Structure
- **Unit Tests** (`Tests/Unit/`): Individual component testing
- **Integration Tests** (`Tests/Integration/`): Cross-component testing
- **Performance Tests** (`Tests/Performance/`): Performance benchmarking

### Testing Frameworks
- Uses .NET built-in testing framework
- Focus on testing prediction models and business logic

## Development Workflow

### Adding New Prediction Models
1. Implement `IPredictionModel` interface
2. Inherit from `PredictionModelBase` for common functionality
3. Add model to `ModelFactory` for instantiation
4. Create corresponding unit tests
5. Update documentation

### Code Style Guidelines
- Follow standard C# naming conventions
- Use meaningful variable and method names
- Keep methods focused and single-responsibility
- Add XML documentation for public APIs
- Use async/await for I/O operations

### Performance Considerations
- Models work with large datasets (thousands of historical games)
- Use efficient data structures (`List<>`, `Dictionary<>`)
- Implement proper disposal patterns for ML models
- Consider memory usage in prediction algorithms

## Troubleshooting

### Common Issues
- **Build Errors**: Ensure .NET 9.0 SDK is installed
- **Missing Data**: Check `Lotofacil.json` exists and is valid
- **Model Initialization**: Verify historical data is loaded before model training
- **UI Responsiveness**: Ensure long-running operations use async patterns

### Debugging
- Use logging through `Microsoft.Extensions.Logging`
- Check diagnostic services for system health
- Monitor prediction accuracy through validation services

## Documentation
Comprehensive documentation is available in the `Docs/` directory:
- `system_overview.md` - System overview and capabilities
- `technical_architecture.md` - Detailed technical architecture
- `user_guide.md` - User operation guide
- `developer_guide.md` - Development guidelines
- `performance_metrics.md` - Performance analysis