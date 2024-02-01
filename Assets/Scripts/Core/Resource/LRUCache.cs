using System;
using System.Collections.Generic;

namespace XH
{
    class LRUCache<TKey, TValue> where TValue : class
    {
        private class Node
        {
            public TKey key;
            public TValue value;
            public Node prev;
            public Node next;

            public Node(TKey k, TValue v)
            {
                key = k;
                value = v;
                prev = null;
                next = null;
            }
        };
        private int n = 0;
        private Node head = null;
        private Dictionary<TKey, Node> cache = new Dictionary<TKey, Node>();
        
        public LRUCache(int capacity)
        { 
            n = capacity; 
            head = null;
        }
        
        public TValue Get(TKey key)
        {
            if (!cache.ContainsKey(key))
                return null;
            Node node = cache[key];
            Node newNode = new Node(node.key, node.value);
            DeleteNode(node);
            AddToHead(newNode);
            return newNode.value;
        }

        public void Put(TKey key, TValue value)
        {
            if (Get(key) != null)
            {
                cache[key].value = value;
                return;
            }
            Node node = new Node(key, value);
            if (cache.Count == n)
                DeleteNode(head.next);
            AddToHead(node);
        }

        private void DeleteNode(Node node)
        {
            if(node == null)
                return;
            Node p = node.next;
            Node q = node.prev;
            p.prev = q;
            q.next = p;

            cache.Remove(node.key);
            if (node == head)
            {
                if (node == node.prev)
                    head = null;
                else
                    head = q;
            }
            node = null;
        }

        private void AddToHead(Node node)
        {
            if (node == null)
                return;
            if (head == null)
            {
                node.next = node;
                node.prev = node;
            }
            else
            {
                Node p = head.next;
                head.next = node;
                p.prev = node;

                node.next = p;
                node.prev = head;
            }
            cache[node.key] = node;
            head = node;
        }
    }
}