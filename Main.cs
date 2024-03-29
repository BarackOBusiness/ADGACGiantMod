using System.Collections;
using System.Reflection;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Giant;

[BepInPlugin("ADGACExt.BarackOBusiness.Giantmod", "Giant Mod", "1.0.0")]
[BepInProcess("A Difficult Game About Climbing.exe")]
public class Main : BaseUnityPlugin
{
	// Wanted to make this configurable, but the other values need so much fine tuning
	// there's not a good way to find out algorithmically what they should be, so this is a constant instead
	private const float scale = 3f;

	private delegate void ResourcesAvailable(ClimberMain climber);
	private event ResourcesAvailable OnResourcesFound;

    private void Awake()
    {
        Logger.LogInfo("Welcome to giantville.");

		Harmony.CreateAndPatchAll(typeof(GamePatches));

		OnResourcesFound += SecondPass;
		SceneManager.sceneLoaded += OnSceneLoad;
	}

	private void OnDestroy() {
		SceneManager.sceneLoaded -= OnSceneLoad;
	}

	private void OnSceneLoad(Scene scene, LoadSceneMode mode) {
		if (scene.name == "MainGameScene") {
			FirstPass(Resources.FindObjectsOfTypeAll<ClimberMain>()[0]);
			StartCoroutine(AwaitPlayer());
		}
	}

	private IEnumerator AwaitPlayer() {
		while (Resources.FindObjectsOfTypeAll<ClimberMain>()[0].body.GetComponent<Rigidbody2D>().centerOfMass == Vector2.zero) {
			yield return null;
		}

		OnResourcesFound(Resources.FindObjectsOfTypeAll<ClimberMain>()[0]);
	}

	private void FirstPass(ClimberMain climber) {
		// Setup the base scale
		Camera.main.orthographicSize = 4.5f;
		Camera.main.transform.Find("ForegroundCamera").GetComponent<Camera>().fieldOfView = 42f;
		climber.transform.localScale = Vector3.one * scale;
	
        DistanceJoint2D jointL = climber.arm_Left.GetComponent<DistanceJoint2D>();
        DistanceJoint2D jointR = climber.arm_Right.GetComponent<DistanceJoint2D>();

		// Set all the left arm values
        jointL.distance = 3f;
        climber.arm_Left.armDistance = 3f;

		// Set cursor distance with reflection
        FieldInfo cursorDistanceL = climber.arm_Left.GetType().GetField("cursorDistance", BindingFlags.NonPublic | BindingFlags.Instance);
		cursorDistanceL.SetValue(climber.arm_Left, 1.6f);

		// Same situation for the right arm
		jointR.distance = 3f;
		climber.arm_Right.armDistance = 3f;

		FieldInfo cursorDistanceR = climber.arm_Right.GetType().GetField("cursorDistance", BindingFlags.NonPublic | BindingFlags.Instance);
		cursorDistanceR.SetValue(climber.arm_Right, 1.6f);
	}

    private void SecondPass(ClimberMain climber) {
		climber.body.GetComponent<Rigidbody2D>().centerOfMass = new Vector2(0f, 3.5f);
		climber.body.GetComponent<Rigidbody2D>().drag = 1f;
	}
}

public static class GamePatches {
	public static float pitchShift = -0.25f;

	[HarmonyPatch(typeof(PlayerSoundManager), "PlayVoiceChargeSound")]
	[HarmonyPrefix]
	public static bool PlayVoiceChargeSound(PlayerSoundManager __instance)
	{
		__instance.PlaySound("VoiceCharge" + UnityEngine.Random.Range(1, 13), false, 1f, pitchShift);
		return false;
	}
	[HarmonyPatch(typeof(PlayerSoundManager), "PlayVoiceGrabSound")]
	[HarmonyPrefix]
	public static bool PlayVoiceGrabSound(PlayerSoundManager __instance)
	{
		__instance.PlaySound("VoiceGrab" + UnityEngine.Random.Range(1, 5), false, 1f, pitchShift);
		return false;
	}
	[HarmonyPatch(typeof(PlayerSoundManager), "PlayVoiceUpsetSound")]
	[HarmonyPrefix]
	public static bool PlayVoiceUpsetSound(PlayerSoundManager __instance)
	{
		__instance.PlaySound("VoiceUpset" + UnityEngine.Random.Range(1, 14), false, 1f, pitchShift);
		return false;
	}
	[HarmonyPatch(typeof(PlayerSoundManager), "PlayVoiceFallSound")]
	[HarmonyPrefix]
	public static bool PlayVoiceFallSound(PlayerSoundManager __instance)
	{
		__instance.PlaySound("VoiceFall" + UnityEngine.Random.Range(1, 3), false, 1f, pitchShift);
		return false;
	}
	[HarmonyPatch(typeof(PlayerSoundManager), "PlayVoiceBigFallSound")]
	[HarmonyPrefix]
	public static bool PlayVoiceBigFallSound(PlayerSoundManager __instance)
	{
		__instance.PlaySound("VoiceBigFall1", false, 1f, pitchShift);
		return false;
	}

	[HarmonyPatch(typeof(Body), "OnTriggerEnter2D")]
	[HarmonyPrefix]
	public static bool OnTriggerEnter2D(Body __instance, ref Rigidbody2D ___rb, Collider2D collision) {
		if (collision.tag == "Water")
		{
			__instance.isInWater = true;
			___rb.inertia = 2.0f;
			___rb.angularDrag = 5f;
		}
		return false;
	}
}
