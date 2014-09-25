using ChipSynthesys.Common.Randoms;

using PlaceModel;

namespace ChipSynthesys.Common.Generators
{
    /// <summary>
    /// Èíòåðôåéñ ãåíåðàòîðà Ñõåìû è Ðàçìåùåíèÿ
    /// </summary>
    public interface IGenerator
    {
        /// <summary>
        /// Ñëó÷àéíàÿ ãåíåðàöèÿ (ðàâíîìåðíûé çàêîí ðàñïðåäåëåíèÿ)
        /// </summary>
        /// <param name="components">×èñëî êîìïîíåíò</param>
        /// <param name="nets">×èñëî ñåòåé</param>
        /// <param name="maxNetSize">Ìàêñèìàëüíîå ÷èñëî êîìïîíåíò â öåïè</param>
        /// <param name="percent">Ïðîöåíò çàïîëíåííîñòè (0,100)</param>
        /// <param name="maxSizeX">Ìàêñèìàëüíàÿ øèðèíà ýëåìåíòà</param>
        /// <param name="maxSizeY">Ìàêñèìàëüíàÿ âûñîòà ýëåìåíòà</param>
        /// <param name="design">Ïîëó÷åííàÿ ñõåìà</param>
        void NextDesign(int components, int nets, int maxNetSize, int percent, int maxSizeX, int maxSizeY, out Design design);

        /// <summary>
        /// Ñëó÷àéíàÿ ãåíåðàöèÿ (çàäàííûé çàêîí ðàñïðåäåëåíèÿ)
        /// </summary>
        /// <param name="components">×èñëî êîìïîíåíò</param>
        /// <param name="nets">×èñëî ñåòåé</param>
        /// <param name="maxNetSize">Ãåíåðàòîð ñëó÷àíûõ äëèí öåïåé</param>
        /// <param name="percent">Ïðîöåíò çàïîëíåííîñòè (0,100)</param>
        /// <param name="maxSizeX">Ãåíåðàòîð ñëó÷àíîé ìàêñèìàëüíîé øèðèíû ýëåìåíòà</param>
        /// <param name="maxSizeY">Ãåíåðàòîð ñëó÷àíîé ìàêñèìàëüíîé âûñîòû ýëåìåíòà</param>
        /// <param name="design">Ïîëó÷åííàÿ ñõåìà</param>
        void NextDesign(int components, int nets, IRandom<int> maxNetSize, int percent,
            IRandom<int> maxSizeX, IRandom<int> maxSizeY, out Design design);

        void NextDesignWithPlacement(int components, int nets, int maxNetSize, int percent, int maxSizeX,
            int maxSizeY, int width, int height, out Design design, out PlacementGlobal placement);

        void NextDesignWithPlacement(int components, int nets, IRandom<int> maxNetSize, int percent,
            IRandom<int> maxSizeX, IRandom<int> maxSizeY, int width, int height, out Design design, out PlacementGlobal placement);
    }
}