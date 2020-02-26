namespace MightyAttributes
{
    [Slider("MinIndex", "MaxIndex")]
    [Label("LabelName")]
    public class LabelledSliderAttribute : MightyWrapperAttribute
    {
        [CallbackName] public string MinIndex { get; }
        [CallbackName] public string MaxIndex { get; }

        [CallbackName] public string LabelName { get; }

        public LabelledSliderAttribute(string labelName, int maxIndex)
        {
            LabelName = labelName;
            MinIndex = "0";
            MaxIndex = maxIndex.ToString();
        }    
        
        public LabelledSliderAttribute(string labelName, int minIndex, int maxIndex)
        {
            LabelName = labelName;
            MinIndex = minIndex.ToString();
            MaxIndex = maxIndex.ToString();
        }  
        
        public LabelledSliderAttribute(string labelName, string maxIndex)
        {
            LabelName = labelName;
            MinIndex = "0";
            MaxIndex = maxIndex;
        }      
        
        public LabelledSliderAttribute(string labelName, string minIndex, string maxIndex)
        {
            LabelName = labelName;
            MinIndex = minIndex;
            MaxIndex = maxIndex;
        }
    }
}