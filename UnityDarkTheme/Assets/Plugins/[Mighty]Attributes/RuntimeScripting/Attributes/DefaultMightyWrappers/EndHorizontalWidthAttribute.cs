namespace MightyAttributes
 {
     [EndHorizontal, Width("LabelWidth", "FieldWidth")]
     public class EndHorizontalWidthAttribute : MightyWrapperAttribute
     {
         public float LabelWidth { get; }
         public float? FieldWidth { get; }
 
         public EndHorizontalWidthAttribute(float labelWidth) => LabelWidth = labelWidth;
 
         public EndHorizontalWidthAttribute(float labelWidth, float fieldWidth)
         {
             LabelWidth = labelWidth;
             FieldWidth = fieldWidth;
         }
     }
 }