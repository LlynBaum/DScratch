using System;
using System.Collections.Generic;
using System.Linq;

namespace DScratch.SourceGenerator;

public class Spec
{
    public string Name { get; set; } = null!;

    public List<Attr> Attrs { get; set; } = null!;

    public class Attr
    {
        public string Name { get; set; } = null!;
        
        public AttrType Type { get; set; }
        
        public object? DefaultValue { get; set; }
        
        public bool IsNullable { get; set; } = false;
        
        public bool HasDefaultValue { get; set; } = false;

        public void From(string str)
        {
            var types = str.Split('|');
            IsNullable = types.Any(t => t == "null");
            Type = GetType(types);
        }

        private static AttrType GetType(string[] types)
        {
            if (types.Any(t => t == "number")) return AttrType.Number;
            if (types.Any(t => t == "boolean")) return AttrType.Bool;
            if (types.Any(t => t == "string")) return AttrType.String;
            throw new InvalidOperationException();
        }

        public string GetTypeString()
        {
            var nullable = IsNullable ? "?" : "";
            return Type switch
            {
                AttrType.Number => "double" + nullable,
                AttrType.Bool => "bool" + nullable,
                AttrType.String => "string" + nullable,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public string GetDefaultValueString()
        {
            return  HasDefaultValue 
                ? $" = {DefaultValue?.ToString() ?? "null"}" 
                : "";
        }
    }
    
    public enum AttrType
    {
        Number,
        Bool,
        String
    }
}