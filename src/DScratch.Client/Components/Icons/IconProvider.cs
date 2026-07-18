namespace DScratch.Client.Components.Icons;

public static class IconProvider
{
    private static readonly Dictionary<EditorIcon, string> SvgMap = new()
    {
        [EditorIcon.Bold] = "<svg xmlns=\"http://www.w3.org/2000/svg\" height=\"20px\" viewBox=\"0 -960 960 960\" width=\"20px\" fill=\"currentColor\"><path d=\"M272-200v-560h221q65 0 120 40t55 111q0 51-23 78.5T602-491q25 11 55.5 41t30.5 90q0 89-65 124.5T501-200H272Zm121-112h104q48 0 58.5-24.5T566-372q0-11-10.5-35.5T494-432H393v120Zm0-228h93q33 0 48-17t15-38q0-24-17-39t-44-15h-95v109Z\"/></svg>",
        
        [EditorIcon.Italic] = "<svg xmlns=\"http://www.w3.org/2000/svg\" height=\"20px\" viewBox=\"0 -960 960 960\" width=\"20px\" fill=\"currentColor\"><path d=\"M200-200v-100h160l120-360H320v-100h400v100H580L460-300h140v100H200Z\"/></svg>",
        
        [EditorIcon.Paragraph] = "<svg xmlns=\"http://www.w3.org/2000/svg\" height=\"20px\" viewBox=\"0 -960 960 960\" width=\"20px\" fill=\"currentColor\"><path d=\"M240-120v-720h280q100 0 170 70t70 170q0 100-70 170t-170 70H400v240H240Zm160-400h128q33 0 56.5-23.5T608-600q0-33-23.5-56.5T528-680H400v160Z\"/></svg>",

        [EditorIcon.Heading1] = "<svg xmlns=\"http://www.w3.org/2000/svg\" height=\"20px\" viewBox=\"0 -960 960 960\" width=\"20px\" fill=\"currentColor\"><path d=\"M200-280v-400h80v160h160v-160h80v400h-80v-160H280v160h-80Zm480 0v-320h-80v-80h160v400h-80Z\"/></svg>",
        
        [EditorIcon.Heading2] = "<svg xmlns=\"http://www.w3.org/2000/svg\" height=\"20px\" viewBox=\"0 -960 960 960\" width=\"20px\" fill=\"currentColor\"><path d=\"M120-280v-400h80v160h160v-160h80v400h-80v-160H200v160h-80Zm400 0v-160q0-33 23.5-56.5T600-520h160v-80H520v-80h240q33 0 56.5 23.5T840-600v80q0 33-23.5 56.5T760-440H600v80h240v80H520Z\"/></svg>",
        
        [EditorIcon.Heading3] = "<svg xmlns=\"http://www.w3.org/2000/svg\" height=\"20px\" viewBox=\"0 -960 960 960\" width=\"20px\" fill=\"currentColor\"><path d=\"M120-280v-400h80v160h160v-160h80v400h-80v-160H200v160h-80Zm400 0v-80h240v-80H600v-80h160v-80H520v-80h240q33 0 56.5 23.5T840-600v240q0 33-23.5 56.5T760-280H520Z\"/></svg>",
        
        [EditorIcon.Heading4] = "<svg xmlns=\"http://www.w3.org/2000/svg\" height=\"20px\" viewBox=\"0 -960 960 960\" width=\"20px\" fill=\"currentColor\"><path d=\"M120-280v-400h80v160h160v-160h80v400h-80v-160H200v160h-80Zm600 0v-120H520v-280h80v200h120v-200h80v200h80v80h-80v120h-80Z\"/></svg>",
        
        [EditorIcon.Heading5] = "<svg xmlns=\"http://www.w3.org/2000/svg\" height=\"20px\" viewBox=\"0 -960 960 960\" width=\"20px\" fill=\"currentColor\"><path d=\"M120-280v-400h80v160h160v-160h80v400h-80v-160H200v160h-80Zm400 0v-80h240v-80H520v-240h320v80H600v80h160q33 0 56.5 23.5T840-440v80q0 33-23.5 56.5T760-280H520Z\"/></svg>",
        
        [EditorIcon.Heading6] = "<svg xmlns=\"http://www.w3.org/2000/svg\" height=\"20px\" viewBox=\"0 -960 960 960\" width=\"20px\" fill=\"currentColor\"><path d=\"M120-280v-400h80v160h160v-160h80v400h-80v-160H200v160h-80Zm480 0q-33 0-56.5-23.5T520-360v-240q0-33 23.5-56.5T600-680h240v80H600v80h160q33 0 56.5 23.5T840-440v80q0 33-23.5 56.5T760-280H600Zm0-160v80h160v-80H600Z\"/></svg>",
        
        [EditorIcon.FormatColorReset] = "<svg xmlns=\"http://www.w3.org/2000/svg\" height=\"20px\" viewBox=\"0 -960 960 960\" width=\"20px\" fill=\"currentColor\"><path d=\"M800-436q0 36-8 69t-22 63l-62-60q6-17 9-34.5t3-37.5q0-47-17.5-89T650-600L480-768l-88 86-56-56 144-142 226 222q44 42 69 99.5T800-436Zm-8 380L668-180q-41 29-88 44.5T480-120q-133 0-226.5-92.5T160-436q0-51 16-98t48-90L56-792l56-56 736 736-56 56ZM480-200q36 0 68.5-10t61.5-28L280-566q-21 32-30.5 64t-9.5 66q0 98 70 167t170 69Zm-37-204Zm110-116Z\"/></svg>",

        [EditorIcon.FormatColorFill] = "<svg xmlns=\"http://www.w3.org/2000/svg\" height=\"20px\" viewBox=\"0 -960 960 960\" width=\"20px\" fill=\"currentColor\"><path d=\"m247-904 57-56 343 343q23 23 23 57t-23 57L457-313q-23 23-57 23t-57-23L153-503q-23-23-23-57t23-57l190-191-96-96Zm153 153L209-560h382L400-751Zm303.5 447.5Q680-327 680-360q0-21 12.5-45t27.5-45q9-12 19-25t21-25q11 12 21 25t19 25q15 21 27.5 45t12.5 45q0 33-23.5 56.5T760-280q-33 0-56.5-23.5ZM80 0v-160h800V0H80Z\"/></svg>"
    };

    public static string GetSvg(EditorIcon icon)
    {
        return SvgMap.TryGetValue(icon, out var svg) ? svg : string.Empty;
    }
}
