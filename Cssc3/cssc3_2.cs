using System;
using System.Collections.Generic;
using System.Linq;



namespace Cssc3
{

    static public partial class SC3
    {
        public class NodeC
        {
            public int nid { get; set; }
            public int value { get; set; }
        }

        public class NodeK
        {
            public int nid { get; set; }
            public String name { get; set; }
            public int deflt { get; set; } = 0;
            public Rate rate { get; set; } = Rate.RateKr;

        }
        public class NodeU
        {
            public int nid { get; set; }
            public String name { get; set; }
            public List<Object> inputs { get; set; }
            public List<Rate> outputs { get; set; }
            public int special { get; set; }
            public int ugenId { get; set; } = 0;
            public Rate rate { get; set; } = Rate.RateKr;
        }
        public class FromPortC
        {
            public int port_nid { get; set; }
        }
        public class FromPortK
        {
            public int port_nid { get; set; }
        }
        public class FromPortU
        {
            public int port_nid { get; set; }
            public int port_idx { get; set; }
        }

        public class Graph
        {
            public int nextId { get; set; }
            public List<NodeC> constants { get; set; }
            public List<NodeK> controls { get; set; }
            public List<NodeU> ugens { get; set; }
        }

        public class MMap
        {
            public List<int> cs { get; set; }
            public List<int> ks { get; set; }
            public List<int> us { get; set; }
        }
        public class Input
        {
            public int u { get; set; }
            public int p { get; set; }
        }

    public static int node_c_value(NodeC node) {
        return node.value;
    }
        
    public static int  node_k_default(NodeK node) {
        return node.deflt;
    }

    public static MMap  mk_map(Graph graph) {
        var cs = new List<int>();
        var ks = new List<int>();
        var us = new List<int>();
        foreach (var el1 in graph.constants) {
            cs.Add(el1.nid);
        }
        foreach (var el2 in graph.controls) {
        	ks.Add(el2.nid);
        }            
        foreach (var el3 in graph.ugens) {
        	us.Add(el3.nid);
        }
            
        return new MMap{cs=cs, ks=ks, us=us};    	
    }




    }
}
