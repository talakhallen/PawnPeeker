using System.Diagnostics;

namespace PawnPeeker
{
    class Debug
    {
        [Conditional("DEBUG")]
        public static void Log(string text, bool ignoreStopLoggingLimit = false)
        {
            Verse.Log.Message(text, ignoreStopLoggingLimit);
        }
    }
}
