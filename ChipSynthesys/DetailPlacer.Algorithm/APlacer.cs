using PlaceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DetailPlacer.Algorithm
{
    public class APlacer : IDetailPlacer, IGlobalPlacer
    {
        public void Place(Design design, PlacementGlobal approximate, out PlacementDetail result)
        {
            throw new NotImplementedException();
        }

        public void Place(Design design, PlacementGlobal result)
        {
            throw new NotImplementedException();
        }
    }
}
