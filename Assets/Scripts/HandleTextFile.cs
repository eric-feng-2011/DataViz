using UnityEngine;
using UnityEditor;
using System.IO;

//@source https://support.unity3d.com/hc/en-us/articles/115000341143-How-do-I-read-and-write-data-from-a-text-file-
public class HandleTextFile
{
	//This creates a menu in the Unity Editor for creating a unity resource and reading from it.
	//Was helpful in figuring out how to read input from outside Unity resources
	[MenuItem("Tools/Write file")]
	static void WriteString()
	{
		string path = "Assets/Resources/test.txt";

		//Write some text to the test.txt file
		StreamWriter writer = new StreamWriter(path, true);
		writer.WriteLine("Test");
		writer.Close();

		//Re-import the file to update the reference in the editor
		AssetDatabase.ImportAsset(path); 
		TextAsset asset = (TextAsset) Resources.Load("test");

		//Print the text from the file
		Debug.Log(asset.text);
	}

	[MenuItem("Tools/Read file")]
	static void ReadString()
	{
		string path = "Assets/Resources/test.txt";

		//Read the text from directly from the test.txt file
		StreamReader reader = new StreamReader(path); 
		Debug.Log(reader.ReadToEnd());
		reader.Close();
	}

}