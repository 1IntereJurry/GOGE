# GOGE // Technical Architecture & Developer Documentation

This document provides an in-depth analysis of the internal mechanics, structural design, and deployment logic of the GOGE system.

---

## 01 // System Architecture

GOGE is architected as a decoupled terminal application, separating data processing from the presentation layer. This ensures that the core logic remains independent of the console's rendering buffer.

### Layered Structure
- **Core Logic Layer**: Handles state management, game rules, and mathematical computations.
- **Abstraction Layer**: Manages the interface between high-level commands and the .NET runtime.
- **Presentation Layer**: Executes frame-based or event-based rendering within the Windows Console Host or Windows Terminal.



---

## 02 // Project Directory Analysis

### /GOGE (Source Code)
The primary workspace for the application logic.
- `Program.cs`: The kernel of the application. It initializes the environment, sets console encoding (UTF-8), and manages the primary execution loop.
- `Logic/`: (Recommended) Contains the algorithms that define the GOGE experience.
- `Data/`: (Recommended) Handles serialization of save states or configuration files.

### /GOGE-setup (Deployment)
A Visual Studio Deployment project (.vdproj) used to compile the `.msi` installer. It handles:
- Registry key entries for the application.
- Shortcut creation within the Windows Start Menu.
- Path variable management.

### /Web (Public Interface)
The landing page and asset delivery system.
- Driven by a custom CSS grid to simulate a CRT/Terminal aesthetic.
- Implements a JS-based "Lazy-Load" for GIFs to optimize initial page weight.

---

## 03 // Technical Specifications

### Runtime Environment
- **Platform**: .NET 8.0 (LTS)
- **Language Version**: C# 12.0
- **Compiler**: Roslyn
- **Architecture**: x64 Target only

### Build Configurations
The project utilizes a **Self-Contained Deployment (SCD)** strategy.
- **Single File**: All IL (Intermediate Language) assemblies and the native JIT compiler are bundled into a single binary.
- **Trimmed**: Unused libraries are stripped during the publishing process to minimize the footprint.
- **ReadyToRun (R2R)**: Ahead-of-time (AOT) compilation is enabled to reduce startup latency.

---

## 04 // Development Workflow

### Prerequisites
- Visual Studio 2022 (Version 17.8+)
- .NET 8 SDK
- Microsoft Visual Studio Installer Projects 2022 (Extension)

### Compilation via CLI
To reproduce the production build manually:
```bash
dotnet publish GOGE.sln -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:PublishReadyToRun=true
