using UnityEngine;
using UnityEditor;
using FBK;

[CustomEditor(typeof(BulletObject))]
public class BulletObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        BulletObject bulletObject = (BulletObject)target;

        // Get the current BulletEffect
        BulletEffect currentEffect = bulletObject.bulletEffect;

        // Create a dropdown for BulletEffect
        BulletEffect newEffect = (BulletEffect)EditorGUILayout.EnumPopup("Bullet Effect", currentEffect);

        // If the selected effect has changed, update the bulletEffect
        if (newEffect != currentEffect)
        {
            Undo.RecordObject(bulletObject, "Change Bullet Effect");
            bulletObject.bulletEffect = newEffect;
            EditorUtility.SetDirty(bulletObject);
        }

        // Draw the rest of the default inspector
        DrawDefaultInspector();
    }
}