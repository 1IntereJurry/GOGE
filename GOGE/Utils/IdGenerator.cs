namespace GOGE.Utils
{
    public static class IdGenerator
    {
        // simple centralized ID generator; uses GUIDs to ensure uniqueness across saves
        public static string NewId() => Guid.NewGuid().ToString("N");
    }
}
