namespace MaxFlowAlgorithms
{
	public class FlowNetwork
	{
		Dictionary<string, Node> nodes = new Dictionary<string, Node>();

		public IReadOnlySet<Node> Nodes = new HashSet<Node>();
		public IReadOnlySet<Edge> Edges = new HashSet<Edge>();

		public FlowNetwork() { }

		public void AddNode(Node node)
		{
			if (node == null || string.IsNullOrWhiteSpace(node.id)) return;
			if (nodes.ContainsKey(node.id)) return;

			nodes[node.id] = node;
			((HashSet<Node>)Nodes).Add(node);
		}

		public Node? AddNode(string id)
		{
			if (string.IsNullOrWhiteSpace(id)) throw new ArgumentNullException("id");
			if (nodes.TryGetValue(id, out var existing)) return existing;

			var node = new Node(id, new List<Edge>());
			AddNode(node);
			return node;
		}

		public Node GetNode(string id)
		{
			nodes.TryGetValue(id, out var node);
			return node ?? throw new ArgumentException(
				"Not found");
		}

		public Edge AddEdge(Node from, Node to, int capacity)
		{
			if (from == null || to == null) throw new ArgumentNullException(nameof(from));
			if (!nodes.ContainsKey(from.id)) AddNode(from);
			if (!nodes.ContainsKey(to.id)) AddNode(to);

			var edge = new Edge(from, to, capacity);
			from.edges.Add(edge);
			((HashSet<Edge>)Edges).Add(edge);
			return edge;
		}

		public Edge AddEdge(string fromId, string toId, int capacity)
		{
			if (string.IsNullOrWhiteSpace(fromId) || string.IsNullOrWhiteSpace(toId)) throw new InvalidOperationException("Edge doesnt exist");
			var from = AddNode(fromId);
			var to = AddNode(toId);
			return AddEdge(from, to, capacity);
		}
	}
}
