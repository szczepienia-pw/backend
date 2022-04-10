using System.ComponentModel;
using System.Reflection;
using backend.Exceptions;

namespace backend.Models.Vaccines
{
    public enum DiseaseEnum
    {
        [Description("COVID-19")]
        COVID19,
        [Description("COVID-21")]
        COVID21,
        [Description("Flu")]
        Flu,
        [Description("OTHER")]
        Other
    }

    public static class DiseaseEnumAdapter
    {
        public static string GetDescription(this DiseaseEnum e)
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

        public static DiseaseEnum ToEnum(this string text)
        {
            switch (text)
            {
                case "COVID-19":
                    return DiseaseEnum.COVID19;
                case "COVID-21":
                    return DiseaseEnum.COVID21;
                case "Flu":
                    return DiseaseEnum.Flu;
                case "OTHER":
                    return DiseaseEnum.Other;
                default:
                    throw new ValidationException("Invalid disease");
            }
        }
    }

}
