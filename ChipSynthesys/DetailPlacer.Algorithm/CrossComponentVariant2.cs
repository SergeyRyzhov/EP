using System.Collections.Generic;

using PlaceModel;

namespace DetailPlacer.Algorithm
{
    /*public class CrossComponentVariant2 : CrossCompPlacer, IDetailPlacer
    {
        public override string ToString()
        {
            return "CrossComponent вариант 2";
        }

        public override void Place(Design design, PlacementGlobal approximate, out PlacementDetail result)
        {
            base.Place(design, approximate, out result);
            var myApproximate = CreateMyApproximate(result, design);

            int width = design.field.cellsx;
            int height = design.field.cellsy;
            int qtcells = width * height;
            int[] XCellCoord;
            int[] YCellCoord;
            List<List<Component>> compInCell = InitCompInCell(qtcells);
            List<Component> fixedComponents = new List<Component>();
            int[] ValueCell = CreatCompValueCells(qtcells);
            //var myApproximate = new PlacementGlobal(design);
            //myApproximate = CreateMyApproximate(startPlacement, design);
            result = new PlacementDetail(design);
            CreateCells(width, height, design, out XCellCoord, out YCellCoord, qtcells);
            foreach (Component comp in design.components)
            {
                FillCells(design, myApproximate, XCellCoord, YCellCoord, qtcells, ValueCell, height, width, compInCell);
                int ind = GetCellIndex((int)myApproximate.x[comp], (int)myApproximate.y[comp], XCellCoord, YCellCoord, qtcells);
                int Coord = BestCell(XCellCoord, YCellCoord, ValueCell, qtcells, ind, comp, design, myApproximate);
                result.x[comp] = XCellCoord[Coord];
                result.y[comp] = YCellCoord[Coord];
                result.placed[comp] = true;
                myApproximate.x[comp] = XCellCoord[Coord];
                myApproximate.y[comp] = YCellCoord[Coord];
                ValueCell[ind] = -1;
                Clear(qtcells, ValueCell, compInCell);
            }
        }
    }*/
}
