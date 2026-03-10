# MaxFlowAlgorithms

A .NET 9 library for computing maximum flow in directed graphs.

## Included Solvers

- `DinicSolver`
- `EdmondsKarpSolver`

Both implementations are exposed through `IMaxFlowSolver` and can be created with `MaxFlowSolverFactory`.

## Project Structure

- `MaxFlowAlgorithms/`: library project
- `MaxFlowAlgorithms.Test/`: NUnit test project
- `MaxFlowAlgorithms.slnx`: solution file

## Requirements

- .NET 9 SDK

## Build

```bash
dotnet build MaxFlowAlgorithms.slnx
```

## Run Tests

```bash
dotnet test MaxFlowAlgorithms.slnx
```

## Usage

```csharp
using MaxFlowAlgorithms;

var network = new FlowNetwork();

network.AddEdge("s", "a", 10);
network.AddEdge("s", "b", 5);
network.AddEdge("a", "t", 7);
network.AddEdge("b", "t", 5);

var factory = new MaxFlowSolverFactory(network);
var solver = factory.CreateSolver(MaxFlowSolverType.Dinic);

var maxFlow = solver.CalculateMaxFlow("s", "t");

Console.WriteLine(maxFlow); // 12
```

To switch algorithms, change the solver type:

```csharp
var solver = factory.CreateSolver(MaxFlowSolverType.EdmondsKarp);
```

## API Overview

### `FlowNetwork`

Use `FlowNetwork` to build a directed graph.

- `AddNode(string id)` adds a node if it does not already exist.
- `AddEdge(string fromId, string toId, int capacity)` adds a directed edge and creates missing nodes automatically.
- `Nodes` exposes the current node set.
- `Edges` exposes the current edge set.

### `IMaxFlowSolver`

```csharp
double CalculateMaxFlow(string source, string sink)
```

Returns the maximum flow between the source and sink node IDs.

## Test Coverage

The test suite covers:

- invalid source and sink arguments
- source equals sink
- disconnected graphs
- zero-capacity edges
- parallel edges
- multiple augmenting paths
- cyclic graphs
- a classic CLRS max-flow example

## Notes

- Graphs are directed.
- Capacities are added as integers.
- Missing source or sink nodes cause an `ArgumentException`.
