# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build & Test

```bash
# Build the project (requires Godot .NET SDK 4.6)
dotnet build

# Run all tests
dotnet test

# Run a single test class
dotnet test --filter "FullyQualifiedName~CalculateHelperBinaryTests"

# Run a single test method
dotnet test --filter "FullyQualifiedName~CalculateHelperBinaryTests.Add_TwoIntegers"
```

Tests use xUnit and are in `tests/Time-To-Twenty-four.Tests/`. The test project references the main project directly — no Godot runtime needed for unit tests of pure C# logic (CalculateHelper only).

## Architecture

**Stack:** Godot 4.6 + C# (.NET 10) + GFramework (0.0.177) — a CQRS/ECS framework from NuGet.

**DI bootstrapping:** `global/GameEntryPoint` (autoload singleton) creates `GameArchitecture`, which installs 4 modules:
- `ModelModule` — settings models with applicators (audio/graphics/localization)
- `SystemModule` — UiRouter, SceneRouter, SettingsSystem
- `UtilityModule` — registries, storage, serialization, factories
- `StateModule` — `GameStateMachineSystem` with 5 states

**State → UI mapping:** Each state (MainMenuState, CalculateMenuState, OptionsMenuState, CreditsState, GameOverState) clears previous UI/scene on enter and pushes its own UI page via `UiRouter.Push()`.

**UI pages** extend `Control`, implement `IUiPageBehaviorProvider` + `ISimpleUiPage`. They follow a partial-class pattern:

| Partial file | Purpose |
|---|---|
| `*.cs` | Core: `_Ready()` calls `ReadyAsync()`, `ConnectSignal()`, `RegisterEvent()` |
| `*.Dependencies.cs` | Godot node references (`%NodeName`), async init logic |
| `*.Properties.cs` | Fields, `UiKeyStr` |
| `*.Events.cs` | CQRS event subscriptions via `RegisterEvent()` |
| `*.Signals.cs` | Godot signal → CQRS event conversion (`ConnectSignal()`) |

**Entity partial classes** follow the same pattern: `Entity.cs`, `Entity.Dependencies.cs`, `Entity.Properties.cs`, `Entity.Events.cs`, `Entity.Signals.cs`.

## Key Patterns

### CQRS communication
Components communicate through GFramework events, not Godot signals:
- **Send:** `this.SendEvent(new SomeEvent { ... })` (fires to all registered handlers)
- **Subscribe:** Inside `RegisterEvent()`, use `this.RegisterEvent<SomeEvent>(e => { ... })`
- Entity events are in `scripts/cqrs/*/event/`; commands in `scripts/cqrs/*/command/`

### Poker state machine
Each card runs a 4-state FSM: `Idle` ↔ `UnSelect` ↔ `OnSelect`, plus `Drag` (entered from Idle on mouse-down, exits to Idle on mouse-up). State changes are dispatched via `PokerStateChangedEvent`.

### Selector
FIFO queue with capacity limit. When the queue is full, oldest selection is evicted. `Pop()` is LIFO for undo-last-selection. Reacts to `SelectorSelectChangedEvent`.

### Logging & Context
- `[Log]` attribute on a class auto-generates a static `Log` property via GFramework source generators
- `[ContextAware]` attribute auto-injects the GFramework architecture context

## Core Game Logic

`scripts/utility/CalculateHelper.cs` — static calculation engine using exact rational arithmetic via a private `Fraction` struct (GCD reduction, continued-fraction conversion from doubles).

- **Binary ops (7):** Add, Subtract, Multiply, Divide, Modulo, Power, NthRoot
- **Unary ops (5):** AbsoluteValue, Factorial, SquareRoot, Ceil, Floor
- **Input:** `IPoker` objects (reads `NumValue` string + `NumType` enum)
- **Output:** String — either a number or `"ERROR:DivByZero"` / `"ERROR:ZeroRootIndex"` / `"ERROR:InvalidSqrt"`

## Current Development Status

The project follows Plan.md's 5-phase roadmap. **Phase 3 (core game loop)** is the active phase:

- `CalculateMenu` creates 4 test cards (20, 4, 6, 8) and starts a 120s timer — this is temporary scaffolding
- `DeckHandCheckedEvent` and `DeckDiscardCheckedEvent` are emitted but **have no subscribers**
- `CalculateBar` has 12 operation buttons (all `ModeType` operations) with **empty click handlers**
- The core loop "select cards → choose operation → calculate → validate result" is **not yet wired up**
- Phases 1, 2, 4, 5 are not started
