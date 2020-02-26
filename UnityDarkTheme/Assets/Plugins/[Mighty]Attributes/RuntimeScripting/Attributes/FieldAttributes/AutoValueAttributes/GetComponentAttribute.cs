namespace MightyAttributes
{
    public class GetComponentAttribute : BaseSearchObjectAttribute
    {
        public GetComponentAttribute(bool executeInPlayMode = false) : base(false, executeInPlayMode)
        {
        }
    }
}