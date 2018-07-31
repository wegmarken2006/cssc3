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
            public UgenL inputs { get; set; }
            public List<Rate> outputs { get; set; }
            public int ugenId { get; set; }
            public int special { get; set; } = 0;
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

        public static Tuple<NodeC, Graph> push_c(int val, Graph gr)
        {
            var node = new NodeC { nid = gr.nextId + 1, value = val };
            var consts = new List<NodeC>();
            consts.Add(node);
            consts.AddRange(gr.constants);
            var gr1 = new Graph { nextId = gr.nextId + 1, constants = consts, controls = gr.controls, ugens = gr.ugens };
            return new Tuple<NodeC, Graph>(node, gr1);
        }

        public static Tuple<NodeC, Graph> mk_node_c(object ugen, Graph gr)
        {
            try
            {
                var val = ((Constant<int>)ugen).value;
                foreach (var nd in gr.constants)
                {
                    if (find_c_p(val, (Object)nd))
                    {
                        return new Tuple<NodeC, Graph>(nd, gr);
                    }
                }
                return push_c(val, gr);

            }
            catch (Exception e)
            {
                throw new Exception("make_node_c");
            }
        }

        public static bool find_k_p(String str, object node)
        {
            if (node is NodeK nk)
            {
                return str == nk.name;
            }
            throw new Exception("find_k_p");
        }

        public static Tuple<NodeK, Graph> push_k_p(object ugen, Graph gr)
        {
            if (ugen is Control ctrl1)
            {

                var node = new NodeK { nid = gr.nextId + 1, name = ctrl1.name, deflt = ctrl1.index, rate = ctrl1.rate };
                var contrs = new List<NodeK>();
                contrs.Add(node);
                contrs.AddRange(gr.controls);
                var gr1 = new Graph { nextId = gr.nextId + 1, constants = gr.constants, controls = contrs, ugens = gr.ugens };
                return new Tuple<NodeK, Graph>(node, gr1);
            }
            throw new Exception("push_k_p");
        }

        public static Tuple<NodeK, Graph> mk_node_k(object ugen, Graph gr)
        {
            try
            {
                var name = ((Control)ugen).name;
                foreach (var nd in gr.controls)
                {
                    if (find_k_p(name, (object)nd))
                    {
                        return new Tuple<NodeK, Graph>(nd, gr);
                    }
                }
                return push_k_p(name, gr);
            }
            catch (Exception e)
            {
                throw new Exception("mk_node_k");
            }
        }

        public static bool find_u_p(Rate rate, String name, int id1, Object node)
        {
            if (node is NodeU nu)
            {
                return (rate == nu.rate) && (name == nu.name) && (id1 == nu.ugenId);
            }
            throw new Exception("find_u_p");
        }

        public static Tuple<NodeU, Graph> push_u(object ugen, Graph gr)
        {
            if (ugen is Primitive pr1)
            {
                var node = new NodeU
                {
                    nid = gr.nextId + 1,
                    name = pr1.name,
                    inputs = pr1.inputs,
                    outputs = pr1.outputs,
                    special = pr1.special,
                    ugenId = pr1.index,
                    rate = pr1.rate
                };

                var ugens = new List<NodeU>();
                ugens.Add(node);
                ugens.AddRange(gr.ugens);
                var gr1 = new Graph { nextId = gr.nextId + 1, constants = gr.constants, controls = gr.controls, ugens = ugens };
                return new Tuple<NodeU, Graph>(node, gr1);
            }
            throw new Exception("push_u_p");
        }

        public static object as_from_port(object node)
        {
            if (node is NodeC nc)
            {
                return new FromPortC { port_nid = nc.nid };
            }
            else if (node is NodeK nk)
            {
                return new FromPortK { port_nid = nk.nid };
            }
            else if (node is NodeU nu)
            {
                return new FromPortU { port_nid = nu.nid, port_idx = 0 };
            }
            throw new Exception("as_from_port");
        }

        public static Tuple<List<object>, Graph> acc(List<object> ll, List<object> nn, Graph gr)
        {
            if (ll.Count == 0)
            {
                nn.Reverse();
                return new Tuple<List<object>, Graph>(nn, gr);
            }
            else
            {
                try
                {
                    var ng = mk_node<object, object>(ll[0], gr);
                    var ng1 = ng.Item1;
                    var ng2 = ng.Item2;
                    nn.Insert(0, ng1);
                    return acc(ll.GetRange(1, ll.Count), nn, ng2);
                }
                catch (Exception e)
                {
                    throw new Exception("mk_node_u acc");
                }
            }
        }
        public static Tuple<NodeU, Graph> mk_node_u(object ugen, Graph gr)
        {
            if (ugen is Primitive pr1)
            {
                var ng = acc(pr1.inputs.l, new List<object>(), gr);
                var gnew = ng.Item2;
                var ng1 = ng.Item1;
                var inputs2 = new UgenL();
                foreach (var nd in ng1)
                {
                    inputs2.l.Add(as_from_port(nd));
                }
                var rate = pr1.rate;
                var name = pr1.name;
                var index = pr1.index;
                foreach (var nd2 in gnew.ugens)
                {
                    if (find_u_p(rate, name, index, nd2))
                    {
                        return new Tuple<NodeU, Graph>(nd2, gnew);
                    }
                }
                var pr = new Primitive{name=name,inputs=inputs2,outputs=pr1.outputs,special=pr1.special,rate=rate} ;
                return push_u(pr, gnew);
            }
            throw new Exception("mk_node_u");
        }

        public static Tuple<T, Graph> mk_node<T, U>(U ugen, Graph gr)
        {
            if (ugen is Constant<int>) {
                return (Tuple<T, Graph>)(object)mk_node_c(ugen, gr);
            }
            else if (ugen is Primitive) {
                return (Tuple<T, Graph>)(object)mk_node_u(ugen, gr);
            }
            else if (ugen is Mrg mg)
            {
                var gn = mk_node<T, U>((U)mg.right, gr);
                var g1 = gn.Item2;
                return (Tuple<T, Graph>)mk_node<T, U>((U)mg.left, g1);

            }
            throw new Exception("mk_node");
        }

    }
}
