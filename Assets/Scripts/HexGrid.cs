using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HexGrid : MonoBehaviour
{

	public int width = 6;
	public int height = 6;

	public BoardManager boardManager;
	public GameObject cellPrefab;

	HexCell[] cells;

	public Text cellLabelPrefab;

	void Awake()
	{

		cells = new HexCell[height * width];

		for (int z = 0, i = 0; z < height; z++)
		{
			for (int x = 0; x < width; x++)
			{
				CreateCell(x, z, i++);
			}
		}
	}

	void CreateCell(int x, int z, int i)
	{
		Vector3 position;
		position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
		position.y = 0f;
		position.z = z * (HexMetrics.outerRadius * 1.5f);

		GameObject cellObj = Instantiate(cellPrefab);
		HexCell cell = cells[i] = cellObj.GetComponentInChildren<HexCell>();
		cellObj.transform.SetParent(transform, false);
		cellObj.transform.localPosition = position;
		cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);

		boardManager.AddPeice(0, cellObj);

	}	
}
