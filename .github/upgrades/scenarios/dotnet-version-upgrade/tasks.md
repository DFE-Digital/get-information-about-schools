# .NET 10 Upgrade Progress

## Overview

Migrate all projects in Web\EdubaseWeb.sln to .NET 10 using dependency-first development and an in-place web migration, with one production cutover. Preserve client contracts, downloads/reports and UI; use multi-targeting and temporary adapters only where justified. The plan awaits review; implementation has not started.

**Progress**: 0/6 tasks complete <progress value="0" max="100"></progress> 0%

## Tasks

- 🔲 01-readiness-and-compatibility: Establish toolchain, compatibility baselines and decision gates
- 🔲 02-configuration-and-dependencies: Map configuration and review assembly/dependency conflicts
- 🔲 03-sdk-project-conversion: Convert legacy project structure without changing target frameworks
- 🔲 04-net10-application-migration: Migrate project TFMs, dependencies and application behaviour
- 🔲 05-release-readiness: Prepare and rehearse the single-cutover release
- 🔲 06-solution-validation: Validate full solution, parity and handover
