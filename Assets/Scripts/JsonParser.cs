//using StaticData;
using UnityEditor;
using UnityEngine;

public class JsonParser : EditorWindow 
{

	[MenuItem("Tools/JsonParser")]
	public static void ParseJson()
	{
		//ParseStatic<WoodenWeaponUpgraidesStatic> ("woodenWeaponUpgraides.json", "WoodenWeaponUpgraides.asset", "woodenWeaponUpgraidesList");

		Debug.Log("Statics parsed");
    }

	private static void ParseStatic<ScriptableObjType> (
		string jsonName, 
		string scriptableObjName, 
		string arrFieldName, 
		bool replaceComma = false
	) where ScriptableObjType : ScriptableObject
	{
		ScriptableObjType sObj = CreateInstance<ScriptableObjType>();

		AssetDatabase.CreateAsset(sObj, "Assets/Balance/" + scriptableObjName);
		AssetDatabase.SaveAssets();

		string json = GetJsonText(jsonName, replaceComma);
		JsonHelper.FromJsonOverwriteArr(json, arrFieldName, sObj);

		EditorUtility.SetDirty(sObj);
	}

	private static string GetJsonText(string fileName, bool replaceComma)
	{
		TextAsset json_text = (TextAsset)AssetDatabase.LoadAssetAtPath("Assets/Balance/" + fileName,
			typeof(TextAsset));

		return replaceComma ? ReplaceComma(json_text.text) : json_text.text;
	}

	private static string ReplaceComma(string json)
	{
		bool isInQuotes = false;
		char[] json_symbols = json.ToCharArray();
		for(int i = 0; i < json_symbols.Length; i++)
		{
			if(json_symbols[i] == '"')
			{
				isInQuotes = !isInQuotes;
			}
			else if(isInQuotes && json_symbols[i] == ',')
			{
				json_symbols[i] = '.';
			}
		}
		return new string(json_symbols);
	}
}

#pragma warning disable
public static class JsonHelper
{
    public static void FromJsonOverwriteArr(string json_array, string fieldName, object obj)
    {
        json_array = WrapArray(json_array, fieldName);
        JsonUtility.FromJsonOverwrite(json_array, obj);
    }

    private static string WrapArray(string json_array, string fieldName)
    {
        return "{\"" + fieldName + "\":" + json_array + "}";
    }
}
#pragma warning restore