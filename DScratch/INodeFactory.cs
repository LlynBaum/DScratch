using DScratch.Nodes;

namespace DScratch;

public interface INodeFactory
{
    TextNode String(string value);

    CharNode Char(char value);
}