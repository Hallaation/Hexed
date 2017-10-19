using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHitByBullet
{
    void HitByBullet(Vector3 a_Vecocity, Vector3 HitPoint);
}

public interface IHitByMelee
{
    void HitByMelee(Weapon meleeWeapon , AudioClip soundEffect , float Volume = 1, float Pitch = 1);
}

public interface Reset
{
    void Reset();
}