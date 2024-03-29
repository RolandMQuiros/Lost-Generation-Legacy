using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace LostGen.Util {
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

        public bool Contains(string path) {
            return _groups.ContainsKey(path);
        }

        public void Add(string path) {
            string[] tokens = path.Split(_delimiter);
            _rootChildren.Add(tokens[0]);

            string partPath = tokens[0];
            for (int t = 1; t <= tokens.Length; t++) {
                HashSet<string> children;
                if (!_groups.TryGetValue(partPath, out children)) {
                    children = new HashSet<string>();
                    _groups.Add(partPath, children);
                }
                if (t < tokens.Length) {
                    partPath += _delimiter + tokens[t];
                    children.Add(partPath);
                }
            }
        }

        public void Add(IEnumerable<string> paths) {
            foreach (string path in paths) {
                Add(path);
            }
        }

        public string GetParent(string path) {
            int lastDelimiter = Math.Max(path.LastIndexOf(_delimiter), 0);
            return path.Substring(0, lastDelimiter); 
        }

        public string GetName(string path) {
            int lastDelimiter = Math.Max(path.LastIndexOf(_delimiter) + 1, 0);
            return path.Substring(lastDelimiter);
        }

        public IEnumerable<string> GetChildren(string path = "") {
            HashSet<string> children;
            if (path.Length == 0) {
                children = _rootChildren;
            } else {
                _groups.TryGetValue(path, out children);
            }
            return children;
        }

        public int GetLevel(string path) {
            return path.Count(c => c == _delimiter);
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
                    foreach (string child in children.OrderByDescending(s => s)) {
                        open.Push(child);
                    }
                }
            }
        }
        
        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }
    }
}