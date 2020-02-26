namespace MightyAttributes
{
	public class TagObjectAttribute : BaseSearchObjectAttribute
	{
		public readonly string Tag;

		public TagObjectAttribute(string tag, bool includeInactive, bool executeInPlayMode) : base(includeInactive, executeInPlayMode)
		{
			Tag = tag;
		}
	}
}