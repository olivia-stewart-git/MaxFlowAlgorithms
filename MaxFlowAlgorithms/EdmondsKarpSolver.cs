namespace MaxFlowAlgorithms
{
	/// <summary>
	/// Edmonds–Karp algorithm (Ford–Fulkerson using BFS).
	/// Time complexity: O(V * E^2)
	/// </summary>
	public class EdmondsKarpSolver(FlowNetwork network) : IMaxFlowSolver
	{
		private sealed class ResidualEdge
		{
			public int To;
			public int Rev;
			public int Cap;

			public ResidualEdge(int to, int rev, int cap)
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
			if (source == sink) return 0;

			var nodes = network.Nodes.ToList();
			var index = new Dictionary<string, int>();

			for (int i = 0; i < nodes.Count; i++)
				index[nodes[i].id] = i;

			if (!index.TryGetValue(source, out var s))
				throw new ArgumentException("Not found");

			if (!index.TryGetValue(sink, out var t))
				throw new ArgumentException("Not found");

			var graph = new List<ResidualEdge>[nodes.Count];
			for (int i = 0; i < graph.Length; i++)
				graph[i] = new List<ResidualEdge>();

			void AddEdge(int u, int v, int cap)
			{
				var fwd = new ResidualEdge(v, graph[v].Count, cap);
				var rev = new ResidualEdge(u, graph[u].Count, 0);
				graph[u].Add(fwd);
				graph[v].Add(rev);
			}

			foreach (var e in network.Edges)
			{
				if (e.Capacity < 0)
					throw new ArgumentOutOfRangeException(nameof(e.Capacity));

				if (e.Capacity == 0)
					continue;

				AddEdge(index[e.From.id], index[e.To.id], e.Capacity);
			}

			var parentV = new int[nodes.Count];
			var parentE = new int[nodes.Count];

			long maxFlow = 0;

			while (Bfs(graph, s, t, parentV, parentE))
			{
				int bottleneck = int.MaxValue;

				for (int v = t; v != s; v = parentV[v])
				{
					var pv = parentV[v];
					var ei = parentE[v];
					bottleneck = Math.Min(bottleneck, graph[pv][ei].Cap);
				}

				for (int v = t; v != s; v = parentV[v])
				{
					var pv = parentV[v];
					var ei = parentE[v];
					var edge = graph[pv][ei];

					edge.Cap -= bottleneck;
					graph[v][edge.Rev].Cap += bottleneck;
				}

				maxFlow += bottleneck;
			}

			return maxFlow;
		}

		private static bool Bfs(
			List<ResidualEdge>[] graph,
			int s,
			int t,
			int[] parentV,
			int[] parentE)
		{
			Array.Fill(parentV, -1);
			Array.Fill(parentE, -1);

			var queue = new Queue<int>();
			queue.Enqueue(s);
			parentV[s] = s;

			while (queue.Count > 0)
			{
				int u = queue.Dequeue();
				for (int i = 0; i < graph[u].Count; i++)
				{
					var e = graph[u][i];
					if (e.Cap > 0 && parentV[e.To] == -1)
					{
						parentV[e.To] = u;
						parentE[e.To] = i;

						if (e.To == t)
							return true;

						queue.Enqueue(e.To);
					}
				}
			}

			return false;
		}
	}
}
