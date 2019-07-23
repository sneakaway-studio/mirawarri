﻿/* // COMMENTING TO TRY TO SPEED THINGS UP, SCRIPT WORKS THO (I THINK)

using UnityEngine;
using UnityEngine.EventSystems; // 1

public class TouchMe : MonoBehaviour
, IPointerClickHandler // 2
, IDragHandler
, IPointerEnterHandler
, IPointerExitHandler
// ... And many more available!
{
	SpriteRenderer sprite;
	Color target = Color.red;


	void Awake()
	{
		sprite = GetComponent<SpriteRenderer>();
	}

	void Update()
	{
		if (sprite)
			sprite.color = Vector4.MoveTowards(sprite.color, target, Time.deltaTime * 10);
	}

	public void OnPointerClick(PointerEventData eventData) // 3
	{
		print("I was clicked = ");
		target = Color.blue;
	}

	public void OnDrag(PointerEventData eventData)
	{
		print("I'm being dragged!");
		target = Color.magenta;



	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		target = Color.green;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		target = Color.red;
	}
}

*/