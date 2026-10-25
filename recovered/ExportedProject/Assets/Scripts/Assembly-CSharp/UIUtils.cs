using System;
using UnityEngine;

public static class UIUtils
{
	public static void SetLowProfileMode(bool enable)
	{
		EtceteraAndroid.setSystemUiVisibilityToLowProfile(enable);
		Debug.Log("SetLowProfileMode: " + enable);
	}

	public static void MakeListCountEqual(UIScrollList _list, int _count)
	{
		MakeListCountEqual(_list, _count, _list.prefabItems[0].item);
	}

	public static void MakeListCountEqual(UIScrollList _list, int _count, GameObject _prefab)
	{
		while (_list.Count > _count)
		{
			_list.RemoveItem(_list.Count - 1, true);
		}
		int num = _count - _list.Count;
		for (int i = 0; i < num; i++)
		{
			_list.CreateItem(_prefab);
		}
	}

	public static string FormatBestLapTime(float _time)
	{
		if (_time < RaceCampaignSave.MaxTimeThreshold)
		{
			return string.Format("{0:0.000}", _time);
		}
		return "-.---";
	}

	public static string FormatBestRaceTime(float _time)
	{
		if (_time < RaceCampaignSave.MaxTimeThreshold)
		{
			TimeSpan timeSpan = TimeSpan.FromSeconds(_time);
			return string.Format("{0:D2}:{1:D2}.{2:D3}", timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);
		}
		return "--:--.---";
	}
}
