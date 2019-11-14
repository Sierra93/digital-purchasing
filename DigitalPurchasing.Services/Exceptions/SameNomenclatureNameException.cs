using System;

namespace DigitalPurchasing.Services.Exceptions
{
    public class SameNomenclatureNameException : Exception
    {
        public SameNomenclatureNameException(string nomenclatureName)
            : base($"Nomenclature name conflict: \"{nomenclatureName}\"")
        {
        }
    }
}
