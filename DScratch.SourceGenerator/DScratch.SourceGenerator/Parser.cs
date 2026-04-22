using System;
using System.Collections.Generic;

namespace DScratch.SourceGenerator;

public class Parser(List<Token> tokens)
{
    private int current;
    
    public List<SchemaDefinitionPart> Parse()
    {
        var schemaParts = new List<SchemaDefinitionPart>();

        while (tokens[current].Type != TokenType.Eof)
        {
            if (tokens[current++].Type == TokenType.Export && tokens[current++].Type == TokenType.Const)
            {
                var part = ParseDefinition();
                if (part is not null)
                {
                    schemaParts.Add(part);
                }
            }
        }

        return schemaParts;
    }

    private SchemaDefinitionPart? ParseDefinition()
    {
        var schemaPart = new SchemaDefinitionPart
        {
            Nodes = []
        };

        if (tokens[current].Type != TokenType.Identifier) throw new InvalidOperationException();
        
        schemaPart.Name = tokens[current++].Text;

        if (schemaPart.Name == "schema") return null;
        
        if (tokens[current++].Type != TokenType.BraceOpen) throw new InvalidOperationException();
        
        while (tokens[current].Type != TokenType.BraceClose)
        {
            schemaPart.Nodes.Add(ParseSpec());
        }

        return schemaPart;
    }

    private Spec ParseSpec()
    {
        var spec = new Spec
        {
            Attrs = []
        };

        if (tokens[current].Type != TokenType.Identifier) throw new InvalidOperationException();
        spec.Name = tokens[current++].Text;

        if (tokens[current++].Type != TokenType.Colon) throw new InvalidOperationException();
        if (tokens[current++].Type != TokenType.BraceOpen) throw new InvalidOperationException();

        while (tokens[current].Type != TokenType.BraceClose)
        {
            if (tokens[current].Type != TokenType.Identifier) throw new InvalidOperationException();
            if (tokens[current++].Text is not "attrs")
            {
                var openBraces = 0;
                while (!(tokens[current].Type is TokenType.Comma or TokenType.BraceClose && openBraces == 0))
                {
                    if (tokens[current].Type is TokenType.BraceOpen or TokenType.ArrayOpen) openBraces++;
                    if (tokens[current].Type is TokenType.BraceClose or TokenType.ArrayClose) openBraces--;
                    current++;
                }
                if (tokens[current].Type == TokenType.Comma) current++;
                continue;
            }
            
            if (tokens[current++].Type != TokenType.Colon) throw new InvalidOperationException();
            if (tokens[current++].Type != TokenType.BraceOpen) throw new InvalidOperationException();

            while (tokens[current].Type != TokenType.BraceClose)
            {
                if (tokens[current].Type == TokenType.Comma) current++;
                spec.Attrs.Add(ParseAttr());
            }

            current++;
            if (tokens[current].Type == TokenType.Comma) current++;
        }

        current++;
        while (tokens[current].Type != TokenType.BraceClose && tokens[current++].Type != TokenType.Comma) { }
        return spec;
    }

    private Spec.Attr ParseAttr()
    {
        if (tokens[current].Type != TokenType.Identifier) throw new InvalidOperationException();
        var attr = new Spec.Attr
        {
            Name = tokens[current++].Text
        };

        if (tokens[current++].Type != TokenType.Colon) throw new InvalidOperationException();
        if (tokens[current++].Type != TokenType.BraceOpen) throw new InvalidOperationException();
    
        // First Param
        if (tokens[current].Type != TokenType.Identifier) throw new InvalidOperationException();
        if (tokens[current].Text is "validate")
        {
            current++;
            if (tokens[current++].Type != TokenType.Colon) throw new InvalidOperationException();
            attr.From((tokens[current++].Literal as string)!);
        }
        else if (tokens[current].Text is "default")
        {
            current++;
            if (tokens[current++].Type != TokenType.Colon) throw new InvalidOperationException();
            attr.HasDefaultValue = true;
            attr.DefaultValue = tokens[current++].Literal;
        }
        else
        {
            throw new InvalidOperationException();
        }
    
        if (tokens[current++].Type is not TokenType.Comma)
        {
            return attr;
        }
    
        // Second Param (Optional)
        if (tokens[current].Text is "validate")
        {
            current++;
            if (tokens[current++].Type != TokenType.Colon) throw new InvalidOperationException();
            attr.From((tokens[current++].Literal as string)!);
        }
        else if (tokens[current].Text is "default")
        {
            current++;
            if (tokens[current++].Type != TokenType.Colon) throw new InvalidOperationException();
            attr.HasDefaultValue = true;
            attr.DefaultValue = tokens[current++].Literal;
        }
        
        if (tokens[current++].Type != TokenType.BraceClose) throw new InvalidOperationException();

        return attr;
    }
}