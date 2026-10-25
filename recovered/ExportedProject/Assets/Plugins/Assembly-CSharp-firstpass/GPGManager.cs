using System;
using System.Collections.Generic;
using Prime31;
using UnityEngine;

public class GPGManager : AbstractManager
{
	public static event Action<string> authenticationSucceededEvent;

	public static event Action<string> authenticationFailedEvent;

	public static event Action userSignedOutEvent;

	public static event Action<string> reloadDataForKeyFailedEvent;

	public static event Action<string> reloadDataForKeySucceededEvent;

	public static event Action licenseCheckFailedEvent;

	public static event Action<string> loadCloudDataForKeyFailedEvent;

	public static event Action<int, string> loadCloudDataForKeySucceededEvent;

	public static event Action<string> updateCloudDataForKeyFailedEvent;

	public static event Action<int, string> updateCloudDataForKeySucceededEvent;

	public static event Action<string> clearCloudDataForKeyFailedEvent;

	public static event Action<string> clearCloudDataForKeySucceededEvent;

	public static event Action<string> deleteCloudDataForKeyFailedEvent;

	public static event Action<string> deleteCloudDataForKeySucceededEvent;

	public static event Action<string, string> unlockAchievementFailedEvent;

	public static event Action<string, bool> unlockAchievementSucceededEvent;

	public static event Action<string, string> incrementAchievementFailedEvent;

	public static event Action<string, bool> incrementAchievementSucceededEvent;

	public static event Action<string, string> revealAchievementFailedEvent;

	public static event Action<string> revealAchievementSucceededEvent;

	public static event Action<string, string> submitScoreFailedEvent;

	public static event Action<string, Dictionary<string, object>> submitScoreSucceededEvent;

	public static event Action<string, string> loadScoresFailedEvent;

	public static event Action<List<GPGScore>> loadScoresSucceededEvent;

	static GPGManager()
	{
		AbstractManager.initialize(typeof(GPGManager));
	}

	private void fireEventWithIdentifierAndError(Action<string, string> theEvent, string json)
	{
		if (theEvent != null)
		{
			Dictionary<string, object> dictionary = json.dictionaryFromJson();
			if (dictionary != null && dictionary.ContainsKey("identifier") && dictionary.ContainsKey("error"))
			{
				theEvent(dictionary["identifier"].ToString(), dictionary["error"].ToString());
			}
			else
			{
				Debug.LogError("json could not be deserialized to an identifier and an error: " + json);
			}
		}
	}

	private void fireEventWithIdentifierAndBool(Action<string, bool> theEvent, string param)
	{
		if (theEvent != null)
		{
			string[] array = param.Split(',');
			if (array.Length == 2)
			{
				theEvent(array[0], array[1] == "1");
			}
			else
			{
				Debug.LogError("param could not be deserialized to an identifier and an error: " + param);
			}
		}
	}

	public void userSignedOut(string empty)
	{
		userSignedOutEvent.fire();
	}

	public void reloadDataForKeyFailed(string error)
	{
		reloadDataForKeyFailedEvent.fire(error);
	}

	public void reloadDataForKeySucceeded(string param)
	{
		reloadDataForKeySucceededEvent.fire(param);
	}

	public void licenseCheckFailed(string param)
	{
		licenseCheckFailedEvent.fire();
	}

	public void loadCloudDataForKeyFailed(string error)
	{
		loadCloudDataForKeyFailedEvent.fire(error);
	}

	public void loadCloudDataForKeySucceeded(string json)
	{
		Dictionary<string, object> dictionary = json.dictionaryFromJson();
		loadCloudDataForKeySucceededEvent.fire(int.Parse(dictionary["key"].ToString()), dictionary["data"].ToString());
	}

	public void updateCloudDataForKeyFailed(string error)
	{
		updateCloudDataForKeyFailedEvent.fire(error);
	}

	public void updateCloudDataForKeySucceeded(string json)
	{
		Dictionary<string, object> dictionary = json.dictionaryFromJson();
		updateCloudDataForKeySucceededEvent.fire(int.Parse(dictionary["key"].ToString()), dictionary["data"].ToString());
	}

	public void clearCloudDataForKeyFailed(string error)
	{
		clearCloudDataForKeyFailedEvent.fire(error);
	}

	public void clearCloudDataForKeySucceeded(string param)
	{
		clearCloudDataForKeySucceededEvent.fire(param);
	}

	public void deleteCloudDataForKeyFailed(string error)
	{
		deleteCloudDataForKeyFailedEvent.fire(error);
	}

	public void deleteCloudDataForKeySucceeded(string param)
	{
		deleteCloudDataForKeySucceededEvent.fire(param);
	}

	public void unlockAchievementFailed(string json)
	{
		fireEventWithIdentifierAndError(unlockAchievementFailedEvent, json);
	}

	public void unlockAchievementSucceeded(string param)
	{
		fireEventWithIdentifierAndBool(unlockAchievementSucceededEvent, param);
	}

	public void incrementAchievementFailed(string json)
	{
		fireEventWithIdentifierAndError(incrementAchievementFailedEvent, json);
	}

	public void incrementAchievementSucceeded(string param)
	{
		string[] array = param.Split(',');
		if (array.Length == 2)
		{
			incrementAchievementSucceededEvent.fire(array[0], array[1] == "1");
		}
	}

	public void revealAchievementFailed(string json)
	{
		fireEventWithIdentifierAndError(revealAchievementFailedEvent, json);
	}

	public void revealAchievementSucceeded(string achievementId)
	{
		revealAchievementSucceededEvent.fire(achievementId);
	}

	public void submitScoreFailed(string json)
	{
		fireEventWithIdentifierAndError(submitScoreFailedEvent, json);
	}

	public void submitScoreSucceeded(string json)
	{
		if (submitScoreSucceededEvent != null)
		{
			Dictionary<string, object> dictionary = json.dictionaryFromJson();
			string arg = "Unknown";
			if (dictionary.ContainsKey("leaderboardId"))
			{
				arg = dictionary["leaderboardId"].ToString();
			}
			submitScoreSucceededEvent(arg, dictionary);
		}
	}

	public void loadScoresFailed(string json)
	{
		fireEventWithIdentifierAndError(loadScoresFailedEvent, json);
	}

	public void loadScoresSucceeded(string json)
	{
		loadScoresSucceededEvent.fire(DTOBase.listFromJson<GPGScore>(json));
	}

	public void authenticationSucceeded(string param)
	{
		authenticationSucceededEvent.fire(param);
	}

	public void authenticationFailed(string error)
	{
		authenticationFailedEvent.fire(error);
	}
}
