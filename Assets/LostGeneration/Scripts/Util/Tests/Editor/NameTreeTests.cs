using System;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;

using LostGen.Util;

namespace Tests {
    public class NameTreeTests {
        [Test]
        public void NameAndParent() {
            string path = "Top Group:Middle Group:Bottom Group";
            NameTree tree = new NameTree(':') { path };

            Assert.AreEqual("Bottom Group", tree.GetName(path));
            Assert.AreEqual("Top Group:Middle Group", tree.GetParent(path));
            Assert.AreEqual("Middle Group", tree.GetName(tree.GetParent(path)));
            Assert.AreEqual("Top Group", tree.GetParent(tree.GetParent(path)));
        }

        [Test]
        public void NoChildren() {
            string path = "Top";
            NameTree tree = new NameTree(':') { path };
            Assert.AreEqual("Top", tree.GetName(path));
            Assert.IsEmpty(tree.GetParent(path));
        }

        [Test]
        public void EndOnDelimiter() {
            string path = "GroupName:";
            NameTree tree = new NameTree(':') { path };
        
            Assert.IsEmpty(tree.GetName(path));
            Assert.AreEqual("GroupName", tree.GetParent(path));
        }

        [Test]
        public void Leaf() {
            string path = "Top Group:Middle Group:Bottom Group";
            NameTree tree = new NameTree(':') { path };

            Assert.IsTrue(tree.IsLeaf(path));
        }

        [Test]
        public void Walk() {
            string path = "Abc:Def:Geh";
            NameTree tree = new NameTree(':') { path };

            IEnumerator<string> walk = tree.GetEnumerator();

            Assert.IsTrue(walk.MoveNext());
            Assert.AreEqual("Abc", walk.Current);
            Assert.IsTrue(walk.MoveNext());
            Assert.AreEqual("Abc:Def", walk.Current);
            Assert.IsTrue(walk.MoveNext());
            Assert.AreEqual("Abc:Def:Geh", walk.Current);
            Assert.IsFalse(walk.MoveNext());
        }
    }
}