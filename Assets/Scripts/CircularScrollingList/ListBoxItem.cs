using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ListBoxItem : MonoBehaviour {

	int numOfListBox;

	public int listBoxID;	// Must be unique, and count from 0
	public Text content;		// The content of the list box
	public ListBoxItem lastListBox;
	public ListBoxItem nextListBox;
	public int contentID;

	private Vector3 slidingDistance_L;	// The sliding distance at each frame
	private Vector3 slidingDistanceLeft_L;
	
	private Vector3 originalLocalScale;
	
	private bool keepSliding = false;
	private int slidingFrames;

	public float currentAngle = 0;
	

	void Start ()
	{
		numOfListBox = ListPositionControl.Instance.listBoxes.Length;
		originalLocalScale = transform.localScale;

		initialContent();
		initialPosition( listBoxID );
	}

	void initialPosition (int listBoxID)
	{
		currentAngle = (360 / numOfListBox) * listBoxID;
		float _x = ListPositionControl.Instance.hypotenuse * Mathf.Sin (currentAngle  * Mathf.Deg2Rad);
		Debug.Log ("listBoxID "+listBoxID+" x "+currentAngle);
		transform.localPosition = new Vector3 (_x, 0, 0);
	//	                                      unitPos_L.y * (float)( listBoxID * -1 + numOfListBox / 2 ),
	//	                                      0.0f );

	}


	void initialContent()
	{
		if ( listBoxID == numOfListBox / 2 )
			contentID = 0;
		else if ( listBoxID < numOfListBox / 2 )
			contentID = ListBank.Instance.getListLength() - ( numOfListBox / 2 - listBoxID );
		else
			contentID = listBoxID - numOfListBox / 2;
		
		while ( contentID < 0 )
			contentID += ListBank.Instance.getListLength();
		contentID = contentID % ListBank.Instance.getListLength();
		
		updateContent( ListBank.Instance.getListContent( contentID ) );
	}

	void updateContent( string content )
	{
		this.content.text = content;
	}



	public void setSlidingDistance (Vector3 distance)
	{
		ListPositionControl.snapping ++;
		keepSliding = true;
		slidingFrames = ListPositionControl.Instance.slidingFrames;
		
		slidingDistanceLeft_L = distance;
		slidingDistance_L = Vector3.Lerp( Vector3.zero, distance, ListPositionControl.Instance.slidingFactor );
	}



	void Update ()
	{
		if ( keepSliding )
		{
			--slidingFrames;
			if ( slidingFrames == 0 )
			{
				keepSliding = false;
				if ( ListPositionControl.Instance.alignToCenter )
					CurrentAngle += slidingDistanceLeft_L.x;

				ListPositionControl.snapping --;
				return;
			}
			
			CurrentAngle += slidingDistance_L.x;
			slidingDistanceLeft_L -= slidingDistance_L;
			slidingDistance_L = Vector3.Lerp( Vector3.zero, slidingDistanceLeft_L, ListPositionControl.Instance.slidingFactor );
		}

	}

	bool CanShow(float angle)
	{
		return angle >= 270 || angle <= 90;
	}

	public void updatePosition (Vector3 deltaPosition_L)
	{
		CurrentAngle -= deltaPosition_L.x;
	}

	public float CurrentAngle
	{
		get {
			return currentAngle;
		}
		set{
			if(CanShow(value) && !CanShow(currentAngle)) {
				updateToNextContent();
			}
			else if(!CanShow(value) && CanShow(currentAngle)) {
				updateToLastContent();
				transform.SetAsLastSibling();
			}
			if(value >= 360)
				currentAngle = value % 360;
			else if(value < 0)
				currentAngle = 360 + (value % 360);
			else
				currentAngle = value;


			float _x = ListPositionControl.Instance.hypotenuse * Mathf.Sin (currentAngle * Mathf.Deg2Rad);
			float z = ListPositionControl.Instance.hypotenuse * Mathf.Cos (currentAngle * Mathf.Deg2Rad);
			transform.localPosition = new Vector3 (_x, 0, z);

			float scale = ListPositionControl.Instance.hypotenuse - z;
			updateSize( 2 * ListPositionControl.Instance.hypotenuse,  scale);
		}
	}

	void updateSize( float smallest_at, float target_value )
	{
		float val = Mathf.InverseLerp( ListPositionControl.Instance.hypotenuse, smallest_at, Mathf.Abs( target_value ));
		transform.localScale = originalLocalScale *
			( 1.0f + ListPositionControl.Instance.scaleFactor * val);
	}

	public int getCurrentContentID()
	{
		return contentID;
	}

	/* Update to the last content of the next ListBox
	 * when the ListBox appears at the top of camera.
	 */
	public void updateToLastContent()
	{
		contentID = nextListBox.getCurrentContentID() - 1;
		contentID = ( contentID < 0 ) ? ListBank.Instance.getListLength() - 1 : contentID;
		
		updateContent( ListBank.Instance.getListContent( contentID ) );
	}
	
	/* Update to the next content of the last ListBox
	 * when the ListBox appears at the bottom of camera.
	 */
	protected void updateToNextContent()
	{
		contentID = lastListBox.getCurrentContentID() + 1;
		contentID = ( contentID == ListBank.Instance.getListLength() ) ? 0 : contentID;
		
		updateContent( ListBank.Instance.getListContent( contentID ) );
	}
}
