using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//@source http://wiki.unity3d.com/index.php/BoolPrefs
//Allows for PlayerPrefs bools by translating booleanValue to 1 (true) or 0 (false).
public class PlayerPrefsX : MonoBehaviour {
	public static void SetBool(string name, bool booleanValue) 
	{
		PlayerPrefs.SetInt(name, booleanValue ? 1 : 0);
	}

	public static bool GetBool(string name)  
	{
		return PlayerPrefs.GetInt(name) == 1 ? true : false;
	}

	//GetBool, but with the added option of returning a default value if PlayerPrefs does not have key
	public static bool GetBool(string name, bool defaultValue)
	{
		if(PlayerPrefs.HasKey(name)) 
		{
			return GetBool(name);
		}

		return defaultValue;
	}
}
