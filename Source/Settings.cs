using Verse;

using UnityEngine;

namespace PawnPeeker
{
    class Settings : ModSettings
    {
        public class Hover
        {
            public static float StartDelaySeconds;
            public static float StartDelayTimeoutSeconds;
        }

        public class Peek
        {
            public static float LingerTimeSeconds;
            public static bool PawnsAnywhere;
            public static bool Select;
        }

        public static Settings Get()
        {
            return LoadedModManager.GetMod<PawnPeeker.Mod>().GetSettings<Settings>();
        }

        public void DoWindowContents(Rect wrect)
        {
            Listing_Standard settings = new Listing_Standard();

            settings.Begin(wrect);

            /* Hover start delay. */
            settings.Label(string.Format("Hover start delay in seconds: {0:0.00}",
                                         Hover.StartDelaySeconds),
                           -1,
                           "Number of seconds to wait while hovering over a pawn's portrait before peeking that pawn.");
            Hover.StartDelaySeconds = settings.Slider(Hover.StartDelaySeconds, 0.0f, 10.0f);

            /* Hover start delay timeout. */
            settings.Label(string.Format("Hover start delay timeout in seconds: {0:0.00}",
                                         Hover.StartDelayTimeoutSeconds),
                           -1,
                           "Number of seconds to wait while not hovering over a pawn's portrait before timing out and resetting the hover start delay.");
            float hoverStartWaitTimeoutSeconds = settings.Slider(Hover.StartDelayTimeoutSeconds, 0.0f, 10.0f);

            /* Peek linger time. */
            settings.Label(string.Format("Peek linger time in seconds: {0:0.00}",
                                         Peek.LingerTimeSeconds),
                           -1,
                           "Number of seconds to linger on a pawn while not hovering over that pawn's portrait.");
            float hoverLingerTimeSeconds = settings.Slider(Peek.LingerTimeSeconds, 0.0f, 10.0f);

            if (hoverStartWaitTimeoutSeconds != Hover.StartDelayTimeoutSeconds)
            {
                if (hoverLingerTimeSeconds > hoverStartWaitTimeoutSeconds)
                {
                    hoverLingerTimeSeconds = hoverStartWaitTimeoutSeconds;
                }
            }

            if (hoverLingerTimeSeconds != Peek.LingerTimeSeconds)
            {
                if (hoverStartWaitTimeoutSeconds < hoverLingerTimeSeconds)
                {
                    hoverStartWaitTimeoutSeconds = hoverLingerTimeSeconds;
                }
            }

            Peek.LingerTimeSeconds = hoverLingerTimeSeconds;
            Hover.StartDelayTimeoutSeconds = hoverStartWaitTimeoutSeconds;

            if (Peek.LingerTimeSeconds > Hover.StartDelayTimeoutSeconds)
            {
                Log.Error("Peek linger time must be less than or equal to hover start delay timeout!");

                Peek.LingerTimeSeconds = Hover.StartDelayTimeoutSeconds;
            }

            /* Experimental settings. */
            settings.Label("WARNING: EXPERIMENTAL!");

            /* Peek pawns anywhere. */
            settings.CheckboxLabeled("Peek pawns anywhere",
                                     ref Peek.PawnsAnywhere,
                                     "If true, peek pawns anywhere. " +
                                     "If false, only peek pawns on the same map.");

            /* Peek select. */
            settings.CheckboxLabeled("Peek and select",
                                     ref Peek.Select,
                                     "If true, select the pawn being peeked. " +
                                     "If false, do not select the pawn being peeked. " +
                                     "The peeked pawn remains selected even after peeking and it is impossible to select multiple pawns while peeking.");

            settings.End();
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref Peek.Select, "peekAndSelect", false);

            Scribe_Values.Look(ref Peek.PawnsAnywhere, "peekPawnsAnywhere", false);

            Scribe_Values.Look(ref Hover.StartDelaySeconds, "hoverStartDelaySeconds", 0.5f);
            Scribe_Values.Look(ref Peek.LingerTimeSeconds, "hoverLingerTimeSeconds", 0.5f);
            Scribe_Values.Look(ref Hover.StartDelayTimeoutSeconds, "hoverStartDelayTimeoutSeconds", 0.5f);
        }
    }
}
