using Verse;

using UnityEngine;

namespace PawnPeeker
{
    class Settings : ModSettings
    {
        public class Peek
        {
            public static bool PawnsAnywhere;
            public static bool AndSelect;
            public static bool Selected;
        }

        public static Settings Get()
        {
            return LoadedModManager.GetMod<Mod>().GetSettings<Settings>();
        }

        public void DoWindowContents(Rect wrect)
        {
            Listing_Standard settings = new Listing_Standard();

            settings.Begin(wrect);

            /* Experimental settings. */
            settings.Label("WARNING: EXPERIMENTAL!");

            /* Peek pawns anywhere. */
            settings.CheckboxLabeled("Peek pawns anywhere",
                                     ref Peek.PawnsAnywhere,
                                     "If true, peek pawns anywhere. " +
                                     "If false, only peek pawns on the same map.");

            /* Peek and select. */
            settings.CheckboxLabeled("Peek and select",
                                     ref Peek.AndSelect,
                                     "If true, select the pawn being peeked. " +
                                     "If false, do not select the pawn being peeked. " +
                                     "The peeked pawn remains selected even after peeking and it is impossible to select multiple pawns while peeking.");

            /* Peek selected. */
            settings.CheckboxLabeled("Peek selected",
                                     ref Peek.Selected,
                                     "If true, peek the selected thing. " +
                                     "If false, do not peek the selected thing.");

            settings.End();
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref Peek.AndSelect, "peekAndSelect", false);
            Scribe_Values.Look(ref Peek.PawnsAnywhere, "peekPawnsAnywhere", false);
            Scribe_Values.Look(ref Peek.Selected, "peekSelected", false);
        }
    }
}
