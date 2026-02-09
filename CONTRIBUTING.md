# Contributing Guidelines

Thank you for your interest in contributing to this project! This document guides you on how to contribute.

## Development Environment Setup

### Prerequisites
- Unity 6000.0.45f1 or higher
- XR Interaction Toolkit 3.0.8
- Git

### Project Setup
1. Fork the repository
2. Clone locally
3. Open project in Unity
4. Verify dependencies in Package Manager

## Coding Style

### C# Coding Conventions
- **Naming**: PascalCase (classes, methods), camelCase (variables)
- **Comments**: Korean or English (maintain consistency)
- **Indentation**: 4 spaces (tab)
- **Braces**: K&R style

### Example
```csharp
public class ExampleClass
{
    private float exampleVariable;
    
    public void ExampleMethod()
    {
        // Comments should be clear
        if (condition)
        {
            DoSomething();
        }
    }
}
```

## Commit Messages

### Format
```
[Type] Brief description

Detailed description (optional)
```

### Types
- `[Feature]`: New feature addition
- `[Fix]`: Bug fix
- `[Refactor]`: Code refactoring
- `[Docs]`: Documentation modification
- `[Style]`: Code style change
- `[Test]`: Test addition/modification

### Example
```
[Feature] Add waypoint loop functionality

- Added loopPath option to WaypointManager
- Implemented functionality to return to first waypoint when last waypoint is reached
```

## Pull Request Process

1. **Create Branch**
   ```bash
   git checkout -b feature/your-feature-name
   ```

2. **Commit Changes**
   ```bash
   git add .
   git commit -m "[Feature] Feature description"
   ```

3. **Push and Create PR**
   ```bash
   git push origin feature/your-feature-name
   ```

4. **PR Checklist**
   - [ ] Code follows project style
   - [ ] Tests completed
   - [ ] Documentation updated (if needed)
   - [ ] No conflicts

## Testing

### Testing Methods
1. Run Play mode in Unity Editor
2. Test main features
3. Check error logs
4. Performance profiling (if needed)

### Test Checklist
- [ ] Autonomous driving works correctly
- [ ] Steering wheel rotation works correctly
- [ ] Collision trigger works correctly
- [ ] Quiz system works correctly
- [ ] Waypoint management works correctly

## Issue Reporting

### Bug Report
Please include the following information:
- Unity version
- Reproduction steps
- Expected behavior
- Actual behavior
- Screenshots/logs (if available)

### Feature Request
- Feature description
- Use cases
- Expected behavior

## License

Contributions will be distributed under the MIT License.
