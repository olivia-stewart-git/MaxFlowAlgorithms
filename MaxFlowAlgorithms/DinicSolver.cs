namespace MaxFlowAlgorithms
{
	/// <summary>
	/// Dinics algorithm solution sourced from https://www.geeksforgeeks.org/dsa/dinics-algorithm-maximum-flow/ and adapted to 
	/// data structures
	/// </summary>
	/// <param name="network"></param>
	public class DinicSolver(FlowNetwork network) : IMaxFlowSolver
	{
		// Represents one direction of an edge in the residual graph.
		// Rev points back to the paired reverse edge so we can push flow back.
		private sealed class ResidualEdge
		{
			public int To;
			public int Rev;
			public double Cap;

			public ResidualEdge(int to, int rev, double cap)
			{
				To = to;
				Rev = rev;
				Cap = cap;
			}
		}

		public double CalculateMaxFlow(string source, string sink)
		{
			if (network is null) throw new ArgumentNullException(nameof(network));
			if (string.IsNullOrWhiteSpace(source)) throw new ArgumentNullException(nameof(source));
			if (string.IsNullOrWhiteSpace(sink)) throw new ArgumentNullException(nameof(sink));
			if (string.Equals(source, sink, StringComparison.Ordinal)) return 0;

			// Map node ids to integer indices so we can use array-based adjacency lists
			var nodes = network.Nodes.ToList();
			var index = new Dictionary<string, int>(StringComparer.Ordinal);
			for (var i = 0; i < nodes.Count; i++)
				index[nodes[i].id] = i;

			if (!index.TryGetValue(source, out var s)) throw new ArgumentException("Not found");
			if (!index.TryGetValue(sink, out var t)) throw new ArgumentException("Not found");

			// Build the residual graph - each original edge gets a forward (capacity) and
			// reverse (0 capacity) edge. The reverse edge lets us "undo" flow later.
			var g = new List<ResidualEdge>[nodes.Count];
			for (var i = 0; i < g.Length; i++) g[i] = new List<ResidualEdge>();

			void AddResidualEdge(int u, int v, double cap)
			{
				var fwd = new ResidualEdge(v, g[v].Count, cap);
				var rev = new ResidualEdge(u, g[u].Count, 0);
				g[u].Add(fwd);
				g[v].Add(rev);
			}

			foreach (var e in network.Edges)
			{
				if (!index.TryGetValue(e.From.id, out var u) || !index.TryGetValue(e.To.id, out var v))
					continue;
				if (e.Capacity < 0) throw new ArgumentOutOfRangeException(nameof(e.Capacity));
				if (e.Capacity == 0) continue;
				AddResidualEdge(u, v, e.Capacity);
			}

			// level[v] = BFS distance from source (used to build the level graph)
			// it[v] = current-arc index for DFS, avoids re-scanning dead-end edges
			var level = new int[nodes.Count];
			var it = new int[nodes.Count];

			// BFS to build a level graph - only edges going one level deeper are valid.
			// Returns false when sink is unreachable, meaning we're done.
			bool Bfs()
			{
				Array.Fill(level, -1);
				var q = new Queue<int>();
				level[s] = 0;
				q.Enqueue(s);
				while (q.Count > 0)
				{
					var v = q.Dequeue();
					foreach (var edge in g[v])
					{
						if (edge.Cap <= 0) continue;
						if (level[edge.To] >= 0) continue;
						level[edge.To] = level[v] + 1;
						q.Enqueue(edge.To);
					}
				}
				return level[t] >= 0;
			}

			// DFS along the level graph to find a blocking flow.
			// Uses current-arc optimisation (it[]) so each edge is visited at most once per phase.
			double Dfs(int v, double pushed)
			{
				if (pushed <= 0) return 0;
				if (v == t) return pushed;

				for (; it[v] < g[v].Count; it[v]++)
				{
					var e = g[v][it[v]];
					if (e.Cap <= 0) continue;
					if (level[e.To] != level[v] + 1) continue; // only follow level graph edges

					var tr = Dfs(e.To, Math.Min(pushed, e.Cap));
					if (tr <= 0) continue;

					// push flow forward and add it back to the reverse edge
					e.Cap -= tr;
					g[e.To][e.Rev].Cap += tr;
					return tr;
				}

				return 0;
			}

			// Main loop: each iteration builds a new level graph (BFS) then pushes as
			// much flow as possible through it (DFS). The shortest path from s->t grows
			// by at least 1 each phase, so this terminates in at most O(V) phases.
			double flow = 0;
			while (Bfs())
			{
				Array.Fill(it, 0);
				while (true)
				{
					var pushed = Dfs(s, double.PositiveInfinity);
					if (pushed <= 0) break;
					flow += pushed;
				}
			}

			return flow;
		}
	}
}
