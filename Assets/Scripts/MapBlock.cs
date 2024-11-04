public struct MapBlock
{
	public MapBlock(bool[,] block)
	{
		this.block = block;
	}

	public readonly bool[,] block;
}