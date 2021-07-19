using System.Diagnostics;

namespace PawnPeeker
{
    class Debug
    {
        [Conditional("DEBUG")]
        public static void Log(string text)
        {
            Verse.Log.Message(text);
        }
    }
}
