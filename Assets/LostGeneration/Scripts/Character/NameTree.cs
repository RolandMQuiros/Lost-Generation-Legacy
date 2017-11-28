using System.Collections;
using System.Collections.Generic;

public class NameTree : IEnumerable<string> {
    private char _delimiter;
    private Dictionary<string, HashSet<string>> _groups = new Dictionary<string, HashSet<string>>();
    private HashSet<string> _rootChildren = new HashSet<string>();

    public NameTree(char delimiter) {
        _delimiter = delimiter;
    }

    public NameTree(char delimiter, IEnumerable<string> paths) {
        _delimiter = delimiter;
        Add(paths);
    }

    public bool IsLeaf(string path) {
        HashSet<string> children;
        return _groups.TryGetValue(path, out children) && children.Count == 0;
    }

    public void Add(string path) {
        string[] tokens = path.Split(_delimiter);
        _rootChildren.Add(tokens[0]);

        string partPath;
        for (int t = 0; t < tokens.Length; t++) {
            HashSet<string> children;
            if (!_groups.TryGetValue(path, out children)) {
                children = new HashSet<string>();
                _groups.Add(path, children);
            }
            path += ':' + tokens[t];
            children.Add(path);
        }
    }

    public void Add(IEnumerable<string> paths) {
        foreach (string path in paths) {
            Add(path);
        }
    }

    public string GetName(string path) {
        int lastDelimiter = path.LastIndexOf(_delimiter);
        return path.Substring(lastDelimiter + 1);
    }

    public IEnumerator<string> GetEnumerator() {
        Stack<string> open = new Stack<string>();
        
        foreach (string child in _rootChildren) {
            open.Push(child);
        }

        while (open.Count > 0) {
            string path = open.Pop();
            yield return path;

            HashSet<string> children;
            if (_groups.TryGetValue(path, out children)) {
                foreach (string child in children) {
                    open.Push(child);
                }
            }
        }
    }
    
    IEnumerator IEnumerable.GetEnumerator() {
        return this.GetEnumerator();
    }
}