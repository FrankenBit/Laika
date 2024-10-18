# Laika for Stryker.NET

A small tool that runs [Stryker.NET](https://github.com/stryker-mutator/stryker-net) in the background when a source file changes.

## Installation

### Global installation

```dotnet tool install -g laika```

### Local installation

```dotnet tool install laika --create-manifest-if-needed```

## Prerequisites

- Stryker.NET must be installed [globally](https://stryker-mutator.io/docs/stryker-net/getting-started/#install-globally) or [locally in the project](https://stryker-mutator.io/docs/stryker-net/getting-started/#install-in-project).

## Usage

```dotnet laika [--impatient] [stryker arguments]```
