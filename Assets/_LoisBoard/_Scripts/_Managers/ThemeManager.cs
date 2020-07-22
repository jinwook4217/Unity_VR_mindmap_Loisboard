using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ThemeManager : MonoBehaviour
{
  public static ThemeManager Instance { get; private set; }
  public static Theme MainTheme { get; set; }

  // private List<Theme> _dayTimeThemes = new List<Theme>();
  // private List<Theme> _nightTimeThemes = new List<Theme>();

  // private Dictionary<int, Theme> indexToTheme = new Dictionary<int, Theme>();

  private List<Theme> _indexToTheme = new List<Theme>();

  [System.Serializable]
  public class DayTimeTheme1
  {
    [HideInInspector]
    public int id = 0;
    public Color topColor;
    public Color bottomColor;
  }
  public DayTimeTheme1 dayTimeTheme1;

  [System.Serializable]
  public class DayTimeTheme2
  {
    [HideInInspector]
    public int id = 1;
    public Color topColor;
    public Color bottomColor;
  }
  public DayTimeTheme2 dayTimeTheme2;

  [System.Serializable]
  public class DayTimeTheme3
  {
    [HideInInspector]
    public int id = 2;
    public Color topColor;
    public Color bottomColor;
  }
  public DayTimeTheme3 dayTimeTheme3;

  [System.Serializable]
  public class DayTimeTheme4
  {
    [HideInInspector]
    public int id = 3;
    public Color topColor;
    public Color bottomColor;
  }
  public DayTimeTheme4 dayTimeTheme4;

  [System.Serializable]
  public class DayTimeTheme5
  {
    [HideInInspector]
    public int id = 4;
    public Color topColor;
    public Color bottomColor;
  }
  public DayTimeTheme5 dayTimeTheme5;

  [System.Serializable]
  public class DayTimeTheme6
  {
    [HideInInspector]
    public int id = 5;
    public Color topColor;
    public Color bottomColor;
  }
  public DayTimeTheme6 dayTimeTheme6;

  [System.Serializable]
  public class NightTimeTheme1
  {
    [HideInInspector]
    public int id = 6;
    public Color topColor;
    public Color bottomColor;
  }
  public NightTimeTheme1 nightTimeTheme1;

  [System.Serializable]
  public class NightTimeTheme2
  {
    [HideInInspector]
    public int id = 7;
    public Color topColor;
    public Color bottomColor;
  }
  public NightTimeTheme2 nightTimeTheme2;

  [System.Serializable]
  public class NightTimeTheme3
  {
    [HideInInspector]
    public int id = 8;
    public Color topColor;
    public Color bottomColor;
  }
  public NightTimeTheme3 nightTimeTheme3;

  [System.Serializable]
  public class NightTimeTheme4
  {
    [HideInInspector]
    public int id = 9;
    public Color topColor;
    public Color bottomColor;
  }
  public NightTimeTheme4 nightTimeTheme4;

  [System.Serializable]
  public class NightTimeTheme5
  {
    [HideInInspector]
    public int id = 10;
    public Color topColor;
    public Color bottomColor;
  }
  public NightTimeTheme5 nightTimeTheme5;

  [System.Serializable]
  public class NightTimeTheme6
  {
    [HideInInspector]
    public int id = 11;
    public Color topColor;
    public Color bottomColor;
  }
  public NightTimeTheme6 nightTimeTheme6;


  public class Theme
  {
		public int id;
    public Color topColor;
    public Color bottomColor;

    public Theme(int id, Color topColor, Color bottomColor)
    {
			this.id = id;
      this.topColor = topColor;
      this.bottomColor = bottomColor;
    }
  }

  private void Awake()
  {
    if (Instance == null)
    {
      Instance = this;
    }
  }

  private void Start()
  {
    // _dayTimeThemes.Add(new Theme(dayTimeTheme1.id, dayTimeTheme1.topColor, dayTimeTheme1.bottomColor));
    // _dayTimeThemes.Add(new Theme(dayTimeTheme2.id,dayTimeTheme2.topColor, dayTimeTheme2.bottomColor));
    // _dayTimeThemes.Add(new Theme(dayTimeTheme3.id,dayTimeTheme3.topColor, dayTimeTheme3.bottomColor));
    // _dayTimeThemes.Add(new Theme(dayTimeTheme4.id,dayTimeTheme4.topColor, dayTimeTheme4.bottomColor));
    // _dayTimeThemes.Add(new Theme(dayTimeTheme5.id,dayTimeTheme5.topColor, dayTimeTheme5.bottomColor));
    // _dayTimeThemes.Add(new Theme(dayTimeTheme6.id,dayTimeTheme6.topColor, dayTimeTheme6.bottomColor));

    // _nightTimeThemes.Add(new Theme(nightTimeTheme1.id, nightTimeTheme1.topColor, nightTimeTheme1.bottomColor));
    // _nightTimeThemes.Add(new Theme(nightTimeTheme2.id, nightTimeTheme2.topColor, nightTimeTheme2.bottomColor));
    // _nightTimeThemes.Add(new Theme(nightTimeTheme3.id, nightTimeTheme3.topColor, nightTimeTheme3.bottomColor));
    // _nightTimeThemes.Add(new Theme(nightTimeTheme4.id, nightTimeTheme4.topColor, nightTimeTheme4.bottomColor));
    // _nightTimeThemes.Add(new Theme(nightTimeTheme5.id, nightTimeTheme5.topColor, nightTimeTheme5.bottomColor));
    // _nightTimeThemes.Add(new Theme(nightTimeTheme6.id, nightTimeTheme6.topColor, nightTimeTheme6.bottomColor));

    // indexToTheme.Add(dayTimeTheme1.id, new Theme(dayTimeTheme1.topColor, dayTimeTheme1.bottomColor));
    // indexToTheme.Add(dayTimeTheme2.id, new Theme(dayTimeTheme2.topColor, dayTimeTheme2.bottomColor));
    // indexToTheme.Add(dayTimeTheme3.id, new Theme(dayTimeTheme3.topColor, dayTimeTheme3.bottomColor));
    // indexToTheme.Add(dayTimeTheme4.id, new Theme(dayTimeTheme4.topColor, dayTimeTheme4.bottomColor));
    // indexToTheme.Add(dayTimeTheme5.id, new Theme(dayTimeTheme5.topColor, dayTimeTheme5.bottomColor));
    // indexToTheme.Add(dayTimeTheme6.id, new Theme(dayTimeTheme6.topColor, dayTimeTheme6.bottomColor));

    // indexToTheme.Add(nightTimeTheme1.id, new Theme(nightTimeTheme1.topColor, nightTimeTheme1.bottomColor));
    // indexToTheme.Add(nightTimeTheme2.id, new Theme(nightTimeTheme2.topColor, nightTimeTheme2.bottomColor));
    // indexToTheme.Add(nightTimeTheme3.id, new Theme(nightTimeTheme3.topColor, nightTimeTheme3.bottomColor));
    // indexToTheme.Add(nightTimeTheme4.id, new Theme(nightTimeTheme4.topColor, nightTimeTheme4.bottomColor));
    // indexToTheme.Add(nightTimeTheme5.id, new Theme(nightTimeTheme5.topColor, nightTimeTheme5.bottomColor));
    // indexToTheme.Add(nightTimeTheme6.id, new Theme(nightTimeTheme6.topColor, nightTimeTheme6.bottomColor));

    _indexToTheme.Add(new Theme(dayTimeTheme1.id, dayTimeTheme1.topColor, dayTimeTheme1.bottomColor));
    _indexToTheme.Add(new Theme(dayTimeTheme2.id, dayTimeTheme2.topColor, dayTimeTheme1.bottomColor));
    _indexToTheme.Add(new Theme(dayTimeTheme3.id, dayTimeTheme3.topColor, dayTimeTheme1.bottomColor));
    _indexToTheme.Add(new Theme(dayTimeTheme4.id, dayTimeTheme4.topColor, dayTimeTheme1.bottomColor));
    _indexToTheme.Add(new Theme(dayTimeTheme5.id, dayTimeTheme5.topColor, dayTimeTheme1.bottomColor));
    _indexToTheme.Add(new Theme(dayTimeTheme6.id, dayTimeTheme6.topColor, dayTimeTheme1.bottomColor));
    _indexToTheme.Add(new Theme(nightTimeTheme1.id, nightTimeTheme1.topColor, nightTimeTheme1.bottomColor));
    _indexToTheme.Add(new Theme(nightTimeTheme2.id, nightTimeTheme2.topColor, nightTimeTheme2.bottomColor));
    _indexToTheme.Add(new Theme(nightTimeTheme3.id, nightTimeTheme3.topColor, nightTimeTheme3.bottomColor));
    _indexToTheme.Add(new Theme(nightTimeTheme4.id, nightTimeTheme4.topColor, nightTimeTheme4.bottomColor));
    _indexToTheme.Add(new Theme(nightTimeTheme5.id, nightTimeTheme5.topColor, nightTimeTheme5.bottomColor));
    _indexToTheme.Add(new Theme(nightTimeTheme6.id, nightTimeTheme6.topColor, nightTimeTheme6.bottomColor));
  }

  public void OnGetRandomThemeByDayTime()
  {
    int nowHour24 = Int32.Parse(System.DateTime.Now.ToString("HH"));
    int randomIndex = Mathf.FloorToInt(UnityEngine.Random.Range(0f, 6f));

    // Debug.Log("random index: " + randomIndex);

    if (nowHour24 >= 6 && nowHour24 < 18)
    {
      // MainTheme = GetDayTimeTheme(randomIndex);
			MainTheme = GetThemeByInherenceIndex(randomIndex);
      Debug.Log(MainTheme.id);
    }
    else
    {
      // MainTheme = GetNightTimeTheme(randomIndex);
			MainTheme = GetThemeByInherenceIndex(randomIndex + 6);
      Debug.Log(MainTheme.id);
    }
  }

  // public Theme GetDayTimeTheme(int index)
  // {
  //   return _dayTimeThemes[index];
  // }

  // public Theme GetNightTimeTheme(int index)
  // {
  //   return _nightTimeThemes[index];
  // }

  public Theme GetThemeByInherenceIndex(int index)
  {
    return _indexToTheme[index];
  }
}
