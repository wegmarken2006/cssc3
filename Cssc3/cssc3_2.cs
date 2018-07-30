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

        public static object mk_ugen(string name, UgenL inputs, RateList outputs,
        int ind = 0, int sp = 0, Rate rate = Rate.RateKr)
        {
            var pr1 = new Primitive { name = name, inputs = inputs, outputs = outputs, index = ind, special = sp, rate = rate };
            return proxify(pr1);
        }

        public static int node_c_value(NodeC node)
        {
            return node.value;
        }

        public static int node_k_default(NodeK node)
        {
            return node.deflt;
        }

        public static MMap mk_map(Graph graph)
        {
            var cs = new List<int>();
            var ks = new List<int>();
            var us = new List<int>();
            foreach (var el1 in graph.constants)
            {
                cs.Add(el1.nid);
            }
            foreach (var el2 in graph.controls)
            {
                ks.Add(el2.nid);
            }
            foreach (var el3 in graph.ugens)
            {
                us.Add(el3.nid);
            }

            return new MMap { cs = cs, ks = ks, us = us };
        }

        public static int fetch(int val, List<int> lst)
        {
            foreach (var it in lst.Select((Value, Index) => new { Value, Index }))
            {
                if (val == it.Value)
                {
                    return it.Index;
                }
            }
            return -1;
        }

        public static bool find_c_p(int val, object node)
        {
            if (node is NodeC nc)
            {
                return val == nc.value;
            }
            throw new Exception("find_c_p");
        }

        public static Tuple<object, Graph> push_c(int val, Graph gr)
        {
            var node = new NodeC { nid = gr.nextId + 1, value = val };
            var consts = new List<NodeC>();
            consts.Add(node);
            consts.AddRange(gr.constants);
            var gr1 = new Graph { nextId = gr.nextId + 1, constants = consts, controls = gr.controls, ugens = gr.ugens };
            return new Tuple<object, Graph>(node, gr1);
        }

        public static Tuple<object, Graph> mk_node_c(object ugen, Graph gr)
        {
            try
            {
                var val = ((Constant<int>)ugen).value;
                foreach (var nd in gr.constants)
                {
                    if (find_c_p(val, (Object)nd))
                    {
                        return new Tuple<object, Graph>(nd, gr);
                    }
                }
                return push_c(val, gr);

            }
            catch (Exception e)
            {
                throw new Exception("make_node_c");
            }
        }
    }
}
