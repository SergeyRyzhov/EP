using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using DetailPlacer.Algorithm.CompontsOrderer;
using DetailPlacer.Algorithm.PositionSearcher;
using DetailPlacer.Algorithm.PositionSearcher.Impl;
using DetailPlacer.Algorithm.PositionSorter;
using DetailPlacer.Algorithm.PositionSorter.Impl;
using DetailPlacer.Algorithm.PositionSorter.PositionComparer;
using DetailPlacer.Algorithm.PositionSorter.PositionComparer.Impl;
using DetailPlacer.Algorithm.CriterionPositionSearcher;
using ChipSynthesys.Common.Classes;
using System.Drawing;
using System.Threading;
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
                    int index = GetNomberCell(XCellCoord, YCellCoord, x[i], y[i]);
                    Array[i] = index;
                }

                bestCoord = GetBestCell(XCellCoord, YCellCoord, ValueCell, bestComp, design, approximate, result, Array);
                helper.PlaceComponent(bestComp, XCellCoord[bestCoord], YCellCoord[bestCoord]);
            }
            else
            {
                bestCoord = GetNomberCell(XCellCoord, YCellCoord, (int)approximate.x[bestComp], (int)approximate.y[bestComp]);
                helper.PlaceComponent(bestComp, XCellCoord[bestCoord], YCellCoord[bestCoord]);
            }

            

            return bestCoord;
        }

        public virtual void Place(Design design, PlacementGlobal approximate, out PlacementDetail result)
        {
            Width = design.field.cellsx;
            Height = design.field.cellsy;
            QtCells = Width * Height;

            int[] XCellCoord;
            int[] YCellCoord;

            int enumerator;
            int indCell = 0;


            List<List<Component>> compInCell = InitCompInCell(QtCells);
            List<Component> fixedComponents = new List<Component>();
           
            CreateCells(design, out XCellCoord, out YCellCoord);
            int[] ValueCell = CreatCompValueCells(QtCells);
            result = new PlacementDetail(design);

            Mask helper = new Mask(design, result);
            helper.BuildUp();

            FillCells(design, approximate,result, XCellCoord, YCellCoord, ValueCell, compInCell);
            do
            {
                enumerator = ValueCell.Max();
                indCell = Array.IndexOf(ValueCell, enumerator);

                if (enumerator > 1)
                {

                    Component bestComp = GetComponentWithMaxSquare(compInCell[indCell], result);
                    if (bestComp != null)
                    {
                        // выбор лучшей позиции 
                        int bestCoord = GetBestCellWitnComponentSearcher(helper,design,approximate,bestComp,result, XCellCoord, YCellCoord, ValueCell);                        
                        ClearCells(ValueCell, design, approximate, XCellCoord, YCellCoord, bestComp, compInCell);

                        // размещение  текущего компонента 
                        result.x[bestComp] = XCellCoord[bestCoord];
                        result.y[bestComp] = YCellCoord[bestCoord];                      
                        result.placed[bestComp] = true;

                        // очищение ячейки 
                        
                        ValueCell[indCell] = -1;
                        compInCell[indCell].Clear();
                        
                        // удаление и добавление значений для компоненты в ячейки

                        AddCells(ValueCell, design, result, bestCoord, XCellCoord, YCellCoord, bestComp, compInCell);

                        // фиксация компонента                      
                        fixedComponents.Add(bestComp);

                    }
                    else
                    { ValueCell[indCell] = -1; compInCell[indCell].Clear(); }
                }
            } while (fixedComponents.Count() != design.components.Count() && enumerator != 1);

            // размещение не размещенных компонентов

            foreach (Component comp in design.components)
            {
                if (result.placed[comp] == false)
                {
                    int indCurrent = GetNomberCell(XCellCoord, YCellCoord, (int)approximate.x[comp], (int)approximate.y[comp]);

                    int bestCell = GetBestCellWitnComponentSearcher(helper, design, approximate, comp, result, XCellCoord, YCellCoord, ValueCell);
                    ClearCells(ValueCell, design, approximate, XCellCoord, YCellCoord, comp, compInCell);


                    ValueCell[indCurrent] = -1;
                    compInCell[indCurrent].Clear();

                    result.x[comp] = XCellCoord[bestCell];
                    result.y[comp] = YCellCoord[bestCell];
                    result.placed[comp] = true;


                    AddCells(ValueCell, design, result, bestCell, XCellCoord, YCellCoord, comp, compInCell);
                }
            }
        }
    }
        
}
