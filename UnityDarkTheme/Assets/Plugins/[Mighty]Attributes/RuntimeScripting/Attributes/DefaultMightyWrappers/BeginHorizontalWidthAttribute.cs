namespace MightyAttributes
 {
     [BeginHorizontal, Width("LabelWidth", "FieldWidth")]
     public class BeginHorizontalWidthAttribute : MightyWrapperAttribute
     {
         public float LabelWidth { get; }
         public float? FieldWidth { get; }
 
         public BeginHorizontalWidthAttribute(float labelWidth) => LabelWidth = labelWidth;
 
         public BeginHorizontalWidthAttribute(float labelWidth, float fieldWidth)
         {
             LabelWidth = labelWidth;
             FieldWidth = fieldWidth;
         }
     }
 }