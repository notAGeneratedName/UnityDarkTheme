namespace MightyAttributes
{
    public class GetComponentsAttribute : BaseSearchObjectAttribute
    {
        public GetComponentsAttribute(bool executeInPlayMode = false) : base(false, executeInPlayMode)
        {
        }
    }
}