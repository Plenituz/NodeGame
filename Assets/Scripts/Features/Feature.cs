using UnityEngine;
using System.Collections;

public class Feature : MonoBehaviour {

	public static void DestroyAll(GameObject g){
		Feature[] fList = g.GetComponents<Feature> ();
		for (int i = 0; i < fList.Length; i++) {
			Destroy (fList [i]);
		}
	}

	public static void EnableAll(GameObject g){
		Feature[] fList = g.GetComponents<Feature> ();
		for (int i = 0; i < fList.Length; i++) {
			fList [i].enabled = true;
		}
	}
}
