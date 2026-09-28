using System;
using UnityEngine;

namespace Crypto
{
	public class CryptoDetector : MonoBehaviour
	{
		private static Action detectionAction;

		public static void StartDetection(Action action)
		{
			detectionAction = action;
		}

		public static void Detected()
		{
			if (detectionAction != null)
			{
				detectionAction();
			}
		}
	}
}
