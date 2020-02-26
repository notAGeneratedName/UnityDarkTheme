namespace MightyAttributes
{
    public class AnimatorParameterAttribute : BaseAutoValueAttribute
    {
        public string ParameterName { get; }

        public bool NameAsCallback { get; }

        public AnimatorParameterAttribute(string parameterName, bool nameAsCallback = false, bool executeInPlayMode = false) : 
            base(executeInPlayMode)
        {
            ParameterName = parameterName;
            NameAsCallback = nameAsCallback;
        }
    }
}