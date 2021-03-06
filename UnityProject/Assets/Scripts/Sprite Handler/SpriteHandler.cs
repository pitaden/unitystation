﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility = UnityEngine.Networking.Utility;
using Mirror;
using UnityEditor;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine.Serialization;

///	<summary>
///	for Handling sprite animations
///	</summary>
public class SpriteHandler : MonoBehaviour
{
	public SpriteData spriteData = new SpriteData();

	public List<SpriteSheetAndData> Sprites = new List<SpriteSheetAndData>();

	public class SpriteInfo
	{
		public Sprite sprite;
		public float waitTime;
	}
	private SpriteRenderer spriteRenderer;

	[SerializeField]
	private int spriteIndex;

	[SerializeField]
	[FormerlySerializedAs("VariantIndex")]
	private int variantIndex;

	private int animationIndex = 0;

	private float timeElapsed = 0;

	private float waitTime;

	private bool isAnimation = false;

	[SerializeField]
	private bool SetSpriteOnStartUp = true;


	private bool Initialised;

	private IEnumerator WaitForInitialisation()
	{
		yield return WaitFor.EndOfFrame;
		Initialised = true;
		spriteRenderer = this.GetComponent<SpriteRenderer>();
		if (spriteData == null) {
			spriteData = new SpriteData();
		}
		if (spriteRenderer != null && SetSpriteOnStartUp && spriteData.HasSprite())
		{
			PushTexture();
		}
	}

	private void OnDisable()
	{
		TryToggleAnimationState(false);
	}

	public void SetColor(Color value)
	{
		if (spriteRenderer == null)
		{
			spriteRenderer = this.GetComponent<SpriteRenderer>();
		}
		spriteRenderer.color = value;
	}

	public void PushClear()
	{
		spriteRenderer.sprite = null;
		TryToggleAnimationState(false);
	}

	public void PushTexture()
	{
		if (Initialised)
		{
			if (spriteData != null && spriteData.List != null)
			{
				if (spriteIndex < spriteData.List.Count &&
					variantIndex < spriteData.List[spriteIndex].Count &&
					animationIndex < spriteData.List[spriteIndex][variantIndex].Count)
				{
					SetSprite(spriteData.List[spriteIndex][variantIndex][animationIndex]);
					TryToggleAnimationState(spriteData.List[spriteIndex][variantIndex].Count > 1);
					return;
				}
				else if (spriteIndex < spriteData.List.Count &&
						 variantIndex < spriteData.List[spriteIndex].Count)
				{
					animationIndex = 0;
					SetSprite(spriteData.List[spriteIndex][variantIndex][animationIndex]);
					TryToggleAnimationState(spriteData.List[spriteIndex][variantIndex].Count > 1);
					return;
				}
			}
			spriteRenderer.sprite = null;
			TryToggleAnimationState(false);
		}
	}

	public void UpdateMe()
	{

		timeElapsed += Time.deltaTime;
		if (spriteData.List.Count > spriteIndex &&
			timeElapsed >= waitTime)
		{
			animationIndex++;
			if (animationIndex >= spriteData.List[spriteIndex][variantIndex].Count)
			{
				animationIndex = 0;
			}
			SetSprite(spriteData.List[spriteIndex][variantIndex][animationIndex]);
		}
		if (!isAnimation)
		{
			UpdateManager.Instance.Remove(UpdateMe);
			spriteRenderer.sprite = null;
		}
	}

	private void SetSprite(SpriteInfo animationStills)
	{
		timeElapsed = 0;
		waitTime = animationStills.waitTime;
		spriteRenderer.sprite = animationStills.sprite;
	}

	public void ChangeSprite(int newSprites)
	{
		if (spriteData.List != null)
		{
			if (newSprites < spriteData.List.Count &&
				spriteIndex != newSprites &&
				variantIndex < spriteData.List[newSprites].Count)
			{
				spriteIndex = newSprites;
				animationIndex = 0;
				SetSprite(spriteData.List[spriteIndex][variantIndex][animationIndex]);
				TryToggleAnimationState(spriteData.List[spriteIndex][variantIndex].Count > 1);
			}
		}
	}

	public void ChangeSpriteVariant(int spriteVariant)
	{
		if (spriteData.List != null)
		{
			if (spriteIndex < spriteData.List.Count &&
				spriteVariant < spriteData.List[spriteIndex].Count &&
				variantIndex != spriteVariant)
			{
				if (spriteData.List[spriteIndex][spriteVariant].Count <= animationIndex)
				{
					animationIndex = 0;
				}
				SetSprite(spriteData.List[spriteIndex][spriteVariant][animationIndex]);
				variantIndex = spriteVariant;
				TryToggleAnimationState(spriteData.List[spriteIndex][variantIndex].Count > 1);
			}
		}
	}

	private void TryToggleAnimationState(bool turnOn)
	{
		//UpdateManager.Instance.Remove(UpdateMe);
		if (turnOn && !isAnimation)
		{
			UpdateManager.Instance.Add(UpdateMe);
			isAnimation = true;
		}
		else if (!turnOn && isAnimation)
		{
			UpdateManager.Instance.Remove(UpdateMe);
			isAnimation = false;
		}
	}

	/// <summary>
	/// Used to Set sprite handlers internal buffer to the single Texture specified and set Sprite
	/// </summary>
	/// <param name="_SpriteSheetAndData">specified Texture.</param>
	/// <param name="_variantIndex">Variant index.</param>
	public void SetSprite(SpriteSheetAndData _SpriteSheetAndData, int _variantIndex = 0)
	{
		spriteData.List.Clear();
		spriteData.List.Add(SpriteFunctions.CompleteSpriteSetup(_SpriteSheetAndData));
		variantIndex = _variantIndex;
		if (Initialised)
		{
			PushTexture();
		}
		else {
			SetSpriteOnStartUp = true;		}
	}

	/// <summary>
	/// Used to Set sprite handlers internal buffer to To a different internal buffer
	/// </summary>
	/// <param name="_Info">internal buffer.</param>
	/// <param name="_spriteIndex">Sprite index.</param>
	/// <param name="_variantIndex">Variant index.</param>
	public void SetInfo(SpriteData _Info, int _spriteIndex = 0, int _variantIndex = 0)
	{
		spriteIndex = _spriteIndex;
		variantIndex = _variantIndex;
		spriteData = _Info;
		if (Initialised)
		{
			PushTexture();
		}
		else {
			SetSpriteOnStartUp = true;
		}
	}

	void Start()
	{
		AddSprites();
		spriteRenderer = this.GetComponent<SpriteRenderer>();
		StartCoroutine(WaitForInitialisation());
	}

	private void AddSprites()
	{
		foreach (var Data in Sprites)
		{
			if (spriteData.List == null)
			{
				spriteData.List = new List<List<List<SpriteInfo>>>();
			}
			spriteData.List.Add(SpriteFunctions.CompleteSpriteSetup(Data));
		}

	}

	public SpriteHandlerState ReturnState()
	{
		return (new SpriteHandlerState
		{
			spriteIndex = spriteIndex,
			variantIndex = variantIndex,
			animationIndex = animationIndex,
			hasSprite = spriteRenderer.sprite != null
		});
	}
}
public class SpriteHandlerState
{
	//public string Name; //for  synchronising of network
	//public List<Something> The current Textures being used, idkW hat will be used fo networkingr them
	public int spriteIndex;
	public int variantIndex;
	public int animationIndex;
	public bool hasSprite;
}