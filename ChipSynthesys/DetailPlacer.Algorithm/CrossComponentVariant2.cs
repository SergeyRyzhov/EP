using System.Collections.Generic;
using System.Linq;
using DetailPlacer.Algorithm.PositionSearcher;
using DetailPlacer.Algorithm.PositionSearcher.Impl;
using PlaceModel;

namespace DetailPlacer.Algorithm
{
    public class CrossComponentVariant2 : CrossCompPlacer, IDetailPlacer
    {
        public override string ToString()
        {
            return "CrossComponent вариант 2";
        }
        protected readonly IPositionSearcher m_positionSearcher;

        public CrossComponentVariant2()
        {
            m_positionSearcher = new SpiralPositionSearcher();
        }
        public virtual int GetBestCellWitnComponentSearcher(Mask helper, Design design, PlacementGlobal approximate, Component bestComp, PlacementDetail result, int[] XCellCoord, int[] YCellCoord, int[] ValueCell)
        {
            int bestCoord = 0;
            int[] x = new int[m_positionSearcher.PositionAmount];
            int[] y = new int[m_positionSearcher.PositionAmount];
            int[] Array;
            if (m_positionSearcher.AlvailablePositions(helper, bestComp, (int)(approximate.x[bestComp]), (int)approximate.y[bestComp], x, y))
            {
                Array = new int[x.Length];
                for (int i = 0; i < x.Length; i++)
                {
                    int index = (x[i] - design.field.beginx) + (y[i] - design.field.beginy) * design.field.cellsx;
                    Array[i] = index;
                }

                bestCoord = GetBestCell(XCellCoord, YCellCoord, ValueCell, bestComp, design, approximate, result, Array);
                helper.PlaceComponent(bestComp, XCellCoord[bestCoord], YCellCoord[bestCoord]);
            }
            else
            {
                bestCoord = ((int)approximate.x[bestComp] - design.field.beginx) + ((int)approximate.y[bestComp] - design.field.beginy) * design.field.cellsx;
                helper.PlaceComponent(bestComp, XCellCoord[bestCoord], YCellCoord[bestCoord]);
            }
            return bestCoord;
        }

        public void Place(Design design, PlacementGlobal approximate, out PlacementDetail result)
        {
            Width = design.field.cellsx;
            Height = design.field.cellsy;
            QtCells = Width * Height;
            int[] XCellCoord;
            int[] YCellCoord;
            int enumerator = 0;
            int indCell = 0;
            int fixcomp = 0;
            List<List<Component>> compInCell = InitCompInCell(QtCells);
            CreateCells(design, out XCellCoord, out YCellCoord);
            int[] ValueCell = CreatCompValueCells(QtCells);
            result = new PlacementDetail(design);
            Mask helper = new Mask(design, result);
            helper.BuildUp();
            FillCells(design, approximate, result, XCellCoord, YCellCoord, ValueCell, compInCell);

            do
            {
                enumerator = 0;
                for (int i = 0; i < QtCells; i++)
                {
                    if (ValueCell[i] > enumerator && ValueCell[i] > 1)
                    {
                        enumerator = ValueCell[i];
                        indCell = i;
                    }
                }

                if (enumerator > 1)
                {   
                    Component bestComp = GetComponentWithMaxSquare(compInCell[indCell], result);
                    if (bestComp != null)
                    {

                        ClearCells(ValueCell, design, approximate, XCellCoord, YCellCoord, bestComp, compInCell);
                        int bestCoord = GetBestCellWitnComponentSearcher(helper, design, approximate, bestComp, result, XCellCoord, YCellCoord, ValueCell);

                        result.x[bestComp] = XCellCoord[bestCoord];
                        result.y[bestComp] = YCellCoord[bestCoord];
                        result.placed[bestComp] = true;

                        ValueCell[indCell] = -1;
                        //compInCell[indCell].Clear();

                        fixcomp++;

                    }
                    else
                    { 
                        ValueCell[indCell] = -1;
                       // compInCell[indCell].Clear(); 
                    }

                }
            } while (fixcomp != design.components.Count() && enumerator != 0);


            foreach (Component comp in design.components)
            {
                if (result.placed[comp] == false)
                {
                    int bestCell = GetBestCellWitnComponentSearcher(helper, design, approximate, comp, result, XCellCoord, YCellCoord, ValueCell);
                    result.x[comp] = XCellCoord[bestCell];
                    result.y[comp] = YCellCoord[bestCell];
                    result.placed[comp] = true;
                }
            }
        }
    }

}
