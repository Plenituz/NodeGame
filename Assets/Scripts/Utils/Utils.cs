using System;
using UnityEngine;
using UnityEngine.UI;

public class Utils
{
	public static Quaternion LookAt2D(Vector2 lookAt){
		return Quaternion.Euler (new Vector3 (0f, 0f, Mathf.Rad2Deg * Mathf.Atan2 (lookAt.y, lookAt.x)));
	}

    public static Vector2 Forward2D(Transform trans)
    {
        return Quaternion.Euler(0f, 0f, -90f) * trans.up;
    }

    public static void SetupFieldAsDecimal(InputField field)
    {
        field.contentType = InputField.ContentType.DecimalNumber;
        field.textComponent.alignment = TextAnchor.MiddleCenter;
        field.textComponent.horizontalOverflow = HorizontalWrapMode.Overflow;
    }
}


