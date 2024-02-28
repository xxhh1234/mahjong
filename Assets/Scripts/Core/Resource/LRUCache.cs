using System;
using System.Collections.Generic;
using UnityEngine;

namespace XH
{
    class LRUCache
    {
        private class Node
        {
            public string key;
            public object value;
            public Node prev;
            public Node next;

            public Node(string k, object v)
            {
                key = k;
                value = v;
                prev = null;
                next = null;
            }
        };
        private int n = 0;
        private Node head = null;
        private Dictionary<string, Node> cache = new Dictionary<string, Node>();
        
        public LRUCache(int capacity)
        { 
            n = capacity; 
            head = null;
        }

        public object Get(string key)
        {
            if (!cache.ContainsKey(key))
                return null;
            Node node = cache[key];
            Node newNode = new Node(node.key, node.value);
            DeleteNode(node);
            AddToHead(newNode);
            return newNode.value;
        }
        public void Put(string key, object value)
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
        public void ClearCache()
        {
            ResourceManager.Instance.UnLoad();
            cache.Clear();
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
            ResourceManager.Instance.UnLoad();
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