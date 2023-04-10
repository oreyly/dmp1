using System.ComponentModel;

namespace dmp1
{
    public enum DruhSpusteni {
        [Description("Učení")]
        Uceni=1,
        [Description("Procvičování")]
        Procvicovani,
        [Description("Učení + procvičování")]
        Oboje,
        [Description("Test")]
        Test,
        [Description("Kontrola")]
        Kontrola
    }
}
