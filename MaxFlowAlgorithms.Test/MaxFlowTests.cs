namespace MaxFlowAlgorithms.Test
{
	[TestFixture(MaxFlowSolverType.Dinic)]
	[TestFixture(MaxFlowSolverType.EdmondsKarp)]
	public class MaxFlowTests(MaxFlowSolverType solverType)
	{
		FlowNetwork network;
		IMaxFlowSolver solver;

		[SetUp]
		public void SetUp()
		{
			network = new FlowNetwork();
			var factory = new MaxFlowSolverFactory(network);
			solver = factory.CreateSolver(solverType);
		}

		[Test]
		public void TestInvalidNodesArgumentException()
		{
			Assert.That(() => solver.CalculateMaxFlow("non-existent", "non-existent2"), Throws.ArgumentException);
		}

		[Test]
		public void SourceEqualsSink_MaxFlowIs0()
		{
			network.AddNode("s");
			Assert.That(solver.CalculateMaxFlow("s", "s"), Is.EqualTo(0));
		}

		[Test]
		public void NoEdges_MaxFlowIs0()
		{
			network.AddNode("s");
			network.AddNode("t");
			Assert.That(solver.CalculateMaxFlow("s", "t"), Is.EqualTo(0));
		}

		[Test]
		public void DisconnectedGraph_MaxFlowIs0()
		{
			network.AddEdge("s", "a", 5);
			network.AddEdge("b", "t", 7);
			Assert.That(solver.CalculateMaxFlow("s", "t"), Is.EqualTo(0));
		}

		[Test]
		public void SingleEdge_MaxFlowEqualsCapacity()
		{
			network.AddEdge("s", "t", 7);
			Assert.That(solver.CalculateMaxFlow("s", "t"), Is.EqualTo(7));
		}

		[Test]
		public void ZeroCapacityEdge_DoesNotContribute()
		{
			network.AddEdge("s", "t", 0);
			Assert.That(solver.CalculateMaxFlow("s", "t"), Is.EqualTo(0));
		}

		[Test]
		public void TwoEdgesInSeries_BottleneckLimitsFlow()
		{
			// s->a (10), a->t (3) => 3
			network.AddEdge("s", "a", 10);
			network.AddEdge("a", "t", 3);
			Assert.That(solver.CalculateMaxFlow("s", "t"), Is.EqualTo(3));
		}

		[Test]
		public void ParallelEdges_BothCount()
		{
			// Two parallel edges s->t: 3 and 5 => 8
			network.AddEdge("s", "t", 3);
			network.AddEdge("s", "t", 5);
			Assert.That(solver.CalculateMaxFlow("s", "t"), Is.EqualTo(8));
		}

		[Test]
		public void TwoDisjointPaths_MaxFlowIsSum()
		{
			// s->a->t (3) and s->b->t (5) => 8
			network.AddEdge("s", "a", 3);
			network.AddEdge("a", "t", 3);
			network.AddEdge("s", "b", 5);
			network.AddEdge("b", "t", 5);
			Assert.That(solver.CalculateMaxFlow("s", "t"), Is.EqualTo(8));
		}

		[Test]
		public void DiamondGraph_RespectsSharedSourceAndSinkCaps()
		{
			// s->a 10, s->b 10, a->t 4, b->t 6
			// => 10 total limited by out of middle nodes to t: 4+6
			network.AddEdge("s", "a", 10);
			network.AddEdge("s", "b", 10);
			network.AddEdge("a", "t", 4);
			network.AddEdge("b", "t", 6);
			Assert.That(solver.CalculateMaxFlow("s", "t"), Is.EqualTo(10));
		}

		[Test]
		public void CycleInGraph_DoesNotBreakCorrectness()
		{
			// There is a cycle a<->b. Max flow should still be 5.
			network.AddEdge("s", "a", 5);
			network.AddEdge("a", "b", 100);
			network.AddEdge("b", "a", 100);
			network.AddEdge("b", "t", 5);
			Assert.That(solver.CalculateMaxFlow("s", "t"), Is.EqualTo(5));
		}

		[Test]
		public void MultipleAugmentsNeeded_UsesDifferentPaths()
		{
			// First path s->a->t (1), second path s->b->t (1), plus cross edges.
			network.AddEdge("s", "a", 1);
			network.AddEdge("a", "t", 1);
			network.AddEdge("s", "b", 1);
			network.AddEdge("b", "t", 1);
			network.AddEdge("a", "b", 1);
			network.AddEdge("b", "a", 1);
			Assert.That(solver.CalculateMaxFlow("s", "t"), Is.EqualTo(2));
		}

		[Test]
		public void ClassicCormenExample_MaxFlowIs23()
		{
			// CLRS (directed) max flow = 23
			network.AddEdge("s", "v1", 16);
			network.AddEdge("s", "v2", 13);
			network.AddEdge("v1", "v2", 10);
			network.AddEdge("v2", "v1", 4);
			network.AddEdge("v1", "v3", 12);
			network.AddEdge("v2", "v4", 14);
			network.AddEdge("v3", "v2", 9);
			network.AddEdge("v4", "v3", 7);
			network.AddEdge("v3", "t", 20);
			network.AddEdge("v4", "t", 4);

			Assert.That(solver.CalculateMaxFlow("s", "t"), Is.EqualTo(23));
		}
	}
}
