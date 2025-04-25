//______________________________________________
// ALIyerEdon
// https://assetstore.unity.com/publishers/23606
//______________________________________________

using System;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Collections;
using ALIyerEdon;

namespace ALIyerEdon
{
	[RequireComponent(typeof(EasyCarController))]
	public class EasyCarAudio : MonoBehaviour
	{
		[Header("Audio Sources ____________________________________________________" +
			"____________________________________________________")]
		[Space(5)]
		// Audio Sources
		public AudioSource engineSource;
		public AudioSource collisionSource;
		public AudioSource gearSource;
		public AudioSource startSkidSource;
		public AudioSource asphaltSkidSource;
		public AudioSource grassSkidSource;
		public AudioSource flameSource;

		[Header("Audio Clips ____________________________________________________" +
			"____________________________________________________")]
		[Space(5)]
		// Audio Clips
		public AudioClip engineSound;
		public AudioClip gearShift;
		public AudioClip collisionClip;
		public AudioClip startSkidClip;
		public AudioClip asphaltSkidClip, grassSkidClip;
		public AudioClip flameClip;

		[Header("Volume ____________________________________________________" +
			"____________________________________________________")]
		[Space(5)]
		public float engineVolume = 1f;
		public float gearVolume = 1f;
		public float collisionVolume = 1f;
		public float startSkidVolume = 1f;
		public float skidVolume = 1f;
		public float flameVolume = 1f;

		[Header("Pitch ____________________________________________________" +
			"____________________________________________________")]
		[Space(5)]
		public float pitchMultiplier = 1f;
		public float revMultiplier = 1f;

		public float PitchMin = 0.43f;

		public float PitchMax = 1.2f;

		[Header("Settings ____________________________________________________" +
			"____________________________________________________")]
		[Space(5)]
		public float collisionVelocity = 5f;
		public float startSkidDuration = 2.3f;
		[Header("Effects____________________________________________________" +
			"____________________________________________________")]
		[Space(5)]
		public GameObject[] wheelSmokes;
		public GameObject[] exhaustFlame;
		public GameObject collisionSpark;

		[Header("Disable Vehicle Audio in Garage")]
		[Space(5)]
		public string garageSceneName = "Garage";

		EasyCarController m_vehicleController;
		[HideInInspector] public bool raceIsStarted;
		[HideInInspector] public Shake_Utility shakeUtility;

		void Start()
		{

			Stop_Effects();

			m_vehicleController = GetComponent<EasyCarController>();
			
			shakeUtility = FindObjectOfType<Shake_Utility>();

			checkWheel = new bool[m_vehicleController.Wheel_Colliders.Length];

			gearSource.loop = false;
			gearSource.playOnAwake = false;
			gearSource.clip = gearShift;

			engineSource.clip = engineSound;
			engineSource.loop = true;
			engineSource.volume = engineVolume;
			engineSource.Play();

			collisionSource.loop = false;
			collisionSource.playOnAwake = false;
			collisionSource.clip = collisionClip;
			collisionSource.volume = collisionVolume;

			if (startSkidSource)
			{
				startSkidSource.loop = false;
				startSkidSource.playOnAwake = false;
				startSkidSource.clip = startSkidClip;
				startSkidSource.volume = startSkidVolume;
			}
			if (asphaltSkidSource)
			{
				asphaltSkidSource.loop = true;
				asphaltSkidSource.playOnAwake = false;
				asphaltSkidSource.clip = asphaltSkidClip;
				asphaltSkidSource.volume = skidVolume;
			}
			if (grassSkidSource)
			{
				grassSkidSource.loop = true;
				grassSkidSource.playOnAwake = false;
				grassSkidSource.clip = grassSkidClip;
				grassSkidSource.volume = skidVolume;
			}
			if (flameSource)
			{
				flameSource.loop = false;
				flameSource.playOnAwake = false;
				flameSource.clip = flameClip;
				flameSource.volume = flameVolume;
			}
			//_____________________________

			if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Contains("Garage")
				|| UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == garageSceneName)
			{
				engineSource.Stop();
				this.enabled = false;
			}

		}

		private void Update()
		{
			// The pitch is interpolated between the min and max values, according to the vehicle's revs.
			float pitch = ULerp(PitchMin, PitchMax, m_vehicleController.Revs * revMultiplier);

			// clamp to minimum pitch (note, not clamped to max for high revs while burning out)
			pitch = Mathf.Min(PitchMax, pitch);

			engineSource.pitch =
				Mathf.Lerp(engineSource.pitch, pitch * pitchMultiplier,
				Time.deltaTime * 5);
		}


		private static float ULerp(float from, float to, float value)
		{
			return (1.0f - value) * from + value * to;
		}


		public void Stop_Effects()
		{
			for (int a = 0; a < wheelSmokes.Length; a++)
			{
				var emi = wheelSmokes[a].GetComponent<ParticleSystem>().emission;
				emi.enabled = false;
			}

			if (collisionSpark)
			{
				var emi = collisionSpark.GetComponent<ParticleSystem>().emission;
				emi.enabled = false;
			}
		}
		public void Play_StartSkid_Sound()
		{
			StartCoroutine(StartSkid());
		}

		IEnumerator StartSkid()
		{
			startSkidSource.Play();

			for (int a = 0; a < 2; a++)
			{
				var emi = wheelSmokes[a].GetComponent<ParticleSystem>().emission;
				emi.enabled = true;
			}

			// Reduce mass of the car at start skidding
			float mass = 0;
			mass = GetComponent<Rigidbody>().mass;
			GetComponent<Rigidbody>().mass = mass / 2;

			yield return new WaitForSeconds(startSkidDuration);

			GetComponent<Rigidbody>().mass = mass;

			if (startSkidSource.isPlaying)
				startSkidSource.Stop();

			for (int a = 0; a < 2; a++)
			{
				var emi = wheelSmokes[a].GetComponent<ParticleSystem>().emission;
				emi.enabled = false;
			}

			raceIsStarted = true;





			/*yield return new WaitForSeconds(startSkidDuration);

			for (int a = 0; a < startSmokes.Length; a++)
				startSmokes[a].SetActive(false);*/

		}
		public void Play_ChangeGear_Sound()
		{
			gearSource.PlayOneShot(gearShift);
		}

		// Flame

		[HideInInspector] public bool isFlamePlaying;
		[HideInInspector] public bool stopRandom;
		public void Play_Flame_Sound()
		{
			flameSource.PlayOneShot(flameClip);
			for (int a = 0; a < exhaustFlame.Length; a++)
			{
				exhaustFlame[a].GetComponent<ParticleSystem>().Play();
			}
		}
		public void Play_RandomFlame_Sound()
		{
			if (!isFlamePlaying)
				StartCoroutine(RandomFlame());
		}
		public void Stop_RandomFlame_Sound()
		{
			StopCoroutine(RandomFlame());
		}
		IEnumerator RandomFlame()
		{
			isFlamePlaying = true;

			while (!stopRandom)
			{
				yield return new WaitForSeconds(Random.Range(0.3f, 1));

				flameSource.PlayOneShot(flameClip);

				for (int a = 0; a < exhaustFlame.Length; a++)
				{
					exhaustFlame[a].GetComponent<ParticleSystem>().Play();
				}

			}

			isFlamePlaying = false;
		}

		// Wheel skiddmark sound manager

		[HideInInspector] public bool inRoadCheck;

		[HideInInspector] public bool[] checkWheel;

		public void Check_InRoad()
		{
			for (int a = 0; a < checkWheel.Length; a++)
			{
				if (checkWheel[a] == true)
					inRoadCheck = true;
				else
					inRoadCheck = false;
			}
		}

		void OnCollisionEnter(Collision collision)
		{
			if (collision.relativeVelocity.magnitude > collisionVelocity)
			{
				if (m_vehicleController.isPlayer)
				{
					if (shakeUtility)
					{
						shakeUtility.collisionShaking = true;
						shakeUtility.shakeIntensity = (m_vehicleController.currentSpeed / 5)
							 * m_vehicleController.collisionShakeIntensity;
					}
				}

				collisionSource.gameObject.transform.position =
				collision.GetContact(0).point;

				if (collisionSpark && collision.relativeVelocity.magnitude > collisionVelocity + 3)
				{
					collisionSpark.transform.position =
						collision.GetContact(0).point;

					var emi = collisionSpark.GetComponent<ParticleSystem>().emission;
					emi.enabled = true;
				}

				if (!collisionSource.isPlaying)
				{
					collisionSource.PlayOneShot(collisionClip);
				}
			}
		}
		void OnCollisionExit(Collision collision)
		{
			if (collisionSpark)
			{
				Debug.Log(m_vehicleController);
                if (m_vehicleController.isPlayer)
				{
					if (shakeUtility)
						shakeUtility.collisionShaking = false;
				}

				var emi = collisionSpark.GetComponent<ParticleSystem>().emission;
				emi.enabled = false;
			}
		}
		void OnCollisionStay(Collision collision)
        {
			if (collisionSpark && collision.relativeVelocity.magnitude < collisionVelocity + 3)
            {
				if (m_vehicleController.isPlayer)
				{
					if (shakeUtility)
						shakeUtility.collisionShaking = false;
				}

				var emi = collisionSpark.GetComponent<ParticleSystem>().emission;
				emi.enabled = false;
			}
		}
	}
}