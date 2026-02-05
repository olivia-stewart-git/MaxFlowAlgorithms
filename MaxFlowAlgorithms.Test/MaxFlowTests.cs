namespace MaxFlowAlgorithms.Test
{
	[TestFixture(nameof(MaxFlowSolverType))]
	public class MaxFlowTests(MaxFlowSolverType maxFlowSolver)
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
			solver = factory.CreateSolver(maxFlowSolver);
		}
	}
}
