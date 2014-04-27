using System;
using System.Collections.Generic;
using System.Linq;
using PlaceModel;

namespace DetailPlacer.Algorithm.CompontsOrderer.Impl
{
    public class NetAreaCompontsOrderer : ICompontsOrderer
    {
        protected class NetInfo
        {
            public NetInfo(Net net)
            {
                NetId = net.id;
            }

            public readonly int NetId;
            public double Criteria;
            public int Area;
        }

        protected class CompInfo
        {
            public CompInfo(Component component)
            {
                ComId = component.id;
            }

            public readonly int ComId;
            public double Criteria;
            public int Area;
        }

        protected class CompInfoComparer : IComparer<CompInfo>
        {
            public int Compare(CompInfo x, CompInfo y)
            {
                //-1 x < y хуже, 0 x == y, 1 x > y лучше

                if (x.Criteria < y.Criteria)
                {
                    return 1;
                }
                if (x.Criteria > y.Criteria)
                {
                    return -1;
                }
                if (x.Area > y.Area)
                {
                    return 1;
                }
                if (x.Area < y.Area)
                {
                    return -1;
                }
                return x.ComId < y.ComId ? 1 : -1;
            }
        }

        protected class NetInfoComparer : IComparer<NetInfo>
        {
            public int Compare(NetInfo x, NetInfo y)
            {
                //-1 x < y хуже, 0 x == y, 1 x > y лучше

                if (x.Criteria < y.Criteria)
                {
                    return 1;
                }
                if (x.Criteria > y.Criteria)
                {
                    return -1;
                }
                if (x.Area > y.Area)
                {
                    return 1;
                }
                if (x.Area < y.Area)
                {
                    return -1;
                }
                return x.NetId < y.NetId ? 1 : -1;
            }
        }

        public override string ToString()
        {
            return "Упорядочение компонент согласно вклада цепей в критерий";
        }

        public void SortComponents(Design design, PlacementGlobal approximate, PlacementDetail result, Component[] unplacedComponents,
            ref int[] perm)
        {
            var markedNets = new Dictionary<int, NetInfo>();
            for (int i = 0; i < design.nets.Length; i++)
            {
                Net net = design.nets[i];
                var info = new NetInfo(net) {Criteria = MarkNet(approximate, result, net), Area = MarkNet(net)};
                markedNets.Add(i, info);
            }

            var sortedComp = new SortedList<CompInfo, int>(new CompInfoComparer());
            for (int i = 0; i < unplacedComponents.Length; i++)
            {
                var com = unplacedComponents[i];
                Net[] nets = design.Nets(com);
                var info = new CompInfo(com)
                {
                    Area = nets.Sum(n => markedNets[n.id].Area),
                    Criteria = nets.Sum(n => markedNets[n.id].Criteria)
                };
                sortedComp.Add(info, i);
            }

            for (int i = 0; i < sortedComp.Values.Count; i++)
            {
                perm[i] = sortedComp.Values[i];
            }
        }

        private static int MarkNet(Net net)
        {
            return net.items.Sum(c => c.sizex*c.sizey);
        }

        private static double MarkNet(PlacementGlobal approximate, PlacementDetail result, Net net)
        {
            if (net.items.Length == 0)
            {
                return 0.0;
            }

            double mark = 0.0;

            double l;
            double r;
            double t;
            double b;
            var first = net.items[0];

            if (!result.placed[first])
            {
                l = approximate.x[first];
                r = approximate.x[first] + first.sizex;
                t = approximate.y[first];
                b = approximate.y[first] + first.sizey;
            }
            else
            {
                l = result.x[first];
                r = result.x[first] + first.sizex;
                t = result.y[first];
                b = result.y[first] + first.sizey;
            }

            foreach (var component in net.items)
            {
                if (!result.placed[component])
                {
                    l = Math.Min(l, approximate.x[component]);
                    r = Math.Max(r, approximate.x[component] + component.sizex);
                    t = Math.Min(t, approximate.y[component]);
                    b = Math.Min(b, approximate.y[component] + component.sizey);
                }
                else
                {
                    l = Math.Min(l, result.x[component]);
                    r = Math.Max(r, result.x[component] + component.sizex);
                    t = Math.Min(t, result.y[component]);
                    b = Math.Min(b, result.y[component] + component.sizey);
                }
            }

                mark += (r - l) + (b - t);
            return mark;
        }
    }
}