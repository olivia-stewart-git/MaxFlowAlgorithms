namespace MaxFlowAlgorithms
{
	public enum MaxFlowSolverType
	{
		Dinic,
		EdmondsKarp,
	}

	public class MaxFlowSolverFactory(FlowNetwork network)
	{
		public IMaxFlowSolver CreateSolver(MaxFlowSolverType solverType)
		{
			switch (solverType)
			{
				case MaxFlowSolverType.Dinic:
					return new DinicSolver(network);
				case MaxFlowSolverType.EdmondsKarp:
					return new EdmondsKarpSolver(network);
				default:
					throw new NotImplementedException();
			}
		}
	}
}
