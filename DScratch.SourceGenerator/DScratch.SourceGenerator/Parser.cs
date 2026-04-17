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
            if (tokens[current++].Type == TokenType.ConstExport)
            {
                current++; 
                schemaParts.Add(ParseDefinition());
            }
        }

        return schemaParts;
    }

    private SchemaDefinitionPart ParseDefinition()
    {
        var schemaPart = new SchemaDefinitionPart();

        if (tokens[current].Type != TokenType.Identifier) throw new InvalidOperationException();
        
        schemaPart.Name = (string)tokens[current++].Literal!;
        
        if (tokens[current++].Type != TokenType.Colon) throw new InvalidOperationException();
        if (tokens[current++].Type != TokenType.BraceOpen) throw new InvalidOperationException();
        
        while (tokens[current].Type != TokenType.BraceClose)
        {
            schemaPart.Nodes.Add(ParseNode());
        }

        return schemaPart;
    }

    private Node ParseNode()
    {
        var node = new Node
        {
            Attrs = []
        };

        if (tokens[current].Type != TokenType.Identifier) throw new InvalidOperationException();
        node.Name = (string)tokens[current++].Literal!;

        if (tokens[current++].Type != TokenType.Colon) throw new InvalidOperationException();
        if (tokens[current++].Type != TokenType.BraceOpen) throw new InvalidOperationException();

        while (tokens[current++].Type != TokenType.BraceClose)
        {
            if (tokens[current].Type != TokenType.Identifier) throw new InvalidOperationException();
            if (tokens[current++].Literal is not "attrs")
            {
                var openBraces = 0;
                while (tokens[current].Type != TokenType.Comma && openBraces == 0)
                {
                    if (tokens[current].Type == TokenType.BraceOpen) openBraces++;
                    if (tokens[current].Type == TokenType.BraceClose) openBraces--;
                    current++;
                }
                continue;
            }
            
            if (tokens[current++].Type != TokenType.Colon) throw new InvalidOperationException();
            if (tokens[current++].Type != TokenType.BraceOpen) throw new InvalidOperationException();

            while (tokens[current++].Type != TokenType.BraceClose)
            {
                if (tokens[current].Type != TokenType.Comma) current++;
                node.Attrs.Add(ParseAttr());
            }
            if (tokens[current++].Type != TokenType.BraceClose) throw new InvalidOperationException();
            if (tokens[current].Type != TokenType.Comma) current++;
        }

        return node;
    }

    private Node.Attr ParseAttr()
    {
        if (tokens[current].Type != TokenType.Identifier) throw new InvalidOperationException();
        var attr = new Node.Attr
        {
            Name = (string)tokens[current++].Literal!
        };

        if (tokens[current++].Type != TokenType.Colon) throw new InvalidOperationException();
        if (tokens[current++].Type != TokenType.BraceOpen) throw new InvalidOperationException();
    
        // First Param
        if (tokens[current].Type != TokenType.Identifier) throw new InvalidOperationException();
        if (tokens[current].Literal is "validate")
        {
            current++;
            if (tokens[current++].Type != TokenType.Colon) throw new InvalidOperationException();
            attr.From((tokens[current++].Literal as string)!);
        }
        else if (tokens[current].Literal is "default")
        {
            current++;
            if (tokens[current++].Type != TokenType.Colon) throw new InvalidOperationException();
            attr.HasDefaultValue = true;
            attr.DefaultValue = tokens[current++].Literal;
        }
    
        if (tokens[current++].Type != TokenType.Comma) throw new InvalidOperationException();
    
        // Second Param (Optional)
        if (tokens[current].Literal is "validate")
        {
            current++;
            if (tokens[current++].Type != TokenType.Colon) throw new InvalidOperationException();
            attr.From((tokens[current++].Literal as string)!);
        }
        else if (tokens[current].Literal is "default")
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