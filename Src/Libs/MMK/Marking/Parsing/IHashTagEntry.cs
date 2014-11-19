namespace MMK.Marking.Parsing
{
    public interface IHashTagEntry
    {
        int Index { get; }
        int Length { get; }
        HashTag HashTag { get; }

        bool IsNotEmpty();
    }
}
