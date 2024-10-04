using System;

[Serializable]
public struct HeroStatus
{
    [ReadOnlyInspector]
    public HeroTribe eTribe;
    [ReadOnlyInspector]
    public HeroClass eClass_1, eClass_2;
    [ReadOnlyInspector]
    public int iBeastAttack, iMagicianPower;
}
