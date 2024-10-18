# Laika for STRYKER.NET

A small tool that runs Stryker.NET in the background when a source file changes.

## Installation

### Global installation

```dotnet tool install -g laika```

### Local installation

```dotnet tool install laika --create-manifest-if-needed```

## Prerequisites

- Stryker.NET must be installed globally or locally in the project.

## Usage

```dotnet laika [--impatient] [stryker arguments]```
