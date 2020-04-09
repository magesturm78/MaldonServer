namespace MaldonServer
{
    public enum SpellTargetType
    {
        Location,
        Player,
        None
    }

    public interface ISpell
    {
        int SpellID { get; }
        int CastID { get;  }
        SpellTargetType SpellTargetType { get; }
    }
}
