namespace MaxFlowAlgorithms.Test
{
	[TestFixture(MaxFlowSolverType.Dinic)]
	[TestFixture(MaxFlowSolverType.EdmondsKarp)]
	public class MaxFlowTests(MaxFlowSolverType solverType)
	{
		[Test]
		public void TestInvalidNodesArgumentException()
		{
			Assert.That(() => solver.CalculateMaxFlow("non-existent", "non-existent2"), Throws.ArgumentException);
		}

		FlowNetwork network;

		IMaxFlowSolver solver;

		[SetUp]
		public void SetUp()
		{
			network = new FlowNetwork();
			var factory = new MaxFlowSolverFactory(network);
			solver = factory.CreateSolver(solverType);
		}
	}
}
