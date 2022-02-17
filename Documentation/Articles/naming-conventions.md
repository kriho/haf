# Naming conventions
Naming conventions are for readability and orientation purposes only. HAF enforces no naming conventions. The following rules have been used for many projects and have proven useful.

## Events
All events are prefixed with `On`. This allows easy identification of events when browsing the API of services. 

Example:
```csharp
IReadOnlyEvent OnProjectsChanged { get; }
```

## Relay Commands
All instances of `IRelayCommand` implementations are prefixed with `Do`. This is especially useful in the views where these commands need to be identified.

Example: 
```csharp
IRelayCommand<IProject> DoLoadProject { get; }
```

## States
States are used for multiple things in the framework. When an [ICompoundState](xref:HAF.ICompoundState) is used to inject a dependency into a service these states are prefixed with `May`.

Example:
```csharp
ICompoundState MayChangeProject { get; }
```
When states are used to describe the internal state of a service, the prefixes `Is` or `Can` are used.

Example:
```csharp
IReadOnlyState CanChangeProject { get; }
```

