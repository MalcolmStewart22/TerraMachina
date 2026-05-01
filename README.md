# TerraMachina

A procedural world generation and simulation engine. Build a world by hand or generate one procedurally, then watch human history emerge on top of it.

**Live demo:** https://terramachina.studionex.dev
---

## Status

**Active development.** The world Builder is functional and deployable. The simulation engine is the next major milestone.

This is a long-running learning project that doubles as a portfolio piece. Architecture and code quality are intentional priorities; feature completeness is intentionally not the priority.

---

## What works today

### World Builder (live)
Interactive editor for hand-crafting worlds:
- Multi-layer painting (elevation, biome) with adjustable brush size and power
- Tools: raise, lower, smooth, align elevation; biome selection
- Custom WebGL rendering pipeline (two-camera system, GPU buffer management)
  
### Procedural Generation (paused)
Substantial groundwork exists, currently parked while the simulation track takes priority:
- Icosphere geometry generation with neighbor graph (320,000 cells)
- Tectonic ridge generation between procedurally-placed plumes
- Spherical math primitives (rotation, displacement, great-circle arcs)
- Plate fill via BFS with Euler pole consistency checks
- SignalR streaming pipeline for real-time generation visualization

---

## Roadmap

### Current focus — Simulation on Handcrafted Worlds
The Builder is the input; simulation is the output.

**Milestone 1 — Builder produces complete World objects**  
Fill in the data fields the simulation needs (climate baseline, soil types, geological resources) so a painted world is ready for simulation, not just rendering.

**Milestone 2 — Base weather simulation**  
Tick-based weather modeling. Temperature, precipitation, wind patterns derived from latitude, elevation, and ocean proximity.

**Milestone 3 — Resource and biome dynamics**  
Biomes shift based on weather. Resource deposits deplete and replenish. World becomes responsive instead of static.

**Milestone 4 — Human settlement and population growth**  
Initial populations settle viable locations, grow based on local resources, expand into neighboring cells. First emergent behavior on top of the simulated world.

### Future work — procedural generation
Resume the WorldGen pipeline once the simulation track is in a stable state. Goal: generate believable landforms procedurally instead of requiring hand-painted worlds. Tectonic simulation, ocean fill, atmospheric circulation, climate baseline derivation, biome formation, resource placement.

---

## Tech Stack

**Backend:** C# / .NET, ASP.NET Core Web API, SignalR  
**Frontend:** React, Three.js, Tailwind CSS  
**Architecture:** Web Workers + OffscreenCanvas for rendering, SignalR for real-time streaming  
**Hosting:** Azure (Static Web Apps + App Service)

## Project Structure

```
src/
  TerraMachina.Abstractions/   Shared interfaces and data contracts
  TerraMachina.WorldGen/       Procedural world generation
  TerraMachina.WorldSim/       Simulation systems (in development)
  TerraMachina.Runtime/        ASP.NET Core API and SignalR hub

client/                        React + Three.js frontend
docs/                          Project documentation
```
