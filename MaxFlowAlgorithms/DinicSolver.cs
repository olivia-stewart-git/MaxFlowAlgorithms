namespace MaxFlowAlgorithms
{
	/// <summary>
	/// Dinics algorithm solution sourced from https://www.geeksforgeeks.org/dsa/dinics-algorithm-maximum-flow/ and adapted to 
	/// data structures
	/// </summary>
	/// <param name="network"></param>
	public class DinicSolver(FlowNetwork network) : IMaxFlowSolver
	{
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

			var nodes = network.Nodes.ToList();
			var index = new Dictionary<string, int>(StringComparer.Ordinal);
			for (var i = 0; i < nodes.Count; i++)
				index[nodes[i].id] = i;

			if (!index.TryGetValue(source, out var s)) throw new ArgumentException("Not found");
			if (!index.TryGetValue(sink, out var t)) throw new ArgumentException("Not found");

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

			var level = new int[nodes.Count];
			var it = new int[nodes.Count];

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

			double Dfs(int v, double pushed)
			{
				if (pushed <= 0) return 0;
				if (v == t) return pushed;

				for (; it[v] < g[v].Count; it[v]++)
				{
					var e = g[v][it[v]];
					if (e.Cap <= 0) continue;
					if (level[e.To] != level[v] + 1) continue;

					var tr = Dfs(e.To, Math.Min(pushed, e.Cap));
					if (tr <= 0) continue;

					e.Cap -= tr;
					g[e.To][e.Rev].Cap += tr;
					return tr;
				}

				return 0;
			}

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
