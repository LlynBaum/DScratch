namespace DScratch.Client.Components;

public enum EditorIcon
{
    Bold,
    Italic,
    Paragraph,
    Heading1,
    Heading2,
    Heading3,
    Heading4,
    Heading5,
    Heading6,
    FormatColorReset,
    FormatColorFill,
    FormatAlignCenter,
    FormatAlignJustify,
    FormatAlignLeft,
    FormatAlignRight,
    FormatColorText,
    FormatListBulleted,
    FormatListNumbered,
    FormatStrikethrough,
    FormatUnderlined,
    FormatUnderlinedSquiggle,
    TextDecrease,
    TextIncrease
}

public static class EditorIconExtensions
{
    public static string GetFileName(this EditorIcon icon) => icon switch
    {
        EditorIcon.Bold => "format_bold.svg",
        EditorIcon.Italic => "format_italic.svg",
        EditorIcon.Paragraph => "format_paragraph.svg",
        EditorIcon.Heading1 => "format_h1.svg",
        EditorIcon.Heading2 => "format_h2.svg",
        EditorIcon.Heading3 => "format_h3.svg",
        EditorIcon.Heading4 => "format_h4.svg",
        EditorIcon.Heading5 => "format_h5.svg",
        EditorIcon.Heading6 => "format_h6.svg",
        EditorIcon.FormatColorReset => "format_color_reset.svg",
        EditorIcon.FormatColorFill => "format_color_fill.svg",
        EditorIcon.FormatAlignCenter => "format_align_center.svg",
        EditorIcon.FormatAlignJustify => "format_align_justify.svg",
        EditorIcon.FormatAlignLeft => "format_align_left.svg",
        EditorIcon.FormatAlignRight => "format_align_right.svg",
        EditorIcon.FormatColorText => "format_color_text.svg",
        EditorIcon.FormatListBulleted => "format_list_bulleted.svg",
        EditorIcon.FormatListNumbered => "format_list_numbered.svg",
        EditorIcon.FormatStrikethrough => "format_strikethrough.svg",
        EditorIcon.FormatUnderlined => "format_underlined.svg",
        EditorIcon.FormatUnderlinedSquiggle => "format_underlined_squiggle.svg",
        EditorIcon.TextDecrease => "text_decrease.svg",
        EditorIcon.TextIncrease => "text_increase.svg",
        _ => throw new ArgumentOutOfRangeException(nameof(icon), icon, null)
    };
}

