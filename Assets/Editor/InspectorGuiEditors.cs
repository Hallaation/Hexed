using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/*
[CustomEditor(typeof(HitByBulletAction), true)]
public class BulletAudioEditor : Editor
{
    override public void OnInspectorGUI()
    {

        var TargetClass = target as HitByBulletAction;

        TargetClass.m_audioClip = EditorGUILayout.ObjectField("Audio Clip", TargetClass.m_audioClip, typeof(AudioClip), false) as AudioClip;
        TargetClass.m_bRandomizePitch = EditorGUILayout.Toggle("Randomize Pitch", TargetClass.m_bRandomizePitch);
        TargetClass.clipVolume = EditorGUILayout.Slider("Clip Volume", TargetClass.clipVolume, 0, 1);
        TargetClass.m_bShakeCamera = EditorGUILayout.Toggle("Shake Camera", TargetClass.m_bShakeCamera);
        TargetClass.PlayParticles = EditorGUILayout.Toggle("Play particle", TargetClass.PlayParticles);
        if (TargetClass.PlayParticles)
        {
            TargetClass.ParticlePrefab = EditorGUILayout.ObjectField("Particle Prefab", TargetClass.ParticlePrefab, typeof(GameObject), false) as GameObject;
        }
    }
}


[CustomEditor(typeof(HitByMeleeAction), true)]
public class MeleeAudioEditor : Editor
{
    override public void OnInspectorGUI()
    {

        var TargetClass = target as HitByMeleeAction;
        TargetClass.CopyBulletAudio = EditorGUILayout.Toggle("Copy bullet audio/particles", TargetClass.CopyBulletAudio);
        if (!TargetClass.CopyBulletAudio)
        {
            TargetClass.m_audioClip = EditorGUILayout.ObjectField("Audio Clip", TargetClass.m_audioClip, typeof(AudioClip), false) as AudioClip;
            TargetClass.m_bRandomizePitch = EditorGUILayout.Toggle("Randomize Pitch", TargetClass.m_bRandomizePitch);
            TargetClass.clipVolume = EditorGUILayout.Slider("Clip Volume", TargetClass.clipVolume, 0, 1);
            TargetClass.m_bShakeCamera = EditorGUILayout.Toggle("Shake Camera", TargetClass.m_bShakeCamera);
            TargetClass.PlayParticles = EditorGUILayout.Toggle("Play particle", TargetClass.PlayParticles);
            if (TargetClass.PlayParticles)
            {
                TargetClass.ParticlePrefab = EditorGUILayout.ObjectField("Particle Prefab", TargetClass.ParticlePrefab, typeof(GameObject), false) as GameObject;
            }
        }
    }
}
*/

