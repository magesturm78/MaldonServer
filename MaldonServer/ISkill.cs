
namespace MaldonServer
{
    public enum SkillTargetType
    {
        Location,
        ByteID,
        ShortID,
        NPCorPlayer,
        Trade,
        None
    }

    public interface ISkill
    {
        int SkillID { get; set; }
        int UseID { get; set; }
        SkillTargetType TargetType { get; set; }
    }
}
