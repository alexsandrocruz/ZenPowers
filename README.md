# 🧠 ZenPowers

**Give Claude Code .NET superpowers.**

A C#-native skill and agent library that brings structured reasoning, automation, and collaborative workflows to Claude Code — the Zen way.

[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-9.0+-violet.svg)](https://dotnet.microsoft.com/)
[![Build](https://img.shields.io/github/actions/workflow/status/alexsandrocruz/ZenPowers/dotnet.yml?branch=main)](https://github.com/alexsandrocruz/ZenPowers/actions)

---

## ✨ Overview

ZenPowers is a modular collection of **agents** and **skills** that extend Claude Code with advanced .NET capabilities, inspired by the original [Superpowers](https://github.com/obra/superpowers) project.

It provides structured workflows, reusable skills, and C# patterns designed to enhance productivity, maintain code quality, and enable collaboration between human and AI agents in professional software projects.

---

## 🧩 What You Get

### 🔬 Development Skills

- TDD and BDD patterns in C#
- Async testing and dependency injection samples
- Modular architecture blueprints for agent workflows

### 🪲 Debugging Skills

- Systematic debugging patterns
- Root cause tracing with structured logs
- Verification-before-completion workflow

### 🤝 Collaboration Skills

- Pair-programming with Claude
- Agent orchestration and task handoffs
- Code review and merge decision patterns

### 🧠 Meta & AI Skills

- Creating and testing new skills
- Integrating external AI endpoints
- Automatic activation of skills via triggers or context

---

## ⚙️ Installation

Via Claude Code Plugin Marketplace (coming soon):

```bash
/plugin marketplace add sapienzae/zenpowers-marketplace
/plugin install zenpowers@zenpowers-marketplace
```

Or clone directly for local development:

```bash
git clone https://github.com/sapienzae/zenpowers-marketplace.git zenpowers
cd zenpowers

# Build the core library
dotnet build src/ZenPowers.Core/

# Run example tests to verify setup
dotnet test --logger console
```

---

## 🚀 Quick Start

Use ZenPowers inside Claude Code:

```bash
/zenpowers:brainstorm
/zenpowers:write-plan
/zenpowers:execute-plan
```

Each command activates a **skill agent** that follows proven .NET engineering practices and assists you through complete workflows — from design to delivery.

---

## 🧠 Example Skills

| Category | Skill | Description |
|-----------|-------|-------------|
| Testing | `test-driven-development` | Implements full TDD workflow with xUnit/MSTest |
| Testing | `condition-based-waiting` | Replaces arbitrary delays with TaskCompletionSource patterns |
| Debugging | `systematic-debugging` | Four-phase debugging with dotnet CLI and Visual Studio |
| Debugging | `root-cause-tracing` | Traces bugs backward through .NET call stacks |
| Collaboration | `brainstorming` | Socratic design refinement for .NET architectures |
| Collaboration | `writing-plans` | Detailed implementation plans with C# examples |
| Collaboration | `executing-plans` | Batch execution with verification checkpoints |
| Code Review | `requesting-code-review` | Structured code review with .NET best practices |
| Security | `defense-in-depth` | Multi-layer validation with .NET exception patterns |
| Meta | `using-zenpowers` | Core workflow orchestration for ZenPowers |

---

## 🧬 How It Works

ZenPowers uses a pattern-based approach with C# examples that guide both developers and Claude through complex workflows:

1. **Skills** - Markdown files with embedded C# samples for specific scenarios
2. **Triggers** - Keywords or patterns that activate relevant skills automatically  
3. **Context** - The library maintains state through static classes and dependency injection
4. **Execution** - Skills guide action sequences with validation checkpoints

### Core Components

- **ZenPowers.Core** - Enums, interfaces, and utilities
- **Skills/** - Markdown-based skill library with C# examples
- **Commands/** - Quick-reference commands for common operations
- **Docs/** - Architecture guides and skill creation patterns

### Supported .NET Features

- ✅ .NET 9.0+ with latest C# features
- ✅ xUnit and MSTest testing frameworks
- ✅ Entity Framework Core integration
- ✅ ASP.NET Core minimal APIs
- ✅ Dependency injection and configuration patterns
- ✅ Async/await with TaskCompletionSource
- ✅ CancellationToken and IHostedService patterns
- ⚠️  Multi-targeting (planned)
- ⚠️  NuGet package distribution (planned)

---

## 🧘 Philosophy

- **Simplicity over complexity** — clean C# code, clear intentions.
- **Patterns over frameworks** — reusable approaches instead of rigid structures.
- **Documentation as code** — executable examples that serve as living guides.
- **Validation checkpoints** — verify before proceeding to prevent cascading issues.

ZenPowers transforms the relationship between AI and code from "generate and hope" to "guide and verify."

---

## 🧱 Contributing

We welcome new skills, patterns, and agents!

1. Fork this repository  
2. Create a new branch for your skill  
3. Follow the guide in `docs/creating-skills.md`  
4. Test it with `/zenpowers:test-skill`  
5. Ensure all examples use modern C# and .NET patterns
6. Submit a PR

All contributions should follow .NET conventions and include unit tests with xUnit or MSTest.

---

## 📜 License

MIT License — see [LICENSE](LICENSE) file for details.

---

## 💚 Credits

**Original Concept & Python Implementation:**
[Aider Chat](https://github.com/paul-gauthier/aider) by Paul Gauthier — the pioneering AI pair programming tool that inspired structured AI-code collaboration patterns.

**TypeScript Implementation & Extensions:**  
[Claude Engineer](https://github.com/Doriandarko/claude-engineer) by Dorian Darko — expanded the concept with rich agent workflows and interactive capabilities.  

**Enhanced Collaboration Patterns:**  
[ClaudeSync](https://github.com/jahwag/ClaudeSync) by Jahwag — demonstrated advanced synchronization and context management techniques.  

**C#/.NET Conversion & ZenPowers Framework:**  
This repository represents a complete reimagining for the .NET ecosystem, with patterns optimized for modern C# development workflows, dependency injection, and enterprise-grade practices.

---

**ZenPowers** enables a new paradigm where AI doesn't just generate code — it collaborates with intention, guided by proven patterns and verified through systematic checkpoints.

> *"Good code isn't just working code — it's code that can be understood, maintained, and evolved."*
>
> *— The .NET Way*
