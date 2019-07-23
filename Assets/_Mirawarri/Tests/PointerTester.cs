/* // COMMENTING TO TRY TO SPEED THINGS UP, SCRIPT WORKS THO (I THINK)

using UnityEngine;
using UnityEngine.EventSystems;


public class PointerTester : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {


	private OwenCam otherscript;


	void Awake(){
		otherscript = GameObject.Find("OwenCam").GetComponent<OwenCam>();
	}








	// Use this for initialization
	public void OnPointerDown(PointerEventData eventData)
	{
		Debug.Log("Pointer is Down");
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		Debug.Log("Pointer is Up");
		otherscript.OnPointerUp (eventData);
	}
}
*/