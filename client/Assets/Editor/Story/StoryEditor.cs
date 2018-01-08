using UnityEngine;
using UnityEditor;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class StoryEditor : EditorWindow
{
    private string def_path = Application.dataPath + "/Resources/Config/Movie/";

	[MenuItem("GameTools/剧情编辑器")]
	static void Init()
	{
        StoryEditor window = GetWindow<StoryEditor>(typeof(StoryEditor));
        window.titleContent = new GUIContent("剧情编辑器");
		window.wantsMouseMove = true;
		window.Show();
		window.OnShowWindow();
	}

	private float time_line_scale_value = 61f;
	private bool is_create_movie_type = false;//是否是创建类型状态
	private List<StoryEditorBase> m_data = new List<StoryEditorBase>();
    void OnDestroy()
    {
        cleardata();
    }
	void OnGUI()
	{
		if (is_create_movie_type)
		{
			guiCreateMovie();
		}
		else
		{
			guiTopToolbar();
			guiLeftToolbar();
			guiWorkArea();
            guiInspector();
		}
	}
    public void OnShowWindow()
	{
	}

	private float work_real_width;
	private Rect rct_work_area;
	private Vector2 work_area_scroll_pos = new Vector2();
	void guiWorkArea()
	{
		work_real_width = time_line_scale_value * time_rule_interval_pixel;

		rct_work_area = new Rect(left_toolbar_width, top_toolbar_height, position.width - left_toolbar_width - right_inspector_width, position.height - top_toolbar_height);
		GUILayout.BeginArea(rct_work_area);
		Rect view_pos = new Rect(0, 0, rct_work_area.width, rct_work_area.height);
		Rect view_rct = new Rect(0, 0, work_real_width, rct_work_area.height - top_toolbar_height);
		work_area_scroll_pos = GUI.BeginScrollView(view_pos, work_area_scroll_pos, view_rct, false, false);

		GUI.Box(new Rect(0, 0, work_real_width, item_height), "", EditorStyles.toolbar);

		drawWorkareaBack();
		drawTimeLineRule();

		for (int idx = 0; idx < m_data.Count; idx++)
		{
			m_data[idx].DrawWorkSpace();
		}
		GUI.EndScrollView();
		GUILayout.EndArea();
	}

	#region 右
	private float right_inspector_width = 250;
    private Rect rct_inspector;
    void guiInspector()
    {
        rct_inspector = new Rect(position.width - right_inspector_width, top_toolbar_height, right_inspector_width, position.height - top_toolbar_height);
        GUILayout.BeginArea(rct_inspector);

        GUILayout.BeginVertical();
        GUI.color = new Color(0f, 0f, 0f, 0.4f);
        GUI.DrawTexture(new Rect(0, 0, rct_inspector.width, rct_inspector.height), StoryEditorTools.GetWhiteTex());
        GUI.color = new Color(1f, 1f, 1f, 1f);
        for (int idx = 0; idx < m_data.Count; idx++ )
        {
            if(m_data[idx].m_is_select)
            {
                m_data[idx].DrawInspectorSpace();
            }
        }
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
	#endregion

	#region 时间刻度线
	float time_rule_interval_pixel = 50f;
	void drawTimeLineRule()
	{
		int count = (int)(work_real_width / 50f);

		for (int i = 0; i < count; ++i)
		{
			GUI.Label(new Rect(i * time_rule_interval_pixel - 4, 0, 50, top_toolbar_height), "|" + i);
		}
	}
	#endregion
	#region 左侧
	private float left_toolbar_width = 125;
	private float item_height = 20;
	private string cur_time_string = "00:00:00:000";
	private Rect rct_left_toolbar;

	void guiLeftToolbar()
	{
		rct_left_toolbar = new Rect(0, top_toolbar_height, left_toolbar_width, position.height);
		GUILayout.BeginArea(rct_left_toolbar);

		GUI.color = new Color(0f, 0f, 0f, 0.4f);
		GUI.DrawTexture(new Rect(0, 0, rct_left_toolbar.width, rct_left_toolbar.height), StoryEditorTools.GetWhiteTex());
		GUI.color = new Color(1f, 1f, 1f, 1f);

		GUI.Label(new Rect(20, 0, rct_left_toolbar.width - 20, item_height), cur_time_string);

		for (int idx = 0; idx < m_data.Count; idx++)
		{
            StoryEditorBase item = m_data[idx];
			if(item.m_is_select)
				GUI.color = new Color(0.75f, 0.75f, 0.75f, 1.0f);

			if (GUI.Button(new Rect(0, item_height * (idx + 1), rct_left_toolbar.width, item_height), item.m_type, EditorStyles.toolbarPopup))
			{
				for (int idy = 0; idy < m_data.Count; idy++)
				{
					m_data[idy].UnSelect();
				}
				m_data[idx].OnSelect();
			}
			GUI.color = new Color(1f, 1f, 1f, 1f);
		}

		if (GUI.Button(new Rect(0, item_height * (m_data.Count + 1), rct_left_toolbar.width, item_height), "+", EditorStyles.miniButton))
		{
			is_create_movie_type = true;
		}

		GUILayout.EndArea();
	}
	#endregion
	#region 上部
	private float top_toolbar_height = 18;
	private Rect rct_top_toolbar;
	void guiTopToolbar()
	{
		rct_top_toolbar = new Rect(0, 0, position.width, top_toolbar_height);
		GUILayout.BeginArea(rct_top_toolbar);

		GUI.Box(new Rect(0, 0, position.width, top_toolbar_height), "", EditorStyles.toolbar);

		// silder
		time_line_scale_value = GUI.HorizontalSlider(new Rect(5, 0, 120, top_toolbar_height), time_line_scale_value, 17f, 600f);

		// play button
        //if (GUI.Button(new Rect(130, 0, 50, top_toolbar_height), "RePlay", EditorStyles.toolbarButton))
        //{
        //}
        if (GUI.Button(new Rect(rct_top_toolbar.width - 135, 0, 40, top_toolbar_height), "Clear", EditorStyles.toolbarButton))
        {
            if (m_data.Count <= 0)
                return;

            bool clear = EditorUtility.DisplayDialog("警告", "是否清除当前数据？", "确定", "取消");
            if(clear)
            {
                cleardata();
            }
        }
		if (GUI.Button(new Rect(rct_top_toolbar.width - 45, 0, 40, top_toolbar_height), "Save", EditorStyles.toolbarButton))
		{
            string tmp_file = EditorUtility.SaveFilePanel("select file", def_path, "", "xml");
            if(string.IsNullOrEmpty(tmp_file))
            {
                Debug.LogError("保存失败, 路径为空。");
                return;
            }
            XmlDocument xmlDoc = new XmlDocument();
            XmlDeclaration xmldoc = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
            xmlDoc.AppendChild(xmldoc);
            XmlElement rootNode = xmlDoc.CreateElement("Object");
            xmlDoc.AppendChild(rootNode);
            for(int idx = 0; idx < m_data.Count; idx++)
            {
                XmlElement MovieNode = xmlDoc.CreateElement("Movie");
                m_data[idx].SaveData(MovieNode);
                rootNode.AppendChild(MovieNode);
            }
            xmlDoc.Save(tmp_file);
            EditorUtility.DisplayDialog("提示", "保存完成", "确定");
		}

		if (GUI.Button(new Rect(rct_top_toolbar.width - 90, 0, 40, top_toolbar_height), "Load", EditorStyles.toolbarButton))
		{
            string tmp_file = EditorUtility.OpenFilePanel("select file", def_path, "xml");
            if (tmp_file != "")
            {
                loadFromConfigFile(tmp_file);
            }
		}

        //if (GUI.Button(new Rect(rct_top_toolbar.width - 135, 0, 40, top_toolbar_height), "New", EditorStyles.toolbarButton))
        //{
        //}
		GUILayout.EndArea();
	}

    private void cleardata()
    {
        for(int idx = 0; idx < m_data.Count; idx++)
        {
            m_data[idx].OnLeave();
        }
        m_data.Clear();
    }
    void loadFromConfigFile(string file)
    {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(file);
        if (xmlDoc == null)
        {
            string err = "parse movie xml file failed:" + file;
            Debug.LogError(err);
            return;
        }
        m_data.Clear();

        XmlNode root = xmlDoc.SelectSingleNode("Object");
        XmlNodeList list = root.ChildNodes;
        if (list == null || list.Count <= 0)
            return;
        for (int i = 0; i < list.Count; i++)
        {
            XmlNode node = list[i];
            XmlElement element = (XmlElement)node;
            string type = element.GetAttribute("Type");
            StoryEditorBase movie = GetMovieEditor(type);
            if(movie == null)
            {
                Debug.LogError("剧情解析错误，请删除该文件后，重做.");
                return;
            }
            movie.LoadData(element);
            movie.m_index = i;
            m_data.Add(movie);
        }
    }
	#endregion
	#region 创建类型
	private MovieEditorEnum create_type = MovieEditorEnum.None;
	private Rect rct_create_toolbar;
	void guiCreateMovie()
	{
		rct_create_toolbar = new Rect(0, 0, position.width, position.height);
		GUILayout.BeginArea(rct_create_toolbar);
		create_type = (MovieEditorEnum)EditorGUI.EnumPopup(new Rect(0, 0, position.width, item_height), "选择类型：", create_type);

		if (GUI.Button(new Rect(0, 300, position.width, item_height), "create"))
		{
			is_create_movie_type = false;
            StoryEditorBase moive = GetMovieEditor(create_type.ToString());
            if (moive == null)
                return;
			moive.m_index = m_data.Count;
			m_data.Add(moive);
		}

		if (GUI.Button(new Rect(0, 400, position.width, item_height), "return"))
		{
			is_create_movie_type = false;
		}
		GUILayout.EndArea();
	}

    private StoryEditorBase GetMovieEditor(string movie_type)
    {
        var type = Assembly.GetExecutingAssembly().GetType(string.Format("ME{0}", movie_type.ToString()));
        if (type == null)
        {
            Debug.LogError("未知错误。");
            return null;
        }
        //MovieEditorBase moive = ScriptableObject.CreateInstance(string.Format("MovieEditor{0}", movie_type.ToString())) as MovieEditorBase;
        StoryEditorBase moive = System.Activator.CreateInstance(type) as StoryEditorBase;
        if (moive == null)
        {
            Debug.LogError("未知错误。");
            return null;
        }
        moive.m_type = movie_type.ToString();
        moive.OnInit();
        return moive;
    }
	#endregion
	#region 纹理背景
	// 背景纹理
	void drawWorkareaBack()
	{
		int rows = Mathf.CeilToInt(rct_work_area.height / item_height);
		for (int i = 1; i < rows; ++i)
		{
			Rect rct = new Rect(0, i * item_height, work_real_width, item_height);
			if (i % 2 == 0)
				GUI.color = new Color(0.2f, 0.2f, 0.2f, 0.2f);
			else
				GUI.color = new Color(0f, 0f, 0f, 0.2f);

			//if (i % 2 == 0)
			//	GUI.color = new Color(0f, 0f, 0.5f, 0.2f);

			GUI.DrawTexture(rct, StoryEditorTools.GetWhiteTex());
			GUI.color = new Color(1f, 1f, 1f, 1f);
		}
	}
	#endregion
}
