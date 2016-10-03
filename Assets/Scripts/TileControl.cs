using System.Collections;
using UnityEngine;

public class TileControl : MonoBehaviour {
	public GameObject Tiles;
	public GameObject Controller;
	public Light LightSource;
	public Renderer zero;
	public Renderer one;
	public Renderer two;
	public Renderer three;
	public int zoomLvl = 4;
	public int row = 0;
	public int column = 0;
	public int maxZoomLvl;

	private Texture2D zeroTex;
	private Texture2D oneTex;
	private Texture2D twoTex;
	private Texture2D threeTex;
	private bool columnDown = false;
	private bool rowDown = false;
	private bool up = false;
	private bool left = false;
	private bool maxZoomLvlReached = false;
	private float angle = 0;
    private float rotateAngle = 0;

	void Awake () {
		zero.material.mainTexture = Resources.Load(zoomLvl+"/zoom0xx0") as Texture2D;
		one.material.mainTexture = Resources.Load(zoomLvl+"/zoom0xx1") as Texture2D;
		two.material.mainTexture = Resources.Load(zoomLvl+"/zoom1xx0") as Texture2D;
		three.material.mainTexture = Resources.Load(zoomLvl+"/zoom1xx1") as Texture2D;
	}

	/*void Update()
	{
		if (Input.GetKeyDown("r"))
		{
			StartCoroutine(doStuff());
		}
		angle += 2;
		var rads = angle*((Mathf.PI*2)/360);
		Tiles.transform.position = new Vector3(((Mathf.Sin(rads)*10)), 0, (Mathf.Cos(rads)*10));
		Tiles.transform.RotateAround(Tiles.transform.position, Tiles.transform.up, 2);
	}*/

	// Call when zooming in 
	public void ZoomIn() {
		StartCoroutine(CoZoomIn());
	}

	IEnumerator CoZoomIn() {
		GetTiles();
		
		if (oneTex == null) {
			column = column - 1;
			Debug.Log("Column - 1");
			GetTiles();
			columnDown = true;
		} else if (twoTex == null) {
			row = row - 1;
			Debug.Log("Row - 1");
			GetTiles();
			rowDown = true;
		}
		yield return null;
		CheckBounds();
		yield return null;
		SetTiles();
		
		if (columnDown) {
			column = column + 1;
			columnDown = false;
		}
		if (rowDown) {
			row = row + 1;
			rowDown = false;
		}
		
		Debug.Log("Done");
	}

	// Call when a tile is clicked, and call zoomIn(), if further zoom is possible
	public void TileClicked() {
		zoomLvl -= 1;
		if (zoomLvl < maxZoomLvl) {
			maxZoomLvlReached = true;
			Debug.Log("Max ZoomLvl");
			zoomLvl = maxZoomLvl;
		}else{
			ZoomIn();
		}
	}

	// Sets the row
	public void SetRow(int rowNum) {
		if (!maxZoomLvlReached) {
			row = row + (rowNum*2);
		}
	}

	// Set the column
	public void SetColumn(int colNum) {
		if (!maxZoomLvlReached) {
			column = column + (colNum*2);
		}
	}

	// Loads the tiles from the Resources folder
	private void GetTiles() {
		zeroTex = Resources.Load(zoomLvl+"/zoom"+row+"xx"+column) as Texture2D;
		oneTex = Resources.Load(zoomLvl+"/zoom"+row+"xx"+(column+1)) as Texture2D;
		twoTex = Resources.Load(zoomLvl+"/zoom"+(row+1)+"xx"+column) as Texture2D;
		threeTex = Resources.Load(zoomLvl+"/zoom"+(row+1)+"xx"+(column+1)) as Texture2D;
	}

	private void SetTiles() {
		zero.material.mainTexture = zeroTex;
		one.material.mainTexture = oneTex;
		two.material.mainTexture = twoTex;
		three.material.mainTexture = threeTex;

		Debug.Log(zoomLvl+"/zoom"+row+"xx"+column);
		Debug.Log(zoomLvl+"/zoom"+row+"xx"+(column+1));
		Debug.Log(zoomLvl+"/zoom"+(row+1)+"xx"+column);
		Debug.Log(zoomLvl+"/zoom"+(row+1)+"xx"+(column+1));
	}

	public void ScrollLeft() {
		left = true;
		column -= 1;
		GetTiles();
		CheckBounds();
		SetTiles();
	}

	public void ScrollRight() {
		++column;
		GetTiles();
		CheckBounds();
		SetTiles();
	}

	public void ScrollUp() {
		up = true;
		--row;
		GetTiles();
		CheckBounds();
		SetTiles();
	}
	
	public void ScrollDown() {
		++row;
		GetTiles();
		CheckBounds();
		SetTiles();
	}

	private void CheckBounds() {
		if (zeroTex == null && left) {
			column += 1;
			Debug.Log("Column + 1");
			left = false;
			GetTiles();
		} else if (zeroTex == null && up) {
			row += 1;
			Debug.Log("Row + 1");
			up = false;
			GetTiles();
		} else if (oneTex == null) {
			column -= 1;
			Debug.Log("Column - 1");
			GetTiles();
		} else if (twoTex == null) {
			row -= 1;
			Debug.Log("Row - 1");
			GetTiles();
		}
	}

	public void ZoomOut() {
		if (!(zoomLvl >= 4)) {
			zoomLvl += 1;
			column = column/2;
			row = row/2;
			GetTiles();
			CheckBounds();
			SetTiles();
		}
	}

	public void TurnLight() {
		StartCoroutine(CoTurnLight());
	}

	public IEnumerator CoTurnLight() {
        angle = Camera.main.transform.eulerAngles.y;
		var rads = angle*((Mathf.PI*2)/360);
        

        /*yield return null;
		Tiles.transform.localPosition = new Vector3(Mathf.Cos(rads)*10, 0, Mathf.Sin(rads)*10);
		yield return null;
		Tiles.transform.eulerAngles = new Vector3(0,Camera.main.transform.eulerAngles.y, 0);
		yield return null;*/
        Tiles.transform.position = new Vector3(((Mathf.Sin(rads) * 10)), 0, (Mathf.Cos(rads) * 10));
        yield return null;
        Tiles.transform.RotateAround(Tiles.transform.position, Tiles.transform.up, rotateAngle);
        Tiles.transform.rotation = Quaternion.Slerp(rotateAngle, Camera.main.transform.eulerAngles.y, 0);
        yield return null;

        Controller.transform.position = new Vector3(((Mathf.Sin(rads) * 10)), 0, (Mathf.Cos(rads) * 10));
        yield return null;
		Controller.transform.RotateAround(Tiles.transform.position, Tiles.transform.up, rotateAngle);
		yield return null;

		LightSource.transform.RotateAround(Tiles.transform.position, Tiles.transform.up, rotateAngle);
        rotateAngle = Camera.main.transform.eulerAngles.y;
    }

	IEnumerator doStuff(){
		angle += 2;
		var rads = angle*((Mathf.PI*2)/360);
		Debug.Log("Rotate: "+angle);
		Tiles.transform.position = new Vector3(((Mathf.Sin(rads)*10)), 5, (Mathf.Cos(rads)*10));
		yield return null;
		Tiles.transform.RotateAround(Tiles.transform.position, Tiles.transform.up, 2);
		yield return null;
	}
}
