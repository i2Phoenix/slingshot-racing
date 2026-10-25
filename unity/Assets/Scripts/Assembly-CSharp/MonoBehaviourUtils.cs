using UnityEngine;

public static class MonoBehaviourUtils
{
	public static void ReInvoke(MonoBehaviour _this, string _methodName, float _time)
	{
		_this.CancelInvoke(_methodName);
		_this.Invoke(_methodName, _time);
	}

	public static void ReInvokeRepeating(MonoBehaviour _this, string _methodName, float _time, float _repeatRate)
	{
		_this.CancelInvoke(_methodName);
		_this.InvokeRepeating(_methodName, _time, _repeatRate);
	}
}
