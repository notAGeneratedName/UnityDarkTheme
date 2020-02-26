namespace MightyAttributes
{
    public class RequiredAttribute : BaseValidatorAttribute
    {
        public string Message { get; private set; }

        public RequiredAttribute(string message = null)
        {
            this.Message = message;
        }
    }
}
