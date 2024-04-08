using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelField))]
public class LevelFieldInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        LevelField field = (LevelField) target;

        Moveable objectBefore = field.MoveableObject;
        field.MoveableObject = (Moveable) EditorGUILayout.ObjectField("Moveable object", field.MoveableObject, typeof(Moveable), true);
        if (objectBefore != field.MoveableObject)
        {
            if (field.MoveableObject != null)
            {
                if (field.MoveableObject.Field != field && field.MoveableObject.Field != null) field.MoveableObject.Field.MoveableObject = null;
                field.MoveableObject.SetPos(field.ParentWall, field.X, field.Y);
                field.MoveableObject.transform.position = field.transform.position + field.ParentWall.Front;
                field.MoveableObject.transform.LookAt(field.MoveableObject.transform.position + field.ParentWall.Right, field.ParentWall.Front);
            }
        }
    }
}

