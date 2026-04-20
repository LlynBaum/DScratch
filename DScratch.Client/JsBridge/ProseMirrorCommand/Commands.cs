namespace DScratch.Client.JsBridge.ProseMirrorCommand;

public static class Commands
{
    public static class InsertNode // TODO: replace this with a generic "Transform" command, where you can define a transformation in C#
    {
        public const string TypeName = "insertNode";

        public static UpdateCommand HorizontalRule => new UpdateCommand(TypeName, "horizontalRule");
    }
    
    public static class SetBlockType
    {
        public const string TypeName = "setBlockType";
        
        public static UpdateCommand Paragraph => new UpdateCommand(TypeName, "paragraph");
        
        public static UpdateCommand Heading(ushort level) => new UpdateCommand(TypeName, "heading") { Attrs = new ()
        {
            {"level", level}
        }};
        
        public static UpdateCommand CodeBlock => new UpdateCommand(TypeName, "codeBlock");
    }
    
    public static class ToggleMark
    {
        public const string TypeName = "toggleMark";
        
        public static UpdateCommand Italic => new UpdateCommand(TypeName, "em");
        
        public static UpdateCommand Strong => new UpdateCommand(TypeName, "strong");
        
        public static UpdateCommand Link(string href) => new UpdateCommand(TypeName, "link") { Attrs = new()
        {
            {"href", href}
        }};
        
        public static UpdateCommand Code => new UpdateCommand(TypeName, "code");
    }
    
    public static class WrapIn
    {
        public const string TypeName = "wrapIn";
        
        public static UpdateCommand Blockquote => new UpdateCommand(TypeName, "blockquote");
    }

}