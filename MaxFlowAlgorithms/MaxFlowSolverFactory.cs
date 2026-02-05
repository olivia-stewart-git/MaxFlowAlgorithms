namespace MaxFlowAlgorithms
{
	public enum MaxFlowSolverType
	{
		Dinic,
		Stewart,
		Libdy,
	}

	public class MaxFlowSolverFactory(FlowNetwork network)
	{
		public IMaxFlowSolver CreateSolver(MaxFlowSolverType solverType)
		{
			switch (solverType)
			{
				case MaxFlowSolverType.Dinic:
					return new DinicSolver(network);
				default:
					throw new NotImplementedException();
			}
		}
	}
}
