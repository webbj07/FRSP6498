using System;

namespace FRSP6498;

public class InputUtil
{
    /// <summary>Adds a label to any control</summary>
    /// <param name="toWrap">Control to add a label to</param>
    /// <param name="label">text in the label</param>
    /// <param name="orientation">how the stacklayout should be oriented <param/>
    /// <returns>A Horizontal StackLayout that has the label and the original control in the specified orientation</returns>
    public static StackLayout WrapWithLabel(IView toWrap, StackOrientation orientation, string label){
        StackLayout stack = new()
        {
            Orientation = orientation,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Spacing = 5
        };
        Label controlLabel = new() 
        { 
            Text = label,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center
        };
        stack.Add(controlLabel);
        stack.Add(toWrap);
        return stack;
    }

}
