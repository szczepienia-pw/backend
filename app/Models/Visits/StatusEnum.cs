using System.ComponentModel;
using System.Reflection;
using backend.Exceptions;

namespace backend.Models.Visits
{
    public enum StatusEnum
    {
        [Description("PLANNED")]
        Planned,
        [Description("COMPLETED")]
        Completed,
        [Description("CANCELED")]
        Canceled
    }

    public static class StatusEnumAdapter
    {
        public static string GetDescription(this StatusEnum e)
        {
            var attribute =
                e.GetType()
                    .GetTypeInfo()
                    .GetMember(e.ToString())
                    .FirstOrDefault(member => member.MemberType == MemberTypes.Field)
                    .GetCustomAttributes(typeof(DescriptionAttribute), false)
                    .SingleOrDefault()
                    as DescriptionAttribute;

            return attribute?.Description ?? e.ToString();
        }

        public static StatusEnum ToEnum(this string text)
        {
            switch (text)
            {
                case "PLANNED":
                    return StatusEnum.Planned;
                case "COMPLETED":
                    return StatusEnum.Completed;
                case "CANCELED":
                    return StatusEnum.Canceled;
                default:
                    throw new ValidationException("Invalid status");
            }
        }
    }
}
