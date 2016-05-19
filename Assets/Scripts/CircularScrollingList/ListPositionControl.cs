/* Calculate and assign the final position for each ListBoxes.
 *
 * There are three controling modes:
 * 1. Free moving: Control the listBoxes with finger or mouse.
 *    You don't know where the ListBox would stop at.
 * 2. Align to center: It's the same as free moving
 *    but there always has a listBox positioning at the center.
 * 3. Control by button: Control the listBoxes by button on the screen.
 *    There always has a listBox positioning at the center.
 *
 * Author: LanKuDot <airlanser@gmail.com>
 *
 * As long as you retain this notice you can do whatever you want with this stuff.
 * If we meet some day, and you think this stuff is worth it,
 * you can buy me a coffee in return. LanKuDot
 */
using UnityEngine;
using UnityEngine.UI;

public class ListPositionControl : MonoBehaviour{


	public static ListPositionControl Instance;

	public bool alignToCenter = false;

	public int hypotenuse = 100;
	public int canvasDistance = 100;

	public ListBoxItem[] listBoxes;

	public int slidingFrames = 35;
	// Set the sliding speed. The larger, the quicker.
	[Range( 0.0f, 1.0f )]
	public float slidingFactor = 0.2f;
	[Range( -1.0f, 1.0f )]
	public float angularity = 0.2f;
	// Set the scale amount of the center listBox.
	public float scaleFactor = 0.05f;

	private bool isTouchingDevice;

	private Vector3 lastInputWorldPos;
	private Vector3 currentInputWorldPos;
	private Vector3 deltaInputWorldPos;

	public static int snapping = 0;

	void Awake()
	{
		Instance = this;

		switch( Application.platform )
		{
		case RuntimePlatform.WindowsEditor:
			isTouchingDevice = false;
			break;
		case RuntimePlatform.Android:
			isTouchingDevice = true;
			break;
		}
	}



	void Update()
	{
		if (snapping > 0)
			return;
		if ( !isTouchingDevice )
			storeMousePosition();
		else
			storeFingerPosition();
	}

	public static int NumOfListBox {
		get{
			return Instance.listBoxes.Length;
		}
	}

	/* Store the position of mouse when the player clicks the left mouse button.
	 */
	void storeMousePosition()
	{
		if ( Input.GetMouseButtonDown(0) )
		{
			lastInputWorldPos = Camera.main.ScreenToWorldPoint(
				new Vector3( Input.mousePosition.x, Input.mousePosition.y, canvasDistance ) );
		}
		else if ( Input.GetMouseButton(0) )
		{
			currentInputWorldPos = Camera.main.ScreenToWorldPoint(
				new Vector3( Input.mousePosition.x, Input.mousePosition.y, canvasDistance ) );
			deltaInputWorldPos = currentInputWorldPos - lastInputWorldPos;
			foreach ( ListBoxItem listbox in listBoxes )
				listbox.updatePosition( deltaInputWorldPos / transform.parent.localScale.x );

			gameObject.SortChildren();

			lastInputWorldPos = currentInputWorldPos;
		}
		else if ( Input.GetMouseButtonUp(0) )
			setSlidingEffect();
	}


	
	/* Store the position of touching on the mobile.
	 */
	void storeFingerPosition()
	{
		if ( Input.GetTouch(0).phase == TouchPhase.Began )
		{
			lastInputWorldPos = Camera.main.ScreenToWorldPoint(
				new Vector3( Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, canvasDistance ) );
		}
		else if ( Input.GetTouch(0).phase == TouchPhase.Moved )
		{
			currentInputWorldPos = Camera.main.ScreenToWorldPoint(
				new Vector3( Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, canvasDistance ) );
			deltaInputWorldPos = currentInputWorldPos - lastInputWorldPos;
			foreach ( ListBoxItem listbox in listBoxes )
				listbox.updatePosition( deltaInputWorldPos / transform.parent.localScale.x );

			lastInputWorldPos = currentInputWorldPos;
		}
		else if ( Input.GetTouch(0).phase == TouchPhase.Ended )
			setSlidingEffect();
	}

	/* If the touching is ended, calculate the distance to slide and
	 * assign to the listBoxes.
	 */
	void setSlidingEffect()
	{
		Vector3 deltaPos = deltaInputWorldPos / transform.parent.localScale.x;
	
		if (alignToCenter) {
			ListBoxItem item = FindMinumumCloseItemToCenter();
			float deltaX = 180 - item.currentAngle;
			deltaPos = new Vector3(deltaX, 0 , 0);
		}
			
		foreach( ListBoxItem listbox in listBoxes )
			listbox.setSlidingDistance( deltaPos );
	}

	ListBoxItem FindMinumumCloseItemToCenter()
	{
		ListBoxItem lbItem = null;
		foreach( ListBoxItem listBox in listBoxes ) {
			if(lbItem == null) {
				lbItem = listBox;
				continue;
			}
			float deltaAngle = Mathf.Abs(listBox.CurrentAngle) - 180;
			if ( Mathf.Abs( deltaAngle ) <  Mathf.Abs(lbItem.CurrentAngle - 180) ) {
				lbItem = listBox;
			}
			
		}
		return lbItem;
	}

}
