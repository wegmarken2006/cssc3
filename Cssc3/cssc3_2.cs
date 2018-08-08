using System;
using System.Collections.Generic;
using System.Linq;



namespace Cssc3
{

    static public partial class SC3
    {
        public interface INode {
            bool isNode();
        }

        public class NodeC : INode
        {
            bool INode.isNode() { return true; }
            public int nid { get; set; }
            public int value { get; set; }
        }

        public class NodeK : INode
        {
            bool INode.isNode() { return true; }
            public int nid { get; set; }
            public String name { get; set; }
            public int deflt { get; set; } = 0;
            public Rate rate { get; set; } = Rate.RateKr;

        }
        public class NodeU : INode
        {
            bool INode.isNode() { return true; }
            public int nid { get; set; }
            public String name { get; set; }
            public UgenL inputs { get; set; }
            public List<Rate> outputs { get; set; }
            public int ugenId { get; set; }
            public int special { get; set; } = 0;
            public Rate rate { get; set; } = Rate.RateKr;
        }
        public class FromPortC : IUgen
        {
            bool IUgen.isUgen() { return true; }
            public int port_nid { get; set; }
        }
        public class FromPortK : IUgen
        {
            bool IUgen.isUgen() { return true; }
            public int port_nid { get; set; }
        }
        public class FromPortU : IUgen
        {
            bool IUgen.isUgen() { return true; }
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

        public static IUgen mk_ugen(string name, UgenL inputs, RateList outputs,
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

        public static bool find_c_p(int val, INode node)
        {
            if (node is NodeC nc)
            {
                return val == nc.value;
            }
            throw new Exception("find_c_p");
        }

        public static Tuple<INode, Graph> push_c(int val, Graph gr)
        {
            var node = new NodeC { nid = gr.nextId + 1, value = val };
            var consts = new List<NodeC>();
            consts.Add(node);
            consts.AddRange(gr.constants);
            var gr1 = new Graph { nextId = gr.nextId + 1, constants = consts, controls = gr.controls, ugens = gr.ugens };
            return new Tuple<INode, Graph>(node, gr1);
        }

        public static Tuple<INode, Graph> mk_node_c(IUgen ugen, Graph gr)
        {
            try
            {
                var val = ((Constant<int>)ugen).value;
                foreach (var nd in gr.constants)
                {
                    if (find_c_p(val, nd))
                    {
                        return new Tuple<INode, Graph>(nd, gr);
                    }
                }
                return push_c(val, gr);

            }
            catch (Exception)
            {
                throw new Exception("make_node_c");
            }
        }

        public static bool find_k_p(String str, INode node)
        {
            if (node is NodeK nk)
            {
                return str == nk.name;
            }
            throw new Exception("find_k_p");
        }

        public static Tuple<INode, Graph> push_k_p(IUgen ugen, Graph gr)
        {
            if (ugen is Control ctrl1)
            {

                var node = new NodeK { nid = gr.nextId + 1, name = ctrl1.name, deflt = ctrl1.index, rate = ctrl1.rate };
                var contrs = new List<NodeK>();
                contrs.Add(node);
                contrs.AddRange(gr.controls);
                var gr1 = new Graph { nextId = gr.nextId + 1, constants = gr.constants, controls = contrs, ugens = gr.ugens };
                return new Tuple<INode, Graph>(node, gr1);
            }
            throw new Exception("push_k_p");
        }

        public static Tuple<INode, Graph> mk_node_k(IUgen ugen, Graph gr)
        {
            try
            {
                var name = ((Control)ugen).name;
                foreach (var nd in gr.controls)
                {
                    if (find_k_p(name, nd))
                    {
                        return new Tuple<INode, Graph>(nd, gr);
                    }
                }
                return push_k_p(ugen, gr);
            }
            catch (Exception)
            {
                throw new Exception("mk_node_k");
            }
        }

        public static bool find_u_p(Rate rate, String name, int id1, INode node)
        {
            if (node is NodeU nu)
            {
                return (rate == nu.rate) && (name == nu.name) && (id1 == nu.ugenId);
            }
            throw new Exception("find_u_p");
        }

        public static Tuple<INode, Graph> push_u(IUgen ugen, Graph gr)
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
                return new Tuple<INode, Graph>(node, gr1);
            }
            throw new Exception("push_u_p");
        }

        public static IUgen as_from_port(INode node)
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

        public static Tuple<List<INode>, Graph> acc(List<IUgen> ll, List<INode> nn, Graph gr)
        {
            if (ll.Count == 0)
            {
                nn.Reverse();
                return new Tuple<List<INode>, Graph>(nn, gr);
            }
            else
            {
                try
                {
                    var ng = mk_node(ll[0], gr);
                    var ng1 = ng.Item1;
                    var ng2 = ng.Item2;
                    nn.Insert(0, ng1);
                    return acc(ll.GetRange(1, ll.Count), nn, ng2);
                }
                catch (Exception)
                {
                    throw new Exception("mk_node_u acc");
                }
            }
        }
        public static Tuple<INode, Graph> mk_node_u(IUgen ugen, Graph gr)
        {
            if (ugen is Primitive pr1)
            {
                var ng = acc(pr1.inputs.l, new List<INode>(), gr);
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
                        return new Tuple<INode, Graph>(nd2, gnew);
                    }
                }
                var pr = new Primitive { name = name, inputs = inputs2, outputs = pr1.outputs, special = pr1.special, rate = rate };
                return push_u(pr, gnew);
            }
            throw new Exception("mk_node_u");
        }

        public static Tuple<INode, Graph> mk_node(IUgen ugen, Graph gr)
        {
            if (ugen is Constant<int>)
            {
                return mk_node_c(ugen, gr);
            }
            else if (ugen is Primitive)
            {
                return mk_node_u(ugen, gr);
            }
            else if (ugen is Mrg mg)
            {
                var gn = mk_node(mg.right, gr);
                var g1 = gn.Item2;
                return mk_node(mg.left, g1);

            }
            throw new Exception("mk_node");
        }
        public static NodeU sc3_implicit(int num)
        {
            var rates = new RateList();
            for (var ind = 1; ind < num + 1; ind++)
            {
                rates.Add(Rate.RateKr);
            }
            var node = new NodeU
            {
                nid = -1,
                name = "Control",
                inputs = new UgenL(),
                outputs = rates,
                ugenId = 0,
                special = 0,
                rate = Rate.RateKr
            };
            return node;
        }

        public static IUgen mrg_n(UgenL lst)
        {
            if (lst.l.Count == 1)
            {
                return lst.l[0];
            }
            else if (lst.l.Count == 2)
            {
                return new Mrg { left = lst.l[0], right = lst.l[1] };
            }
            else
            {
                var newLst = new UgenL();
                newLst.l.AddRange(lst.l.GetRange(1, lst.l.Count));
                return new Mrg { left = lst.l[0], right = mrg_n(newLst) };
            }
        }

        public static IUgen prepare_root(IUgen ugen)
        {
            if (ugen is Mce mc)
            {
                return mrg_n(mc.ugens);
            }
            else if (ugen is Mrg mg)
            {
                return new Mrg { left = prepare_root(mg.left), right = prepare_root(mg.right) };
            }
            else
            {
                return ugen;
            }
        }

        public static Graph empty_graph()
        {
            return new Graph { nextId = 0, constants = new List<NodeC>(), controls = new List<NodeK>(), ugens = new List<NodeU>() };
        }

        public static Graph synth(IUgen ugen)
        {
            try
            {
                var root = prepare_root(ugen);
                var gn = mk_node(root, empty_graph());
                var gr = gn.Item2;
                var cs = gr.constants;
                var ks = gr.controls;
                var us = gr.ugens;
                var us1 = us;
                us1.Reverse();
                if (ks.Count != 0)
                {
                    var node = sc3_implicit(ks.Count);
                    us1.Insert(0, node);
                }
                var grout = new Graph { nextId = -1, constants = cs, controls = ks, ugens = us1 };
                return grout;

            }
            catch (Exception)
            {
                throw new Exception("synth");
            }
        }

    }
}
