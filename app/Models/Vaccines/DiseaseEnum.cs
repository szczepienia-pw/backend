using System.ComponentModel;
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
