using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Analytics;

public class AnalyticsTracker : MonoBehaviour
{
	[SerializeField] private int someSecretId;

	public void TriggerEvent()
	{
		Analytics.CustomEvent(
			"secret_found",
			new Dictionary<string, object>
			{
				{ "secret_id", someSecretId },
				{ "time_elapsed", Time.timeSinceLevelLoad }
			}
		);

		Debug.Log("Custom triggered");
		Destroy(gameObject);
	}
}
