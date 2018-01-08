using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System;
public class StoryEditorTools
{
	static Texture2D whiteTex;
	public static Texture2D GetWhiteTex()
	{
		if (whiteTex) GameObject.DestroyImmediate(whiteTex);
		if (!whiteTex)
		{
			whiteTex = new Texture2D(1, 1);
			whiteTex.SetPixel(0, 0, new Color(1, 1, 1, 1));
			whiteTex.Apply();
			whiteTex.hideFlags = HideFlags.HideAndDontSave;
		}
		return whiteTex;
	}

    public static void WriteToFile(string filePath, string content)
    {
        try
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            if (!string.IsNullOrEmpty(filePath))
            {
                byte[] b = Encoding.UTF8.GetBytes(content);

                FileStream fs = new FileStream(filePath, FileMode.Append, FileAccess.Write);
                fs.Write(b, 0, b.Length);
                fs.Flush();
                fs.Close();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
        }
    }


    public static string GetAssetsEditorURL(string url)
    {
        string[] urls = url.Split('/');

        bool start_add = false;
        string tmp_url = "";
        for(int idx = 0; idx < urls.Length; idx++)
        {
            string child_url = urls[idx];
            if(child_url == "Assets")
            {
                start_add = true;
            }
            if(start_add)
            {
                tmp_url += child_url;
                if(idx < urls.Length - 1)
                {
                    tmp_url += "/";
                }
            }
        }
        return tmp_url;
    }
}
