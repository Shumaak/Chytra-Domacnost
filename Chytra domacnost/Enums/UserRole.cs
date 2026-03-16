namespace Chytra_domacnost.Enums;

/// <summary>
/// Role uživatelů v systému chytré domácnosti
/// </summary>
public enum UserRole
{
    /// <summary>
    /// Host - omezený přístup
    /// </summary>
    Host = 0,

    /// <summary>
    /// Obyvatel - standardní přístup
    /// </summary>
    Obyvatel = 1,

    /// <summary>
    /// Správce - plný přístup ke všem funkcím
    /// </summary>
    Spravce = 2
}
