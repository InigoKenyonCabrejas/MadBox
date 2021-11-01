using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour
{
	[SerializeField] private Camera uiCamera;
	[SerializeField] private CanvasGroup canvasGroup;
	[SerializeField] private RectTransform myRectTransform;
	[SerializeField] private RectTransform innerJoystickRectTransform;
	[SerializeField] private float maxInnerJoystickSeparation = 30f;
	[SerializeField] private float appearJoystickDuration = 0.25f;
	
	private Vector2 initialMouseCoords;
	private Vector2 mouseCoords;
	private Vector2 dirVector;
	private Vector2 innerJoystickInitialPos;
	private bool isMouseDown = false;
	private Tweener tween;
	private float movementStrength;

	public Action<Vector2, float> DirectionAction;

	private void Awake()
	{
		innerJoystickInitialPos = innerJoystickRectTransform.anchoredPosition;
	}

	private void Update()
	{
		if(Input.GetMouseButtonDown(0))//Activate joystick
		{
			isMouseDown = true;
			initialMouseCoords = Input.mousePosition;

			if(tween != null)
			{
				tween.Kill(true);
			}
			tween = canvasGroup.DOFade(1f, appearJoystickDuration);
		}
		else if(Input.GetMouseButtonUp(0))//Deactivate joystick
		{
			isMouseDown = false;
			
			if(tween != null)
			{
				tween.Kill(true);
			}
			tween = canvasGroup.DOFade(0f, appearJoystickDuration);
			
			dirVector = Vector2.zero;
			movementStrength = 0f;
			MovementChanged();
		}
		
		if(isMouseDown)
		{
			mouseCoords = Input.mousePosition;
			dirVector = mouseCoords - initialMouseCoords;
			
			//Keep inner joystick in its range
			if(dirVector.magnitude > maxInnerJoystickSeparation)
			{
				dirVector = dirVector.normalized * maxInnerJoystickSeparation;
				movementStrength = 1f;
			}
			else
			{
				movementStrength = dirVector.magnitude / maxInnerJoystickSeparation;
			}

			MovementChanged();
		}
		
		void MovementChanged()
		{
			innerJoystickRectTransform.anchoredPosition = innerJoystickInitialPos + dirVector;
			DirectionAction?.Invoke(dirVector.normalized, movementStrength);
		}
	}
}
