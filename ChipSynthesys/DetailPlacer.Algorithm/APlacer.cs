using PlaceModel;

namespace DetailPlacer.Algorithm
{
    /// <summary>
    /// Заглушка для размщения
    /// </summary>
    public class APlacer : IDetailPlacer, IGlobalPlacer
    {
        public void Place(Design design, PlacementGlobal approximate, out PlacementDetail result) 
            // точное размещение по заданной оценк
        {
            result = new PlacementDetail(design);
            foreach (var component in design.components)
            {
                result.placed[component] = true;
                result.x[component] = 1;
                result.y[component] = 2;
            }
        }

        public void Place(Design design, out PlacementGlobal result) 
            //оценка размещения
        {
            result = new PlacementGlobal(design);
            foreach (var component in design.components)
            {
                result.placed[component] = true;
                result.x[component] = 1;
                result.y[component] = 2;
            }
        }
    }
}
