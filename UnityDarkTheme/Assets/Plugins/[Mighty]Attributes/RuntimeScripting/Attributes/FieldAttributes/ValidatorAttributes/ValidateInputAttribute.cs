namespace MightyAttributes
{
    public class ValidateInputAttribute : BaseValidatorAttribute
    {
        public string CallbackName { get; }
        public string Message { get; }

        public ValidateInputAttribute(string callbackName, string message = null)
        {
            CallbackName = callbackName;
            Message = message;
        }
    }
}
